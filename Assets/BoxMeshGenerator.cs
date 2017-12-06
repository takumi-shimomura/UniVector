using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
	public class BoxMeshGenerator : MonoBehaviour
	{
		[SerializeField]
		float size = 5;

		[SerializeField, Range (1, 10)]
		float time = 1;

		[SerializeField]
		MeshFilter meshFilter;

		List<Vector3> points = new List<Vector3> ();
		List<Color32> colors = new List<Color32> ();
		List<int> indices = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();
		Mesh mesh;

		void OnValidate ()
		{
			if (!meshFilter) {
				return;
			}

			if (!mesh) {
				mesh = new Mesh ();
			}

			points.Clear ();
			colors.Clear ();
			indices.Clear ();
			uvs.Clear ();

			// 外周
			points.Add (new Vector3 (-size, -size));
			points.Add (new Vector3 (-size, -size));
			points.Add (new Vector3 (-size, size));
			points.Add (new Vector3 (-size, size));
			points.Add (new Vector3 (size, size));
			points.Add (new Vector3 (size, size));
			points.Add (new Vector3 (size, -size));
			points.Add (new Vector3 (size, -size));
			points.Add (new Vector3 (-size, -size));

			for (int i = 0; i < points.Count; ++i) {
				indices.Add (i);
				colors.Add (Color.white);
				var thisTime = time;
				if (i % 2 == 0) {
//					thisTime *= 0.5f;
				}
				uvs.Add (new Vector2 (thisTime, thisTime));
			}

			mesh.Clear ();
			mesh.SetVertices (points);
			mesh.SetColors (colors);
			mesh.SetUVs (0, uvs);
			mesh.SetIndices (indices.ToArray (), MeshTopology.LineStrip, 0);

			meshFilter.mesh = mesh;
		}

		void AddStrip (params Vector2[] stripPoints)
		{
			AddStrip (Color.white, stripPoints);
		}

		void AddStrip (Color32 color, params Vector2[] stripPoints)
		{
			var index = points.Count;
			for (int i = 0; i < stripPoints.Length; ++i) {
				var p = stripPoints [i];
				points.Add (new Vector3 (p.x, p.y, 0));
				colors.Add (color);
			}
			for (int i = 0; i < stripPoints.Length - 1; ++i) {
				indices.Add (index + i);
				indices.Add (index + i + 1);
			}
		}
	}
}

