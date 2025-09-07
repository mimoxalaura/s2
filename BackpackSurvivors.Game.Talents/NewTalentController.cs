using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Talents.Events;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Talents;

public class NewTalentController : BaseSingletonModalUIController<NewTalentController>, IClearable
{
	public delegate void OnTalentSelectedHandler(object sender, TalentSelectedEventArgs e);

	[Header("Base")]
	[SerializeField]
	private TalentCameraDrag _talentCameraDrag;

	[Header("Prefabs")]
	[SerializeField]
	private TalentConnectorLine _talentConnectorLinePrefab;

	[Header("Lines")]
	[SerializeField]
	private TalentConnectorLine _talentActiveConnectorLinePrefab;

	[SerializeField]
	private Transform _activeLinesContainer;

	[Header("Structure")]
	[SerializeField]
	private Transform _talentConnectorLineContainer;

	[SerializeField]
	private List<TalentConnector> _connectors;

	[SerializeField]
	private List<CenterTalentConnector> _centerConnectors;

	[SerializeField]
	private Transform _talentTreeComponentParent;

	[SerializeField]
	private SerializableDictionaryBase<int, List<int>> _graph;

	[Header("Center")]
	[SerializeField]
	private TalentTreeCenter _centerNode;

	[SerializeField]
	private Image _centerImage;

	[Header("Canvas")]
	[SerializeField]
	private Canvas _talentsCanvas;

	[SerializeField]
	private Canvas _uiCanvas;

	[SerializeField]
	private Canvas _backdropCanvas;

	[SerializeField]
	private TextMeshProUGUI _talentPointsUI;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _talentPointActivatedAudioClip;

	[SerializeField]
	private AudioClip _talentPointDeactivatedAudioClip;

	[SerializeField]
	private AudioClip _keystoneActivatedAudioClip;

	[SerializeField]
	private AudioClip _keystoneDeactivatedAudioClip;

	[Header("Searching")]
	[SerializeField]
	private TMP_InputField _searchBar;

	[SerializeField]
	private bool _DEBUG;

	[SerializeField]
	private List<TalentSO> DEBUGTALENTSOS;

	private Dictionary<int, bool> _talentUnlockedStates = new Dictionary<int, bool>();

	private List<TalentConnectorLine> _talentConnectorLines;

	public int TalentPoints { get; private set; }

	public List<TalentNode> Nodes { get; set; } = new List<TalentNode>();

	public event OnTalentSelectedHandler OnTalentSelected;

	private void Start()
	{
		RegisterEvents();
		UpdateTalentPoints();
		UpdateTalentPointText();
	}

	public List<TalentSO> GetActiveTalents()
	{
		if (_DEBUG)
		{
			return DEBUGTALENTSOS;
		}
		List<TalentSO> list = new List<TalentSO>();
		foreach (KeyValuePair<int, bool> item in _talentUnlockedStates.Where((KeyValuePair<int, bool> x) => x.Key > 0 && x.Value))
		{
			list.Add(GameDatabaseHelper.GetTalentFromId(item.Key));
		}
		return list;
	}

	[Command("talent.addpoints", Platform.AllPlatforms, MonoTargetType.Single)]
	public void AddTalentPoints(int points)
	{
		TalentPoints += points;
	}

	[Command("talent.removepoints", Platform.AllPlatforms, MonoTargetType.Single)]
	public void RemoveTalentPoints(int points)
	{
		TalentPoints -= points;
	}

	public bool GetUnlockedState(int talentId)
	{
		switch (talentId)
		{
		case -1:
			return true;
		case 0:
			return true;
		default:
			if (!_talentUnlockedStates.ContainsKey(talentId))
			{
				return false;
			}
			return _talentUnlockedStates[talentId];
		}
	}

