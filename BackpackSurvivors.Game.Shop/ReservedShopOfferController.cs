using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Shop;

internal class ReservedShopOfferController : SingletonController<ReservedShopOfferController>
{
	private Dictionary<int, BaseDraggable> _reservedDraggablesPerSlotId = new Dictionary<int, BaseDraggable>();

	private void Start()
	{
		base.IsInitialized = true;
	}

	internal bool HasReservationForSlotId(int shopOfferSlotId)
	{
		return _reservedDraggablesPerSlotId.ContainsKey(shopOfferSlotId);
	}

	internal BaseDraggable GetReservationForSlotId(int shopOfferSlotId)
	{
		if (!_reservedDraggablesPerSlotId.ContainsKey(shopOfferSlotId))
		{
			return null;
		}
		return _reservedDraggablesPerSlotId[shopOfferSlotId];
	}

	internal void SaveReservedDraggables()
	{
		ShopController controllerByType = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		if (!(controllerByType == null))
		{
			_reservedDraggablesPerSlotId = controllerByType.GetReservedOffers();
		}
	}

	internal void RemoveReservationForShopOfferSlotId(int shopOfferSlotId)
	{
		if (_reservedDraggablesPerSlotId.ContainsKey(shopOfferSlotId))
		{
			_reservedDraggablesPerSlotId.Remove(shopOfferSlotId);
		}
	}

	public override void Clear()
	{
		_reservedDraggablesPerSlotId?.Clear();
	}

	public override void ClearAdventure()
	{
		_reservedDraggablesPerSlotId?.Clear();
	}
}
