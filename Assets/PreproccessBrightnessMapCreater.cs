using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreproccessBrightnessMapCreater : MonoBehaviour
{
	[SerializeField]
	AnimationCurve powerCurve;

	Texture2D CreateBrightnessMap (int widthResolution, int integralResolution)
	{
		var powerMap = new float[widthResolution, integralResolution];
		var pixels = new Color[widthResolution * integralResolution];

		var reverseWidth = 1.0f / (float)(widthResolution - 1);
		var reverseIntengral = 1.0f / (float)(integralResolution - 1);
		for (int y = 0; y < integralResolution; ++y) {
			for (int x = 0; x < widthResolution; ++x) {
				var distance = (new Vector2 (x * reverseWidth, y * reverseIntengral * 2 - 1)).magnitude;
				var power = Mathf.Max (0.0f, powerCurve.Evaluate (distance) * reverseIntengral);
				if (y > 0) {
					power += powerMap [x, y - 1];
				}
				powerMap [x, y] = power;
				pixels [x + y * widthResolution] = new Color {
					r = power,
				};
			}
		}

		var texture = new Texture2D (widthResolution, integralResolution, TextureFormat.RFloat, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		texture.anisoLevel = 0;
		texture.SetPixels (pixels);
		texture.Apply ();

		return texture;
	}

	Texture2D CreateBalanceMap (int widthResolution, int integralResolution, float maxLength = 8)
	{
		var pixels = new Color[widthResolution * integralResolution];

		var reverseWidth = 1.0f / (float)(widthResolution - 1);
		var reverseIntengral = 1.0f / (float)(integralResolution - 1);
		for (int y = 0; y < integralResolution; ++y) {
			var length = y * maxLength * reverseIntengral;
			for (int x = 0; x < widthResolution; ++x) {
				var xtime = x * reverseWidth;
				float factor = 0.0f;
				float division = 0.0f;
				for (int i = 0; i < 101; ++i) {
					var a = i * 0.01f;
					var pos1 = xtime * (length + 2) - 1;
					var pos2 = a * length;
					var distance = Mathf.Abs (pos1 - pos2);
					var power = Mathf.Max (0.0f, powerCurve.Evaluate (distance));
					factor += power * a;
					division += power;
				}
				pixels [x + y * widthResolution] = new Color {
					r = division > 0 ? factor / division : Mathf.Sign (xtime - 0.5f),
				};
			}
		}
		// yが

		var texture = new Texture2D (widthResolution, integralResolution, TextureFormat.RFloat, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		texture.anisoLevel = 0;
		texture.SetPixels (pixels);
		texture.Apply ();

		return texture;
	}

	Mesh CreateWaveMesh ()
	{
		var mesh = new Mesh ();
		var vertices = new List<Vector3> ();
		var indices = new List<int> ();

		for (int i = 0; i < 101; ++i) {
			var t = (float)i * 0.01f;
			var v = 0.0f;
			v += Mathf.Sin (t * Mathf.PI * 2) * 0.7f;
			v += Mathf.Sin (t * Mathf.PI * 4.2f + 3) * 0.5f;
			v += Mathf.Sin (t * Mathf.PI * 7.9f + 5.1f) * 0.35f;
			vertices.Add (new Vector3 (t * 2, v, 0));
			indices.Add (i);
		}
		mesh.SetVertices (vertices);
		mesh.SetIndices (indices.ToArray (), MeshTopology.LineStrip, 0);

		return mesh;
	}

	#if UNITY_EDITOR
	[ContextMenu ("Create BrightnessMap")]
	void CreateBrightnessMapMenu ()
	{
		var path = UnityEditor.EditorUtility.SaveFilePanelInProject ("保存先", "", "asset", "");
		var texture = CreateBrightnessMap (128, 128);
		UnityEditor.AssetDatabase.CreateAsset (texture, path);
	}

	[ContextMenu ("Create BalanceMap")]
	void CreateBalanceMapMenu ()
	{
		var path = UnityEditor.EditorUtility.SaveFilePanelInProject ("保存先", "", "asset", "");
		var texture = CreateBalanceMap (128, 128);
		UnityEditor.AssetDatabase.CreateAsset (texture, path);
	}

	[ContextMenu ("Create WaveMesh")]
	void CreateWaveMeshMenu ()
	{
		var path = UnityEditor.EditorUtility.SaveFilePanelInProject ("保存先", "", "asset", "");
		var texture = CreateWaveMesh ();
		UnityEditor.AssetDatabase.CreateAsset (texture, path);
	}
	#endif
}
