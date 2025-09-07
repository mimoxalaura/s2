using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

public class BackpackVisualGrid : MonoBehaviour
{
	[SerializeField]
	private Transform _gridParent;

	[SerializeField]
	private BackpackCell _backpackCellPrefab;

	[SerializeField]
	private Enums.Backpack.GridType _gridType;

	[SerializeField]
	private Image _unlockedCellsImage;

	[SerializeField]
	private BackpackVisualLockArea[] _unlockedRowObjects;

	[SerializeField]
	private BackpackVisualLockArea[] _unlockedColumnsObjects;

	private float _defaultAnchoredPositionY;

	private float _defaultSizeDeltaY;

	private float _defaultSizeDeltaX;

	private int _numberOfCellsToCreate;

	private List<BackpackCell> _backpackCells = new List<BackpackCell>();

	public Enums.Backpack.GridType GridType => _gridType;

	private void Start()
	{
		SetNumberOfCellsToCreate();
		CreateCells();
		if (_gridType == Enums.Backpack.GridType.Backpack)
		{
			SetDefaultRectTransformPositioningAndSizingValues();
		}
	}

	private void SetDefaultRectTransformPositioningAndSizingValues()
	{
		RectTransform component = _unlockedCellsImage.GetComponent<RectTransform>();
		_defaultAnchoredPositionY = component.anchoredPosition.y;
		_defaultSizeDeltaY = component.sizeDelta.y;
		_defaultSizeDeltaX = component.sizeDelta.x;
	}

	private void CreateCells()
	{
		for (int i = 0; i < _numberOfCellsToCreate; i++)
		{
			BackpackCell backpackCell = Object.Instantiate(_backpackCellPrefab, _gridParent);
			backpackCell.Init(i);
			_backpackCells.Add(backpackCell);
		}
	}

	public void FadeAlphaCells(BackpackCell.BackpackCellAlpha backpackCellAlpha)
	{
		foreach (BackpackCell backpackCell in _backpackCells)
		{
			backpackCell.FadeAlpha(backpackCellAlpha);
		}
	}

	public void ToggleLockedCell(List<int> cellIndexes, bool locked)
	{
		foreach (int cellIndex in cellIndexes)
		{
			_backpackCells[cellIndex].ToggleLockedState(locked);
		}
	}

	public void SetLockedCellBorder(List<int> lockedRowIds, List<int> lockedColumnIds)
	{
		RectTransform component = _unlockedCellsImage.GetComponent<RectTransform>();
		SetLockedBorderSize(lockedRowIds, lockedColumnIds, component);
		SetLockedBorderPosition(lockedRowIds, lockedColumnIds, component);
	}

	public void SetLockedColumnRowOverlay(List<int> lockedRowIds, List<int> lockedColumnIds)
	{
		BackpackVisualLockArea[] unlockedRowObjects = _unlockedRowObjects;
		for (int i = 0; i < unlockedRowObjects.Length; i++)
		{
			unlockedRowObjects[i].ToggleLocked(locked: false);
		}
		unlockedRowObjects = _unlockedColumnsObjects;
		for (int i = 0; i < unlockedRowObjects.Length; i++)
		{
			unlockedRowObjects[i].ToggleLocked(locked: false);
		}
		foreach (int lockedRowId in lockedRowIds)
		{
			_unlockedRowObjects[lockedRowId].ToggleLocked(locked: true);
		}
		foreach (int lockedColumnId in lockedColumnIds)
		{
			_unlockedColumnsObjects[lockedColumnId].ToggleLocked(locked: true);
		}
	}

	private void SetLockedBorderPosition(List<int> lockedRowIds, List<int> lockedColumnIds, RectTransform rectTransform)
	{
		int topHalfRows = 6;
		float num = 0f - (float)lockedRowIds.Count((int r) => r < topHalfRows) * 48f;
		int topHalfColumns = 6;
		float x = (float)lockedColumnIds.Count((int r) => r < topHalfColumns) * 48f;
		rectTransform.anchoredPosition = new Vector2(x, _defaultAnchoredPositionY + num);
	}

	private void SetLockedBorderSize(List<int> lockedRowIds, List<int> lockedColumnIds, RectTransform rectTransform)
	{
		SetDefaultRectTransformPositioningAndSizingValues();
		float size = _defaultSizeDeltaY - (float)lockedRowIds.Count * 48f;
		float size2 = _defaultSizeDeltaX - (float)lockedColumnIds.Count * 48f;
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size2);
	}

	private void SetNumberOfCellsToCreate()
	{
		if (_gridType == Enums.Backpack.GridType.Backpack)
		{
			_numberOfCellsToCreate = 144;
		}
		else if (_gridType == Enums.Backpack.GridType.Storage)
		{
			_numberOfCellsToCreate = 72;
		}
	}

	public void ShowCellStatusColor(int slotId, Color color)
	{
		BackpackCell backpackCell = _backpackCells.FirstOrDefault((BackpackCell c) => c.SlotId == slotId);
		if (!(backpackCell == null))
		{
			backpackCell.ShowStatusColorImage(color);
		}
	}

	public void HideCellStatusColor(int slotId)
	{
		BackpackCell backpackCell = _backpackCells.FirstOrDefault((BackpackCell c) => c.SlotId == slotId);
		if (!(backpackCell == null))
		{
			backpackCell.HideStatusColorImage();
		}
	}

	public void HideAllCellStatusColors()
	{
		foreach (BackpackCell backpackCell in _backpackCells)
		{
			backpackCell.HideStatusColorImage();
		}
	}

	public Vector2 GetCenterOfCells(List<int> slotIds)
	{
		List<BackpackCell> source = _backpackCells.Where((BackpackCell c) => slotIds.Contains(c.SlotId)).ToList();
		float num = source.Min((BackpackCell c) => c.transform.position.x);
		float num2 = source.Max((BackpackCell c) => c.transform.position.x);
		float num3 = source.Min((BackpackCell c) => c.transform.position.y);
		float num4 = source.Max((BackpackCell c) => c.transform.position.y);
		float x = (num + num2) / 2f;
		float y = (num3 + num4) / 2f;
		return new Vector2(x, y);
	}

	public void SetStarImageVisibility(int slotId, bool visibile, bool showStarredFilled, bool starEffectIsPositive)
	{
		BackpackCell backpackCell = _backpackCells.FirstOrDefault((BackpackCell c) => c.SlotId == slotId);
		if (!(backpackCell == null))
		{
			backpackCell.SetStarImageVisibility(visibile, showStarredFilled, starEffectIsPositive);
		}
	}

	public void SetCellOutlineVisibility(int slotId, bool visible)
	{
		BackpackCell backpackCell = _backpackCells.FirstOrDefault((BackpackCell c) => c.SlotId == slotId);
		if (!(backpackCell == null))
		{
			backpackCell.SetCellOutlineImageVisibility(visible);
		}
	}

	public void HideAllStarImages()
	{
		foreach (BackpackCell backpackCell in _backpackCells)
		{
			backpackCell.SetStarImageVisibility(visibility: false);
		}
	}

	public void HideAllCellOutlines()
	{
		foreach (BackpackCell backpackCell in _backpackCells)
		{
			backpackCell.SetCellOutlineImageVisibility(visibility: false);
		}
	}

	internal List<BackpackCell> GetBackpackCells(List<int> cellIds)
	{
		return _backpackCells.Where((BackpackCell c) => cellIds.Contains(c.SlotId)).ToList();
	}
}
