using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

internal class PreviewShopBag : MonoBehaviour
{
	[SerializeField]
	private BagTooltipTrigger _bagTooltipTrigger;

	[SerializeField]
	private Image _image;

	internal void Init(BagSO bagSO)
	{
		SetImage(bagSO.BackpackImage);
		new BagInstance(bagSO);
		_bagTooltipTrigger.SetBag(bagSO, active: true, Enums.Backpack.DraggableOwner.Shop);
	}

	internal void SetImage(Sprite sprite)
	{
		_image.sprite = sprite;
	}
}
