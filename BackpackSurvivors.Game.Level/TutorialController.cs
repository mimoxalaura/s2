using System;
using System.Collections;
using BackpackSurvivors.Game.Analytics;
using BackpackSurvivors.Game.Analytics.Events;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.Game.World;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.GameplayFeedback;
using BackpackSurvivors.UI.Stats;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class TutorialController : SingletonController<TutorialController>
{
	public delegate void TutorialCompletedHandler(object sender, TutorialCompletedEventArgs e);

	[SerializeField]
	private AudioClip _fortressGateOpening;

	private Enums.Tutorial _activeTutorial;

	private float _tutorialStartTime;

	private Canvas _tutorialCanvas;

	private GameObject _workingGameplayCanvas;

	private Transform _finalPosition;

	private GameObject _fortressGate;

	private UnlockableInTown _unlockAltar;

	private UnlockableInTown _unlockPortal;

	private BackpackSurvivors.Game.Player.Player _player;

	private TitanicSoulsTutorialController _titanicSoulsTutorialController;

	public bool IsRunningTutorial { get; internal set; }

	public event TutorialCompletedHandler OnTutorialCompleted;

	private void Start()
	{
		base.IsInitialized = true;
		SingletonController<InputController>.Instance.OnCancelHandler += InputController_OnCancelHandler;
	}

	private void InputController_OnCancelHandler(object sender, EventArgs e)
	{
		LogTutorialSkipped();
		switch (_activeTutorial)
		{
		case Enums.Tutorial.Town:
			StartCoroutine(FinishTownTutorial());
			break;
		case Enums.Tutorial.Talents:
			StartCoroutine(FinishTalentTreeTutorial());
			break;
		case Enums.Tutorial.TitanicSouls:
			StartCoroutine(FinishTitanicSoulTutorial());
			break;
		case Enums.Tutorial.None:
		case Enums.Tutorial.Backpack:
			break;
		}
	}

	private void LogTutorialSkipped()
	{
		if (_activeTutorial != Enums.Tutorial.None)
		{
			float skippedTutorialAfterTime = Time.realtimeSinceStartup - _tutorialStartTime;
			TownTutorialSkippedEvent eventToRecord = new TownTutorialSkippedEvent
			{
				SkippedTutorialAfterTime = skippedTutorialAfterTime,
				ActiveTutorialEnumWhenSkipped = (int)_activeTutorial
			};
			SingletonController<AnalyticsController>.Instance.RecordEvent(eventToRecord);
		}
	}

	internal TutorialSaveState GetSaveState()
	{
		TutorialSaveState tutorialSaveState = new TutorialSaveState();
		tutorialSaveState.SetState(SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownTownTutorial, SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownTalentTutorial, SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownBackpackTutorial, SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownTitanicSoulTutorial);
		return tutorialSaveState;
	}

	public void ShowTownTutorial(BackpackSurvivors.Game.Player.Player player, TownController townController, UnlockableInTown unlockAltar, UnlockableInTown unlockPortal, Transform infrontOfGatePosition, Transform centerTownPosition, Transform infrontOfPortalPosition, Transform infrontOfUnlockAltarPosition, GameObject fortressGate, GameObject workingGameplayCanvas, GameObject _movementTip, Canvas tutorialCanvas)
	{
		_tutorialStartTime = Time.realtimeSinceStartup;
		SingletonController<GameController>.Instance.CanOpenMenu = false;
		IsRunningTutorial = true;
		_activeTutorial = Enums.Tutorial.Town;
		_player = player;
		_tutorialCanvas = tutorialCanvas;
		_workingGameplayCanvas = workingGameplayCanvas;
		_finalPosition = infrontOfPortalPosition;
		_fortressGate = fortressGate;
		_unlockAltar = unlockAltar;
		_unlockPortal = unlockPortal;
		DisableWellInteraction();
		_tutorialCanvas.gameObject.SetActive(value: true);
		StartCoroutine(ActivateTownTutorial(player, townController, unlockAltar, unlockPortal, infrontOfGatePosition, centerTownPosition, infrontOfPortalPosition, infrontOfUnlockAltarPosition, fortressGate, _movementTip));
		SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownTownTutorial = true;
	}

	private void DisableWellInteraction()
	{
		WellInteraction wellInteraction = UnityEngine.Object.FindObjectOfType<WellInteraction>();
		if (!(wellInteraction == null))
		{
			wellInteraction.CanInteract = false;
		}
	}

	private void EnableWellInteraction()
	{
		WellInteraction wellInteraction = UnityEngine.Object.FindObjectOfType<WellInteraction>();
		if (!(wellInteraction == null))
		{
			wellInteraction.ResetCanInteract();
		}
	}

	private IEnumerator FinishTownTutorial()
	{
		_activeTutorial = Enums.Tutorial.None;
		SingletonController<UnlocksController>.Instance.SetUnlock(Enums.Unlockable.UnlockAltar);
		_unlockAltar.UnlockStable();
		SingletonController<UnlocksController>.Instance.SetUnlock(Enums.Unlockable.AdventurePortal);
		_unlockPortal.UnlockStable();
		yield return new WaitForSeconds(0.5f);
		StopAllCoroutines();
		_player.ToggleSpeakCanvas(active: false);
		_player.StopAllMovement();
		_player.StopMoveToPosition();
		_player.ContinueMovement();
		LeanTween.moveLocalY(_fortressGate, -106.717f, 0f).setIgnoreTimeScale(useUnScaledTime: true);
		_fortressGate.SetActive(value: true);
		_tutorialCanvas.gameObject.SetActive(value: false);
		_workingGameplayCanvas.SetActive(value: true);
		SingletonController<GameController>.Instance.CanOpenMenu = true;
		IsRunningTutorial = false;
		_player.MoveToPositionInstant(_finalPosition);
		UnityEngine.Object.FindObjectOfType<EnvelopController>().HideEnvelop();
		EnableWellInteraction();
		SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownTownTutorial = true;
		SingletonController<SaveGameController>.Instance.SaveProgression();
		this.OnTutorialCompleted?.Invoke(this, new TutorialCompletedEventArgs(Enums.Tutorial.Town));
	}

	private IEnumerator ActivateTownTutorial(BackpackSurvivors.Game.Player.Player player, TownController townController, UnlockableInTown unlockAltar, UnlockableInTown unlockPortal, Transform infrontOfGatePosition, Transform centerTownPosition, Transform infrontOfPortalPosition, Transform infrontOfUnlockAltarPosition, GameObject fortressGate, GameObject _movementTip)
	{
		yield return new WaitForSeconds(2f);
		UnityEngine.Object.FindObjectOfType<EnvelopController>().ShowEnvelop(80f);
		player.MoveToPosition(infrontOfGatePosition);
		yield return new WaitForSeconds(2f);
		player.Emote(Enums.Emotes.Attention);
		yield return new WaitForSeconds(2f);
		float seconds = player.Speak(Constants.Tutorial.Town.Speak1);
		yield return new WaitForSeconds(seconds);
		player.ToggleSpeakCanvas(active: false);
		LeanTween.moveLocalY(fortressGate, -105.32f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_fortressGateOpening, 1f);
		yield return new WaitForSeconds(1f);
		fortressGate.SetActive(value: false);
		yield return new WaitForSeconds(1f);
		player.MoveToPosition(centerTownPosition);
		yield return new WaitForSeconds(6f);
		LeanTween.moveLocalY(fortressGate, -106.717f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
		fortressGate.SetActive(value: true);
		float seconds2 = player.Speak(Constants.Tutorial.Town.Speak2);
		yield return new WaitForSeconds(seconds2);
		player.ToggleSpeakCanvas(active: false);
		player.Emote(Enums.Emotes.Question);
		yield return new WaitForSeconds(2f);
		float seconds3 = player.Speak(Constants.Tutorial.Town.Speak3);
		yield return new WaitForSeconds(seconds3);
		player.ToggleSpeakCanvas(active: false);
		player.MoveToPosition(infrontOfPortalPosition);
		yield return new WaitForSeconds(4f);
		player.StopAllMovement();
		player.ShowItemEffect();
		SingletonController<UnlocksController>.Instance.SetUnlock(Enums.Unlockable.AdventurePortal);
		unlockPortal.UnlockAnimate(reopenUI: false, focusOnElement: true);
		yield return new WaitForSeconds(4f);
		player.ContinueMovement();
		float seconds4 = player.Speak(Constants.Tutorial.Town.Speak4);
		yield return new WaitForSeconds(seconds4);
		player.ToggleSpeakCanvas(active: false);
		player.MoveToPosition(infrontOfUnlockAltarPosition);
		yield return new WaitForSeconds(5.5f);
		player.StopAllMovement();
		player.ShowItemEffect();
		SingletonController<UnlocksController>.Instance.SetUnlock(Enums.Unlockable.UnlockAltar);
		unlockAltar.UnlockAnimate(reopenUI: false, focusOnElement: true);
		yield return new WaitForSeconds(5.5f);
		player.ContinueMovement();
		float seconds5 = player.Speak(Constants.Tutorial.Town.Speak5);
		yield return new WaitForSeconds(seconds5);
		player.ToggleSpeakCanvas(active: false);
		player.MoveToPosition(infrontOfPortalPosition);
		yield return new WaitForSeconds(6f);
		float seconds6 = player.Speak(Constants.Tutorial.Town.Speak6);
		yield return new WaitForSeconds(seconds6);
		player.ToggleSpeakCanvas(active: false);
		SingletonController<InputController>.Instance.SetInputEnabled(enabled: true);
		StartCoroutine(FinishTownTutorial());
	}

	public void ShowTitanicSoulTutorial(BackpackSurvivors.Game.Player.Player player, Transform infrontOfUnlockAltarPosition, Canvas tutorialCanvas, GameObject workingGameplayCanvas, Transform insideUnlockAltarInteractionRangePosition, TitanicSoulsTutorialController titanicSoulsTutorialController, Sprite titanSoulSprite)
	{
		UnityEngine.Object.FindObjectOfType<UnlockableAltarInteraction>().CanInteract = false;
		SingletonController<GameController>.Instance.CanOpenMenu = false;
		IsRunningTutorial = true;
		_activeTutorial = Enums.Tutorial.TitanicSouls;
		_workingGameplayCanvas = workingGameplayCanvas;
		_titanicSoulsTutorialController = titanicSoulsTutorialController;
		_tutorialCanvas = tutorialCanvas;
		_player = player;
		StartCoroutine(ActivateTitanicSoulTutorial(player, infrontOfUnlockAltarPosition, insideUnlockAltarInteractionRangePosition, titanSoulSprite));
	}

	private IEnumerator ActivateTitanicSoulTutorial(BackpackSurvivors.Game.Player.Player player, Transform infrontOfUnlockAltarPosition, Transform insideUnlockAltarInteractionRangePosition, Sprite titanSoulSprite)
	{
		_workingGameplayCanvas.SetActive(value: false);
		yield return new WaitForSeconds(2f);
		UnityEngine.Object.FindObjectOfType<EnvelopController>().ShowEnvelop(20f);
		float seconds = player.Speak(Constants.Tutorial.Town.Speak1_1);
		yield return new WaitForSeconds(seconds);
		player.ToggleSpeakCanvas(active: false);
		player.ShowItemEffect(titanSoulSprite, playAudio: true);
		yield return new WaitForSeconds(1.5f);
		float seconds2 = player.Speak(Constants.Tutorial.Town.Speak1_2);
		yield return new WaitForSeconds(seconds2);
		player.ToggleSpeakCanvas(active: false);
		float seconds3 = player.Speak(Constants.Tutorial.Town.Speak1_3);
		yield return new WaitForSeconds(seconds3);
		player.ToggleSpeakCanvas(active: false);
		player.MoveToPosition(infrontOfUnlockAltarPosition);
		yield return new WaitForSeconds(5.5f);
		float seconds4 = player.Speak(Constants.Tutorial.Town.Speak1_4);
		yield return new WaitForSeconds(seconds4);
		player.ToggleSpeakCanvas(active: false);
		player.MoveToPosition(insideUnlockAltarInteractionRangePosition);
		yield return new WaitForSeconds(3f);
		UnityEngine.Object.FindObjectOfType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.Unlocks);
		SingletonController<UnlocksController>.Instance.SelectUpgrade(Enums.Unlockable.ExtraShopOffers);
		_titanicSoulsTutorialController.StartTutorial(this);
		yield return new WaitForSeconds(_titanicSoulsTutorialController.TutorialDuration);
		StartCoroutine(FinishTitanicSoulTutorial());
	}

	internal IEnumerator FinishTitanicSoulTutorial()
	{
		_activeTutorial = Enums.Tutorial.None;
		_titanicSoulsTutorialController.ExitTutorial();
		UnityEngine.Object.FindObjectOfType<EnvelopController>().HideEnvelop();
		yield return new WaitForSeconds(0.5f);
		UnityEngine.Object.FindObjectOfType<UnlockableAltarInteraction>().CanInteract = true;
		_tutorialCanvas.gameObject.SetActive(value: false);
		_workingGameplayCanvas.SetActive(value: true);
		SingletonController<GameController>.Instance.CanOpenMenu = true;
		IsRunningTutorial = false;
		_player.ContinueMovement();
		_player.ToggleSpeakCanvas(active: false);
		SingletonController<InputController>.Instance.SetInputEnabled(enabled: true);
		SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownTitanicSoulTutorial = true;
		SingletonController<SaveGameController>.Instance.SaveProgression();
		this.OnTutorialCompleted?.Invoke(this, new TutorialCompletedEventArgs(Enums.Tutorial.TitanicSouls));
		StopAllCoroutines();
	}

	public void ShowTalentTreeTutorial(BackpackSurvivors.Game.Player.Player player)
	{
	}

	private IEnumerator FinishTalentTreeTutorial()
	{
		_activeTutorial = Enums.Tutorial.None;
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ActivateTalentTreeTutorial()
	{
		yield return new WaitForSeconds(2f);
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
