using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.CraftingResources;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.Game.Items.RuneEffects;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shared.Interfaces;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.BackpackVFX;
using BackpackSurvivors.UI.GameplayFeedback;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Shop;
using BackpackSurvivors.UI.Stats;
using BackpackSurvivors.UI.Tooltip;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Shop;

internal class ShopController : BaseModalUIController, IClearable, IInitializable
{
	internal delegate void OnShopClosedHandler(object sender, ShopClosedEventArgs e);

	[SerializeField]
	private ShopUI _shopUI;

	[SerializeField]
	private Canvas _vendorSellAreaCanvas;

	[SerializeField]
	internal AudioClip SoldAudio;

	[SerializeField]
	private PreviewShopItem _previewShopItems;

	[SerializeField]
	private BaseItemSO[] PreviewIems;

	[Header("Exit")]
	[SerializeField]
	private MainMenuButton _exitButton;

	[SerializeField]
	private MainMenuButton _continueButton;

	private ShopRandomizer _shopRandomizer = new ShopRandomizer();

	private ShopGenerator _shopGenerator = new ShopGenerator();

	private LevelMusicPlayer _levelMusicPlayer;

	private int _rerollCost = 1;

	private float _globalDiscount;

	private bool _hasVisitedShopBefore;

	private int _nextItemGuaranteeCounter = 1;

	private int _nextWeaponGuaranteeCounter = 1;

	private int _nextBagGuaranteeCounter = 1;

	private bool _isInitial = true;

	private bool _continueAdventureAfterClosing = true;

	private bool _shopWasOpenedFromAdventureVendor;

	private bool _closing;

	private float _levelMusicProgress;

	internal ShopGenerator ShopGenerator => _shopGenerator;

	internal List<ShopOfferSlot> ShopOfferSlots => _shopUI.ShopOfferSlots;

	public bool IsInitialized { get; private set; }

	internal event Action OnShopOpened;

	internal event OnShopClosedHandler OnShopClosed;

	internal void Start()
	{
		RegisterEvents();
		_shopGenerator.Init(this);
		IsInitialized = true;
	}

	private LevelMusicPlayer GetLevelMusicPlayer()
	{
		if (_levelMusicPlayer == null)
		{
			_levelMusicPlayer = SingletonCacheController.Instance.GetControllerByType<LevelMusicPlayer>();
		}
		return _levelMusicPlayer;
	}

