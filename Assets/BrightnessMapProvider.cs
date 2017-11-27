using UnityEngine;

namespace VectorScan
{
	public class BrightnessMapProvider : MonoBehaviour
	{
		[SerializeField]
		int widthResolution = 128;

		[SerializeField]
		int integralResolution = 128;

		[SerializeField]
		AnimationCurve powerCurve;

		Texture2D brightnessMap;

		public int WidthResolution {
			get { return widthResolution; }
			set {
				value = Mathf.Max (value, 2);
				if (value != widthResolution) {
					widthResolution = value;
					SetDirty ();
				}
			}
		}

		public int IntegralResolution {
			get { return integralResolution; }
			set {
				value = Mathf.Max (value, 2);
				if (value != integralResolution) {
					integralResolution = value;
					SetDirty ();
				}
			}
		}

		public AnimationCurve PowerCurve {
			get { return new AnimationCurve (powerCurve.keys); }
			set {
				powerCurve.keys = value.keys;
				SetDirty ();
			}
		}

		public Texture2D GetBrightnessMap ()
		{
			if (!brightnessMap) {
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

				brightnessMap = new Texture2D (widthResolution, integralResolution, TextureFormat.RFloat, false);
				brightnessMap.wrapMode = TextureWrapMode.Clamp;
				brightnessMap.filterMode = FilterMode.Bilinear;
				brightnessMap.anisoLevel = 0;
				brightnessMap.SetPixels (pixels);
				brightnessMap.Apply ();
			}

			return brightnessMap;
		}

		[ContextMenu ("Rebake")]
		public void SetDirty ()
		{
			brightnessMap = null;
		}

		#if UNITY_EDITOR

		void OnValidate ()
		{
			WidthResolution = widthResolution;
			IntegralResolution = integralResolution;
		}

		#endif
	}
}

