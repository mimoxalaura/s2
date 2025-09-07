using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Backpack.Events;
using BackpackSurvivors.Game.Backpack.Highlighting;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Video;
using BackpackSurvivors.UI.BackpackVFX;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.Merging;

[RequireComponent(typeof(CameraEnabler))]
internal class MergeController : SingletonController<MergeController>
{
	[SerializeField]
	private Vector2 _graphScale = new Vector2(1f, 1f);

	[SerializeField]
	private Canvas _canvas;

	[Header("Lines")]
	[SerializeField]
	private Transform _potentialmergableLineContainer;

	[SerializeField]
	private Transform _incompleteMergableLineContainer;

	[SerializeField]
	private Transform _mergableLineContainer;

	[SerializeField]
	private PotentialMergableItemLine _couldMergableLinePrefab;

	[SerializeField]
	private IncompleteMergableItemLine _incompleteMergableLinePrefab;

	[SerializeField]
	private MergableItemLine _mergableLinePrefab;

	[SerializeField]
	private Camera _mergeCamera;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _mergeStartAudio;

	[SerializeField]
	private AudioClip _mergeAudio;

	[SerializeField]
	private AudioClip _activeMergeAudio;

	[SerializeField]
	private AudioClip _mergeLockActive;

	[SerializeField]
	private AudioClip _mergeLockInactive;

	[Header("VFX")]
	[SerializeField]
	private BackpackMergeItemVFX _backpackMergeItemVFX;

	[SerializeField]
	private BackpackMergeFinishedItemVFX _backpackMergeFinishedItemVFX;

	[Header("Toast")]
	[SerializeField]
	private MergeToastNotification _mergeToastNotificationPrefab;

	[SerializeField]
	private Transform _mergeToastNotificationContainer;

	private List<MergableSO> _mergableSOs = new List<MergableSO>();

	private List<MergableItemLine> _mergableLines = new List<MergableItemLine>();

	private List<IncompleteMergableItemLine> _incompleteMergableLines = new List<IncompleteMergableItemLine>();

	private List<PotentialMergableItemLine> _potentialMergableLines = new List<PotentialMergableItemLine>();

	private List<MergeRecipeSet> _mergeRecipeSets = new List<MergeRecipeSet>();

	private List<MergeRecipeSet> _previousMergeRecipeSets = new List<MergeRecipeSet>();

	private BaseItemInstance _lastDroppedBaseItemInstance;

	private CameraEnabler _cameraEnabler;

	private void Start()
	{
		InitMergables();
		RegisterEvents();
		base.IsInitialized = true;
	}

	public override void AfterBaseAwake()
	{
		_cameraEnabler = GetComponent<CameraEnabler>();
	}

	internal void SetCamerasEnabled(bool enabled)
	{
		_cameraEnabler.SetCamerasEnabled(enabled);
	}

	private void InitMergables()
	{
		_mergableSOs.AddRange(SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableMergables);
	}

