using System;
using System.Collections;
using System.Linq;
using BackpackSurvivors.Assets.Game.Revive;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Stastistics;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Steam;
using BackpackSurvivors.UI.Adventure;
using BackpackSurvivors.UI.GameplayFeedback;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using QFSW.QC;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.Game.Adventure;

public class AdventureCompletedController : BaseSingletonModalUIController<AdventureCompletedController>
{
	public delegate void OnAdventureCompletedHandler(object sender, AdventureCompletedEventArgs e);

	[SerializeField]
	private AdventureCompletedUINew _adventureCompletedUI;

	[SerializeField]
	private SceneReference _townScene;

	[SerializeField]
	private Camera _adventureCompletedCamera;

	private AdventureSO _activeAdventure;

	private bool _succesfullCompletion;

	public event OnAdventureCompletedHandler OnAdventureCompleted;

	public override bool CloseOnCancelInput()
	{
		return false;
	}

	private void Start()
	{
		RegisterEvents();
		base.IsInitialized = true;
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<AdventureController>.Instance, RegisterAdventureStarted);
	}

	public void RegisterAdventureStarted()
	{
		SingletonController<AdventureController>.Instance.OnAdventureStarted += AdventureController_OnAdventureStarted;
	}

	private void AdventureController_OnAdventureStarted(object sender, AdventureStartedEventArgs e)
	{
		Init(e.Adventure);
	}

	public void Init(AdventureSO activeAdventure)
	{
		_activeAdventure = activeAdventure;
	}

	[Command("adventure.complete", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		base.OpenUI();
		_adventureCompletedCamera.gameObject.SetActive(value: true);
		_adventureCompletedUI.Init(_activeAdventure, _succesfullCompletion);
		_adventureCompletedUI.OnAdventureCompleteButtonClicked += _adventureCompletedUI_OnAdventureCompleteButtonClicked;
		if (_succesfullCompletion)
		{
			SingletonController<GameController>.Instance.Player.ShowSuccesEffect();
		}
		UnityEngine.Object.FindAnyObjectByType<DamageTakenFeedback>().DisableFeedbackVisual();
		_adventureCompletedUI.OpenUI();
		SingletonCacheController.Instance.GetControllerByType<ReviveController>().SetReviveCountUIVisibility(visible: false);
		GameObject.Find("WorkingGameplayCanvas").SetActive(value: false);
		GameObject.Find("MinimapCanvas").SetActive(value: false);
		GameObject.Find("UICamera").SetActive(value: false);
		SingletonController<InputController>.Instance.SwitchToUIActionMap();
		Time.timeScale = 1f;
	}

	public override void CloseUI()
	{
		base.CloseUI();
		_adventureCompletedCamera.gameObject.SetActive(value: false);
		_adventureCompletedUI.CloseUI();
		_adventureCompletedUI.OnAdventureCompleteButtonClicked -= _adventureCompletedUI_OnAdventureCompleteButtonClicked;
		SingletonController<InputController>.Instance.RevertToPreviousActionMap();
	}

	private void _adventureCompletedUI_OnAdventureCompleteButtonClicked(object sender, EventArgs e)
	{
		CompleteAdventure();
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.AdventureCompleted;
	}

	public void CompleteAdventure()
	{
		HandleRewards();
		if (_succesfullCompletion)
		{
			SingletonController<StatisticsController>.Instance.SetAdventureCompleted(_activeAdventure.AdventureId, SingletonController<DifficultyController>.Instance.ActiveDifficulty);
			SingletonController<UnlocksController>.Instance.SetUnlock(Enums.Unlockable.Difficulties, SingletonController<DifficultyController>.Instance.ActiveDifficulty + 1);
			if (GameDatabase.IsDemo)
			{
				UnlockDemoWonAchievement();
			}
		}
		SingletonController<GameController>.Instance.ClearControllersOfAdventureState();
		this.OnAdventureCompleted?.Invoke(this, new AdventureCompletedEventArgs(_activeAdventure, _succesfullCompletion));
		SingletonController<SaveGameController>.Instance.SaveProgression();
		StartCoroutine(NavigateBackToTownAsync());
	}

	private void UnlockDemoWonAchievement()
	{
		SingletonController<SteamController>.Instance.UnlockAchievement(Enums.AchievementEnum.Demo_AdventureWon);
	}

	private IEnumerator NavigateBackToTownAsync()
	{
		yield return new WaitForSecondsRealtime(1f);
		SingletonController<GameController>.Instance.ExitingFromPortal = true;
		Time.timeScale = 1f;
		SingletonController<SceneChangeController>.Instance.ChangeScene(GameDatabaseHelper.GetSceneFromType(Enums.SceneType.Town), LoadSceneMode.Single, isLevelScene: false, showLoadingScreen: true, "Fortress");
	}

	public void CloseAllUIs()
	{
		UnityEngine.Object.FindAnyObjectByType<ModalUiController>().CloseAll();
	}

	private void HandleRewards()
	{
		if (!_succesfullCompletion)
		{
			return;
		}
		foreach (RewardSO item in _activeAdventure.CompletionRewards.Where((RewardSO x) => !x.Repeatable && x.HellfireLevel == SingletonController<DifficultyController>.Instance.ActiveDifficulty))
		{
			switch (item.CompletionRewardType)
			{
			case Enums.RewardType.Weapon:
				SingletonController<UnlockedEquipmentController>.Instance.AddWeaponReward(item);
				break;
			case Enums.RewardType.Item:
				SingletonController<UnlockedEquipmentController>.Instance.AddItemReward(item);
				break;
			case Enums.RewardType.Relic:
				SingletonController<UnlockedEquipmentController>.Instance.AddRelicReward(item);
				break;
			case Enums.RewardType.TitanicSouls:
				SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.TitanSouls, item.Amount, Enums.CurrencySource.Reward);
				break;
			case Enums.RewardType.Bag:
				SingletonController<UnlockedEquipmentController>.Instance.AddBagReward(item);
				break;
			}
		}
		foreach (RewardSO item2 in _activeAdventure.CompletionRewards.Where((RewardSO x) => x.Repeatable && x.HellfireLevel <= SingletonController<DifficultyController>.Instance.ActiveDifficulty))
		{
			if (item2.CompletionRewardType == Enums.RewardType.TitanicSouls)
			{
				float rewardMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetRewardMultiplierFromHellfire(_activeAdventure);
				int numberOfCurrency = (int)((float)item2.Amount * rewardMultiplierFromHellfire);
				SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.TitanSouls, numberOfCurrency, Enums.CurrencySource.Reward);
			}
		}
	}

	private void OnDestroy()
	{
		SingletonController<AdventureController>.Instance.OnAdventureStarted -= AdventureController_OnAdventureStarted;
	}

	internal void SetSuccess()
	{
		_succesfullCompletion = true;
	}

	public override void Clear()
	{
		_succesfullCompletion = false;
	}

	public override void ClearAdventure()
	{
		_succesfullCompletion = false;
	}

	public override bool AudioOnOpen()
	{
		return false;
	}

	public override bool AudioOnClose()
	{
		return false;
	}
}
