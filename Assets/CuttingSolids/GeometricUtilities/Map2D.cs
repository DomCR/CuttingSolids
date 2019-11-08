using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CuttingSolids.GeometricUtilities
{
    public class Map2D
    {
        public Plane Map { get; set; }
        public Vector3 UVect { get; set; }
        public Vector3 VVect { get; set; }

        public Map2D(Plane plane)
        {
            Map = plane;

            //Set 2 plane vectors to get the reference
            UVect = Vector3.Normalize(Vector3.Cross(Map.normal, Vector3.up));
            //In case is the same vector
            if (Vector3.zero == UVect)
            {
                //Change the default vector
                UVect = Vector3.Normalize(Vector3.Cross(Map.normal, Vector3.forward));
            }
            //Get the second from the first one
            VVect = Vector3.Cross(UVect, Map.normal);
        }
    }
}
