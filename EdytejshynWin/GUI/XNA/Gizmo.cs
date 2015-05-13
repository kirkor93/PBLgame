using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Edytejshyn.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

// -------------------------------------------------------------
// -- XNA 3D Gizmo (Component)
// -------------------------------------------------------------
// -- open-source gizmo component for any 3D level editor.
// -- contains any feature you may be looking for in a transformation gizmo.
// -- 
// -- for additional information and instructions visit codeplex.
// --
// -- codeplex url: http://xnagizmo.codeplex.com/
// --
// -----------------Please Do Not Remove ----------------------
// -- Work by Tom Looman, licensed under Ms-PL
// -- My Blog: http://coreenginedev.blogspot.com
// -- My Portfolio: http://tomlooman.com
// -- You may find additional XNA resources and information on these sites.
// ------------------------------------------------------------

// rewritten for Edytejshyn

namespace Edytejshyn.GUI.XNA
{
    public class Gizmo
    {
        #region Variables

        #region Private

        /// <summary>
        /// Only active if at least one entity is selected.
        /// </summary>
        private bool _isActive = false;

        private readonly ViewportControl _viewport;
        private readonly GraphicsDevice _graphics;
        private readonly SpriteBatch _spriteBatch;
        private readonly BasicEffect _lineEffect;
        private readonly BasicEffect _meshEffect;
        private readonly BasicEffect _selectionBoxEffect;
        private List<VertexPositionColor> _selectionBoxVertices = new List<VertexPositionColor>();
        private readonly SpriteFont _font;

        private Matrix _view = Matrix.Identity;
        private Matrix _projection = Matrix.Identity;
        private Camera _camera;

        // -- Screen Scale -- //
        private Matrix _screenScaleMatrix;
        private float _screenScale;

        // -- Position - Rotation -- //
        private Vector3 _position = Vector3.Zero;
        private Matrix _rotationMatrix = Matrix.Identity;

        //private Vector3 _localForward = Vector3.Forward;
        //private Vector3 _localUp = Vector3.Up;
        //private Vector3 _localRight;

        // -- Matrices -- //
        //private Matrix _objectOrientedWorld;
        //private Matrix _axisAlignedWorld;
        private Matrix[] _modelLocalSpace;

        // used for all drawing, assigned by local- or world-space matrices
        private Matrix _gizmoWorld = Matrix.Identity;

        // the matrix used to apply to your whole scene, usually matrix.identity (default scale, origin on 0,0,0 etc.)
        public Matrix SceneWorld;

        // -- Lines (Vertices) -- //
        private VertexPositionColor[] _translationLineVertices;
        private const float LINE_LENGTH = 3f;
        private const float LINE_OFFSET = 1f;

        // -- Quads -- //
        private Quad[] _quads;
        private readonly BasicEffect _quadEffect;

        // -- Colors -- //
        private Color[] _axisColors;
        private Color _highlightColor;

        // -- UI Text -- //
        private string[] _axisText;
        private Vector3 _axisTextOffset = new Vector3(0, 0.5f, 0);

        private const float MULTI_AXIS_THICKNESS = 0.05f;
        private const float SINGLE_AXIS_THICKNESS = 0.2f;

        private BoundingBox XAxisBox;
        private BoundingBox YAxisBox;
        private BoundingBox ZAxisBox;
        private BoundingBox XZAxisBox;
        private BoundingBox XYAxisBox;
        private BoundingBox YZAxisBox;

        private const float RADIUS = 1f;
        private BoundingSphere XSphere { get { return new BoundingSphere(Vector3.Transform(_translationLineVertices[1] .Position, _gizmoWorld), RADIUS * _screenScale); } }
        private BoundingSphere YSphere { get { return new BoundingSphere(Vector3.Transform(_translationLineVertices[7] .Position, _gizmoWorld), RADIUS * _screenScale); } }
        private BoundingSphere ZSphere { get { return new BoundingSphere(Vector3.Transform(_translationLineVertices[13].Position, _gizmoWorld), RADIUS * _screenScale); } }


        /// <summary>
        /// The value to adjust all transformation when precisionMode is active.
        /// </summary>
        private const float PRECISION_MODE_SCALE = 0.1f;

