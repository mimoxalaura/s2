using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Interactable.ByTouching;

internal class InteractableAltar : InteractableActor
{
	[SerializeField]
	internal Animator _animator;

	[SerializeField]
	internal AudioClip _activationAudio;

	[SerializeField]
	private BuffSO _buffSO;

	[SerializeField]
	private int _interactions = 1;

	[SerializeField]
	internal GameObject[] _hideAfterInteractionsCompleted;

	internal int _interactionsDone;

	internal bool _canInteract => _interactionsDone < _interactions;

	private void Start()
	{
	}

	public override void Act()
	{
		_interactionsDone++;
		_animator.SetBool("Active", _canInteract);
		SingletonController<AudioController>.Instance.PlaySFXClip(_activationAudio, 1f);
		SingletonController<GameController>.Instance.Player.AddBuff(_buffSO);
		GameObject[] hideAfterInteractionsCompleted = _hideAfterInteractionsCompleted;
		for (int i = 0; i < hideAfterInteractionsCompleted.Length; i++)
		{
			hideAfterInteractionsCompleted[i].SetActive(_canInteract);
		}
	}
}