	public bool BaseItemSOIsUsedInAnyMerge(BaseItemSO baseItemSO)
	{
		return _mergableSOs.Any((MergableSO x) => x.Input.Any((MergeableIngredient y) => y.BaseItem.Id == baseItemSO.Id && y.BaseItem.ItemType == baseItemSO.ItemType));
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<InputController>.Instance, RegisterInputControllerEvents);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<BackpackController>.Instance, RegisterBackpackControllerControllerEvents);
	}

	private void RegisterInputControllerEvents()
	{
		SingletonController<InputController>.Instance.OnToggleMergingAllowedHandler += InputController_OnToggleMergingAllowedHandler;
	}

	private void RegisterBackpackControllerControllerEvents()
	{
		SingletonController<BackpackController>.Instance.OnDraggableDropped += BackpackController_OnDraggableDropped;
	}

	private void BackpackController_OnDraggableDropped(object sender, DraggableDroppedEventArgs e)
	{
		if (e.DraggableType == Enums.Backpack.DraggableType.Bag)
		{
			_lastDroppedBaseItemInstance = null;
			return;
		}
		_lastDroppedBaseItemInstance = e.DroppedBase.BaseItemInstance;
		if (_lastDroppedBaseItemInstance != null && e.Success)
		{
			UpdateMergePossibilities();
		}
	}

	[Command("merge.Update", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UpdateMergePossibilities()
	{
		_previousMergeRecipeSets = new List<MergeRecipeSet>();
		_previousMergeRecipeSets.AddRange(_mergeRecipeSets);
		_mergeRecipeSets = new List<MergeRecipeSet>();
		UpdatePotentialItems();
		UpdateCompleteMergeItemsNew();
		UpdateIncompleteMergeItemsNew();
		DrawCompleteMergableLines();
		DrawIncompleteMergableLines();
		ShowMergeToasts();
	}

	internal void ShowMergeToasts()
	{
		IEnumerable<MergeRecipeSet> completedPrev = _previousMergeRecipeSets.Where((MergeRecipeSet x) => x.RecipeIsComplete);
		IEnumerable<MergeRecipeSet> source = _mergeRecipeSets.Where((MergeRecipeSet x) => x.RecipeIsComplete);
		IEnumerable<MergeRecipeSet> inCompletedPrev = _previousMergeRecipeSets.Where((MergeRecipeSet x) => !x.RecipeIsComplete);
		IEnumerable<MergeRecipeSet> source2 = _mergeRecipeSets.Where((MergeRecipeSet x) => !x.RecipeIsComplete);
		IEnumerable<MergeRecipeSet> enumerable = source.Where((MergeRecipeSet x) => !completedPrev.Contains(x));
		IEnumerable<MergeRecipeSet> enumerable2 = source2.Where((MergeRecipeSet x) => !inCompletedPrev.Contains(x));
		foreach (MergeRecipeSet item in enumerable)
		{
			SingletonController<MergeToastController>.Instance.ShowCompleteMergeToast(item.PrimaryBaseItemInstance);
		}
		foreach (MergeRecipeSet item2 in enumerable2)
		{
			SingletonController<MergeToastController>.Instance.ShowIncompleteMergeToast(item2.PrimaryBaseItemInstance);
		}
	}

	private void InputController_OnToggleMergingAllowedHandler(object sender, EventArgs e)
	{
		BaseDraggable currentlyHoveredBaseDraggable = SingletonController<BackpackController>.Instance.CurrentlyHoveredBaseDraggable;
		if (!(currentlyHoveredBaseDraggable == null))
		{
			switch (currentlyHoveredBaseDraggable.ToggleMergingAllowed())
			{
			case 0:
				SingletonController<AudioController>.Instance.PlaySFXClip(_mergeLockActive, 0.7f);
				ClearPotentialLines();
				break;
			case 1:
				SingletonController<AudioController>.Instance.PlaySFXClip(_mergeLockInactive, 0.7f);
				DrawLinesToPotentialMergables(currentlyHoveredBaseDraggable);
				currentlyHoveredBaseDraggable.BaseItemInstance.ClearActiveToast();
				break;
			}
			SingletonController<MergeToastController>.Instance.ShowCombiningLockedToast(currentlyHoveredBaseDraggable);
			UpdateMergePossibilities();
			SingletonController<BackpackController>.Instance.BackpackStorage.UpdateTimeOfLastChange();
		}
	}

	private List<BaseItemInstance> GetAllBaseItemInstances(bool includeShop = true, bool includeStorage = true)
	{
		List<BaseItemInstance> list = new List<BaseItemInstance>();
		ShopController controllerByType = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		list.AddRange(SingletonController<BackpackController>.Instance.GetItemsFromBackpack());
		list.AddRange(SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack());
		if (includeStorage)
		{
			list.AddRange(SingletonController<BackpackController>.Instance.GetWeaponsFromStorage());
		}
		if (includeStorage)
		{
			list.AddRange(SingletonController<BackpackController>.Instance.GetItemsFromStorage());
		}
		if (includeShop)
		{
			list.AddRange(controllerByType.GetWeaponsFromShop());
		}
		if (includeShop)
		{
			list.AddRange(controllerByType.GetItemsFromShop());
		}
		return list;
	}

	public override void Clear()
	{
		_previousMergeRecipeSets = new List<MergeRecipeSet>();
		_previousMergeRecipeSets.Clear();
		_mergeRecipeSets = new List<MergeRecipeSet>();
		_mergeRecipeSets.Clear();
	}

	public override void ClearAdventure()
	{
		Clear();
	}

	private Vector2 GetCorrectionFactor()
	{
		return Vector2.one;
	}

	private Vector2 GetResolutionCorrectionFactor()
	{
		float width = Camera.main.rect.width;
		float height = Camera.main.rect.height;
		return new Vector2(width, height);
	}

	internal void UpdatePotentialItems()
	{
		List<BaseItemInstance> allBaseItemInstances = GetAllBaseItemInstances();
		foreach (BaseItemInstance item in allBaseItemInstances.Where((BaseItemInstance x) => x.MergingAllowed))
		{
			item.ClearPotentialMergeItems();
			foreach (MergableSO item2 in _mergableSOs.Where((MergableSO x) => x.Input.Any((MergeableIngredient mergeableIngredient) => mergeableIngredient.ItemName == item.BaseItemSO.Name)))
			{
				foreach (MergeableIngredient otherItemNeededForThisRecipe in item2.Input.Where((MergeableIngredient x) => x.ItemName != item.BaseItemSO.Name))
				{
					IEnumerable<BaseItemInstance> source = allBaseItemInstances.Where((BaseItemInstance x) => x.BaseItemSO.Name == otherItemNeededForThisRecipe.ItemName && x.MergingAllowed);
					if (source.Any())
					{
						item.AddToPotentialMergeItems(source.Select((BaseItemInstance x) => x.Guid).ToList());
					}
				}
			}
		}
	}

	internal void DrawLinesToPotentialMergables(BaseDraggable draggable)
	{
		if (!draggable.BaseItemInstance.MergingAllowed)
		{
			return;
		}
		ClearPotentialLines();
		BaseDraggable[] source = UnityEngine.Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None);
		List<MergeLineSet> list = new List<MergeLineSet>();
		foreach (Guid draggableGuid in draggable.BaseItemInstance.PotentialMergeItems)
		{
			BaseDraggable endDraggable = source.FirstOrDefault((BaseDraggable x) => x.BaseItemInstance.Guid == draggableGuid);
			list.Add(new MergeLineSet(draggable, endDraggable));
		}
		foreach (MergeLineSet item in list)
		{
			if (!(item.StartDraggable == null) && !(item.EndDraggable == null))
			{
				PotentialMergableItemLine potentialMergableItemLine = UnityEngine.Object.Instantiate(_couldMergableLinePrefab, _potentialmergableLineContainer);
				Vector2 startPos = item.StartDraggable.transform.position * GetScaleMod();
				Vector2 vector = item.EndDraggable.transform.position * GetScaleMod();
				potentialMergableItemLine.Init(startPos, vector);
			}
		}
	}

	internal void ClearPotentialLines()
	{
		_potentialMergableLines.Clear();
		for (int num = _potentialmergableLineContainer.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(_potentialmergableLineContainer.GetChild(num).gameObject);
		}
	}

	internal void UpdateIncompleteMergeItemsNew()
	{
		List<BaseItemInstance> allBaseItemInstances = GetAllBaseItemInstances(includeShop: false, includeStorage: false);
		foreach (BaseItemInstance item in allBaseItemInstances.Where((BaseItemInstance x) => x.MergingAllowed && x.CurrentMergeRecipeSet == null))
		{
			if (item.CurrentMergeRecipeSet != null)
			{
				continue;
			}
			IEnumerable<MergableSO> enumerable = _mergableSOs.Where((MergableSO x) => x.Input.Any((MergeableIngredient mergeableIngredient) => mergeableIngredient.ItemName == item.BaseItemSO.Name && mergeableIngredient.IsPrimary));
			List<BaseItemInstance> adjacentBaseItemInstances = new List<BaseItemInstance>();
			SingletonController<BackpackController>.Instance.BackpackStorage.GetAdjacentPlaceables(item, out adjacentBaseItemInstances);
			List<BaseItemInstance> list = new List<BaseItemInstance>();
			adjacentBaseItemInstances = adjacentBaseItemInstances.Where((BaseItemInstance x) => x.MergingAllowed).ToList();
			list.AddRange(adjacentBaseItemInstances);
			adjacentBaseItemInstances.Add(item);
			foreach (MergableSO item2 in enumerable)
			{
				List<MergeableIngredient> input = item2.Input;
				bool flag = false;
				foreach (MergeableIngredient itemNeededForThisRecipe in input)
				{
					bool flag2 = list.Any((BaseItemInstance x) => x.BaseItemSO.Name == itemNeededForThisRecipe.ItemName);
					int num = item2.Input.Count((MergeableIngredient x) => x.BaseItem.Id == item.Id);
					if ((itemNeededForThisRecipe.BaseItem.Id != item.Id || num != 1) && flag2)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					continue;
				}
				List<BaseItemInstance> list2 = new List<BaseItemInstance>();
				list2.Add(item);
				foreach (MergeableIngredient itemNeededForThisRecipe2 in input)
				{
					IEnumerable<BaseItemInstance> source = adjacentBaseItemInstances.Where((BaseItemInstance x) => x.BaseItemSO.Name == itemNeededForThisRecipe2.ItemName);
					IEnumerable<BaseItemInstance> source2 = source.Where((BaseItemInstance x) => x.CurrentMergeRecipeSet == null && x.MergingAllowed);
					int num2 = source.Count() - source2.Count();
					_ = itemNeededForThisRecipe2.Amount;
					source.Count();
					int num3 = ((itemNeededForThisRecipe2.ItemName == item.BaseItemSO.Name) ? 1 : 0);
					IEnumerable<BaseItemInstance> collection = source2.Take(itemNeededForThisRecipe2.Amount - num3);
					list2.AddRange(collection);
				}
				if (list2.Select((BaseItemInstance x) => x.Id).Distinct().Count() <= 1)
				{
					continue;
				}
				foreach (BaseItemInstance item3 in list2)
				{
					MergeRecipeSet mergeRecipeSet = new MergeRecipeSet();
					mergeRecipeSet.SetData(item2, list2.ToList());
					item3.CurrentMergeRecipeSet = mergeRecipeSet;
				}
			}
		}
		foreach (BaseItemInstance primaryItemInMerges in allBaseItemInstances.Where((BaseItemInstance x) => x.CurrentMergeRecipeSet != null && x.CurrentMergeRecipeSet.PrimaryBaseItemInstance.Guid == x.Guid))
		{
			if (primaryItemInMerges.CurrentMergeRecipeSet != null && !_mergeRecipeSets.Any((MergeRecipeSet x) => x.GetHashCode() == primaryItemInMerges.CurrentMergeRecipeSet.GetHashCode()))
			{
				primaryItemInMerges.CurrentMergeRecipeSet.RecipeIsComplete = false;
				_mergeRecipeSets.Add(primaryItemInMerges.CurrentMergeRecipeSet);
			}
		}
	}

	internal void DrawIncompleteMergableLines()
	{
		ClearIncompleteMergableLines();
		List<MergeLineSet> list = new List<MergeLineSet>();
		BaseDraggable[] source = UnityEngine.Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None);
		GetAllBaseItemInstances(includeShop: false, includeStorage: false);
		foreach (MergeRecipeSet item in _mergeRecipeSets.Where((MergeRecipeSet x) => !x.RecipeIsComplete))
		{
			foreach (BaseItemInstance start in item.AvailableItemsForMerge)
			{
				foreach (BaseItemInstance end in item.AvailableItemsForMerge)
				{
					BaseDraggable startItem = source.FirstOrDefault((BaseDraggable x) => x.BaseItemInstance.Guid == start.Guid);
					BaseDraggable endItem = source.FirstOrDefault((BaseDraggable x) => x.BaseItemInstance.Guid == end.Guid);
					bool flag = list.Any((MergeLineSet x) => (x.StartDraggable == startItem && x.EndDraggable == endItem) || (x.EndDraggable == startItem && x.StartDraggable == endItem));
					if (startItem != null && endItem != null && startItem.BaseItemSO.Name != endItem.BaseItemSO.Name && !flag)
					{
						list.Add(new MergeLineSet(startItem, endItem));
					}
				}
			}
		}
		foreach (MergeLineSet item2 in list)
		{
			IncompleteMergableItemLine incompleteMergableItemLine = UnityEngine.Object.Instantiate(_incompleteMergableLinePrefab, _incompleteMergableLineContainer);
			GetCorrectionFactor();
			Vector2 startPos = item2.StartDraggable.transform.position * GetScaleMod();
			Vector2 vector = item2.EndDraggable.transform.position * GetScaleMod();
			incompleteMergableItemLine.Init(startPos, vector);
		}
	}

	internal void ClearIncompleteMergableLines()
	{
		_incompleteMergableLines.Clear();
		for (int num = _incompleteMergableLineContainer.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(_incompleteMergableLineContainer.GetChild(num).gameObject);
		}
	}

	internal void UpdateCompleteMergeItemsNew()
	{
		List<BaseItemInstance> allBaseItemInstances = GetAllBaseItemInstances(includeShop: false, includeStorage: false);
		List<BaseItemInstance> list = new List<BaseItemInstance>();
		foreach (BaseItemInstance item2 in allBaseItemInstances)
		{
			item2.CurrentMergeRecipeSet = null;
		}
		foreach (BaseItemInstance item in allBaseItemInstances.Where((BaseItemInstance x) => x.MergingAllowed))
		{
			if (list.Contains(item))
			{
				continue;
			}
			IEnumerable<MergableSO> enumerable = _mergableSOs.Where((MergableSO x) => x.Input.Any((MergeableIngredient mergeableIngredient) => mergeableIngredient.ItemName == item.BaseItemSO.Name && mergeableIngredient.IsPrimary));
			List<BaseItemInstance> adjacentBaseItemInstances = new List<BaseItemInstance>();
			SingletonController<BackpackController>.Instance.BackpackStorage.GetAdjacentPlaceables(item, out adjacentBaseItemInstances);
			adjacentBaseItemInstances.Add(item);
			foreach (MergableSO item3 in enumerable)
			{
				bool flag = true;
				List<MergeableIngredient> input = item3.Input;
				List<BaseItemInstance> list2 = new List<BaseItemInstance> { item };
				foreach (MergeableIngredient itemNeededForThisRecipe in input)
				{
					IEnumerable<BaseItemInstance> source = adjacentBaseItemInstances.Where((BaseItemInstance x) => x.BaseItemSO.Name == itemNeededForThisRecipe.ItemName && x.MergingAllowed);
					IEnumerable<BaseItemInstance> source2 = source.Where((BaseItemInstance x) => x.CurrentMergeRecipeSet == null);
					int num = source.Count() - source2.Count();
					if (itemNeededForThisRecipe.Amount > source.Count() - num)
					{
						flag = false;
						continue;
					}
					int num2 = ((itemNeededForThisRecipe.ItemName == item.BaseItemSO.Name) ? 1 : 0);
					IEnumerable<BaseItemInstance> collection = source2.Take(itemNeededForThisRecipe.Amount - num2);
					list2.AddRange(collection);
				}
				if (!flag)
				{
					continue;
				}
				list.AddRange(list2);
				foreach (BaseItemInstance item4 in list2)
				{
					MergeRecipeSet mergeRecipeSet = new MergeRecipeSet();
					mergeRecipeSet.RecipeIsComplete = true;
					mergeRecipeSet.SetData(item3, list2.ToList());
					item4.CurrentMergeRecipeSet = mergeRecipeSet;
				}
			}
		}
		foreach (BaseItemInstance primaryItemsInMerges in allBaseItemInstances.Where((BaseItemInstance x) => x.CurrentMergeRecipeSet != null && x.CurrentMergeRecipeSet.PrimaryBaseItemInstance.Guid == x.Guid))
		{
			if (primaryItemsInMerges.CurrentMergeRecipeSet != null && primaryItemsInMerges.CurrentMergeRecipeSet.RecipeIsComplete && !_mergeRecipeSets.Any((MergeRecipeSet x) => x.GetHashCode() == primaryItemsInMerges.CurrentMergeRecipeSet.GetHashCode()))
			{
				primaryItemsInMerges.CurrentMergeRecipeSet.RecipeIsComplete = true;
				_mergeRecipeSets.Add(primaryItemsInMerges.CurrentMergeRecipeSet);
			}
		}
	}

	internal void DrawCompleteMergableLines()
	{
		ClearCompleteMergableLines();
		List<MergeLineSet> list = new List<MergeLineSet>();
		BaseDraggable[] source = UnityEngine.Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None);
		GetAllBaseItemInstances(includeShop: false, includeStorage: false);
		foreach (MergeRecipeSet item in _mergeRecipeSets.Where((MergeRecipeSet x) => x.RecipeIsComplete))
		{
			foreach (BaseItemInstance start in item.AvailableItemsForMerge)
			{
				foreach (BaseItemInstance end in item.AvailableItemsForMerge)
				{
					BaseDraggable startItem = source.FirstOrDefault((BaseDraggable x) => x.BaseItemInstance.Guid == start.Guid);
					BaseDraggable endItem = source.FirstOrDefault((BaseDraggable x) => x.BaseItemInstance.Guid == end.Guid);
					bool flag = list.Any((MergeLineSet x) => (x.StartDraggable == startItem && x.EndDraggable == endItem) || (x.EndDraggable == startItem && x.StartDraggable == endItem));
					if (startItem != null && endItem != null && startItem.BaseItemSO.Name != endItem.BaseItemSO.Name && !flag)
					{
						list.Add(new MergeLineSet(startItem, endItem));
					}
				}
			}
		}
		foreach (MergeLineSet item2 in list)
		{
			MergableItemLine mergableItemLine = UnityEngine.Object.Instantiate(_mergableLinePrefab, _mergableLineContainer);
			GetCorrectionFactor();
			Vector2 startPos = item2.StartDraggable.ActualPosition * GetScaleMod();
			Vector2 vector = item2.EndDraggable.ActualPosition * GetScaleMod();
			mergableItemLine.Init(startPos, vector);
		}
	}

	private float GetScaleMod()
	{
		return 1f / _canvas.transform.localScale.y;
	}

	internal void ClearCompleteMergableLines()
	{
		_mergableLines.Clear();
		for (int num = _mergableLineContainer.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(_mergableLineContainer.GetChild(num).gameObject);
		}
	}

	[Command("merge.Execute", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void MergeMergables()
	{
		StartCoroutine(MergeMergablesAsync());
	}

	private IEnumerator MergeMergablesAsync()
	{
		List<Mergable> mergables = new List<Mergable>();
		foreach (MergeRecipeSet item2 in _mergeRecipeSets.Where((MergeRecipeSet x) => x.RecipeIsComplete))
		{
			Mergable item = new Mergable(item2.PrimaryBaseItemInstance);
			mergables.Add(item);
			SingletonController<CollectionController>.Instance.UnlockRecipe(item2.Recipe.Id);
		}
		SingletonController<InputController>.Instance.SetCursorEnabled(enabled: false);
		SingletonController<InputController>.Instance.SetInputEnabled(enabled: false);
		SingletonCacheController.Instance.GetControllerByType<ShopController>().SetButtonInteraction(allowInteraction: false);
		float singleMergeAnimationDuration = 1f + 1f / (float)mergables.Count();
		ClearCompleteMergableLines();
		if (mergables.Any())
		{
			yield return new WaitForSecondsRealtime(1f);
		}
		foreach (Mergable mergable in mergables)
		{
			if (mergable.CanPlaceMergeResult(showCannotPlaceMergeResultToast: true))
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_mergeStartAudio, 1f, 0f, 0.7f);
				Transform coreItemInMerge = mergable.AnimateMerge(singleMergeAnimationDuration);
				UnityEngine.Object.Instantiate(_backpackMergeItemVFX, coreItemInMerge);
				yield return new WaitForSecondsRealtime(singleMergeAnimationDuration);
				SingletonController<AudioController>.Instance.PlaySFXClip(_mergeAudio, 1f, 0.2f);
				SingletonController<BackpackController>.Instance.AddToCanvas(UnityEngine.Object.Instantiate(_backpackMergeFinishedItemVFX, coreItemInMerge).transform);
				mergable.ExecuteMerge();
			}
		}
		SingletonController<InputController>.Instance.SetCursorEnabled(enabled: true);
		SingletonController<InputController>.Instance.SetInputEnabled(enabled: true);
		UpdateMergePossibilities();
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: false);
		SingletonController<GameController>.Instance.Player.ResetVisuals();
		SingletonController<GameController>.Instance.Player.RefreshVisuals();
		SingletonCacheController.Instance.GetControllerByType<ShopController>().SetButtonInteraction(allowInteraction: true);
	}
}
