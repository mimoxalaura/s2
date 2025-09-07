using BackpackSurvivors.Game.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback.Weapon;

public abstract class PassiveWeaponBase : MonoBehaviour
{
	[SerializeField]
	internal Image _progressImage;

	[SerializeField]
	internal TextMeshProUGUI _progressText;

	[SerializeField]
	internal Image _weaponIcon;

	[SerializeField]
	internal Image _weaponRarityBackdrop;

	[SerializeField]
	internal GameObject _weaponReadyBorder;

	public abstract void Init(CombatWeapon weapon);

	public abstract void UpdateValues(CombatWeapon weapon);

	public abstract void SetReady(bool isReady);
}
