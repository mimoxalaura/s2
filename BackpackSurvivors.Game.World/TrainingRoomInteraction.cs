using BackpackSurvivors.System;
using Tymski;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class TrainingRoomInteraction : Interaction
{
	[SerializeField]
	private SceneReference _trainingRoomScene;

	[SerializeField]
	private Animator _trainingAnimator;

	public override void DoStart()
	{
		base.DoStart();
	}

	public override void DoInRange()
	{
		base.DoInRange();
		_trainingAnimator.SetTrigger("Open");
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
		_trainingAnimator.SetTrigger("Close");
	}

	public override void DoInteract()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.TrainingRoom))
		{
			SingletonController<SceneChangeController>.Instance.ChangeScene(_trainingRoomScene);
		}
	}
}
