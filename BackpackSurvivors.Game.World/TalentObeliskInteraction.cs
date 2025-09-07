using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class TalentObeliskInteraction : Interaction
{
	[SerializeField]
	public Animator _obeliskAnimator;

	public override void DoStart()
	{
		base.DoStart();
	}

	public override void DoInRange()
	{
		base.DoInRange();
		_obeliskAnimator.SetBool("Activated", value: true);
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
		_obeliskAnimator.SetBool("Activated", value: false);
	}

	public override void DoInteract()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Talents))
		{
			GetModalUiController().OpenModalUI(Enums.ModalUITypes.Talents);
		}
	}
}