	internal void UpdateTalentPoints()
	{
		_graph.Clear();
		Nodes = new List<TalentNode>();
		AddNode(_centerNode);
		_graph.Add(-1, new List<int>());
		_talentUnlockedStates.Clear();
		List<TalentTreeComponent> list = new List<TalentTreeComponent>();
		foreach (Transform item in _talentTreeComponentParent)
		{
			TalentTreeComponent component = item.GetComponent<TalentTreeComponent>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		foreach (TalentTreeComponent item2 in list)
		{
			foreach (TalentTreePoint item3 in item2.UpdateTalentPoints(_DEBUG))
			{
				if (!_graph.ContainsKey(item3.GetId()))
				{
					AddNode(item3);
					_graph.Add(item3.GetId(), new List<int>());
					_talentUnlockedStates.Add(item3.GetId(), value: false);
					item3.OnTalentSelected += Item_OnTalentSelected;
				}
				_graph[item3.GetId()].AddRange(item3.Neighbors.Select((TalentTreePoint x) => x.GetId()));
			}
		}
		ClearConnectors();
		AddConnectors();
		AddCenterConnectors();
		CreateConnectorLines();
		CreateCenterConnectorLines();
		_talentUnlockedStates[-1] = true;
	}

	private void ClearConnectors()
	{
		_talentConnectorLines = new List<TalentConnectorLine>();
		for (int num = _talentConnectorLineContainer.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.DestroyImmediate(_talentConnectorLineContainer.GetChild(num).gameObject);
		}
	}

	private void CreateConnectorLines()
	{
		foreach (TalentConnector connector in _connectors)
		{
			TalentNode talentNode = Nodes.FirstOrDefault((TalentNode x) => x.GetId() == connector.TalentOne.Id);
			TalentNode talentNode2 = Nodes.FirstOrDefault((TalentNode x) => x.GetId() == connector.TalentTwo.Id);
			if (talentNode != null && talentNode2 != null)
			{
				TalentConnectorLine talentConnectorLine = UnityEngine.Object.Instantiate(_talentConnectorLinePrefab, _talentConnectorLineContainer);
				talentConnectorLine.Init(talentNode, talentNode2);
				_talentConnectorLines.Add(talentConnectorLine);
			}
		}
	}

	private void CreateCenterConnectorLines()
	{
		foreach (CenterTalentConnector connector in _centerConnectors)
		{
			TalentConnectorLine talentConnectorLine = UnityEngine.Object.Instantiate(_talentConnectorLinePrefab, _talentConnectorLineContainer);
			TalentNode talentNode = Nodes.FirstOrDefault((TalentNode x) => x.GetId() == connector.TalentOne.Id);
			if (talentNode != null && _centerNode != null)
			{
				talentConnectorLine.Init(talentNode, _centerNode);
			}
			_talentConnectorLines.Add(talentConnectorLine);
		}
	}

	private void AddConnectors()
	{
		foreach (TalentConnector connector in _connectors)
		{
			if (!_graph.ContainsKey(connector.TalentOne.Id))
			{
				_graph.Add(connector.TalentOne.Id, new List<int>());
			}
			if (!_graph[connector.TalentOne.Id].Contains(connector.TalentTwo.Id))
			{
				_graph[connector.TalentOne.Id].Add(connector.TalentTwo.Id);
			}
			if (!_graph.ContainsKey(connector.TalentTwo.Id))
			{
				_graph.Add(connector.TalentTwo.Id, new List<int>());
			}
			if (!_graph[connector.TalentTwo.Id].Contains(connector.TalentOne.Id))
			{
				_graph[connector.TalentTwo.Id].Add(connector.TalentOne.Id);
			}
		}
	}

	private void AddCenterConnectors()
	{
		foreach (CenterTalentConnector centerConnector in _centerConnectors)
		{
			if (!_graph.ContainsKey(centerConnector.TalentOne.Id))
			{
				_graph.Add(centerConnector.TalentOne.Id, new List<int>());
			}
			if (!_graph[centerConnector.TalentOne.Id].Contains(-1))
			{
				_graph[centerConnector.TalentOne.Id].Add(-1);
			}
			if (!_graph.ContainsKey(-1))
			{
				_graph.Add(-1, new List<int>());
			}
			if (!_graph[-1].Contains(centerConnector.TalentOne.Id))
			{
				_graph[-1].Add(centerConnector.TalentOne.Id);
			}
		}
	}

	private void AddNode(TalentNode node)
	{
		Nodes.Add(node);
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegistedSaveGameLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, RegisterCharacterLeveled);
		_searchBar.onValueChanged.AddListener(SearchForTalents);
	}