        // Delta values of transformation for every update
        private Vector3 _translationDelta = Vector3.Zero;
        private Vector3 _rotationDelta = Vector3.Zero;
        private Vector3 _scaleDelta = Vector3.Zero;

        // Translation Variables
        private Vector3 _firstIntersectionPosition;
        private Vector3 _oldSelectedPosition;
        private Vector3 _intersectPosition;
        private float _firstMousePosition;
        private Vector3 _oldSelectedRotation;

        //private Vector3 _translationScaleSnapDelta;
        //private float _rotationSnapDelta;

        #endregion

        /// <summary>
        /// Enabled if gizmo should be able to select objects and axis.
        /// </summary>
        public bool Enabled { get; set; }

        public bool SelectionBoxesIsVisible = true;
        public bool SnapEnabled = false;
        public bool PrecisionModeEnabled;
        public float TranslationSnapValue = 1;
        public float RotationSnapValue = 30;
        public float ScaleSnapValue = 0.5f;

        // Modes
        private GizmoAxis ActiveAxis = GizmoAxis.None;
        public GizmoMode ActiveMode = GizmoMode.Translate;
        public TransformSpace ActiveSpace = TransformSpace.Local;
        public PivotType ActivePivot = PivotType.ObjectCenter;

        public delegate void VoidDelegate();
        public event VoidDelegate ActiveSpaceChanged;

        #endregion

