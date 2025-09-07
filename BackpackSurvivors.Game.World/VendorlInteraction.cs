using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Stats;

namespace BackpackSurvivors.Game.World;

public class VendorlInteraction : Interaction
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
		SingletonController<GameController>.Instance.SetGamePaused(gamePaused: true);
		SingletonController<InLevelTransitionController>.Instance.Transition(OpenShopDuringTransition);
	}

	internal void OpenShopDuringTransition()
	{
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.Shop);
	}
}
