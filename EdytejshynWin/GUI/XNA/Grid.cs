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
// customized for Edytejshyn

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.GUI.XNA
{
    public class Grid
    {
        #region Variables

        private ViewportControl _viewport;
        private BasicEffect _effect;
        private readonly GraphicsDevice _graphics;
        private VertexPositionColor[] _vertexData;

        private readonly Color _lineColor = Color.SkyBlue;
        private readonly Color _highlightColor = Color.White;

        /// <summary>
        /// Number of lines in total.
        /// </summary>
        private int _nrOfLines;
        private int _spacing;
        private int _gridSize;

        public bool Enabled = true;

        public int GridSpacing
        {
            get { return _spacing; }
            set
            {
                _spacing = value;
                ResetLines();
            }
        }

        /// <summary>
        /// Size of grid: _gridSize x _gridSize.
        /// </summary>

        public int GridSize
        {
            get { return _gridSize; }
            set
            {
                _gridSize = value;
                ResetLines();
            }
        }

        #endregion

        public Grid(ViewportControl viewport, int gridspacing, int gridSize)
        {
            _viewport = viewport;
             _graphics = _viewport.GraphicsDevice;
            _effect = new BasicEffect(_graphics);
            _effect.VertexColorEnabled = true;
            _effect.World = Matrix.Identity;

            _spacing = gridspacing;
            _gridSize = gridSize;

            ResetLines();
        }

        public void ResetLines()
        {
            // calculate nr of lines, +2 for the highlights, +12 for boundingbox
            _nrOfLines = ((_gridSize / _spacing) * 4) + 2;

            List<VertexPositionColor> vertexList = new List<VertexPositionColor>(_nrOfLines);

            // fill array
            for (int i = 1; i < (_gridSize / _spacing) + 1; i++)
            {
                vertexList.Add(new VertexPositionColor(new Vector3((i * _spacing), 0, _gridSize), _lineColor));
                vertexList.Add(new VertexPositionColor(new Vector3((i * _spacing), 0, -_gridSize), _lineColor));

                vertexList.Add(new VertexPositionColor(new Vector3((-i * _spacing), 0, _gridSize), _lineColor));
                vertexList.Add(new VertexPositionColor(new Vector3((-i * _spacing), 0, -_gridSize), _lineColor));

                vertexList.Add(new VertexPositionColor(new Vector3(_gridSize, 0, (i * _spacing)), _lineColor));
                vertexList.Add(new VertexPositionColor(new Vector3(-_gridSize, 0, (i * _spacing)), _lineColor));

                vertexList.Add(new VertexPositionColor(new Vector3(_gridSize, 0, (-i * _spacing)), _lineColor));
                vertexList.Add(new VertexPositionColor(new Vector3(-_gridSize, 0, (-i * _spacing)), _lineColor));
            }

            // add highlights
            vertexList.Add(new VertexPositionColor(Vector3.Forward * _gridSize, _highlightColor));
            vertexList.Add(new VertexPositionColor(Vector3.Backward * _gridSize, _highlightColor));

            vertexList.Add(new VertexPositionColor(Vector3.Right * _gridSize, _highlightColor));
            vertexList.Add(new VertexPositionColor(Vector3.Left * _gridSize, _highlightColor));


            // add boundingbox
            //BoundingBox box = new BoundingBox(new Vector3(-_gridSize, -_gridSize, -_gridSize), new Vector3(_gridSize, _gridSize, _gridSize));
            //Vector3[] corners = new Vector3[8];

            //box.GetCorners(corners);
            //vertexList.Add(new VertexPositionColor(corners[0], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[1], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[0], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[3], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[0], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[4], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[1], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[2], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[1], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[5], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[2], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[3], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[2], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[6], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[3], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[7], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[4], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[5], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[4], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[7], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[5], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[6], _lineColor));

            //vertexList.Add(new VertexPositionColor(corners[6], _lineColor));
            //vertexList.Add(new VertexPositionColor(corners[7], _lineColor));


            // convert to array for drawing
            _vertexData = vertexList.ToArray();
        }

        public void Draw()
        {
            if (!Enabled) return;
            //_graphics.DepthStencilState = DepthStencilState.Default;

            _effect.View = Camera.MainCamera.ViewMatrix;
            _effect.Projection = Camera.MainCamera.ProjectionMatrix;

            _effect.CurrentTechnique.Passes[0].Apply();
            _graphics.DrawUserPrimitives(PrimitiveType.LineList, _vertexData, 0, _nrOfLines);
        }
    }
}
