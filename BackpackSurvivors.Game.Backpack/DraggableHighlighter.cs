using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.ScriptableObjects.Backpack;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

public class DraggableHighlighter : MonoBehaviour
{
	[SerializeField]
	private Image _cellHighlightPrefab;

	[SerializeField]
	private Image _noneHighlightPrefab;

	[SerializeField]
	private Image _starHighlightPrefab;

	[SerializeField]
	private Image _starNegativeHighlightPrefab;

	[SerializeField]
	private List<Image> _cellHighlights;

	public void Init(ItemSizeSO ItemSize, bool starredEffectIsPositive)
	{
		GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		GetComponent<GridLayoutGroup>().constraintCount = 10;
		_cellHighlights = new List<Image>();
		Enums.Backpack.ItemSizeCellType[] sizeInfo = ItemSize.SizeInfo;
		foreach (Enums.Backpack.ItemSizeCellType itemSizeCellType in sizeInfo)
		{
			if (itemSizeCellType.GetUniqueFlags().Contains(Enums.Backpack.ItemSizeCellType.CellContainsPlacable))
			{
				_cellHighlights.Add(Object.Instantiate(_cellHighlightPrefab, base.transform));
			}
			else if (itemSizeCellType.GetUniqueFlags().Contains(Enums.Backpack.ItemSizeCellType.CellContainsStar))
			{
				if (starredEffectIsPositive)
				{
					_cellHighlights.Add(Object.Instantiate(_starHighlightPrefab, base.transform));
				}
				else
				{
					_cellHighlights.Add(Object.Instantiate(_starNegativeHighlightPrefab, base.transform));
				}
			}
			else
			{
				_cellHighlights.Add(Object.Instantiate(_noneHighlightPrefab, base.transform));
			}
		}
	}

	public void SetHighlight(bool highlight)
	{
		foreach (Image cellHighlight in _cellHighlights)
		{
			cellHighlight.enabled = highlight;
		}
	}
}
