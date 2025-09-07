using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Collection;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureRewardWeapon : AdventureReward
{
	public delegate void OnClickHandler(object sender, WeaponCollectionSelectedEventArgs e);

	[SerializeField]
	private WeaponTooltipTrigger _tooltip;

	private WeaponSO _weapon;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(WeaponSO weapon, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		SetImage(weapon.Icon);
		SetText(weapon.Name);
		_weapon = weapon;
		_tooltip.SetWeaponContent(weapon, Enums.Backpack.DraggableOwner.Player);
		_tooltip.ToggleEnabled(unlocked);
		LeanTween.scale(base.Image.gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.3f).setEaseInOutBounce().setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(base.Image.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new WeaponCollectionSelectedEventArgs(_weapon, null));
		}
	}
}
