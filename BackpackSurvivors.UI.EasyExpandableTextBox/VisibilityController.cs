using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.EasyExpandableTextBox;

[ExecuteAlways]
public class VisibilityController : MonoBehaviour
{
	private void Awake()
	{
		base.transform.GetChild(0).hideFlags = HideFlags.HideInHierarchy;
		GetComponent<VisibilityController>().hideFlags = HideFlags.HideInInspector;
		GetComponent<VerticalLayoutGroup>().hideFlags = HideFlags.HideInInspector;
	}

	private void Update()
	{
	}
}
