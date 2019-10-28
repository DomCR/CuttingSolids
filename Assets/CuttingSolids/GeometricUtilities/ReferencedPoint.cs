using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CuttingSolids.GeometricUtilities
{
    public class ReferencedPoint
    {
        public Vector3 Original { get; set; }
        public Vector2 Reference { get; set; }

        public ReferencedPoint(Vector3 original, Vector3 uVect, Vector3 vVect)
        {
            this.Original = original;
            this.Reference = new Vector2(Vector3.Dot(Original, uVect), Vector3.Dot(Original, vVect));
        }
    }
}
