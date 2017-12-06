using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
	public class GraphMeshGenerator : MonoBehaviour
	{
		[SerializeField]
		AnimationCurve xCurve;

		[SerializeField]
		AnimationCurve yCurve;

		[SerializeField, Range (1, 10)]
		float xPower = 1;

		[SerializeField, Range (1, 10)]
		float yPower = 1;

		[SerializeField, Range (1, 10)]
		float timeLength = 1;

		[SerializeField, Range (1, 100)]
		float timeScale = 1;

		[SerializeField]
		float stepTime = 0.01f;

		[SerializeField]
		MeshFilter meshFilter;

		List<Vector3> points = new List<Vector3> ();
		List<Color32> colors = new List<Color32> ();
		List<int> indices = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();
		Mesh mesh;

		void OnValidate ()
		{
			stepTime = Mathf.Max (stepTime, timeLength * 0.001f);

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
			for (float time = 0; time < timeLength; time += stepTime) {
				var time_ = time;
				var x = xCurve.Evaluate (time_) * xPower;
				var y = yCurve.Evaluate (time_) * yPower;
				points.Add (new Vector3 (x, y));
				indices.Add (points.Count - 1);
				colors.Add (Color.white);
				uvs.Add (new Vector2 (stepTime * timeScale, 0));
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

