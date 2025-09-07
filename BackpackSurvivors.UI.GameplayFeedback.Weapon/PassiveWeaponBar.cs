using BackpackSurvivors.Game.Combat;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback.Weapon;

public class PassiveWeaponBar : PassiveWeaponBase
{
	[SerializeField]
	private TextMeshProUGUI _weaponNameText;

	public override void Init(CombatWeapon weapon)
	{
		_weaponNameText.SetText(weapon.Name);
		_weaponIcon.sprite = weapon.Icon;
	}

	public override void UpdateValues(CombatWeapon weapon)
	{
		if (weapon.CurrentCooldown > 0f)
		{
			_progressImage.fillAmount = (weapon.Cooldown - weapon.CurrentCooldown) / weapon.Cooldown;
			_progressText.SetText($"{weapon.CurrentCooldown:0.00}s");
			SetReady(isReady: false);
		}
		else
		{
			_progressImage.fillAmount = 1f;
			_progressText.SetText(string.Empty);
		}
	}

	public override void SetReady(bool isReady)
	{
		_weaponReadyBorder.SetActive(isReady);
	}
}
