using BackpackSurvivors.ScriptableObjects.CraftingResource;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.CraftingResources;

internal class CraftingResourceVisualItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _amountText;

	[SerializeField]
	private Image _iconImage;

	[SerializeField]
	private CraftingResourceTooltipTrigger _craftingResourceTooltipTrigger;

	private CraftingResourceSO _craftingResourceSO;

	private int _currentAmount;

	internal Enums.CraftingResource CraftingResource => _craftingResourceSO.CraftingResource;

	public void Init(CraftingResourceSO craftingResource, int amount)
	{
		_iconImage.sprite = craftingResource.Icon;
		_currentAmount = amount;
		_amountText.SetText($"{amount}");
		_craftingResourceSO = craftingResource;
		_craftingResourceTooltipTrigger.SetCraftingResource(craftingResource, amount);
	}

	public void UpdateValue(int amount)
	{
		_currentAmount = amount;
		_amountText.SetText($"{amount}");
		_craftingResourceTooltipTrigger.UpdateAmount(amount);
	}
}
