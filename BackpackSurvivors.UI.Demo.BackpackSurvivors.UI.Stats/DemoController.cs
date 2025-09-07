using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.UI.Demo.BackpackSurvivors.UI.Stats;

public class DemoController : BaseModalUIController
{
	[SerializeField]
	private DemoUI _demoUI;

	private void Start()
	{
		if (GameDatabase.IsDemo)
		{
			SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameLoaded);
			ShowPopup();
		}
	}

	private void RegisterSaveGameLoaded()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		SingletonController<GameController>.Instance.HasShownDemoPopup = e.SaveGame.DemoSaveState.HasShownDemoPopup;
		ShowPopup();
	}

	private void ShowPopup()
	{
		if (!SingletonController<GameController>.Instance.HasShownDemoPopup && SingletonController<GameController>.Instance.ShouldShowDemoPopup)
		{
			SingletonController<GameController>.Instance.HasShownDemoPopup = true;
			SingletonController<SaveGameController>.Instance.SaveProgression();
			Object.FindObjectOfType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.Demo);
		}
	}

	public override bool AudioOnClose()
	{
		return true;
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool CloseOnCancelInput()
	{
		return false;
	}

	public void CloseDemoUI()
	{
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().CloseAll();
	}

	public override void CloseUI()
	{
		base.CloseUI();
		_demoUI.CloseUI();
		SingletonController<GameController>.Instance.ShouldShowDemoPopup = false;
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Demo;
	}

	[Command("Demo.OpenUI", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		base.OpenUI();
		_demoUI.OpenUI();
	}

	public void WishlistButtonClick()
	{
		Application.OpenURL("steam://openurl/https://store.steampowered.com/app/2294780/Backpack_Survivors#game_area_purchase");
	}

	public void DiscordButtonClick()
	{
		Application.OpenURL("https://discord.gg/xjDKSkGWhU");
	}

	private void OnDestroy()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded -= SaveGameController_OnSaveGameLoaded;
	}
}
