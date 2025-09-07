using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class UnlockableAltarInteraction : Interaction
{
	[SerializeField]
	public Animator _chaliceAnimator;

	[SerializeField]
	public Animator _AltarAnimator;

	public override void DoStart()
	{
		base.DoStart();
	}

	public override void DoInRange()
	{
		base.DoInRange();
		_chaliceAnimator.SetBool("Activated", value: true);
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
		_chaliceAnimator.SetBool("Activated", value: false);
	}

	public override void DoInteract()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Unlockables))
		{
			GetModalUiController().OpenModalUI(Enums.ModalUITypes.Unlocks);
		}
	}
}
