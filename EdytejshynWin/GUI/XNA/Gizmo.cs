using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private bool _isActive = true;

        private readonly GraphicsDevice _graphics;
        private readonly SpriteBatch _spriteBatch;
        private readonly BasicEffect _lineEffect;
        private readonly BasicEffect _meshEffect;
        private readonly BasicEffect _selectionBoxEffect;
        private List<VertexPositionColor> _selectionBoxVertices = new List<VertexPositionColor>();
        private readonly SpriteFont _font;

        private Matrix _view = Matrix.Identity;
        private Matrix _projection = Matrix.Identity;
        private Vector3 _cameraPosition;

        // -- Screen Scale -- //
        private Matrix _screenScaleMatrix;
        private float _screenScale;

        // -- Position - Rotation -- //
        private Vector3 _position = Vector3.Zero;
        private Matrix _rotationMatrix = Matrix.Identity;

        private Vector3 _localForward = Vector3.Forward;
        private Vector3 _localUp = Vector3.Up;
        private Vector3 _localRight;

        // -- Matrices -- //
        private Matrix _objectOrientedWorld;
        private Matrix _axisAlignedWorld;
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

        #region BoundingSpheres

        private const float RADIUS = 1f;
        private BoundingSphere XSphere { get { return new BoundingSphere(Vector3.Transform(_translationLineVertices[1] .Position, _gizmoWorld), RADIUS * _screenScale); } }
        private BoundingSphere YSphere { get { return new BoundingSphere(Vector3.Transform(_translationLineVertices[7] .Position, _gizmoWorld), RADIUS * _screenScale); } }
        private BoundingSphere ZSphere { get { return new BoundingSphere(Vector3.Transform(_translationLineVertices[13].Position, _gizmoWorld), RADIUS * _screenScale); } }

        #endregion


        /// <summary>
        /// The value to adjust all transformation when precisionMode is active.
        /// </summary>
        private const float PRECISION_MODE_SCALE = 0.1f;

        // -- Selection -- //
        //public List<ITransformable> Selection = new List<ITransformable>();
        //private IEnumerable<ITransformable> _selectionPool = null;

        // Delta values of transformation for every update
        private Vector3 _translationDelta = Vector3.Zero;
        private Matrix _rotationDelta = Matrix.Identity;
        private Vector3 _scaleDelta = Vector3.Zero;

        // -- Translation Variables -- //
        private Vector3 _tDelta;
        private Vector3 _lastIntersectionPosition;
        private Vector3 _intersectPosition;

        private Vector3 _translationScaleSnapDelta;
        private float _rotationSnapDelta;

        #endregion

        /// <summary>
        /// Enabled if gizmo should be able to select objects and axis.
        /// </summary>
        public bool Enabled { get; set; }
        public bool SelectionBoxesIsVisible = true;
        public bool SnapEnabled = false;
        public bool PrecisionModeEnabled;
        public float TranslationSnapValue = 5;
        public float RotationSnapValue = 30;
        public float ScaleSnapValue = 0.5f;

        #endregion

        public Gizmo(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font)
        {
            ZAxisBox = new BoundingBox(new Vector3(0, 0, LINE_OFFSET), new Vector3(SINGLE_AXIS_THICKNESS, SINGLE_AXIS_THICKNESS, LINE_OFFSET + LINE_LENGTH));
            YAxisBox = new BoundingBox(new Vector3(0, LINE_OFFSET, 0), new Vector3(SINGLE_AXIS_THICKNESS, LINE_OFFSET + LINE_LENGTH, SINGLE_AXIS_THICKNESS));
            XAxisBox = new BoundingBox(new Vector3(LINE_OFFSET, 0, 0), new Vector3(LINE_OFFSET + LINE_LENGTH, SINGLE_AXIS_THICKNESS, SINGLE_AXIS_THICKNESS));
            YZAxisBox = new BoundingBox(Vector3.Zero, new Vector3(MULTI_AXIS_THICKNESS, LINE_OFFSET, LINE_OFFSET));
            XYAxisBox = new BoundingBox(Vector3.Zero, new Vector3(LINE_OFFSET, LINE_OFFSET, MULTI_AXIS_THICKNESS));
            XZAxisBox = new BoundingBox(Vector3.Zero, new Vector3(LINE_OFFSET, MULTI_AXIS_THICKNESS, LINE_OFFSET));

            SceneWorld = Matrix.Identity;
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _font = font;

            Enabled = true;

            _selectionBoxEffect = new BasicEffect(graphics)
            {
                VertexColorEnabled = true
            };
            _lineEffect = new BasicEffect(graphics)
            {
                VertexColorEnabled = true,
                AmbientLightColor = Vector3.One,
                EmissiveColor = Vector3.One
            };
            _meshEffect = new BasicEffect(graphics);
            _quadEffect = new BasicEffect(graphics)
            {
                World = Matrix.Identity,
                DiffuseColor = _highlightColor.ToVector3(),
                Alpha = 0.5f
            };
            _quadEffect.EnableDefaultLighting();

            //Initialize();
        }
    }
}
