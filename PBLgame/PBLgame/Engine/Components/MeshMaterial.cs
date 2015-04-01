using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PBLgame.Engine.Components
{
    public class MeshMaterial
    {
        public int Id { get; set; }
        public Texture2D Diffuse { get; set; }
        public Texture2D Normal { get; set; }
        public Texture2D Specular { get; set; }
        public Texture2D Emissive { get; set; }
        public Effect ShaderEffect { get; set; }

        public MeshMaterial(int id, Texture2D diffuse, Texture2D normal, Texture2D specular, Texture2D emissive, Effect shaderEffect)
        {
            Id = id;
            Diffuse = diffuse;
            Normal = normal;
            Specular = specular;
            Emissive = emissive;
            ShaderEffect = shaderEffect;
        }
    }
}
