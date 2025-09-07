using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.CraftingResources;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.ScriptableObjects.CraftingResource;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.System.Video;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

namespace BackpackSurvivors.UI.Tooltip;

[RequireComponent(typeof(CameraEnabler))]
public class TooltipController : SingletonController<TooltipController>
{
	private TooltipTrigger _currentTooltipTrigger;

	[SerializeField]
	private DefaultTooltip _tooltip;

	[SerializeField]
	private CleanItemTooltip _itemTooltip;

	[SerializeField]
	private InlineTextTooltip _inlineTextTooltip;

	[SerializeField]
	private CleanWeaponTooltip _weaponTooltip;

	[SerializeField]
	private TalentTooltip _talentTooltip;

	[SerializeField]
	private RelicTooltip _relicTooltip;

	[SerializeField]
	private CleanBagTooltip _bagTooltip;

	[SerializeField]
	private StatTooltip _statTooltip;

	[SerializeField]
	private BuffTooltip _buffTooltip;

	[SerializeField]
	private ResourceItemTooltip _resourceItemTooltip;

	private CameraEnabler _cameraEnabler;

	private void Start()
	{
		base.IsInitialized = true;
		SingletonController<InputController>.Instance.OnAltHandler += InputController_OnAltHandler;
		SingletonController<InputController>.Instance.OnInputModeSwitched += InputController_OnInputModeSwitched;
	}

	public override void AfterBaseAwake()
	{
		_cameraEnabler = GetComponent<CameraEnabler>();
	}

	private void InputController_OnInputModeSwitched(object sender, InputMapSwitchedEventArgs e)
	{
		_cameraEnabler.SetCamerasEnabled(!e.IsInPlayerInputMap);
	}

	private void InputController_OnAltHandler(object sender, AltEventArgs e)
	{
		ToggleAltTooltip(e.Pressed);
	}

	private void ToggleAltTooltip(bool show)
	{
		if (!(_currentTooltipTrigger == null) && SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity != Enums.TooltipComplexity.AlwaysVisible)
		{
			switch (_currentTooltipTrigger.TooltipType)
			{
			case Enums.TooltipType.Default:
				SingletonController<TooltipController>.Instance._tooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.Item:
				SingletonController<TooltipController>.Instance._itemTooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.Weapon:
				SingletonController<TooltipController>.Instance._weaponTooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.Talent:
				SingletonController<TooltipController>.Instance._talentTooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.Relic:
				SingletonController<TooltipController>.Instance._relicTooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.Stat:
				SingletonController<TooltipController>.Instance._statTooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.Buff:
				SingletonController<TooltipController>.Instance._buffTooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.Bag:
				SingletonController<TooltipController>.Instance._bagTooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.Inline:
				SingletonController<TooltipController>.Instance._inlineTextTooltip.ToggleAltTooltip(show);
				break;
			case Enums.TooltipType.CraftingResource:
				break;
			}
		}
	}

	public void Show(string content, TooltipTrigger currentTooltipTrigger, string header = "")
	{
		if (!SingletonController<InputController>.Instance.IsInPlayerInputMap)
		{
			Hide(null);
			_currentTooltipTrigger = currentTooltipTrigger;
			switch (_currentTooltipTrigger.TooltipType)
			{
			case Enums.TooltipType.Default:
				SingletonController<TooltipController>.Instance._tooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._tooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.Item:
				SingletonController<TooltipController>.Instance._itemTooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._itemTooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.Weapon:
				SingletonController<TooltipController>.Instance._weaponTooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._weaponTooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.Talent:
				SingletonController<TooltipController>.Instance._talentTooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._talentTooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.Relic:
				SingletonController<TooltipController>.Instance._relicTooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._relicTooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.Stat:
				SingletonController<TooltipController>.Instance._statTooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._statTooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.Buff:
				SingletonController<TooltipController>.Instance._buffTooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._buffTooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.Bag:
				SingletonController<TooltipController>.Instance._bagTooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._bagTooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.Inline:
				SingletonController<TooltipController>.Instance._inlineTextTooltip.gameObject.SetActive(value: true);
				SingletonController<TooltipController>.Instance._inlineTextTooltip.RepositionTooltip();
				break;
			case Enums.TooltipType.CraftingResource:
				break;
			}
		}
	}

	public void SetBuffContent(BuffSO talentSO, float remainingTime)
	{
		_buffTooltip.SetBuff(talentSO, remainingTime);
	}

	public void SetStatContent(Enums.ItemStatType statType, List<ItemStatModifier> itemStatModifiers, bool active)
	{
		_statTooltip.SetStat(statType, itemStatModifiers, active);
	}

	public void SetStatContent(Enums.DamageType damageType, List<DamageTypeValueModifier> damageTypeValueModifiers, bool active)
	{
		_statTooltip.SetStat(damageType, damageTypeValueModifiers, active);
	}

