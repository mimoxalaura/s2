using UnityEngine;

namespace BackpackSurvivors.UI.Tooltip;

public class TooltipTagLine : MonoBehaviour
{
	[SerializeField]
	private Transform _tagLineContainer;

	internal void Clear()
	{
		for (int num = _tagLineContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(_tagLineContainer.GetChild(num).gameObject);
		}
	}

	internal void ReverseOrder()
	{
		for (int i = 0; i < base.transform.childCount - 1; i++)
		{
			base.transform.GetChild(0).SetSiblingIndex(base.transform.childCount - 1 - i);
		}
	}
}