        public Gizmo(ViewportControl viewport, SpriteBatch spriteBatch, SpriteFont font)
        {
            XAxisBox  = new BoundingBox(new Vector3(LINE_OFFSET, 0, 0), new Vector3(LINE_OFFSET + LINE_LENGTH, SINGLE_AXIS_THICKNESS, SINGLE_AXIS_THICKNESS));
            YAxisBox  = new BoundingBox(new Vector3(0, LINE_OFFSET, 0), new Vector3(SINGLE_AXIS_THICKNESS, LINE_OFFSET + LINE_LENGTH, SINGLE_AXIS_THICKNESS));
            ZAxisBox  = new BoundingBox(new Vector3(0, 0, LINE_OFFSET), new Vector3(SINGLE_AXIS_THICKNESS, SINGLE_AXIS_THICKNESS, LINE_OFFSET + LINE_LENGTH));
            XYAxisBox = new BoundingBox(Vector3.Zero,                   new Vector3(LINE_OFFSET,  LINE_OFFSET,  MULTI_AXIS_THICKNESS));
            XZAxisBox = new BoundingBox(Vector3.Zero,                   new Vector3(LINE_OFFSET,  MULTI_AXIS_THICKNESS,  LINE_OFFSET));
            YZAxisBox = new BoundingBox(Vector3.Zero,                   new Vector3(MULTI_AXIS_THICKNESS,  LINE_OFFSET,  LINE_OFFSET));

            SceneWorld = Matrix.Identity;
            _spriteBatch = spriteBatch;
            _font = font;
            _viewport = viewport;
            _camera = _viewport.Camera;
            _graphics = _viewport.GraphicsDevice;

            Enabled = true;

            _selectionBoxEffect = new BasicEffect(_graphics)
            {
                VertexColorEnabled = true
            };
            _lineEffect = new BasicEffect(_graphics)
            {
                VertexColorEnabled = true,
                AmbientLightColor = Vector3.One,
                EmissiveColor = Vector3.One
            };
            _meshEffect = new BasicEffect(_graphics);
            _quadEffect = new BasicEffect(_graphics)
            {
                World = Matrix.Identity,
                DiffuseColor = _highlightColor.ToVector3(),
                Alpha = 0.5f
            };
            _quadEffect.EnableDefaultLighting();

            // Set local-space offset
            _modelLocalSpace    = new Matrix[3];
            _modelLocalSpace[0] = Matrix.CreateWorld(new Vector3(LINE_LENGTH, 0, 0), Vector3.Left,    Vector3.Up);
            _modelLocalSpace[1] = Matrix.CreateWorld(new Vector3(0, LINE_LENGTH, 0), Vector3.Down,    Vector3.Left);
            _modelLocalSpace[2] = Matrix.CreateWorld(new Vector3(0, 0, LINE_LENGTH), Vector3.Forward, Vector3.Up);

            // Colours: X, Y, Z, Highlight
            _axisColors     = new Color[4];
            _axisColors[0]  = Color.Red;
            _axisColors[1]  = Color.Lime;
            _axisColors[2]  = Color.Blue;
            _axisColors[3]  = Color.Pink;
            _highlightColor = Color.Gold;

            // Helpers to apply colors
            Color xColor = _axisColors[0];
            Color yColor = _axisColors[1];
            Color zColor = _axisColors[2];

            // Text projected in 3D
            _axisText    = new string[3];
            _axisText[0] = "X";
            _axisText[1] = "Y";
            _axisText[2] = "Z";

            // Translucent quads
            const float halfLineOffset = LINE_OFFSET / 2;
            _quads    = new Quad[3];
            _quads[0] = new Quad(new Vector3(halfLineOffset, halfLineOffset, 0), Vector3.Backward, Vector3.Up,    LINE_OFFSET, LINE_OFFSET); //XY
            _quads[1] = new Quad(new Vector3(halfLineOffset, 0, halfLineOffset), Vector3.Up,       Vector3.Right, LINE_OFFSET, LINE_OFFSET); //XZ
            _quads[2] = new Quad(new Vector3(0, halfLineOffset, halfLineOffset), Vector3.Right,    Vector3.Up,    LINE_OFFSET, LINE_OFFSET); //ZY 

            var vertexList = new List<VertexPositionColor>(18);

            
            // -- X Axis -- // index 0 - 5
            vertexList.Add(new VertexPositionColor(new Vector3(halfLineOffset, 0, 0), xColor));
            vertexList.Add(new VertexPositionColor(new Vector3(LINE_LENGTH, 0, 0),    xColor));
            vertexList.Add(new VertexPositionColor(new Vector3(LINE_OFFSET, 0, 0),    xColor));
            vertexList.Add(new VertexPositionColor(new Vector3(LINE_OFFSET, LINE_OFFSET, 0), xColor));
            vertexList.Add(new VertexPositionColor(new Vector3(LINE_OFFSET, 0, 0),           xColor));
            vertexList.Add(new VertexPositionColor(new Vector3(LINE_OFFSET, 0, LINE_OFFSET), xColor));

            // -- Y Axis -- // index 6 - 11
            vertexList.Add(new VertexPositionColor(new Vector3(0, halfLineOffset, 0), yColor));
            vertexList.Add(new VertexPositionColor(new Vector3(0, LINE_LENGTH, 0),    yColor));
            vertexList.Add(new VertexPositionColor(new Vector3(0, LINE_OFFSET, 0),    yColor));
            vertexList.Add(new VertexPositionColor(new Vector3(LINE_OFFSET, LINE_OFFSET, 0), yColor));
            vertexList.Add(new VertexPositionColor(new Vector3(0, LINE_OFFSET, 0),           yColor));
            vertexList.Add(new VertexPositionColor(new Vector3(0, LINE_OFFSET, LINE_OFFSET), yColor));

            // -- Z Axis -- // index 12 - 17
            vertexList.Add(new VertexPositionColor(new Vector3(0, 0, halfLineOffset), zColor));
            vertexList.Add(new VertexPositionColor(new Vector3(0, 0, LINE_LENGTH),    zColor));
            vertexList.Add(new VertexPositionColor(new Vector3(0, 0, LINE_OFFSET),    zColor));
            vertexList.Add(new VertexPositionColor(new Vector3(LINE_OFFSET, 0, LINE_OFFSET), zColor));
            vertexList.Add(new VertexPositionColor(new Vector3(0, 0, LINE_OFFSET),           zColor));
            vertexList.Add(new VertexPositionColor(new Vector3(0, LINE_OFFSET, LINE_OFFSET), zColor));

            _translationLineVertices = vertexList.ToArray();
        }

