using System;
using UnityEngine;

namespace GeometricUtilities
{
	public struct TVertex: IEquatable<TVertex>
	{
		public Vector3 Vertex { get; set; }
		public Vector3 Normal { get; set; }
		public Vector2 UV { get; set; }

		public bool Equals(TVertex other)
		{
			return Vertex.Equals(other.Vertex);
		}
	}
}
