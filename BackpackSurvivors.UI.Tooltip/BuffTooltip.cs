using BackpackSurvivors.ScriptableObjects.Buffs;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class BuffTooltip : BaseTooltip
{
	[Header("Visuals")]
	[SerializeField]
	private Image _backgroundImage;

	public void SetBuff(BuffSO buffSO, float remainingTime)
	{
		SetText(buffSO.Description, buffSO.Name);
	}

	public void UpdateDescription(BuffSO buffSO, float remainingTime)
	{
		if (buffSO.TooltipShowsDuration)
		{
			int num = (int)remainingTime;
			string text = ((num > 0) ? (num + ".") : "0.");
			int startIndex = 2;
			if (num > 9)
			{
				startIndex = 3;
			}
			if (num > 99)
			{
				startIndex = 4;
			}
			string text2 = ((remainingTime - (float)num > 0f) ? (remainingTime - (float)num).ToString().Substring(startIndex, 2) : string.Empty);
			SetText(buffSO.Description, buffSO.Name + " (" + text + text2 + ")");
		}
		else
		{
			SetText(buffSO.Description, buffSO.Name ?? "");
		}
	}

	internal void RefreshUI()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)base.transform);
		Canvas.ForceUpdateCanvases();
	}
}
