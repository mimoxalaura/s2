using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class DifficultyChangeInteraction : Interaction
{
	[SerializeField]
	public Animator _animator;

	public override void DoStart()
	{
		base.DoStart();
	}

	public override void DoInRange()
	{
		base.DoInRange();
		_animator.SetBool("Activated", value: true);
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
		_animator.SetBool("Activated", value: false);
	}

	public override void DoInteract()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Difficulties))
		{
			GetModalUiController().OpenModalUI(Enums.ModalUITypes.Difficulty);
		}
	}
}
