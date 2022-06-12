using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LSEngine
{
    internal class Material
    {
        public Texture DiffuseMap;
        public Texture SpecularMap;
        public Texture NormalMap;

        public Vector3 AmbientCoefficient;
        public Vector3 DiffuseCoefficient;
        public Vector3 SpecularCoefficient;
        public float SpecularExponent;


        public string Name { get; set; }

        public Material(string name, Texture diffuseMap = null, Texture specularMap = null, Texture normalMap = null)
        {
            Name = name;
            DiffuseMap = diffuseMap;
            SpecularMap = specularMap;
            NormalMap = normalMap;
        }
    }
}
