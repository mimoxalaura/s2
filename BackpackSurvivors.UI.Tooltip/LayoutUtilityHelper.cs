using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public static class LayoutUtilityHelper
{
	public static void ForceRebuildAllLayouts(GameObject root)
	{
		List<RectTransform> list = new List<RectTransform>();
		LayoutGroup[] componentsInChildren = root.GetComponentsInChildren<LayoutGroup>(includeInactive: true);
		foreach (LayoutGroup layoutGroup in componentsInChildren)
		{
			list.Add(layoutGroup.GetComponent<RectTransform>());
		}
		list.Sort((RectTransform a, RectTransform b) => GetHierarchyDepth(b).CompareTo(GetHierarchyDepth(a)));
		foreach (RectTransform item in list)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(item);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(root.GetComponent<RectTransform>());
	}

	private static int GetHierarchyDepth(Transform t)
	{
		int num = 0;
		while (t.parent != null)
		{
			num++;
			t = t.parent;
		}
		return num;
	}
}
