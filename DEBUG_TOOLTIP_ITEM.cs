using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

public class DEBUG_TOOLTIP_ITEM : MonoBehaviour
{
	[SerializeField]
	private ItemSO itemSO;

	[SerializeField]
	private ItemTooltipTrigger itemTooltipTrigger;

	private void Start()
	{
		ItemInstance itemInstance = new ItemInstance(itemSO);
		itemTooltipTrigger.SetItemContent(itemInstance, Enums.Backpack.DraggableOwner.Player);
	}
}
