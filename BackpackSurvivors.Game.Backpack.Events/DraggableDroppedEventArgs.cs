using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Backpack.Events;

internal class DraggableDroppedEventArgs : EventArgs
{
	internal Enums.Backpack.DraggableType DraggableType { get; private set; }

	public bool Success { get; }

	internal DraggableBag DroppedBag { get; private set; }

	internal DraggableItem DroppedItem { get; private set; }

	internal DraggableWeapon DroppedWeapon { get; private set; }

	internal BaseDraggable DroppedBase { get; private set; }

	internal DraggableDroppedEventArgs(Enums.Backpack.DraggableType draggableType, BaseDraggable draggableDropped, bool success)
	{
		DraggableType = draggableType;
		Success = success;
		SetDraggable(draggableType, draggableDropped);
	}

	private void SetDraggable(Enums.Backpack.DraggableType draggableType, BaseDraggable draggableDropped)
	{
		DroppedBase = draggableDropped;
		switch (draggableType)
		{
		case Enums.Backpack.DraggableType.Bag:
			DroppedBag = (DraggableBag)draggableDropped;
			break;
		case Enums.Backpack.DraggableType.Item:
			DroppedItem = (DraggableItem)draggableDropped;
			break;
		case Enums.Backpack.DraggableType.Weapon:
			DroppedWeapon = (DraggableWeapon)draggableDropped;
			break;
		}
	}
}
