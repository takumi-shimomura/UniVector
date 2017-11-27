using UnityEngine;

namespace VectorScan
{
	[RequireComponent (typeof(MeshFilter))]
	public class VectorFilter : MonoBehaviour
	{
		[SerializeField]
		BrightnessMapProvider brightnessMapProvider;

		[SerializeField]
		Mesh mesh;
	}
}

