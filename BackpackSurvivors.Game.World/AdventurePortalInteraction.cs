using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class AdventurePortalInteraction : Interaction
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
		_animator.SetBool("IsOpen", value: true);
		_animator.SetTrigger("Open");
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
		_animator.SetBool("IsOpen", value: false);
		_animator.SetTrigger("Close");
	}

	public override void DoInteract()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Adventures))
		{
			GetModalUiController().OpenModalUI(Enums.ModalUITypes.Adventure);
		}
	}
}