        public void Draw()
        {
            if (!_isActive) return;
            DepthStencilState oldDepthStencilState = _graphics.DepthStencilState;
            _graphics.DepthStencilState = DepthStencilState.None;

            // Draw Lines
            _lineEffect.World = _gizmoWorld;
            _lineEffect.View = _view;
            _lineEffect.Projection = _projection;

            _lineEffect.CurrentTechnique.Passes[0].Apply();
            _graphics.DrawUserPrimitives(PrimitiveType.LineList, _translationLineVertices, 0, _translationLineVertices.Length / 2);

            DrawQuads();
            Draw3DMeshes();
            Draw2D();

            // ..

            // Cleanup
            _graphics.DepthStencilState = oldDepthStencilState;
        }


        private void DrawQuads()
        {

            switch (ActiveMode)
            {
                case GizmoMode.Translate:
                case GizmoMode.NonUniformScale:
                case GizmoMode.UniformScale:
                    
                    BlendState oldBlendState = _graphics.BlendState;
                    RasterizerState oldRasterizerState = _graphics.RasterizerState;
                     _graphics.BlendState = BlendState.AlphaBlend;
                    _graphics.RasterizerState = RasterizerState.CullNone;
                    _quadEffect.World = _gizmoWorld;
                    _quadEffect.View = _view;
                    _quadEffect.Projection = _projection;
                    _quadEffect.CurrentTechnique.Passes[0].Apply();

                    switch (ActiveMode)
                    {
                        case GizmoMode.Translate:
                        case GizmoMode.NonUniformScale:
                            switch (ActiveAxis)
                            {
                                case GizmoAxis.ZX:
                                case GizmoAxis.YZ:
                                case GizmoAxis.XY:
                                    Quad activeQuad = new Quad();
                                    switch (ActiveAxis)
                                    {
                                        case GizmoAxis.XY:
                                            activeQuad = _quads[0];
                                            break;
                                        case GizmoAxis.ZX:
                                            activeQuad = _quads[1];
                                            break;
                                        case GizmoAxis.YZ:
                                            activeQuad = _quads[2];
                                            break;
                                    }

                                    _graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, activeQuad.Vertices, 0, 4, activeQuad.Indexes, 0, 2);
                                break;
                            }
                            break;
                        case GizmoMode.UniformScale:
                            if (ActiveAxis != GizmoAxis.None)
                            {
                                for (int i = 0; i < _quads.Length; i++)
                                    _graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _quads[i].Vertices, 0, 4, _quads[i].Indexes, 0, 2);

                            }
                            break;
                    }

