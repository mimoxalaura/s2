using System.Linq;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Backpack;

public class DragController : MonoBehaviour
{
	[SerializeField]
	private AudioClip _defaultDropAudio;

	private BaseDraggable _draggable;

	private bool _isDragging;

	private ShopController _shopController;

	internal bool IsDragging => _isDragging;

	[SerializeField]
	private bool IsDraggingDataDEBUG => IsDragging;

	private void Start()
	{
		RegisterObjects();
		RegisterEvents();
		ResetBackpackCellAlpha();
	}

	internal static void DestroyDraggable(BaseItemInstance baseItemInstance)
	{
		if (baseItemInstance.BaseDraggable != null)
		{
			baseItemInstance.BaseDraggable.DestroyDraggable(2f);
		}
	}

	private void ResetBackpackCellAlpha()
	{
		SingletonController<BackpackController>.Instance.FadeAlphaCells(BackpackCell.BackpackCellAlpha.None);
	}

	private void RegisterObjects()
	{
		_shopController = SingletonCacheController.Instance.GetControllerByType<ShopController>();
	}

	private void RegisterEvents()
	{
		SingletonController<InputController>.Instance.OnCursorMovementHandler += InputController_OnCursorMovedHandler;
		SingletonController<InputController>.Instance.OnSubmitHandler += InputController_OnSubmitHandler;
		SingletonController<InputController>.Instance.OnRotateHandler += InputController_OnRotateHandler;
		SingletonController<InputController>.Instance.OnRightClickHandler += InputController_OnRightClickHandler;
	}

	private void InputController_OnRightClickHandler(object sender, RightClickEventArgs e)
	{
		if (e.Pressed)
		{
			BaseDraggable hoveredDraggable = GetHoveredDraggable();
			if (!(hoveredDraggable == null) && hoveredDraggable.Owner != Enums.Backpack.DraggableOwner.Shop)
			{
				hoveredDraggable.MoveToStorage();
			}
		}
	}

	private void InputController_OnRotateHandler(object sender, RotationEventArgs e)
	{
		if (e != null)
		{
			HandleRotation(e.Clockwise);
		}
	}

	private void InputController_OnSubmitHandler(object sender, SubmitEventArgs e)
	{
		if (_isDragging)
		{
			HandleDrop();
		}
		else if (e.Pressed)
		{
			StartDrag();
		}
	}

	private void InputController_OnCursorMovedHandler(object sender, CursorPositionEventArgs e)
	{
		if (_isDragging)
		{
			HandleDrag(e.CursorPosition);
		}
	}

	private void HandleRotation(bool clockwise)
	{
		if (_isDragging)
		{
			_draggable.Rotate(clockwise);
			SingletonController<BackpackController>.Instance.DraggableDragging(_draggable, _draggable.BaseItemSO.StarringEffectIsPositive);
		}
	}

