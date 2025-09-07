using System.Collections.Generic;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack.Highlighting;

internal class BagCellHighlightImage : MonoBehaviour
{
	[SerializeField]
	private Image _highlightImage;

	private int _slotId;

	public bool CanHighlight { get; private set; }

	public int HighlightImageRowIndex { get; private set; }

	public int HighlightImageColumnIndex { get; private set; }

	public void Init(bool canHighlight, int rowIndex, int columnIndex)
	{
		CanHighlight = canHighlight;
		HighlightImageRowIndex = rowIndex;
		HighlightImageColumnIndex = columnIndex;
	}

	public void SetSlotId(int slotId)
	{
		_slotId = slotId;
	}

	public void SetHighlightVisibilityBySlotIds(List<int> placeableSlotIds, List<int> emptyBagSlotIds)
	{
		bool visibility = placeableSlotIds.Contains(_slotId);
		SetHighlightImageVisibility(visibility, emptyBagSlotIds.Contains(_slotId));
	}

	internal void ResetSlotHighlights()
	{
		SetHighlightImageVisibility(visibility: false, slotIsFree: false);
	}

	private void SetHighlightImageVisibility(bool visibility, bool slotIsFree)
	{
		_highlightImage.color = (slotIsFree ? Constants.Colors.ValidBackpackPlacementColor : Constants.Colors.InvalidBackpackPlacementColor);
		_highlightImage.enabled = visibility && CanHighlight;
	}
}
