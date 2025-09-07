using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.World;

public class QuestGiverInteraction : Interaction
{
	public override void DoStart()
	{
		base.DoStart();
	}

	public override void DoInRange()
	{
		base.DoInRange();
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
	}

	public override void DoInteract()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Quests))
		{
			GetModalUiController().OpenModalUI(Enums.ModalUITypes.Quests);
		}
	}
}
