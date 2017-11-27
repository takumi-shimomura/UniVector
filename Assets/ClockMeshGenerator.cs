using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ClockMeshGenerator : MonoBehaviour
	{
		[SerializeField]
		float radius = 10;

		[SerializeField]
		Vector2 hourHandSize = new Vector2 (1.5f, 6);

		[SerializeField]
		Vector2 minuiteHandSize = new Vector2 (1, 8);

		[SerializeField]
		float secondHandLength = 9.0f;

		[SerializeField]
		float hourScaleLength = 0.8f;

		[SerializeField]
		float minuiteScaleLength = 0.3f;

		[SerializeField]
		Color32 hourHandColor = Color.white;

		[SerializeField]
		Color32 minuiteHandColor = Color.white;

		[SerializeField]
		Color32 secondHandColor = Color.white;

		[SerializeField]
		MeshFilter meshFilter;

		public List<Vector3> points = new List<Vector3> ();
		public List<Color32> colors = new List<Color32> ();
		public List<int> indices = new List<int> ();
		Mesh mesh;

		void Update ()
		{
			if (!mesh) {
				mesh = new Mesh ();
			}

			points.Clear ();
			colors.Clear ();
			indices.Clear ();

			// 外周
			var aroundPoints = new Vector2[61];
			for (int i = 0; i < 61; ++i) {
				var r = Mathf.PI * 2 * (float)i / 60;
				aroundPoints [i] = new Vector2 (Mathf.Sin (r) * radius, Mathf.Cos (r) * radius);
			}
			AddStrip (aroundPoints);

			// 目盛り
			for (int i = 0; i < 60; ++i) {
				var r = Mathf.PI * 2 * (float)i / 60;
				var l = i % 5 == 0 ? hourScaleLength : minuiteScaleLength;
				var v = new Vector2 (Mathf.Sin (r), Mathf.Cos (r));
				AddStrip (v * radius, v * (radius - l));
			}

			var time = System.DateTime.Now;
			var secondTime = (float)time.Second / 60;
			var minuiteTime = ((float)time.Minute + secondTime) / 60;
			var hourTime = ((float)time.Hour + minuiteTime) / 12;

			// 時針
			{
				var r = Mathf.PI * 2 * hourTime;
				var up = new Vector2 (Mathf.Sin (r), Mathf.Cos (r));
				var right = new Vector2 (up.y, -up.x);
				var p1 = up * hourHandSize.y;
				var p2 = right * hourHandSize.x * 0.5f;
				var p3 = -up * hourHandSize.y * 0.3f;
				var p4 = -right * hourHandSize.x * 0.5f;
				AddStrip (hourHandColor, p1, p2, p3, p4, p1);
			}
			// 分針
			{
				var r = Mathf.PI * 2 * minuiteTime;
				var up = new Vector2 (Mathf.Sin (r), Mathf.Cos (r));
				var right = new Vector2 (up.y, -up.x);
				var p1 = up * minuiteHandSize.y;
				var p2 = right * minuiteHandSize.x * 0.5f;
				var p3 = -up * minuiteHandSize.y * 0.3f;
				var p4 = -right * minuiteHandSize.x * 0.5f;
				AddStrip (minuiteHandColor, p1, p2, p3, p4, p1);
			}
			// 秒針
			{
				var r = Mathf.PI * 2 * secondTime;
				var up = new Vector2 (Mathf.Sin (r), Mathf.Cos (r));
				var p1 = up * secondHandLength;
				var p2 = -up * secondHandLength * 0.3f;
				AddStrip (secondHandColor, p1, p2);
			}

			mesh.Clear ();
			mesh.SetVertices (points);
			mesh.SetColors (colors);
			mesh.SetIndices (indices.ToArray (), MeshTopology.Lines, 0);

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

