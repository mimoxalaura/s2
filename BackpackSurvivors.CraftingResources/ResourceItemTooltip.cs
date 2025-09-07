using BackpackSurvivors.ScriptableObjects.CraftingResource;
using BackpackSurvivors.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.CraftingResources;

internal class ResourceItemTooltip : BaseTooltip
{
	[Header("Visuals")]
	[SerializeField]
	private Image _iconImage;

	[SerializeField]
	private TextMeshProUGUI _descriptionText;

	[SerializeField]
	private TextMeshProUGUI _location;

	[SerializeField]
	private TextMeshProUGUI _amountText;

	public void SetResourceItem(CraftingResourceSO craftingResourceSO, int amount)
	{
		_iconImage.sprite = craftingResourceSO.Icon;
		SetText(craftingResourceSO.Description ?? "", craftingResourceSO.Name);
		if (craftingResourceSO.LevelSource != null)
		{
			_location.SetText(craftingResourceSO.LevelSource.LevelName);
		}
		else
		{
			_location.SetText("???");
		}
		_amountText.SetText($"x{amount}");
	}

	internal void RefreshUI()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)base.transform);
		Canvas.ForceUpdateCanvases();
	}
}