	private void RegisterEvents()
	{
		_shopUI.OnShopRerollClicked += ShopUI_OnShopRerollClicked;
		List<IInitializable> initializables = new List<IInitializable>
		{
			SingletonController<GameController>.Instance.Player,
			SingletonController<UnlocksController>.Instance
		};
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(initializables, InitShopOfferSlots);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<UnlocksController>.Instance, RegisterUnlockEvent);
	}

	private void RegisterInputEvents()
	{
		SingletonController<InputController>.Instance.OnCancelHandler += InputController_OnCancelHandler;
		SingletonController<InputController>.Instance.OnSpecial1Handler += Instance_OnSpecial1Handler;
	}

	private void Instance_OnSpecial1Handler(object sender, EventArgs e)
	{
		if (!SingletonCacheController.Instance.GetControllerByType<DragController>().IsDragging)
		{
			CloseShopUI(checkCombatViablity: true);
		}
	}

	private void UnRegisterInputEvents()
	{
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
		SingletonController<InputController>.Instance.OnSpecial1Handler -= Instance_OnSpecial1Handler;
	}

	private void InputController_OnCancelHandler(object sender, EventArgs e)
	{
		if (!SingletonCacheController.Instance.GetControllerByType<DragController>().IsDragging && !SingletonController<TutorialController>.Instance.IsRunningTutorial)
		{
			ExitAdventure();
		}
	}

	private void RegisterUnlockEvent()
	{
		SingletonController<UnlocksController>.Instance.OnUnlockableUnlocked += UnlocksController_OnUnlockableUnlocked;
	}

	private void UnlocksController_OnUnlockableUnlocked(object sender, UnlockableUnlockedEventArgs e)
	{
		if (!e.FromLoad)
		{
			if (e.UnlockedItem.BaseUnlockable.Unlockable == Enums.Unlockable.ExtraShopOffers)
			{
				int num = 6;
				int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ExtraShopOffers);
				_shopUI.UpdateNumberOfUnlockedShopOfferSlots(num + unlockedCount);
			}
			SingletonController<SaveGameController>.Instance.SaveProgression();
		}
	}

	public List<ItemInstance> GetItemsFromShop()
	{
		return _shopUI.GetItemsFromShop();
	}

	public List<WeaponInstance> GetWeaponsFromShop()
	{
		return _shopUI.GetWeaponsFromShop();
	}

	public List<BagInstance> GetBagsFromShop()
	{
		return _shopUI.GetBagsFromShop();
	}

	internal Dictionary<int, BaseDraggable> GetReservedOffers()
	{
		return _shopUI.GetReservedOffers();
	}

	internal void SetDiscount(float discount)
	{
		_globalDiscount = discount;
	}

	internal void InitShopOfferSlots()
	{
		int num = 6;
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ExtraShopOffers);
		_shopUI.InitShopOfferSlots(num + unlockedCount, _shopRandomizer, _shopGenerator, _globalDiscount, _isInitial);
		_isInitial = false;
	}

	public override bool CloseOnCancelInput()
	{
		return false;
	}

	private void ShopUI_OnShopRerollClicked(object sender, EventArgs e)
	{
		RerollShop();
	}

	private void ResetRerollCost()
	{
		_rerollCost = 1;
		_shopUI.UpdateRerollButtonText(_rerollCost, initial: false);
	}

	[Command("shop.reroll", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void RerollShop()
	{
		bool flag = false;
		if (SingletonController<BackpackController>.Instance.UsedShopRerolls < SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopRerolls))
		{
			SingletonController<BackpackController>.Instance.UsedShopRerolls++;
			flag = true;
			_rerollCost = 0;
		}
		if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, _rerollCost))
		{
			SingletonController<CurrencyController>.Instance.TrySpendCurrency(Enums.CurrencyType.Coins, _rerollCost);
			if (flag)
			{
				_rerollCost = 1;
				_shopUI.UpdateFreeRerollButtonText(SingletonController<BackpackController>.Instance.UsedShopRerolls, SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopRerolls));
			}
			else
			{
				_rerollCost *= 2;
				_shopUI.UpdateRerollButtonText(_rerollCost, initial: false);
			}
			_shopUI.RerollShop(_rerollCost, _globalDiscount);
			if (SingletonController<BackpackController>.Instance.UsedShopRerolls < SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopRerolls))
			{
				_shopUI.UpdateRerollButtonText(0, initial: false);
			}
			else
			{
				_shopUI.UpdateRerollButtonText(_rerollCost, initial: false);
			}
		}
	}

	private void RefreshShop()
	{
		if (!(_shopUI == null) && _shopUI.IsInitialized)
		{
			if (SingletonController<BackpackController>.Instance.UsedShopRerolls < SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopRerolls))
			{
				_rerollCost = 0;
			}
			else
			{
				_rerollCost = 1;
			}
			HandleSpecialEffectRunes();
			_shopUI.UpdateRerollButtonText(_rerollCost, initial: true);
			_shopUI.UpdateFreeRerollButtonText(SingletonController<BackpackController>.Instance.UsedShopRerolls, SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopRerolls));
			_shopUI.RerollShop(_rerollCost, _globalDiscount);
		}
	}

	[Command("backpack.ActivateSpecialEffects", Platform.AllPlatforms, MonoTargetType.Single)]
	private static void HandleSpecialEffectRunes()
	{
		foreach (RuneSpecialEffect item in from x in UnityEngine.Object.FindObjectsByType<RuneSpecialEffect>(FindObjectsSortMode.None)
			orderby x.TriggerPriority descending
			select x)
		{
			if (item.BaseDraggable.Owner != Enums.Backpack.DraggableOwner.Player || item.BaseDraggable.StoredInGridType != Enums.Backpack.GridType.Backpack)
			{
				continue;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (item.RuneSpecialEffectDestructionType == Enums.RuneSpecialEffectDestructionType.DestroyAfterShopEntering)
			{
				flag = true;
			}
			if (item.ShouldTrigger())
			{
				flag2 = item.Trigger();
				flag3 = true;
				if (flag2 && item.RuneSpecialEffectDestructionType == Enums.RuneSpecialEffectDestructionType.DestroyAfterTrigger)
				{
					flag = true;
				}
			}
			if (item.RuneSpecialEffectDestructionType == Enums.RuneSpecialEffectDestructionType.DestroyAfterX && item.CurrentTriggerCount >= item.MaxTriggerCount)
			{
				flag = true;
			}
			if (flag)
			{
				item.Dissolve();
			}
			if (flag3 && !flag2)
			{
				item.FailedToTrigger();
			}
		}
	}

	internal bool TryBuyDraggable(BaseDraggable draggable)
	{
		bool num = SingletonController<CurrencyController>.Instance.TrySpendCurrency(Enums.CurrencyType.Coins, draggable.BuyingPrice);
		if (num)
		{
			draggable.ChangeOwner(Enums.Backpack.DraggableOwner.Player);
			bool isBag = draggable is DraggableBag;
			Transform newParentTransform = GetNewParentTransform(isBag);
			draggable.transform.SetParent(newParentTransform);
			draggable.Enabled = true;
			if (draggable is DraggableBag)
			{
				SingletonController<CollectionController>.Instance.UnlockBag(draggable.BaseItemSO.Id);
				return num;
			}
			if (draggable is DraggableWeapon)
			{
				SingletonController<CollectionController>.Instance.UnlockWeapon(draggable.BaseItemSO.Id);
				return num;
			}
			if (draggable is DraggableItem)
			{
				SingletonController<CollectionController>.Instance.UnlockItem(draggable.BaseItemSO.Id);
			}
		}
		return num;
	}

	private Transform GetNewParentTransform(bool isBag)
	{
		if (isBag)
		{
			return SingletonController<BackpackController>.Instance.BoughtBagsParentTransform;
		}
		return SingletonController<BackpackController>.Instance.BoughtItemsAndWeaponsParentTransform;
	}

	[Command("shop.guaranteebag", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void GuaranteeBagInShop(int bagId)
	{
		BagSO bagById = GameDatabaseHelper.GetBagById(bagId);
		if (!(bagById == null))
		{
			_shopUI.SetGuaranteedItemToShopOfferSlot(bagById.ItemType, bagById);
		}
	}

	[Command("shop.guaranteeweapon", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void GuaranteeWeaponInShop(int weaponId)
	{
		WeaponSO weaponById = GameDatabaseHelper.GetWeaponById(weaponId);
		if (!(weaponById == null))
		{
			_shopUI.SetGuaranteedItemToShopOfferSlot(weaponById.ItemType, weaponById);
		}
	}

	[Command("shop.guaranteeitem", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void GuaranteeItemInShop(int itemId)
	{
		ItemSO itemById = GameDatabaseHelper.GetItemById(itemId);
		if (!(itemById == null))
		{
			_shopUI.SetGuaranteedItemToShopOfferSlot(itemById.ItemType, itemById);
		}
	}

	[Command("DB_GuaranteeNextItems", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void DB_GuaranteeNextItems(int numberOfItems = 9, bool resetCounter = false)
	{
		if (resetCounter)
		{
			_nextItemGuaranteeCounter = 1;
		}
		for (int i = 0; i < numberOfItems; i++)
		{
			GuaranteeItemInShop(_nextItemGuaranteeCounter + i);
		}
		_nextItemGuaranteeCounter += numberOfItems;
		RerollShop();
		ResetRerollCost();
	}

	[Command("DB_GuaranteeNextWeapons", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void DB_GuaranteeNextWeapons(int numberOfWeapons = 9, bool resetCounter = false)
	{
		if (resetCounter)
		{
			_nextWeaponGuaranteeCounter = 1;
		}
		for (int i = 0; i < numberOfWeapons; i++)
		{
			GuaranteeWeaponInShop(_nextWeaponGuaranteeCounter + i);
		}
		_nextWeaponGuaranteeCounter += numberOfWeapons;
		RerollShop();
		ResetRerollCost();
	}

	[Command("DB_GuaranteeNextBags", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void DB_GuaranteeNextBags(int numberOfBags = 9, bool resetCounter = false)
	{
		if (resetCounter)
		{
			_nextBagGuaranteeCounter = 1;
		}
		for (int i = 0; i < numberOfBags; i++)
		{
			GuaranteeBagInShop(_nextBagGuaranteeCounter + i);
		}
		_nextBagGuaranteeCounter += numberOfBags;
		RerollShop();
		ResetRerollCost();
	}

	[Command("shop.open", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		base.OpenUI();
		ToggleDamageTakenFeedbackIfNeeded(insideShop: true);
		_continueAdventureAfterClosing = true;
		GameObject gameObject = GameObject.Find("UICamera");
		if (gameObject != null)
		{
			gameObject.SetActive(value: false);
		}
		GameObject gameObject2 = GameObject.Find("DustParticles");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(value: false);
		}
		CraftingResourceVisualsController controllerByType = SingletonCacheController.Instance.GetControllerByType<CraftingResourceVisualsController>();
		if (controllerByType != null)
		{
			controllerByType.ChangeGridVisibility(visible: false);
		}
		this.OnShopOpened?.Invoke();
		SingletonController<StatController>.Instance.SetDisplayToggleButton(showButton: false);
		_shopUI.gameObject.SetActive(value: true);
		_shopUI.ShowOrHideRerollButtonText();
		_shopUI.OpenUI();
		_vendorSellAreaCanvas.gameObject.SetActive(value: true);
		SingletonController<BackpackController>.Instance.OpenUI();
		SingletonController<BackpackController>.Instance.LogDPSLoggableActiveTime();
		SingletonController<StatController>.Instance.ToggleOpenWithSoloBackdrop(openWithSoloBackdrop: false);
		SingletonController<StatController>.Instance.ToggleOpenWithBuffsAndDebuffs(openWithBuffsAndDebuffs: false);
		SingletonController<StatController>.Instance.OpenUI();
		SingletonCacheController.Instance.GetControllerByType<AdventureVendorController>()?.ToggleVendorMarker(active: false);
		PlayShopMusic();
		SingletonController<BackpackController>.Instance.ResetBagSlotHighlights();
		SingletonController<BackpackController>.Instance.ClearAllVisualGridHighlights();
		TimeBasedLevelController controllerByType2 = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
		if ((controllerByType2 != null && controllerByType2.CurrentLevel.AllowMultipleShops) || !_hasVisitedShopBefore)
		{
			RefreshShop();
		}
		else
		{
			_shopUI.UpdateFreeRerollButtonText(SingletonController<BackpackController>.Instance.UsedShopRerolls, SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopRerolls));
		}
		RefreshPreviewSlots();
		SingletonController<MergeController>.Instance.MergeMergables();
		SingletonController<MergeController>.Instance.UpdateMergePossibilities();
		RegisterInputEvents();
		_hasVisitedShopBefore = true;
	}

	private void ToggleDamageTakenFeedbackIfNeeded(bool insideShop)
	{
		DamageTakenFeedback damageTakenFeedback = UnityEngine.Object.FindAnyObjectByType<DamageTakenFeedback>();
		if (!(damageTakenFeedback == null))
		{
			if (insideShop)
			{
				damageTakenFeedback.DisableFeedbackVisual();
			}
			else
			{
				damageTakenFeedback.CheckHealthState();
			}
		}
	}

	[Command("shop.close", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void CloseUI()
	{
		if (!SingletonCacheController.Instance.GetControllerByType<DragController>().IsDragging)
		{
			_closing = true;
			base.CloseUI();
			SingletonController<MergeController>.Instance.ClearCompleteMergableLines();
			SingletonController<MergeController>.Instance.ClearPotentialLines();
			SingletonController<MergeController>.Instance.ClearIncompleteMergableLines();
			SingletonController<BackpackController>.Instance.ResetBagSlotHighlights();
			SingletonController<BackpackController>.Instance.ClearAllVisualGridHighlights();
			SingletonController<BackpackController>.Instance.SetDPSLoggableActiveStartTime();
			SingletonController<StarLineController>.Instance.ClearStarredLines();
			RemoveAllRemainingDropVFXEffects();
			_shopUI.ResetSellAreaCurrentlyHovered();
			SingletonController<InLevelTransitionController>.Instance.Transition(CloseUiDuringTransition);
		}
	}

	private void CloseUiDuringTransition()
	{
		Camera camera = UnityEngine.Object.FindObjectsOfType<Camera>(includeInactive: true).FirstOrDefault((Camera sr) => sr.gameObject.name == "UICamera");
		if (camera != null)
		{
			camera.gameObject.SetActive(value: true);
		}
		GameObject gameObject = GameObject.Find("DustParticles");
		if (gameObject != null)
		{
			gameObject.SetActive(value: true);
		}
		CraftingResourceVisualsController controllerByType = SingletonCacheController.Instance.GetControllerByType<CraftingResourceVisualsController>();
		if (controllerByType != null)
		{
			controllerByType.ChangeGridVisibility(visible: true);
		}
		if (!_continueAdventureAfterClosing)
		{
			SingletonController<BackpackController>.Instance.UsedShopRerolls = 0;
			SingletonController<BackpackController>.Instance.UsedShopBanishes = 0;
		}
		SingletonController<BackpackController>.Instance.HideAllVFX();
		this.OnShopClosed?.Invoke(this, new ShopClosedEventArgs(_continueAdventureAfterClosing));
		ToggleDamageTakenFeedbackIfNeeded(insideShop: false);
		_shopUI.CloseUI();
		_shopUI.gameObject.SetActive(value: false);
		_vendorSellAreaCanvas.gameObject.SetActive(value: false);
		SingletonController<StatController>.Instance.SetDisplayToggleButton(showButton: false);
		SingletonController<BackpackController>.Instance.CloseUI();
		SingletonController<StatController>.Instance.CloseUI();
		SingletonController<StatController>.Instance.ToggleOpenWithSoloBackdrop(openWithSoloBackdrop: true);
		SingletonController<StatController>.Instance.ToggleOpenWithBuffsAndDebuffs(openWithBuffsAndDebuffs: true);
		SingletonCacheController.Instance.GetControllerByType<AdventureVendorController>()?.ToggleVendorMarker(active: true);
		SingletonController<TooltipController>.Instance.Hide(null);
		PlayLevelMusic();
		UnRegisterInputEvents();
	}

	private void RemoveAllRemainingDropVFXEffects()
	{
		BackpackDropItemVFX[] array = UnityEngine.Object.FindObjectsByType<BackpackDropItemVFX>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i].gameObject);
		}
	}

	internal void SetShopOpenedFromVendor(bool fromVendor)
	{
		_shopWasOpenedFromAdventureVendor = fromVendor;
	}

	public void CloseShopUI(bool checkCombatViablity)
	{
		if (!_closing)
		{
			_closing = true;
			if (!checkCombatViablity)
			{
				_continueAdventureAfterClosing = true;
				SingletonCacheController.Instance.GetControllerByType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.Shop);
			}
			else
			{
				SingletonCacheController.Instance.GetControllerByType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.Shop, revertInput: true, !_shopWasOpenedFromAdventureVendor);
				_continueAdventureAfterClosing = true;
			}
		}
	}

	public void ExitAdventure()
	{
		_exitButton.OnExitHover();
		_continueAdventureAfterClosing = false;
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.Shop);
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Shop;
	}

	internal void SetSellPriceVisibility(BaseDraggable draggable)
	{
		_shopUI.SetSellPriceVisibility(draggable);
	}

	internal bool TrySellDraggable(BaseDraggable draggable)
	{
		bool num = _shopUI.TrySellDraggable(draggable);
		if (num)
		{
			SingletonController<MergeController>.Instance.UpdateMergePossibilities();
		}
		return num;
	}

	internal void HideSellText()
	{
		_shopUI.HideSellText();
	}

	private void RefreshPreviewSlots()
	{
		if (GameDatabase.IsDemo)
		{
			UnityEngine.Random.Range(0, 3);
			if (PreviewIems.Count() > 0)
			{
				_previewShopItems.gameObject.SetActive(value: true);
				int num = UnityEngine.Random.Range(0, PreviewIems.Count());
				_previewShopItems.Init(PreviewIems[num]);
			}
			else
			{
				_previewShopItems.gameObject.SetActive(value: false);
			}
		}
		else
		{
			_previewShopItems.gameObject.SetActive(value: false);
		}
	}

	private void PlayShopMusic()
	{
		LevelMusicPlayer levelMusicPlayer = GetLevelMusicPlayer();
		if (!(levelMusicPlayer == null))
		{
			_levelMusicProgress = SingletonController<AudioController>.Instance.GetCurrentMusicProgress();
			SingletonController<AudioController>.Instance.StopAmbience(1f);
			levelMusicPlayer.PlayShopMusic();
		}
	}

	private void PlayLevelMusic()
	{
		LevelMusicPlayer levelMusicPlayer = GetLevelMusicPlayer();
		if (!(levelMusicPlayer == null))
		{
			TimeBasedLevelController controllerByType = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
			if (controllerByType != null && controllerByType.BossSpawned)
			{
				controllerByType.PlayBossAudio(_levelMusicProgress);
			}
			else
			{
				levelMusicPlayer.PlayLevelMusic(_levelMusicProgress);
			}
			levelMusicPlayer.PlayLevelAmbience();
		}
	}

	public override bool AudioOnOpen()
	{
		return false;
	}

	public override bool AudioOnClose()
	{
		return false;
	}

	internal void SetSellAreaVisibility(bool visible)
	{
		_shopUI.SetSellAreaVisibility(visible);
	}

	private void OnDestroy()
	{
		UnRegisterInputEvents();
		SingletonController<UnlocksController>.Instance.OnUnlockableUnlocked -= UnlocksController_OnUnlockableUnlocked;
		_shopUI.OnShopRerollClicked -= ShopUI_OnShopRerollClicked;
	}

	public void Clear()
	{
		ResetRerollCost();
		_shopUI.ResetReservedShopOffers();
		_shopUI.RerollShopInstantly();
	}

	public void ClearAdventure()
	{
		ResetRerollCost();
		_shopUI.ResetReservedShopOffers();
		_shopUI.RerollShopInstantly();
	}

	internal void SetButtonInteraction(bool allowInteraction)
	{
		_closing = !allowInteraction;
		_exitButton.Interactable = allowInteraction;
		_continueButton.Interactable = allowInteraction;
	}
}
