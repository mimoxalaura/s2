using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Difficulty;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.Game.Level;

public class DifficultyChanger : UnlockableInTown
{
	[SerializeField]
	private DifficultyChangeVisual[] _difficultyChangeVisuals;

	[SerializeField]
	private Sprite[] _difficultyChangeBackgroundSprites;

	[SerializeField]
	private SpriteRenderer _difficultyChangeBackground;

	[SerializeField]
	private Light2D _light2D;

	private void Start()
	{
		SingletonController<DifficultyController>.Instance.OnDifficultyChanged += Instance_OnDifficultyChanged;
	}

	private void Instance_OnDifficultyChanged(object sender, DifficultyChangedEventArgs e)
	{
		SetDifficulty(e.NewDifficulty);
	}

	public void SetDifficulty(int difficulty)
	{
		if (_difficultyChangeVisuals.Length < difficulty)
		{
			return;
		}
		for (int i = 0; i < _difficultyChangeVisuals.Length; i++)
		{
			bool flag = i + 1 <= difficulty;
			if (_difficultyChangeVisuals[i] != null)
			{
				_difficultyChangeVisuals[i].gameObject.SetActive(flag);
				_difficultyChangeVisuals[i].Toggle(flag);
			}
		}
		if (difficulty > 0)
		{
			_difficultyChangeBackground.sprite = _difficultyChangeBackgroundSprites[difficulty - 1];
		}
		else
		{
			_difficultyChangeBackground.sprite = null;
		}
	}

	private void OnDestroy()
	{
		SingletonController<DifficultyController>.Instance.OnDifficultyChanged -= Instance_OnDifficultyChanged;
	}
}
