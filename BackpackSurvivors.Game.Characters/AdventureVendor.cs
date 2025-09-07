using System.Collections;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Stats;
using UnityEngine;

namespace BackpackSurvivors.Game.Characters;

public class AdventureVendor : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	private TargetArrow _targetArrow;

	[SerializeField]
	private bool _allowInteraction;

	[SerializeField]
	private bool _spawned;

	[SerializeField]
	private AudioClip _spawnAudioClip;

	[SerializeField]
	private AudioClip _despawnAudioClip;

	[SerializeField]
	private AudioClip _enteringAudioClip;

	private void Start()
	{
		if (_spawned)
		{
			Spawn();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_allowInteraction && !(collision.gameObject.GetComponent<BackpackSurvivors.Game.Player.Player>() == null))
		{
			SingletonController<AudioController>.Instance.PlaySFXClipAtPosition(_enteringAudioClip, 1f, base.transform.position);
			SingletonController<GameController>.Instance.SetGamePaused(gamePaused: true);
			SingletonController<InLevelTransitionController>.Instance.Transition(OpenShopDuringTransition);
		}
	}

	internal void OpenShopDuringTransition()
	{
		SingletonCacheController.Instance.GetControllerByType<ShopController>().SetShopOpenedFromVendor(fromVendor: true);
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.Shop);
	}

	internal void Spawn()
	{
		_animator.SetBool("Spawned", value: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_spawnAudioClip, 1f);
		StartCoroutine(AllowInteraction());
	}

	private IEnumerator AllowInteraction()
	{
		yield return new WaitForSeconds(2f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_despawnAudioClip, 1f);
		_allowInteraction = true;
	}

	internal void Despawn()
	{
		_allowInteraction = false;
		_animator.SetBool("Spawned", value: false);
		SingletonController<AudioController>.Instance.PlaySFXClip(_despawnAudioClip, 1f);
		SingletonCacheController.Instance.GetControllerByType<TargetArrowController>().RemoveArrow(_targetArrow);
		StartCoroutine(FinishDespawn());
	}

	private IEnumerator FinishDespawn()
	{
		yield return new WaitForSeconds(1.5f);
		Object.Destroy(base.gameObject);
	}

	internal void SetArrow(TargetArrow targetArrow)
	{
		_targetArrow = targetArrow;
	}
}