	public void RegistedSaveGameLoaded()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	public void RegisterCharacterLeveled()
	{
		SingletonController<CharactersController>.Instance.OnCharacterLeveled += CharactersController_OnCharacterLeveled;
	}

	private void CharactersController_OnCharacterLeveled(object sender, CharacterLeveledEventArgs e)
	{
		AddTalentPoints(1);
		SingletonController<SaveGameController>.Instance.SaveProgression();
	}

	public TalentSaveState GetSaveState()
	{
		TalentSaveState talentSaveState = new TalentSaveState();
		talentSaveState.SetState(_talentUnlockedStates, TalentPoints);
		return talentSaveState;
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		DeactivateAllPoints();
		LoadFromSave(e.SaveGame.TalentsState);
		base.IsInitialized = true;
	}

	private void LoadFromSave(TalentSaveState progressionTalentState)
	{
		_talentUnlockedStates.Clear();
		for (int i = 0; i < GameDatabaseHelper.GetTalents().Count; i++)
		{
			_talentUnlockedStates[i] = false;
		}
		foreach (KeyValuePair<int, bool> talentUnloackedState in progressionTalentState.TalentUnloackedStates)
		{
			_talentUnlockedStates[talentUnloackedState.Key] = talentUnloackedState.Value;
		}
		TalentPoints = progressionTalentState.AvailableTalentPoints;
		foreach (KeyValuePair<int, bool> item in _talentUnlockedStates.Where((KeyValuePair<int, bool> x) => x.Value))
		{
			TalentNode talentNode = Nodes.FirstOrDefault((TalentNode x) => x.GetId() == item.Key);
			if (talentNode != null)
			{
				talentNode.Activate(animate: false);
			}
		}
		RedrawActiveLines();
	}

	public bool TryToActivateTalent(int talentId)
	{
		if (!_talentUnlockedStates.ContainsKey(talentId))
		{
			return false;
		}
		if (TalentPoints <= 0)
		{
			return false;
		}
		if (!_talentUnlockedStates[talentId])
		{
			foreach (int item in _graph[talentId])
			{
				if (GetUnlockedState(item))
				{
					TalentPoints--;
					ActivateTalent(talentId);
					return true;
				}
			}
		}
		return false;
	}

	private void ActivateTalent(int talentId, bool playAudio = true, bool triggerEvents = true)
	{
		_talentUnlockedStates[talentId] = true;
		Nodes.FirstOrDefault((TalentNode x) => x.GetId() == talentId).Activate();
		if (playAudio)
		{
			PlayAudio(talentId);
		}
		RedrawActiveLines();
		UpdateTalentPointText();
		if (triggerEvents)
		{
			this.OnTalentSelected?.Invoke(this, new TalentSelectedEventArgs(null, wasActive: false));
		}
	}

	private void UpdateTalentPointText()
	{
		_talentPointsUI.SetText($"{TalentPoints}/{TalentPoints + Nodes.Count((TalentNode x) => x.IsActive())}");
	}

	public bool TryToDeactivateTalent(int talentId)
	{
		if (!_talentUnlockedStates.ContainsKey(talentId))
		{
			return false;
		}
		if (_talentUnlockedStates[talentId])
		{
			List<int> lockedTalents = (from x in _talentUnlockedStates
				where !x.Value
				select x.Key).ToList();
			foreach (int connectedTalent in GetConnectedTalents(talentId))
			{
				if (GetUnlockedState(connectedTalent) && FindPathToCenter(connectedTalent, talentId, lockedTalents, (int x) => x == 0) != 0)
				{
					return false;
				}
			}
			TalentPoints++;
			DeActivateTalent(talentId);
		}
		return true;
	}

