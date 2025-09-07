using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.ScriptableObjects.Unlockable;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.GameplayFeedback;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Unlockables;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Unlockables;

public class UnlocksController : BaseSingletonModalUIController<UnlocksController>
{
	public delegate void UnlockableUnlockedHandler(object sender, UnlockableUnlockedEventArgs e);

	[SerializeField]
	public UnlocksUI _unlocksUI;

	[SerializeField]
	public Canvas _canvas;

	[SerializeField]
	public Camera[] _postProcessingCamerasToToggle;

	private List<UnlockableItem> _unlockedUpgrades = new List<UnlockableItem>();

	private bool _loadedFromSave;

	public event UnlockableUnlockedHandler OnUnlockableUnlocked;

	private void Start()
	{
		InitUnlocks();
		RegisterEvents();
		_unlocksUI.OnAfterCloseUI += _unlocksUI_OnAfterCloseUI;
		base.IsInitialized = true;
	}

	private void InitUnlocks()
	{
		_unlockedUpgrades.Clear();
		foreach (UnlockableSO unlockable in GameDatabaseHelper.GetUnlockables())
		{
			UnlockableItem unlockableItem = new UnlockableItem(unlockable);
			unlockableItem.OnUnlockableUnlocked += Item_OnUnlockableUnlocked;
			_unlockedUpgrades.Add(unlockableItem);
		}
	}

	public override bool CloseOnCancelInput()
	{
		return true;
	}

	public UnlockedsSaveState GetSaveState()
	{
		UnlockedsSaveState unlockedsSaveState = new UnlockedsSaveState();
		unlockedsSaveState.UnlockedUpgrades = new Dictionary<Enums.Unlockable, int>();
		foreach (UnlockableItem unlockedUpgrade in _unlockedUpgrades)
		{
			unlockedsSaveState.UnlockedUpgrades.Add(unlockedUpgrade.Unlockable, unlockedUpgrade.PointsInvested);
		}
		return unlockedsSaveState;
	}

	public bool IsUnlocked(Enums.Unlockable unlockable)
	{
		if (!_loadedFromSave)
		{
			SingletonController<SaveGameController>.Instance.LoadProgression();
		}
		return _unlockedUpgrades.FirstOrDefault((UnlockableItem x) => x.Unlockable == unlockable)?.IsUnlocked ?? false;
	}