	public void SetRelicContent(RelicSO relicSO, bool active)
	{
		_relicTooltip.SetRelic(relicSO, active);
	}

	public void SetItemContent(ItemInstance item, Enums.Backpack.DraggableOwner owner)
	{
		_itemTooltip.SetItem(item, owner, -1);
	}

	public void SetItemContent(ItemSO item, Enums.Backpack.DraggableOwner owner)
	{
		_itemTooltip.SetItem(item, owner, -1);
	}

	public void SetWeaponContent(WeaponInstance weapon, Enums.Backpack.DraggableOwner owner)
	{
		_weaponTooltip.SetWeapon(weapon, owner, -1);
	}

	public void SetWeaponContent(WeaponSO weapon, Enums.Backpack.DraggableOwner owner)
	{
		_weaponTooltip.SetWeapon(weapon, owner, -1);
	}

	public void SetBaseContent(string content, string header = "")
	{
		_tooltip.SetText(content, header);
	}

	public void Hide(BaseTooltip skip)
	{
		if (skip != _tooltip)
		{
			_tooltip.gameObject.SetActive(value: false);
		}
		if (skip != _relicTooltip)
		{
			_relicTooltip.gameObject.SetActive(value: false);
		}
		if (skip != _itemTooltip)
		{
			_itemTooltip.gameObject.SetActive(value: false);
		}
		if (skip != _weaponTooltip)
		{
			_weaponTooltip.gameObject.SetActive(value: false);
		}
		if (skip != _talentTooltip)
		{
			_talentTooltip.gameObject.SetActive(value: false);
		}
		if (skip != _bagTooltip)
		{
			_bagTooltip.gameObject.SetActive(value: false);
		}
		if (skip != _statTooltip)
		{
			_statTooltip.gameObject.SetActive(value: false);
		}
		if (skip != _buffTooltip)
		{
			_buffTooltip.gameObject.SetActive(value: false);
		}
		if (skip != _inlineTextTooltip)
		{
			_buffTooltip.gameObject.SetActive(value: false);
		}
		if (skip != _resourceItemTooltip)
		{
			_resourceItemTooltip.gameObject.SetActive(value: false);
		}
	}

	public void HideIfTriggerIsCorrect(TooltipTrigger tooltipTriggerToHide)
	{
		if (tooltipTriggerToHide == _currentTooltipTrigger)
		{
			Hide(null);
		}
	}

	public void ShowItem(ItemInstance itemInstance, TooltipTrigger currentTooltipTrigger, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		_itemTooltip.gameObject.SetActive(value: false);
		Hide(_itemTooltip);
		_currentTooltipTrigger = currentTooltipTrigger;
		_itemTooltip.SetItem(itemInstance, owner, overriddenPrice);
		StartCoroutine(ActivateAfterDelay(_itemTooltip));
	}

	private IEnumerator ActivateAfterDelay(BaseTooltip toActivate)
	{
		toActivate.SetFollowCursor(follow: false);
		toActivate.transform.position = new Vector2(99999f, 99999f);
		toActivate.gameObject.SetActive(value: true);
		yield return new WaitForSecondsRealtime(0.05f);
		toActivate.SetFollowCursor(follow: true);
	}

	public void ShowItem(ItemSO itemSO, TooltipTrigger currentTooltipTrigger, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		_itemTooltip.gameObject.SetActive(value: false);
		Hide(_itemTooltip);
		_currentTooltipTrigger = currentTooltipTrigger;
		_itemTooltip.SetItem(itemSO, owner, overriddenPrice);
		StartCoroutine(ActivateAfterDelay(_itemTooltip));
	}

	public void ShowInlineTooltip(TipFeedbackElement tipFeedbackElement, TooltipTrigger currentTooltipTrigger)
	{
		_inlineTextTooltip.gameObject.SetActive(value: false);
		Hide(_inlineTextTooltip);
		_currentTooltipTrigger = currentTooltipTrigger;
		_inlineTextTooltip.SetInlineContent(tipFeedbackElement);
		StartCoroutine(ActivateAfterDelay(_inlineTextTooltip));
	}

	public void ShowWeapon(WeaponInstance weaponInstance, TooltipTrigger currentTooltipTrigger, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		_weaponTooltip.gameObject.SetActive(value: false);
		Hide(_weaponTooltip);
		_currentTooltipTrigger = currentTooltipTrigger;
		_weaponTooltip.SetWeapon(weaponInstance, owner, overriddenPrice);
		StartCoroutine(ActivateAfterDelay(_weaponTooltip));
	}

	public void ShowWeapon(WeaponSO weaponSO, TooltipTrigger currentTooltipTrigger, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		_weaponTooltip.gameObject.SetActive(value: false);
		Hide(_weaponTooltip);
		_currentTooltipTrigger = currentTooltipTrigger;
		_weaponTooltip.SetWeapon(weaponSO, owner, overriddenPrice);
		StartCoroutine(ActivateAfterDelay(_weaponTooltip));
	}