	private void DeActivateTalent(int talentId, bool playAudio = true, bool triggerEvents = true)
	{
		_talentUnlockedStates[talentId] = false;
		Nodes.FirstOrDefault((TalentNode x) => x.GetId() == talentId).Deactivate();
		if (playAudio)
		{
			PlayAudio(talentId);
		}
		RedrawActiveLines();
		UpdateTalentPointText();
		if (triggerEvents)
		{
			this.OnTalentSelected?.Invoke(this, new TalentSelectedEventArgs(null, wasActive: false));
		}
	}

	public void DeactivateAllPoints()
	{
		foreach (TalentNode node in Nodes)
		{
			node.Deactivate();
		}
	}

	public void ResetPoints(bool save = true)
	{
		int num = _talentUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Key > 0 && x.Value);
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, bool> item2 in _talentUnlockedStates.Where((KeyValuePair<int, bool> x) => x.Key > 0 && x.Value))
		{
			list.Add(item2.Key);
		}
		foreach (int item in list)
		{
			_talentUnlockedStates[item] = false;
			TalentNode talentNode = Nodes.FirstOrDefault((TalentNode x) => x.GetId() == item);
			if (talentNode != null)
			{
				talentNode.Deactivate();
			}
		}
		TalentPoints += num;
		UpdateTalentPointText();
		RedrawActiveLines();
		if (save)
		{
			SingletonController<SaveGameController>.Instance.SaveProgression();
		}
	}

	private void RedrawActiveLines()
	{
		for (int num = _activeLinesContainer.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(_activeLinesContainer.GetChild(num).gameObject);
		}
		foreach (KeyValuePair<int, bool> item in _talentUnlockedStates.Where((KeyValuePair<int, bool> x) => x.Value))
		{
			foreach (int unlockedPrevTalentId in _graph[item.Key].Where((int x) => GetUnlockedState(x)))
			{
				TalentNode talentNode = Nodes.FirstOrDefault((TalentNode x) => x.GetId() == item.Key);
				TalentNode talentNode2 = Nodes.FirstOrDefault((TalentNode x) => x.GetId() == unlockedPrevTalentId);
				if (talentNode != null && talentNode2 != null)
				{
					UnityEngine.Object.Instantiate(_talentActiveConnectorLinePrefab, _activeLinesContainer).Init(talentNode, talentNode2);
				}
			}
		}
	}

	private void PlayAudio(int talentId)
	{
		TalentNode talentNode = Nodes.FirstOrDefault((TalentNode x) => x.GetId() == talentId);
		if (!(talentNode == null))
		{
			AudioClip audioClipToPlay = GetAudioClipToPlay(talentNode.IsKeystone(), talentNode.IsActive());
			SingletonController<AudioController>.Instance.PlaySFXClip(audioClipToPlay, 0.5f);
		}
	}

	private AudioClip GetAudioClipToPlay(bool isKeyStoneTalentTreePoint, bool isActive)
	{
		if (!isKeyStoneTalentTreePoint)
		{
			return GetTalentPointAudioClip(isActive);
		}
		return GetKeystoneAudioClip(isActive);
	}

	private AudioClip GetKeystoneAudioClip(bool isActive)
	{
		if (isActive)
		{
			return _keystoneActivatedAudioClip;
		}
		return _keystoneDeactivatedAudioClip;
	}

	private AudioClip GetTalentPointAudioClip(bool isActive)
	{
		if (isActive)
		{
			return _talentPointActivatedAudioClip;
		}
		return _talentPointDeactivatedAudioClip;
	}

	public int FindPathToCenter(int talentId, int rootTalentId, List<int> lockedTalents, Predicate<int> match)
	{
		if (talentId == -1)
		{
			return 0;
		}
		Queue<int> queue = new Queue<int>();
		queue.Enqueue(talentId);
		List<int> list = new List<int>();
		list.Add(rootTalentId);
		list.AddRange(lockedTalents);
		while (queue.Count > 0)
		{
			int num = queue.Dequeue();
			if (match(num))
			{
				return num;
			}
			foreach (int connectedTalent in GetConnectedTalents(num))
			{
				if (connectedTalent == -1)
				{
					return 0;
				}
				if (!list.Contains(connectedTalent))
				{
					queue.Enqueue(connectedTalent);
				}
			}
			list.Add(num);
		}
		return -1;
	}

	public List<int> GetConnectedTalents(int talentId)
	{
		return _graph[talentId];
	}

	public override void OpenUI()
	{
		base.OpenUI();
		_centerImage.sprite = SingletonController<CharactersController>.Instance.ActiveCharacter.Face;
		UpdateTalentPointText();
		SingletonController<StatController>.Instance.SetCamerasEnabled(enabled: true);
		SingletonController<StatController>.Instance.SetDisplayToggleButton(showButton: true);
		SingletonController<InputController>.Instance.SwitchToUIActionMap();
		SingletonController<GameController>.Instance.SetGamePaused(gamePaused: false);
		SingletonController<CursorController>.Instance.TriggerDefaultCursorState();
		_talentsCanvas.gameObject.SetActive(value: true);
		_uiCanvas.gameObject.SetActive(value: true);
		_backdropCanvas.gameObject.SetActive(value: true);
		foreach (TalentConnectorLine talentConnectorLine in _talentConnectorLines)
		{
			talentConnectorLine.RefreshPosition();
		}
		RedrawActiveLines();
	}

	public override void CloseUI()
	{
		base.CloseUI();
		SingletonController<StatController>.Instance.SetCamerasEnabled(enabled: false);
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: true);
		SingletonController<SaveGameController>.Instance.SaveProgression();
		_talentCameraDrag.Reset();
		SingletonController<StatController>.Instance.CloseUI();
		SingletonController<StatController>.Instance.SetDisplayToggleButton(showButton: false);
		SingletonController<InputController>.Instance.RevertToPreviousActionMap();
		SingletonController<CursorController>.Instance.TriggerDefaultCursorState();
		_talentsCanvas.gameObject.SetActive(value: false);
		_uiCanvas.gameObject.SetActive(value: false);
		_backdropCanvas.gameObject.SetActive(value: false);
	}

	public void ButtonClosePressed()
	{
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().CloseModalUI(GetModalUIType());
	}

	public void SearchForTalents(string value)
	{
		foreach (TalentNode node in Nodes)
		{
			node.SetHighlightInSearch(highlight: false);
		}
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		Nodes.OrderBy((TalentNode x) => x.GetId()).ToList();
		foreach (TalentNode item in Nodes.Where((TalentNode x) => x.FitsSearch(value)))
		{
			item.SetHighlightInSearch(highlight: true);
		}
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Talents;
	}

	public override void Clear()
	{
		_talentUnlockedStates.Clear();
		TalentPoints = 0;
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

	public override bool CloseOnCancelInput()
	{
		return true;
	}

	private void Item_OnTalentSelected(object sender, TalentSelectedEventArgs e)
	{
		if (!e.WasActive)
		{
			bool flag = TryToActivateTalent(e.Talent.Id);
		}
		else
		{
			bool flag = TryToDeactivateTalent(e.Talent.Id);
		}
	}

	internal bool HasAnyActiveNeighbors(int id)
	{
		if (_centerConnectors.Any((CenterTalentConnector x) => x.TalentOne.Id == id))
		{
			return true;
		}
		using (List<int>.Enumerator enumerator = _graph[id].GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				int current = enumerator.Current;
				if (_talentUnlockedStates.ContainsKey(current) && _talentUnlockedStates[current])
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}
}
