using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class HealthPoolInteraction : Interaction
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private AudioClip _activationAudio;

	[SerializeField]
	private GameObject _visualOnPlayerPrefab;

	[SerializeField]
	private int _interactions = 1;

	private int _interactionsDone;

	private bool _canInteract => _interactionsDone < _interactions;

	public override void DoStart()
	{
		base.DoStart();
	}

	public override void DoInRange()
	{
		if (_canInteract)
		{
			base.DoInRange();
		}
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
	}

	public override void DoInteract()
	{
		if (_canInteract)
		{
			_interactionsDone++;
			_animator.SetBool("Active", _canInteract);
			SingletonController<AudioController>.Instance.PlaySFXClip(_activationAudio, 1f);
			SingletonController<GameController>.Instance.Player.HealthSystem.HealComplete();
			SingletonController<GameController>.Instance.Player.PlayerVisualController.ShowVisual(_visualOnPlayerPrefab, 4, 0.25f, 2f);
			base.DoOutOfRange();
		}
	}
}
