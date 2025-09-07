using System;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class WeaponTooltipTrigger : TooltipTrigger, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private WeaponInstance _weaponInstance;

	private WeaponSO _weaponSO;

	private Enums.Backpack.DraggableOwner _owner;

	private int _overridenPrice = -1;

	private void Start()
	{
		TooltipType = Enums.TooltipType.Weapon;
	}

	public void SetWeaponContent(WeaponSO weaponSO, Enums.Backpack.DraggableOwner owner)
	{
		_weaponSO = weaponSO;
		ChangeOwner(owner);
	}

	public void SetWeaponContent(WeaponInstance weaponInstance, Enums.Backpack.DraggableOwner owner)
	{
		_weaponInstance = weaponInstance;
		ChangeOwner(owner);
	}

	public void SetDiscountedPrice(int discountedPrice)
	{
		_overridenPrice = discountedPrice;
	}

	public void ChangeOwner(Enums.Backpack.DraggableOwner owner)
	{
		_owner = owner;
	}

	public override void ShowTooltip()
	{
		if (_weaponSO != null)
		{
			if (_instant)
			{
				SingletonController<TooltipController>.Instance.ShowWeapon(_weaponSO, this, _owner, _overridenPrice);
				return;
			}
			LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
			{
				SingletonController<TooltipController>.Instance.ShowWeapon(_weaponSO, this, _owner, _overridenPrice);
			});
			_delayTweenId = lTDescr.uniqueId;
		}
		else
		{
			if (_weaponInstance == null)
			{
				return;
			}
			if (_instant)
			{
				SingletonController<TooltipController>.Instance.ShowWeapon(_weaponInstance, this, _owner, _overridenPrice);
				return;
			}
			LTDescr lTDescr2 = LeanTween.delayedCall(0.5f, (Action)delegate
			{
				SingletonController<TooltipController>.Instance.ShowWeapon(_weaponInstance, this, _owner, _overridenPrice);
			});
			_delayTweenId = lTDescr2.uniqueId;
		}
	}

	public override void HideTooltip()
	{
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.Hide(null);
			return;
		}
		if (_delayTweenId != 0)
		{
			LeanTween.cancel(_delayTweenId);
		}
		SingletonController<TooltipController>.Instance.Hide(null);
	}
}
