using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Minimap;
using UnityEngine;

namespace BackpackSurvivors.Game.Interactable.ByTouching;

internal class InteractableHealthPool : InteractableActor
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private AudioClip _activationAudio;

	[SerializeField]
	private GameObject _visualOnPlayerPrefab;

	[SerializeField]
	private int _interactions = 1;

	[SerializeField]
	private GameObject[] _hideAfterInteractionsCompleted;

	[SerializeField]
	private MinimapPoint _minimapPoint;

	[SerializeField]
	private Collider2D _colliderMinimapClamp;

	private int _interactionsDone;

	private bool _canInteract => _interactionsDone < _interactions;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(collision.gameObject.GetComponent<BackpackSurvivors.Game.Player.Player>() == null))
		{
			_minimapPoint.ActivateMinimapClamp();
			_colliderMinimapClamp.gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
	}

	public override void Act()
	{
		_interactionsDone++;
		_animator.SetBool("Active", _canInteract);
		SingletonController<AudioController>.Instance.PlaySFXClip(_activationAudio, 1f);
		SingletonController<GameController>.Instance.Player.HealthSystem.HealComplete();
		SingletonController<GameController>.Instance.Player.DamageVisualizer.ShowTextPopup("Healed!", Constants.Colors.PositiveEffectColor, 2f);
		SingletonController<GameController>.Instance.Player.PlayerVisualController.ShowVisual(_visualOnPlayerPrefab, 4, 0.25f, 2f);
		GameObject[] hideAfterInteractionsCompleted = _hideAfterInteractionsCompleted;
		for (int i = 0; i < hideAfterInteractionsCompleted.Length; i++)
		{
			hideAfterInteractionsCompleted[i].SetActive(_canInteract);
		}
	}
}
