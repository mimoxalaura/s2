using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack.Events;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Backpack;

public class HoveredSlotProvider
{
	private List<BackpackCellQuadrant> _backpackCellQuadrants = new List<BackpackCellQuadrant>();

	private BackpackCellQuadrant _lastHoveredBackpackCellQuadrant;

	public void RegisterQuadrant(BackpackCellQuadrant quadrant)
	{
		_backpackCellQuadrants.Add(quadrant);
		quadrant.OnBackpackCellQuadrantHoverEnter += Quadrant_OnBackpackCellQuadrantHoverEnter;
		quadrant.OnBackpackCellQuadrantHoverExit += Quadrant_OnBackpackCellQuadrantHoverExit;
	}

	private void Quadrant_OnBackpackCellQuadrantHoverEnter(object sender, BackpackCellQuadrantHoveredEventArgs e)
	{
		_lastHoveredBackpackCellQuadrant = e.BackpackCellQuadrant;
	}

	private void Quadrant_OnBackpackCellQuadrantHoverExit(object sender, BackpackCellQuadrantHoveredEventArgs e)
	{
		if (!(_lastHoveredBackpackCellQuadrant == null) && _lastHoveredBackpackCellQuadrant.BackpackSlotId == e.BackpackCellQuadrant.BackpackSlotId)
		{
			_lastHoveredBackpackCellQuadrant = null;
		}
	}

	public bool IsHoveringOverBackpackGrid()
	{
		if (_lastHoveredBackpackCellQuadrant == null)
		{
			return false;
		}
		return _lastHoveredBackpackCellQuadrant.HoveredCellGridType == Enums.Backpack.GridType.Backpack;
	}

	public bool IsHoveringOverStorageGrid()
	{
		if (_lastHoveredBackpackCellQuadrant == null)
		{
			return false;
		}
		return _lastHoveredBackpackCellQuadrant.HoveredCellGridType == Enums.Backpack.GridType.Storage;
	}

	public HoveredSlotInfo GetHoveredSlotInfo()
	{
		if (_lastHoveredBackpackCellQuadrant == null)
		{
			return HoveredSlotInfo.None;
		}
		int backpackSlotId = _lastHoveredBackpackCellQuadrant.BackpackSlotId;
		Enums.Backpack.GridType hoveredCellGridType = _lastHoveredBackpackCellQuadrant.HoveredCellGridType;
		bool isHoveredSlotOnRight = HoveredQuadrantIsOnRightSide();
		bool isHoveredSlotOnBottom = HoveredQuadrantIsOnBottomSide();
		return new HoveredSlotInfo(backpackSlotId, isHoveredSlotOnRight, isHoveredSlotOnBottom, hoveredCellGridType);
	}

	public bool HoveredQuadrantIsOnRightSide()
	{
		if (_lastHoveredBackpackCellQuadrant == null)
		{
			return false;
		}
		return _lastHoveredBackpackCellQuadrant.QuadrantIdentifier % 2 == 0;
	}

	public bool HoveredQuadrantIsOnBottomSide()
	{
		if (_lastHoveredBackpackCellQuadrant == null)
		{
			return false;
		}
		return _lastHoveredBackpackCellQuadrant.QuadrantIdentifier > 2;
	}
}
