using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Backpack.Events;

internal class DraggableDraggingEventArgs : EventArgs
{
	internal Enums.Backpack.DraggableType DraggableType { get; private set; }

	internal DraggableBag DroppedBag { get; private set; }

	internal DraggableItem DroppedItem { get; private set; }

	internal DraggableWeapon DroppedWeapon { get; private set; }

	internal DraggableDraggingEventArgs(Enums.Backpack.DraggableType draggableType, BaseDraggable draggableDropped)
	{
		DraggableType = draggableType;
		SetDraggable(draggableType, draggableDropped);
	}

	private void SetDraggable(Enums.Backpack.DraggableType draggableType, BaseDraggable draggableDropped)
	{
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