	public void HideItem()
	{
		if (SingletonController<TooltipController>.Instance != null)
		{
			_itemTooltip.gameObject.SetActive(value: false);
		}
	}

	public void HideWeapon()
	{
		if (SingletonController<TooltipController>.Instance != null)
		{
			_weaponTooltip.gameObject.SetActive(value: false);
		}
	}

	public void HideInlineTooltip()
	{
		if (SingletonController<TooltipController>.Instance != null)
		{
			_inlineTextTooltip.gameObject.SetActive(value: false);
		}
	}

	public void HideItemIfTriggerIsCorrect(TooltipTrigger tooltipTriggerToHide)
	{
		if (tooltipTriggerToHide == _currentTooltipTrigger)
		{
			SingletonController<TooltipController>.Instance._itemTooltip.gameObject.SetActive(value: false);
		}
	}

	internal void ShowTalent(TalentSO talentSO, bool active, bool showActivatedPart, TalentTooltipTrigger talentTooltipTrigger)
	{
		Hide(_talentTooltip);
		_currentTooltipTrigger = talentTooltipTrigger;
		UpdateTalentContent(talentSO, active, showActivatedPart);
		_talentTooltip.RefreshUI();
		StartCoroutine(ActivateAfterDelay(_talentTooltip));
	}

	internal void ShowRelic(RelicSO relicSO, bool active, RelicTooltipTrigger talentTooltipTrigger)
	{
		Hide(_talentTooltip);
		_currentTooltipTrigger = talentTooltipTrigger;
		_relicTooltip.SetRelic(relicSO, active);
		_relicTooltip.RefreshUI();
		StartCoroutine(ActivateAfterDelay(_relicTooltip));
	}

	internal void UpdateBuff(BuffSO buffSO, float timeRemaining, BuffTooltipTrigger talentTooltipTrigger)
	{
		_buffTooltip.UpdateDescription(buffSO, timeRemaining);
		_buffTooltip.RefreshUI();
	}

	internal void ShowBuff(BuffSO buffSO, float timeRemaining, BuffTooltipTrigger talentTooltipTrigger)
	{
		Hide(_buffTooltip);
		_currentTooltipTrigger = talentTooltipTrigger;
		_buffTooltip.gameObject.SetActive(value: true);
		_buffTooltip.SetBuff(buffSO, timeRemaining);
		_buffTooltip.RefreshUI();
	}

	internal void ShowBag(BagSO bagSO, bool active, BagTooltipTrigger talentTooltipTrigger, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		Hide(_bagTooltip);
		_currentTooltipTrigger = talentTooltipTrigger;
		_bagTooltip.SetBag(bagSO, owner, overriddenPrice);
		StartCoroutine(ActivateAfterDelay(_bagTooltip));
	}

	internal void ShowCraftingResource(CraftingResourceSO craftingResourceSO, CraftingResourceTooltipTrigger craftingResourceTooltipTrigger, int amount)
	{
		Hide(_bagTooltip);
		_currentTooltipTrigger = craftingResourceTooltipTrigger;
		_resourceItemTooltip.SetResourceItem(craftingResourceSO, amount);
		_resourceItemTooltip.RefreshUI();
		StartCoroutine(ActivateAfterDelay(_resourceItemTooltip));
	}

	internal void ShowStat(Enums.ItemStatType statType, List<ItemStatModifier> itemStatModifiers, bool active, StatTooltipTrigger talentTooltipTrigger)
	{
		Hide(_statTooltip);
		_currentTooltipTrigger = talentTooltipTrigger;
		_statTooltip.SetStat(statType, itemStatModifiers, active);
		_statTooltip.RefreshUI();
		StartCoroutine(ActivateAfterDelay(_statTooltip));
	}

	internal void ShowStat(Enums.DamageType damageType, List<DamageTypeValueModifier> damageTypeValueModifiers, bool active, StatTooltipTrigger talentTooltipTrigger)
	{
		Hide(_statTooltip);
		_currentTooltipTrigger = talentTooltipTrigger;
		_statTooltip.SetStat(damageType, damageTypeValueModifiers, active);
		_statTooltip.RefreshUI();
		StartCoroutine(ActivateAfterDelay(_statTooltip));
	}

	internal void UpdateTalentContent(TalentSO talentSO, bool active, bool showActivatedPart)
	{
		_talentTooltip.SetTalent(talentSO, active, showActivatedPart);
	}

	internal void HideTalent()
	{
		_talentTooltip.gameObject.SetActive(value: false);
	}

	internal void HideRelic()
	{
		_relicTooltip.gameObject.SetActive(value: false);
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
