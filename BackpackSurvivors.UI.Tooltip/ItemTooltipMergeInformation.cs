using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.Tooltip;

public class ItemTooltipMergeInformation : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _text;

	internal void Init(bool willMergeNextShop, Mergable mergable, ItemInstance item)
	{
	}
}
