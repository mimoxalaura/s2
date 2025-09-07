using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.Shop;

public class TrainingRoomDPSAndDamageUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _dpsText;

	[SerializeField]
	private TextMeshProUGUI _damageText;

	internal void UpdateStats(float totalDamage, float dps)
	{
		_dpsText.SetText(dps.ToString("0.00"));
		_damageText.SetText(((int)totalDamage).ToString());
	}
}
