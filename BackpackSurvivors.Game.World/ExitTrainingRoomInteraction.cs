using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.World;

public class ExitTrainingRoomInteraction : Interaction
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
		SingletonController<GameController>.Instance.ExitingFromTrainingRoom = true;
		SingletonController<SceneChangeController>.Instance.ChangeScene("4. Town");
	}
}
