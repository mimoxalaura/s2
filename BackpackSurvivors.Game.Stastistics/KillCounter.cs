using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.Game.Stastistics;

public class KillCounter : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _text;

	private void Start()
	{
		SingletonController<EnemyController>.Instance.OnEnemyKilled += Instance_OnEnemyKilled;
		_text.SetText(SingletonController<EnemyController>.Instance.TotalEnemiesKilled.ToString());
	}

	private void Instance_OnEnemyKilled(object sender, EnemyKilledEventArgs e)
	{
		_text.SetText(e.TotalEnemiesKilled.ToString());
		float num = 1f;
		float num2 = 1.2f;
		LeanTween.cancel(_text.gameObject);
		float num3 = 0.2f;
		LeanTween.scale(_text.gameObject, new Vector3(num2, num2, num2), num3).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_text.gameObject, new Vector3(num, num, num), num3).setDelay(num3).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void OnDestroy()
	{
		SingletonController<EnemyController>.Instance.OnEnemyKilled -= Instance_OnEnemyKilled;
	}
}