                    _graphics.BlendState = oldBlendState;
                    _graphics.RasterizerState = oldRasterizerState;
                    break;
            }
        }

        /// <summary>
        /// Draws 3D on axes, depending on gizmo ActiveMode.
        /// </summary>
        private void Draw3DMeshes()
        {
            for (int i = 0; i <3; i++) //(order: x y z)
            {
                GizmoModel activeModel;
                switch (ActiveMode)
                {
                    case GizmoMode.Translate:
                        activeModel = Geometry.Translate;
                        break;
                    case GizmoMode.Rotate:
                        activeModel = Geometry.Rotate;
                        break;
                    default:
                        activeModel = Geometry.Scale;
                        break;
                }
                Vector3 color;
                switch (ActiveMode)
                {
                    case GizmoMode.UniformScale:
                        color = ( (ActiveAxis != GizmoAxis.None) ? _highlightColor : _axisColors[3] ).ToVector3();
                        break;
                    default:
                        color = ( (ActiveAxis.GetId() == i) ? _highlightColor : _axisColors[i] ).ToVector3();
                        break;
                }

                _meshEffect.World = _modelLocalSpace[i] * _gizmoWorld;
                _meshEffect.View = _view;
                _meshEffect.Projection = _projection;

                _meshEffect.DiffuseColor = color;
                _meshEffect.EmissiveColor = color;
                _meshEffect.CurrentTechnique.Passes[0].Apply();

                _graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, 
                    activeModel.Vertices, 0, activeModel.Vertices.Length,
                    activeModel.Indices,  0, activeModel.Indices.Length / 3);
            }
        }

        /// <summary>
        /// Draws text X Y Z on axes
        /// </summary>
        private void Draw2D()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Draw axis identifiers (X Y Z)
            for (int i = 0; i <3; i++)
            {
                Vector3 screenPos =
                  _graphics.Viewport.Project(_modelLocalSpace[i].Translation + _modelLocalSpace[i].Backward + _axisTextOffset,
                                             _projection, _view, _gizmoWorld);

                if (screenPos.Z < 0f || screenPos.Z > 1.0f)
                    continue;

                Color color = _axisColors[i];
                switch (i)
                {
                    case 0:
                        if (ActiveAxis == GizmoAxis.X || ActiveAxis == GizmoAxis.XY || ActiveAxis == GizmoAxis.ZX)
                            color = _highlightColor;
                        break;
                    case 1:
                        if (ActiveAxis == GizmoAxis.Y || ActiveAxis == GizmoAxis.XY || ActiveAxis == GizmoAxis.YZ)
                            color = _highlightColor;
                        break;
                    case 2:
                        if (ActiveAxis == GizmoAxis.Z || ActiveAxis == GizmoAxis.YZ || ActiveAxis == GizmoAxis.ZX)
                            color = _highlightColor;
                        break;
                }

                _spriteBatch.DrawString(_font, _axisText[i], new Vector2(screenPos.X, screenPos.Y), color);
            }

            //Vector2 stringDims = _font.MeasureString(statusInfo);
            //Vector2 position = new Vector2(_graphics.Viewport.Width - stringDims.X, _graphics.Viewport.Height - stringDims.Y);
            //_spriteBatch.DrawString(_font, statusInfo, position, Color.White);

            _spriteBatch.End();
        }

        public void Update()
        {
            _view = _camera.ViewMatrix;
            _projection = _camera.ProjectionMatrix;

            UpdateGizmoPosition();
        }

        /// <summary>
        /// Set position of the gizmo.
        /// </summary>
        private void UpdateGizmoPosition()
        {
            if (_viewport.MainForm.SelectionManager.Empty)
            {
                _isActive = false;
                return;
            }
            _isActive = true;
            GameObjectWrapper selected = _viewport.MainForm.SelectionManager.CurrentSelection[0];
            Matrix selectedWorld = selected.Nut.transform.World;
            _position = selectedWorld.Translation;
            
            // Scale Gizmo to keep zoom independent
            Vector3 vLength = _camera.transform.Position - _position;
            const float scaleFactor = 25.0f;

            _screenScale = vLength.Length() / scaleFactor;
            _screenScaleMatrix = Matrix.CreateScale(new Vector3(_screenScale));

            if (ActiveSpace == TransformSpace.Global ||
                ActiveMode == GizmoMode.NonUniformScale ||
                ActiveMode == GizmoMode.UniformScale)
            {
                _gizmoWorld = _screenScaleMatrix * Matrix.CreateWorld(_position, SceneWorld.Forward, SceneWorld.Up); ;

            }
            else
            {
                _gizmoWorld = _screenScaleMatrix * Matrix.CreateWorld(_position, selectedWorld.Forward, selectedWorld.Up);

            }

            //switch (ActivePivot)
            //{
            //    case PivotType.ObjectCenter:
            //        if (Selection.Count > 0)
            //            _position = Selection[0].Position;
            //        break;
            //    case PivotType.SelectionCenter:
            //        _position = GetSelectionCenter();
            //        break;
            //    case PivotType.WorldOrigin:
            //        _position = SceneWorld.Translation;
            //        break;
            //}
        }

        /// <summary>
        /// Sets mouse down and enters transformation mode.
        /// </summary>
        /// <param name="currentMouse">Current mouse state</param>
        /// <returns>true if gizmo is selected</returns>
        public bool MouseDown(EditorMouseState currentMouse)
        {
            if (!MouseHover(currentMouse)) return false;

            
            GameObjectWrapper selected = _viewport.MainForm.SelectionManager.CurrentSelection[0];

            _translationDelta = Vector3.Zero;
            _firstIntersectionPosition = Vector3.Zero;
            _intersectPosition = Vector3.Zero;
            _firstMousePosition = float.NaN;
            _oldSelectedPosition = selected.Nut.transform.Position;
            _oldSelectedRotation = selected.Nut.transform.Rotation;
            return true;
        }

        public void MouseUp(EditorMouseState currentMouse)
        {
            if (ActiveAxis == GizmoAxis.None || _viewport.MainForm.SelectionManager.Empty || !_isActive) return;

            GameObjectWrapper selected = _viewport.MainForm.SelectionManager.CurrentSelection[0];
            switch (ActiveMode)
            {
                case GizmoMode.Translate:
                    selected.Nut.transform.Position = _oldSelectedPosition; // set old value to enable undo
                    selected.Transform.Position     = _oldSelectedPosition + _translationDelta; // apply new value via wrapper to save in history
                    break;

                case GizmoMode.Rotate:
                    selected.Nut.transform.Rotation = _oldSelectedRotation; // set old value to enable undo
                    selected.Transform.Rotation     = _oldSelectedRotation + _rotationDelta;
                    break;
            }
        }

        public bool MouseDrag(EditorMouseState currentMouse)
        {
            if (ActiveAxis == GizmoAxis.None) return false;

            GameObjectWrapper selected = _viewport.MainForm.SelectionManager.CurrentSelection[0];

            switch (ActiveMode)
            {
                case GizmoMode.Translate:

                    PerformTranslate(currentMouse.Vector, selected);
                    
                    break;

                case GizmoMode.Rotate:
                    if (float.IsNaN(_firstMousePosition))
                    {
                        _firstMousePosition = currentMouse.Vector.X;
                    }
                    float rotDelta = currentMouse.Vector.X - _firstMousePosition;

                    if (SnapEnabled)
                    {
                        rotDelta = (int) (rotDelta / RotationSnapValue) * RotationSnapValue;
                    }

                    Vector3 rotVector = Vector3.Zero;

                    switch (ActiveAxis)
                    {
                        case GizmoAxis.X:
                            rotVector = new Vector3(rotDelta, 0, 0);
                            break;
                        case GizmoAxis.Y:
                            rotVector = new Vector3(0, rotDelta, 0);
                            break;
                        case GizmoAxis.Z:
                            rotVector = new Vector3(0, 0, rotDelta);
                            break;
                    }
                    _rotationDelta = rotVector;
                    selected.Nut.transform.Rotation = _oldSelectedRotation + _rotationDelta;
                    break;
            }

            return true;
        }

        private void PerformTranslate(Vector2 currentMouse, GameObjectWrapper selected)
        {
            Ray ray = ConvertMouseToRay(currentMouse);

            Matrix gizmoMatrix = Matrix.Identity;
            gizmoMatrix.Forward = Vector3.Normalize(_gizmoWorld.Forward);
            gizmoMatrix.Up      = Vector3.Normalize(_gizmoWorld.Up);
            gizmoMatrix.Right   = Vector3.Normalize(_gizmoWorld.Right);

            Matrix unrotation = Matrix.Invert(gizmoMatrix);

            ray.Position  = Vector3.Transform      (ray.Position,  unrotation);
            ray.Direction = Vector3.TransformNormal(ray.Direction, unrotation);
            Plane plane;
            float? distance = null;
            Vector3 delta = Vector3.Zero;


            switch (ActiveAxis)
            {
                // TODO select plane more parallel to the projection surface
                case GizmoAxis.X:
                case GizmoAxis.XY:
                    plane = new Plane(Vector3.Forward, Vector3.Transform(_position, unrotation).Z);
                    distance = ray.Intersects(plane);
                    break;

                case GizmoAxis.Y:
                case GizmoAxis.YZ:
                    plane = new Plane(Vector3.Left, Vector3.Transform(_position, unrotation).X);
                    distance = ray.Intersects(plane);
                    break;

                case GizmoAxis.Z:
                case GizmoAxis.ZX:
                    plane = new Plane(Vector3.Down, Vector3.Transform(_position, unrotation).Y);
                    distance = ray.Intersects(plane);
                    break;
            }

            if (distance.HasValue)
            {
                _intersectPosition = (ray.Position + (ray.Direction * distance.Value));
                if (_firstIntersectionPosition == Vector3.Zero)
                {
                    _firstIntersectionPosition = _intersectPosition;
                }

                Vector3 tDelta = _intersectPosition - _firstIntersectionPosition;

                switch (ActiveAxis)
                {
                    case GizmoAxis.X:
                        delta = new Vector3(tDelta.X, 0, 0);
                        break;
                    case GizmoAxis.Y:
                        delta = new Vector3(0, tDelta.Y, 0);
                        break;
                    case GizmoAxis.Z:
                        delta = new Vector3(0, 0, tDelta.Z);
                        break;
                    case GizmoAxis.XY:
                        delta = new Vector3(tDelta.X, tDelta.Y, 0);
                        break;
                    case GizmoAxis.YZ:
                        delta = new Vector3(0, tDelta.Y, tDelta.Z);
                        break;
                    case GizmoAxis.ZX:
                        delta = new Vector3(tDelta.X, 0, tDelta.Z);
                        break;
                }
            }

            if (SnapEnabled)
            {
                float snapVal = TranslationSnapValue;
                delta = new Vector3(
                    (int)(delta.X / snapVal) * snapVal,
                    (int)(delta.Y / snapVal) * snapVal,
                    (int)(delta.Z / snapVal) * snapVal
                );
            }

            Matrix invertingMatrix = Matrix.Identity;

            if (ActiveSpace == TransformSpace.Global)
            {
                invertingMatrix = Matrix.Invert(selected.Nut.transform.AncestorsRotation);
            }
            else
            {
                invertingMatrix.Forward = Vector3.Normalize(selected.Nut.transform.WorldRotation.Forward);
                invertingMatrix.Up      = Vector3.Normalize(selected.Nut.transform.WorldRotation.Up);
                invertingMatrix.Right   = Vector3.Normalize(selected.Nut.transform.WorldRotation.Right);
            }

            // allow normal transformation when scaled
            invertingMatrix *= Matrix.Invert(selected.Nut.transform.AncestorsScale);

            _translationDelta = Vector3.Transform(delta, invertingMatrix);
            selected.Nut.transform.Position = _oldSelectedPosition + _translationDelta;
        }

        /// <summary>
        /// Checks whether mouse hovers the gizmo. Call it on mouse move (without drag).
        /// </summary>
        /// <param name="currentMouse">Current mouse state</param>
        /// <returns>true if there is collision with gizmo</returns>
        public bool MouseHover(EditorMouseState currentMouse)
        {
            if (!_isActive) return false;
            SelectAxis(currentMouse.Vector);
            return ActiveAxis != GizmoAxis.None;
        }

        public void ToggleActiveSpace()
        {
            ActiveSpace = ActiveSpace == TransformSpace.Local ? TransformSpace.Global : TransformSpace.Local;
            if (ActiveSpaceChanged != null) ActiveSpaceChanged();
            Update();
        }

        private void SelectAxis(Vector2 mousePosition)
        {
            if (!Enabled) return;

            ActiveAxis = GizmoAxis.None;
            float closestIntersection = float.MaxValue;
            Ray ray = ConvertMouseToRay(mousePosition);

            float? intersection = XAxisBox.Intersects(ray);
            if (intersection < closestIntersection)
            {
                ActiveAxis = GizmoAxis.X;
                closestIntersection = intersection.Value;
            }
            intersection = YAxisBox.Intersects(ray);
            if (intersection < closestIntersection)
            {
                ActiveAxis = GizmoAxis.Y;
                closestIntersection = intersection.Value;
            }
            intersection = ZAxisBox.Intersects(ray);
            if (intersection < closestIntersection)
            {
                ActiveAxis = GizmoAxis.Z;
                closestIntersection = intersection.Value;
            }

            //if (ActiveMode != GizmoMode.Translate)
            {
                intersection = XSphere.Intersects(ray);
                if (intersection < closestIntersection)
                {
                    ActiveAxis = GizmoAxis.X;
                    closestIntersection = intersection.Value;
                }

                intersection = YSphere.Intersects(ray);
                if (intersection < closestIntersection)
                {
                    ActiveAxis = GizmoAxis.Y;
                    closestIntersection = intersection.Value;
                }

                intersection = ZSphere.Intersects(ray);
                if (intersection < closestIntersection)
                {
                    ActiveAxis = GizmoAxis.Z;
                    closestIntersection = intersection.Value;
                }
            }

            if (ActiveMode != GizmoMode.Rotate)
            {
                if (ActiveMode == GizmoMode.Translate)
                {
                    // transform ray into local-space of the boundingboxes.
                    ray.Direction = Vector3.TransformNormal(ray.Direction, Matrix.Invert(_gizmoWorld));
                    ray.Position = Vector3.Transform(ray.Position, Matrix.Invert(_gizmoWorld));
                }

                // If no axis was hit (x,y,z) set value to lowest possible to select the 'farthest' intersection for the XY,XZ,YZ boxes. 
                // This is done so you may still select multi-axis if you're looking at the gizmo from behind
                if (closestIntersection >= float.MaxValue)
                    closestIntersection = float.MinValue;

                intersection = XYAxisBox.Intersects(ray);
                if (intersection > closestIntersection)
                {
                    ActiveAxis = GizmoAxis.XY;
                    closestIntersection = intersection.Value;
                }

                intersection = XZAxisBox.Intersects(ray);
                if (intersection > closestIntersection)
                {
                    ActiveAxis = GizmoAxis.ZX;
                    closestIntersection = intersection.Value;
                }

                intersection = YZAxisBox.Intersects(ray);
                if (intersection > closestIntersection)
                {
                    ActiveAxis = GizmoAxis.YZ;
                    closestIntersection = intersection.Value;
                }
            }

            ApplyColor(GizmoAxis.X, _axisColors[0]);
            ApplyColor(GizmoAxis.Y, _axisColors[1]);
            ApplyColor(GizmoAxis.Z, _axisColors[2]);
            ApplyColor(ActiveAxis, _highlightColor);
        }

        /// <summary>
        /// Helper method for applying color to the gizmo lines.
        /// </summary>
        private void ApplyColor(GizmoAxis axis, Color color)
        {
            switch (ActiveMode)
            {
                case GizmoMode.NonUniformScale:
                case GizmoMode.Translate:
                    switch (axis)
                    {
                        case GizmoAxis.X:
                            ApplyLineColor(0, 6, color);
                            break;
                        case GizmoAxis.Y:
                            ApplyLineColor(6, 6, color);
                            break;
                        case GizmoAxis.Z:
                            ApplyLineColor(12, 6, color);
                            break;
                        case GizmoAxis.XY:
                            ApplyLineColor(0, 4, color);
                            ApplyLineColor(6, 4, color);
                            break;
                        case GizmoAxis.YZ:
                            ApplyLineColor(6, 2, color);
                            ApplyLineColor(12, 2, color);
                            ApplyLineColor(10, 2, color);
                            ApplyLineColor(16, 2, color);
                            break;
                        case GizmoAxis.ZX:
                            ApplyLineColor(0, 2, color);
                            ApplyLineColor(4, 2, color);
                            ApplyLineColor(12, 4, color);
                            break;
                    }
                    break;
                case GizmoMode.Rotate:
                    switch (axis)
                    {
                        case GizmoAxis.X:
                            ApplyLineColor(0, 6, color);
                            break;
                        case GizmoAxis.Y:
                            ApplyLineColor(6, 6, color);
                            break;
                        case GizmoAxis.Z:
                            ApplyLineColor(12, 6, color);
                            break;
                    }
                    break;
                case GizmoMode.UniformScale:
                    ApplyLineColor(0, _translationLineVertices.Length, ActiveAxis == GizmoAxis.None ? _axisColors[3] : _highlightColor);
                    break;
            }
        }

        /// <summary>
        /// Apply color on the lines associated with translation mode (re-used in Scale)
        /// </summary>
        private void ApplyLineColor(int startindex, int count, Color color)
        {
            for (int i = startindex; i < (startindex + count); i++)
            {
                _translationLineVertices[i].Color = color;
            }
        }


        private Ray ConvertMouseToRay(Vector2 mousePosition)
        {
            Vector3 nearPoint = new Vector3(mousePosition, 0);
            Vector3 farPoint  = new Vector3(mousePosition, 1);

            nearPoint = _graphics.Viewport.Unproject(nearPoint, _projection, _view, Matrix.Identity);
            farPoint  = _graphics.Viewport.Unproject(farPoint,  _projection, _view, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

    }
}
