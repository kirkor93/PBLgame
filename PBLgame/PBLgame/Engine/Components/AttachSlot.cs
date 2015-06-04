using Microsoft.Xna.Framework;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class AttachSlot : Component
    {
        private GameObject _owner;
        private GameObject _attachment;
        private Bone _slot;

        public GameObject Owner
        {
            get { return _owner; }
        }

        public GameObject Attachment
        {
            get { return _attachment; }
        }

        public ExtraTransform AttachmentTransform
        {
            get { return (ExtraTransform) _attachment.transform; }
        }

        public Bone Slot
        {
            get { return _slot; }
        }

        public AttachSlot(GameObject owner, GameObject attachment, string boneName) : base(owner)
        {
            _owner = owner;
            _slot = owner.animator.AnimMesh.FindBone(boneName);
            _attachment = attachment;
            _attachment.transform = new ExtraTransform(attachment.transform);
        }

        public override void Update(GameTime gameTime)
        {
            Matrix slotTransform = _slot.AbsoluteTransform;
            
            slotTransform = Matrix.CreateWorld(
                slotTransform.Translation,
                slotTransform.Forward,
                slotTransform.Up
            );

            AttachmentTransform.PreLocalWorld = slotTransform;
        }
    }
}