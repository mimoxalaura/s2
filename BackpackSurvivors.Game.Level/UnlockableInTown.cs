using System.Collections;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.World;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Stats;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class UnlockableInTown : MonoBehaviour
{
	[SerializeField]
	public Enums.Unlockable Unlockable;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private GameObject[] _objectsToShowOnUnlock;

	[SerializeField]
	private GameObject[] _objectsToShowOnUnlockAnimated;

	[SerializeField]
	private GameObject[] _objectsToShowOnUnlockStable;

	[SerializeField]
	private GameObject[] _objectsToHideAfterUnlock;

	[SerializeField]
	private SpriteRenderer[] _spriteRenderersToDisableAfterUnlock;

	[SerializeField]
	private GameObject[] _objectsToShowAfterUnlock;

	[SerializeField]
	private Interaction _interaction;

	[SerializeField]
	private CinemachineSwitcher _cinemachineSwitcher;

	[SerializeField]
	private AudioClip _audioClipOnUnlock;

	[SerializeField]
	public bool FocusOnElement = true;

	public Interaction Interaction => _interaction;

	public void UnlockAnimate(bool reopenUI, bool focusOnElement)
	{
		ToggleLockState(unlocked: true, animate: true, focusOnElement, reopenUI);
	}

	public void UnlockStable()
	{
		ToggleLockState(unlocked: true, animate: false, focusOnUnlock: false, reopenUI: false);
	}

	private IEnumerator DoToggleLockState(bool unlocked, bool animate, bool focusOnUnlock, bool reopenUI, float cameraDelay)
	{
		yield return null;
		if (focusOnUnlock)
		{
			yield return new WaitForSecondsRealtime(cameraDelay);
		}
		GameObject[] objectsToShowOnUnlockAnimated;
		if (animate)
		{
			objectsToShowOnUnlockAnimated = _objectsToShowOnUnlockAnimated;
			for (int i = 0; i < objectsToShowOnUnlockAnimated.Length; i++)
			{
				objectsToShowOnUnlockAnimated[i].SetActive(unlocked);
			}
			_animator.SetBool("Unlocked", unlocked);
			SingletonController<AudioController>.Instance.PlaySFXClip(_audioClipOnUnlock, 1f);
			yield return new WaitForSecondsRealtime(1.7f);
		}
		else
		{
			objectsToShowOnUnlockAnimated = _objectsToShowOnUnlockStable;
			for (int i = 0; i < objectsToShowOnUnlockAnimated.Length; i++)
			{
				objectsToShowOnUnlockAnimated[i].SetActive(unlocked);
			}
			_animator.enabled = false;
		}
		objectsToShowOnUnlockAnimated = _objectsToShowOnUnlock;
		for (int i = 0; i < objectsToShowOnUnlockAnimated.Length; i++)
		{
			objectsToShowOnUnlockAnimated[i].SetActive(unlocked);
		}
		objectsToShowOnUnlockAnimated = _objectsToHideAfterUnlock;
		for (int i = 0; i < objectsToShowOnUnlockAnimated.Length; i++)
		{
			objectsToShowOnUnlockAnimated[i].SetActive(!unlocked);
		}
		SpriteRenderer[] spriteRenderersToDisableAfterUnlock = _spriteRenderersToDisableAfterUnlock;
		for (int i = 0; i < spriteRenderersToDisableAfterUnlock.Length; i++)
		{
			spriteRenderersToDisableAfterUnlock[i].enabled = !unlocked;
		}
		objectsToShowOnUnlockAnimated = _objectsToShowAfterUnlock;
		for (int i = 0; i < objectsToShowOnUnlockAnimated.Length; i++)
		{
			objectsToShowOnUnlockAnimated[i].SetActive(unlocked);
		}
		if (focusOnUnlock && _cinemachineSwitcher != null)
		{
			yield return new WaitForSecondsRealtime(3f);
			_cinemachineSwitcher.SwitchToPlayer();
			yield return new WaitForSecondsRealtime(2f);
			if (reopenUI)
			{
				Object.FindObjectOfType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.Unlocks);
			}
			SingletonController<InputController>.Instance.SetInputEnabled(enabled: true);
		}
	}

	public virtual void ToggleLockState(bool unlocked, bool animate, bool focusOnUnlock, bool reopenUI)
	{
		float cameraDelay = 0f;
		if (focusOnUnlock && unlocked && _cinemachineSwitcher != null)
		{
			SingletonController<InputController>.Instance.SetInputEnabled(enabled: false);
			BackpackSurvivors.Game.Player.Player player = Object.FindObjectOfType<BackpackSurvivors.Game.Player.Player>();
			cameraDelay = _cinemachineSwitcher.GetAndSetDelay(player.transform, base.transform);
			_cinemachineSwitcher.SwitchTo(Unlockable);
			if (reopenUI)
			{
				Object.FindObjectOfType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.Unlocks);
			}
		}
		StartCoroutine(DoToggleLockState(unlocked, animate, focusOnUnlock, reopenUI, cameraDelay));
	}
}
