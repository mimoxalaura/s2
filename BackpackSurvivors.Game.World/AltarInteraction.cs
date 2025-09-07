using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class AltarInteraction : Interaction
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private AudioClip _activationAudio;

	[SerializeField]
	private BuffSO _buffSO;

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
			SingletonController<GameController>.Instance.Player.AddBuff(_buffSO);
			base.DoOutOfRange();
		}
	}
}
