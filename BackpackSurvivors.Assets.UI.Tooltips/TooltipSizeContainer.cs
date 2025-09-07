using BackpackSurvivors.ScriptableObjects.Backpack;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Tooltips;

internal class TooltipSizeContainer : MonoBehaviour
{
	[SerializeField]
	private Image[] _itemSizeSlots;

	[SerializeField]
	private Sprite _slotFilledSprite;

	[SerializeField]
	private Sprite _slotStarredSprite;

	[SerializeField]
	private Sprite _slotNegativeStarredSprite;

	internal void SetSize(ItemSizeSO itemsizeSO, bool starIsPositive)
	{
		int num = 5;
		int num2 = 10 - num;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num3 = i * 10 + j;
				int num4 = num3 - i * num2;
				if (num4 >= num * num)
				{
					continue;
				}
				switch (itemsizeSO.SizeInfo[num3])
				{
				case Enums.Backpack.ItemSizeCellType.None:
					_itemSizeSlots[num4].sprite = null;
					_itemSizeSlots[num4].color = new Color(0f, 0f, 0f, 0f);
					break;
				case Enums.Backpack.ItemSizeCellType.CellContainsPlacable:
					_itemSizeSlots[num4].sprite = _slotFilledSprite;
					_itemSizeSlots[num4].color = new Color(255f, 255f, 255f, 1f);
					break;
				case Enums.Backpack.ItemSizeCellType.CellContainsStar:
					if (starIsPositive)
					{
						_itemSizeSlots[num4].sprite = _slotStarredSprite;
					}
					else
					{
						_itemSizeSlots[num4].sprite = _slotNegativeStarredSprite;
					}
					_itemSizeSlots[num4].color = new Color(255f, 255f, 255f, 1f);
					break;
				}
			}
		}
	}
}
