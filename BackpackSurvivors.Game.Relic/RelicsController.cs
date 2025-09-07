using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Assets.Game.Revive;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Relic;

public class RelicsController : BaseSingletonModalUIController<RelicsController>
{
	public delegate void RelicAddedHandler(object sender, RelicAddedEventArgs e);

	[SerializeField]
	private RelicRewardsUI _relicRewardUI;

	[SerializeField]
	private Relic _relicPrefab;

	[SerializeField]
	private Transform _relicContainer;

	[SerializeField]
	private Transform _activeRelicContainer;

	[SerializeField]
	private AudioClip _relicPickedAudio;

	[SerializeField]
	private AudioClip _relicRerollAudio;

	[SerializeField]
	private AudioClip _relicShowingAudio;

	private Enums.RelicSource _relicSource;

	public List<Relic> ActiveRelics;

	public int RemainingRerolls;

	internal bool RelicPickingInProgress { get; private set; }

	public event RelicAddedHandler OnRelicAdded;

	public override void AfterBaseAwake()
	{
		ActiveRelics = new List<Relic>();
	}

	private void Start()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<UnlocksController>.Instance, InitNumberOfRelicRerolls);
		base.IsInitialized = true;
	}

	private void InitNumberOfRelicRerolls()
	{
		RemainingRerolls = GetNumberOfRelicRerolls();
	}

	public override bool CloseOnCancelInput()
	{
		return false;
	}

	private void RegisterEvents()
	{
		_relicRewardUI.OnRelicRewardPicked += RelicRewardUI_OnRelicRewardPicked;
		_relicRewardUI.OnRerollClicked += _relicRewardUI_OnRerollClicked;
	}

	private void UnregisterEvents()
	{
		_relicRewardUI.OnRelicRewardPicked -= RelicRewardUI_OnRelicRewardPicked;
		_relicRewardUI.OnRerollClicked -= _relicRewardUI_OnRerollClicked;
	}

	private void AddRelic(Relic relic, bool addedByUser)
	{
		relic.RelicHandler.Setup(relic);
		ActiveRelics.Add(relic);
		relic.transform.SetParent(_activeRelicContainer);
		this.OnRelicAdded?.Invoke(this, new RelicAddedEventArgs(relic, addedByUser));
	}

	private void _relicRewardUI_OnRerollClicked(object sender, EventArgs e)
	{
		RerollRelicUI();
	}

	[Command("relics.add", Platform.AllPlatforms, MonoTargetType.Single)]
	public void AddRelicById(int relicId)
	{
		RelicSO relicSO = GameDatabaseHelper.GetRelics().FirstOrDefault((RelicSO x) => x.Id == relicId);
		if (relicSO != null)
		{
			Relic relic = UnityEngine.Object.Instantiate(_relicPrefab, _activeRelicContainer);
			relic.Init(relicSO);
			AddRelic(relic, addedByUser: false);
		}
	}

	public void ClearRelics()
	{
		foreach (Relic activeRelic in ActiveRelics)
		{
			activeRelic.RelicHandler.TearDown();
			UnityEngine.Object.Destroy(activeRelic.gameObject);
		}
		if (SingletonController<StatController>.Instance != null)
		{
			SingletonController<StatController>.Instance.ClearUIRelics();
		}
		ActiveRelics.Clear();
	}

	public List<Relic> GenerateRandomRelics()
	{
		ClearGeneratedRelics();
		List<Relic> rolledRelics = new List<Relic>();
		List<RelicSO> availableRelicSOs = GetAvailableRelicSOs();
		for (int i = 0; i < 3; i++)
		{
			List<RelicSO> list = availableRelicSOs.Where((RelicSO ar) => rolledRelics.All((Relic rr) => rr.RelicSO.Id != ar.Id)).ToList();
			if (list.Count() == 0)
			{
				break;
			}
			int index = UnityEngine.Random.Range(0, list.Count());
			RelicSO relicSO = list[index];
			Relic relic = UnityEngine.Object.Instantiate(_relicPrefab, _relicContainer);
			relic.Init(relicSO);
			rolledRelics.Add(relic);
		}
		return rolledRelics;
	}

	private List<RelicSO> GetAvailableRelicSOs()
	{
		List<RelicSO> source = (from x in GameDatabaseHelper.GetRelics()
			where x.RelicSource == _relicSource
			select x).ToList();
		IEnumerable<int> activeRelicIds = ActiveRelics.Select((Relic ar) => ar.RelicSO.Id);
		return source.Where((RelicSO r) => !activeRelicIds.Contains(r.Id)).ToList();
	}

	private void RelicRewardUI_OnRelicRewardPicked(object sender, RelicRewardPickedEventArgs e)
	{
		if (!RelicPickingInProgress)
		{
			SingletonController<CollectionController>.Instance.UnlockRelic(e.Relic.RelicSO.Id);
			StartCoroutine(RelicRewardPickedAsync(e));
		}
	}

	private IEnumerator RelicRewardPickedAsync(RelicRewardPickedEventArgs e)
	{
		RelicPickingInProgress = true;
		SingletonController<AudioController>.Instance.PlaySFXClip(_relicPickedAudio, 1f);
		yield return new WaitForSecondsRealtime(1f);
		SingletonController<AudioController>.Instance.FadeOutAudioSources();
		AddRelic(e.Relic, addedByUser: true);
		e.Relic.transform.SetParent(_activeRelicContainer);
		ClearGeneratedRelics();
		_relicRewardUI.RemoveRelics(instant: true);
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.RelicReward);
	}

	private void ClearGeneratedRelics()
	{
		for (int num = _relicContainer.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(_relicContainer.GetChild(num).gameObject);
		}
	}

	[Command("relics.reward", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		base.OpenUI();
		RegisterEvents();
		RelicPickingInProgress = false;
		_relicRewardUI.OpenUI();
		InitRelicUI();
		GameObject.Find("Weather")?.SetActive(value: false);
		GameObject.Find("WorkingGameplayCanvas")?.SetActive(value: false);
		GameObject.Find("MinimapCanvas")?.SetActive(value: false);
		GameObject.Find("AdventureVendorController")?.SetActive(value: false);
		SingletonCacheController.Instance.GetControllerByType<ReviveController>().SetReviveCountUIVisibility(visible: false);
	}

	public int GetNumberOfRelicRerolls()
	{
		return SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.RelicRerolls);
	}

	[Command("relics.SetSource", Platform.AllPlatforms, MonoTargetType.Single)]
	public void SetRelicSource(Enums.RelicSource relicSource)
	{
		_relicSource = relicSource;
	}

	[Command("relics.Init", Platform.AllPlatforms, MonoTargetType.Single)]
	private void InitRelicUI()
	{
		List<Relic> relics = GenerateRandomRelics();
		SingletonController<AudioController>.Instance.PlaySFXClip(_relicShowingAudio, 1f);
		_relicRewardUI.Init(relics);
	}

	[Command("relics.Reroll", Platform.AllPlatforms, MonoTargetType.Single)]
	private void RerollRelicUI()
	{
		List<Relic> relics = GenerateRandomRelics();
		SingletonController<AudioController>.Instance.PlaySFXClip(_relicRerollAudio, 1f);
		_relicRewardUI.RerollToGivenList(relics);
	}

	public override void CloseUI()
	{
		base.CloseUI();
		UnregisterEvents();
		_relicRewardUI.CloseUI();
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.RelicReward;
	}

	public override void Clear()
	{
		ClearRelics();
		InitNumberOfRelicRerolls();
	}

	public override void ClearAdventure()
	{
		ClearRelics();
		InitNumberOfRelicRerolls();
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
