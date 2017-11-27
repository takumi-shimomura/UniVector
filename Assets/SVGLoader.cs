using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AssemblyCSharp
{
	public class SVGLoader : MonoBehaviour
	{
		[SerializeField]
		TextAsset svg;

		[ContextMenu ("Load")]
		void Start ()
		{
			var xml = new XmlDocument ();
			xml.LoadXml (svg.text);

			var graphicNode = xml ["svg"] ["g"];
			var graph = graphicNode.FirstChild;
			while (graph != null) {
				if (graph.Name == "polygon") {
					Debug.Log ("polygon fill:" + graph.Attributes ["fill"].Value + " points:" + graph.Attributes ["points"].Value);
				}
				if (graph.Name == "path") {
					var data = graph.Attributes ["d"].Value;
					Debug.Log ("path fill:" + graph.Attributes ["fill"].Value + " d:" + data);
					var ms = Regex.Matches (data, "([a-zA-Z])([^a-zA-Z]*)");
					foreach (var m in ms.Cast<Match> ()) {
						Debug.Log (m.Groups [1].Value + ":" + m.Groups [2].Value);
					}

				}
				graph = graph.NextSibling;
			}


		}
	}
}

