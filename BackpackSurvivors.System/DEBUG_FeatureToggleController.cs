using System.Collections.Generic;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using UnityEngine;

namespace BackpackSurvivors.System;

public class DEBUG_FeatureToggleController : SingletonController<DEBUG_FeatureToggleController>
{
	public enum Feature
	{
		Unlockables,
		Talents,
		Quests,
		Adventures,
		SingleMissions,
		Collection,
		Difficulties,
		Blacksmith,
		CharacterSwapping,
		CharacterSwap1,
		CharacterSwap2,
		CharacterSwap3,
		CharacterSwap4,
		Settings,
		Credits,
		TrainingRoom,
		Well
	}

	[SerializeField]
	private GameObject _featureNotAvailablePopup;

	private Dictionary<Feature, bool> _features;

	private void Start()
	{
		_features = new Dictionary<Feature, bool>();
		_features.Add(Feature.Collection, value: true);
		_features.Add(Feature.Unlockables, value: true);
		_features.Add(Feature.Talents, value: true);
		_features.Add(Feature.Quests, value: false);
		_features.Add(Feature.Adventures, value: true);
		_features.Add(Feature.SingleMissions, value: false);
		_features.Add(Feature.Blacksmith, value: false);
		_features.Add(Feature.CharacterSwapping, value: true);
		_features.Add(Feature.CharacterSwap1, value: true);
		_features.Add(Feature.CharacterSwap2, value: false);
		_features.Add(Feature.CharacterSwap3, value: false);
		_features.Add(Feature.CharacterSwap4, value: false);
		_features.Add(Feature.Settings, value: true);
		_features.Add(Feature.Credits, value: false);
		_features.Add(Feature.TrainingRoom, value: false);
		_features.Add(Feature.Well, value: true);
		base.IsInitialized = true;
	}

	public bool IsFeatureActive(Feature feature)
	{
		if (!_features.ContainsKey(feature))
		{
			return false;
		}
		if (!GameDatabase.IsDemo)
		{
			return true;
		}
		return _features[feature];
	}

	public void ShowFeatureUnavailablePopup()
	{
		_featureNotAvailablePopup.SetActive(value: true);
		SingletonController<GameController>.Instance.SetGamePaused(gamePaused: true);
	}

	public void HideFeatureUnavailablePopup()
	{
		_featureNotAvailablePopup.SetActive(value: false);
		SingletonController<GameController>.Instance.SetGamePaused(gamePaused: false);
	}

	public bool TryFeatureAndPopup(Feature feature)
	{
		bool num = IsFeatureActive(feature);
		if (!num)
		{
			ShowFeatureUnavailablePopup();
		}
		return num;
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
