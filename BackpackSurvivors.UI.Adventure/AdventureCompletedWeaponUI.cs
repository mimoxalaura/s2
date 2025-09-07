using BackpackSurvivors.ScriptableObjects.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureCompletedWeaponUI : MonoBehaviour
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TextMeshProUGUI _name;

	[SerializeField]
	private TextMeshProUGUI _damage;

	[SerializeField]
	private TextMeshProUGUI _dps;

	public void Init(WeaponSO weapon, float damage, float dps)
	{
		_icon.sprite = weapon.Icon;
		_name.SetText(weapon.Name);
		_damage.SetText($"{damage:f0}");
		_dps.SetText($"{dps:f0}");
	}
}