	public bool TryUpgradeOrSpend(UnlockableItem unlockableItem)
	{
		bool num = SingletonController<CurrencyController>.Instance.TrySpendCurrency(Enums.CurrencyType.TitanSouls, unlockableItem.GetCostForNextPoint());
		if (num)
		{
			unlockableItem.Upgrade();
		}
		return num;
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameLoaded);
	}

	public void RegisterSaveGameLoaded()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	public List<UnlockableItem> GetUnlockableItems()
	{
		return _unlockedUpgrades;
	}

	public int GetUnlockedCount(Enums.Unlockable unlockable)
	{
		if (!IsUnlocked(unlockable))
		{
			return 0;
		}
		return _unlockedUpgrades.FirstOrDefault((UnlockableItem x) => x.Unlockable == unlockable).PointsInvested;
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		foreach (KeyValuePair<Enums.Unlockable, int> item in e.SaveGame.UnlockedUpgradesState.UnlockedUpgrades)
		{
			UnlockableSO unlockableFromType = GameDatabaseHelper.GetUnlockableFromType(item.Key);
			if (!(unlockableFromType == null))
			{
				_unlockedUpgrades.RemoveAll((UnlockableItem x) => x.BaseUnlockable.Unlockable == item.Key);
				UnlockableItem unlockableItem = new UnlockableItem(unlockableFromType, item.Value);
				unlockableItem.OnUnlockableUnlocked += Item_OnUnlockableUnlocked;
				_unlockedUpgrades.Add(unlockableItem);
				this.OnUnlockableUnlocked?.Invoke(this, new UnlockableUnlockedEventArgs(unlockableItem, fromLoad: true, fromUI: false));
			}
		}
		base.IsInitialized = true;
		_loadedFromSave = true;
	}

	private void Item_OnUnlockableUnlocked(object sender, UnlockableUnlockedEventArgs e)
	{
		if (e.UnlockedItem.PointsInvested == 1 && e.UnlockedItem.BaseUnlockable.ShowEffect)
		{
			PlayerMessageController playerMessageController = Object.FindObjectOfType<PlayerMessageController>();
			if (playerMessageController != null)
			{
				playerMessageController.ShowMessage(e.UnlockedItem.BaseUnlockable.Name, e.UnlockedItem.BaseUnlockable.IconForEffect, null, Enums.PlayerMessageType.Default, 4f, e.UnlockedItem.BaseUnlockable.ShowEffect);
			}
			else
			{
				Debug.LogWarning("No PlayerMessageController found on scene!");
			}
		}
		this.OnUnlockableUnlocked?.Invoke(this, e);
	}

	[Command("unlockable.unlock", Platform.AllPlatforms, MonoTargetType.Single)]
	public bool TryUnlock(Enums.Unlockable unlockable)
	{
		bool flag = false;
		UnlockableSO unlockableSO = GameDatabaseHelper.GetUnlockables().FirstOrDefault((UnlockableSO x) => x.Unlockable == unlockable);
		if (unlockableSO == null)
		{
			Debug.Log("Unlockable not available");
			return false;
		}
		UnlockableItem unlockableItem = _unlockedUpgrades.FirstOrDefault((UnlockableItem x) => x.BaseUnlockable.Unlockable == unlockable);
		if (unlockableItem != null)
		{
			if (unlockableItem.PointsInvested >= unlockableItem.BaseUnlockable.UnlockAvailableAmount)
			{
				Debug.Log("Unlockable already fully unlocked");
				return false;
			}
			int costForNextPoint = unlockableItem.GetCostForNextPoint();
			if (!SingletonController<CurrencyController>.Instance.CanAfford(unlockableItem.BaseUnlockable.PriceCurrencyType, costForNextPoint))
			{
				Debug.Log("Not enough currency for this unlock");
				return false;
			}
			flag = SingletonController<CurrencyController>.Instance.TrySpendCurrency(unlockableItem.BaseUnlockable.PriceCurrencyType, costForNextPoint);
			unlockableItem.PointsInvested++;
		}
		else
		{
			flag = SingletonController<CurrencyController>.Instance.TrySpendCurrency(unlockableSO.PriceCurrencyType, unlockableSO.UnlockPrice);
			unlockableItem = new UnlockableItem(unlockableSO, 1);
			_unlockedUpgrades.Add(unlockableItem);
			unlockableItem.OnUnlockableUnlocked += Item_OnUnlockableUnlocked;
		}
		if (flag)
		{
			UnlockableItem unlockedItem = _unlockedUpgrades.FirstOrDefault((UnlockableItem x) => x.BaseUnlockable.Unlockable == unlockable);
			SingletonController<SaveGameController>.Instance.SaveProgression();
			this.OnUnlockableUnlocked?.Invoke(this, new UnlockableUnlockedEventArgs(unlockedItem, fromLoad: false, fromUI: false));
			return true;
		}
		return false;
	}

	[Command("hellfire.unlock", Platform.AllPlatforms, MonoTargetType.Single)]
	public void SetUnlock(Enums.Unlockable unlockable, int amount)
	{
		GameDatabaseHelper.GetUnlockables().FirstOrDefault((UnlockableSO x) => x.Unlockable == unlockable);
		UnlockableItem unlockableItem = _unlockedUpgrades.FirstOrDefault((UnlockableItem x) => x.BaseUnlockable.Unlockable == unlockable);
		if (unlockableItem != null && unlockableItem.PointsInvested < amount)
		{
			unlockableItem.PointsInvested = amount;
		}
	}

	public void SetUnlock(Enums.Unlockable unlockable)
	{
		UnlockableSO unlockable2 = GameDatabaseHelper.GetUnlockables().FirstOrDefault((UnlockableSO x) => x.Unlockable == unlockable);
		UnlockableItem unlockableItem = _unlockedUpgrades.FirstOrDefault((UnlockableItem x) => x.BaseUnlockable.Unlockable == unlockable);
		if (unlockableItem != null)
		{
			if (unlockableItem.PointsInvested < unlockableItem.BaseUnlockable.UnlockAvailableAmount)
			{
				unlockableItem.PointsInvested++;
			}
		}
		else
		{
			unlockableItem = new UnlockableItem(unlockable2, 1);
			_unlockedUpgrades.Add(unlockableItem);
		}
		UnlockableItem unlockedItem = _unlockedUpgrades.FirstOrDefault((UnlockableItem x) => x.BaseUnlockable.Unlockable == unlockable);
		this.OnUnlockableUnlocked?.Invoke(this, new UnlockableUnlockedEventArgs(unlockedItem, fromLoad: true, fromUI: false));
	}

	[Command("unlockable.lock", Platform.AllPlatforms, MonoTargetType.Single)]
	public void Lock(Enums.Unlockable unlockable)
	{
		_unlockedUpgrades.FirstOrDefault((UnlockableItem x) => x.BaseUnlockable.Unlockable == unlockable).ResetToLock();
	}

	[Command("unlockable.open", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		base.OpenUI();
		_canvas.GetComponent<AttackCameraToCanvas>().Refresh();
		Camera[] postProcessingCamerasToToggle = _postProcessingCamerasToToggle;
		for (int i = 0; i < postProcessingCamerasToToggle.Length; i++)
		{
			postProcessingCamerasToToggle[i].enabled = false;
		}
		_unlocksUI.Init();
		_unlocksUI.OpenUI();
	}

	public override void CloseUI()
	{
		Camera[] postProcessingCamerasToToggle = _postProcessingCamerasToToggle;
		for (int i = 0; i < postProcessingCamerasToToggle.Length; i++)
		{
			postProcessingCamerasToToggle[i].enabled = true;
		}
		_unlocksUI.CloseUI();
	}

	private void _unlocksUI_OnAfterCloseUI()
	{
		SetCamerasEnabled(enabled: false);
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Unlocks;
	}

	public override void Clear()
	{
		InitUnlocks();
	}

	public override void ClearAdventure()
	{
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool AudioOnClose()
	{
		return true;
	}

	internal void SelectUpgrade(Enums.Unlockable unlockable)
	{
		_unlocksUI.SelectUpgrade(unlockable);
	}
}
