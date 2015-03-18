using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Renderer : Component
    {
        #region Variables
        #region Private
        private Mesh _myMesh;
        #endregion
        #endregion  

        #region Properites
        public Mesh MyMesh
        {
            get 
            {
                return _myMesh;
            }
            set
            {
                _myMesh = value;
            }
        }
        #endregion

        #region Methods
        public Renderer(GameObject owner) : base(owner)
        {
            _myMesh = null;
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Draw()
        {
            _myMesh.Draw();
        }
        #endregion
    }
}
