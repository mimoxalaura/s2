using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Adventure;

internal class WorkingGameplayHellfire : MonoBehaviour
{
	[SerializeField]
	private GameObject _container;

	[SerializeField]
	private TextMeshProUGUI _hellfireLevel;

	private void Start()
	{
		_container.SetActive(SingletonController<DifficultyController>.Instance.ActiveDifficulty > 0);
		_hellfireLevel.SetText(SingletonController<DifficultyController>.Instance.ActiveDifficulty.ToString());
	}
}