	private void StartDrag()
	{
		BaseDraggable hoveredDraggable = GetHoveredDraggable();
		if (!(hoveredDraggable == null) && hoveredDraggable.CanDrag(showCannotAffordUI: true) && SingletonController<BackpackController>.Instance.AllowDragging)
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(hoveredDraggable.BaseItemSO.BeginDragAudio, 1f);
			hoveredDraggable.StartDragPosition = hoveredDraggable.transform.position;
			hoveredDraggable.BeginDrag();
			hoveredDraggable.BaseItemInstance.ClearActiveToast();
			_isDragging = true;
			_draggable = hoveredDraggable;
			_draggable.transform.position = SingletonController<InputController>.Instance.CursorPosition;
			SingletonController<InputController>.Instance.SetCanvasGroupRaycastBlocking(shouldBlock: false);
			if (hoveredDraggable is DraggableBag)
			{
				SingletonController<BackpackController>.Instance.FadeAlphaCells(BackpackCell.BackpackCellAlpha.Partially);
			}
		}
	}

	private BaseDraggable GetHoveredDraggable()
	{
		return Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None).FirstOrDefault((BaseDraggable d) => d.IsCurrentlyHovered);
	}

	internal BaseDraggable GetDraggable(BaseItemInstance baseItemInstance)
	{
		return Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None).FirstOrDefault((BaseDraggable x) => x.BaseItemInstance.Guid == baseItemInstance.Guid);
	}

	private void HandleDrag(Vector2 cursorPosition)
	{
		if (SingletonController<BackpackController>.Instance.AllowDragging && _isDragging && _draggable.CanInteract)
		{
			if (_shopController != null)
			{
				_shopController.SetSellPriceVisibility(_draggable);
			}
			_draggable.transform.position = cursorPosition;
			SingletonController<BackpackController>.Instance.DraggableDragging(_draggable, _draggable.BaseItemSO.StarringEffectIsPositive);
			_draggable.Drag();
		}
	}

	private void HandleDrop()
	{
		if (!_isDragging || !_draggable.CanInteract)
		{
			return;
		}
		_shopController.SetSellAreaVisibility(visible: false);
		if (TrySellDraggable())
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(_shopController.SoldAudio, 1f);
			SingletonController<BackpackController>.Instance.DraggableSold();
			EndDrag();
			return;
		}
		bool flag = _draggable.Drop();
		if (flag)
		{
			_draggable.ActualPosition = _draggable.transform.position;
		}
		PlayDropAudio(flag);
		if (_draggable.StoredInGridType == Enums.Backpack.GridType.Storage)
		{
			SingletonController<BackpackController>.Instance.ToggleStorageChest(open: false, playAudio: true);
		}
		else
		{
			SingletonController<BackpackController>.Instance.ToggleStorageChest(open: false, playAudio: false);
		}
		SingletonController<BackpackController>.Instance.DraggableDropped(_draggable, flag);
		_draggable.EndDrag(flag);
		EndDrag();
	}

	private void PlayDropAudio(bool dropSuccess)
	{
		if (_draggable.StoredInGridType == Enums.Backpack.GridType.Backpack)
		{
			if (dropSuccess)
			{
				AudioClip endDropSuccesAudio = _draggable.BaseItemSO.EndDropSuccesAudio;
				if (endDropSuccesAudio != null)
				{
					SingletonController<AudioController>.Instance.PlaySFXClip(endDropSuccesAudio, _draggable.BaseItemSO.EndDropSuccesAudioVolume, _draggable.BaseItemSO.EndDropSuccesAudioOffset);
				}
				else
				{
					SingletonController<AudioController>.Instance.PlayOnDraggableDroppedAudio(_draggable.BaseItemSO.ItemRarity, 0.6f);
				}
			}
			else
			{
				AudioClip endDropFailedAudio = _draggable.BaseItemSO.EndDropFailedAudio;
				if (endDropFailedAudio != null)
				{
					SingletonController<AudioController>.Instance.PlaySFXClip(endDropFailedAudio, 1f, _draggable.BaseItemSO.EndDropFailedAudioVolume, _draggable.BaseItemSO.EndDropFailedAudioOffset);
				}
				else
				{
					SingletonController<AudioController>.Instance.PlayOnDraggableDroppedAudio(_draggable.BaseItemSO.ItemRarity, 0.6f);
				}
			}
		}
		else if (_draggable.StoredInGridType == Enums.Backpack.GridType.Storage && dropSuccess)
		{
			SingletonController<AudioController>.Instance.PlayOnDraggableDroppedAudio(_draggable.BaseItemSO.ItemRarity, 0.4f);
		}
	}

	private void EndDrag()
	{
		_isDragging = false;
		SingletonController<InputController>.Instance.SetCanvasGroupRaycastBlocking(shouldBlock: true);
		SingletonController<BackpackController>.Instance.FadeAlphaCells(BackpackCell.BackpackCellAlpha.None);
		SingletonController<StatController>.Instance.UpdateStats();
	}

	private bool TrySellDraggable()
	{
		if (_shopController == null)
		{
			return false;
		}
		if (_draggable.Owner == Enums.Backpack.DraggableOwner.Shop)
		{
			_shopController.HideSellText();
			return false;
		}
		return _shopController.TrySellDraggable(_draggable);
	}

	private void OnDestroy()
	{
		if (SingletonController<InputController>.Instance != null)
		{
			SingletonController<InputController>.Instance.OnCursorMovementHandler -= InputController_OnCursorMovedHandler;
			SingletonController<InputController>.Instance.OnSubmitHandler -= InputController_OnSubmitHandler;
			SingletonController<InputController>.Instance.OnRotateHandler -= InputController_OnRotateHandler;
		}
	}
}
