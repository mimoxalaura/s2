namespace BackpackSurvivors.Game.Relic.RelicHandlers;

public class EmptyHandler : RelicHandler
{
	public override void Setup(Relic relic)
	{
		base.Setup(relic);
	}

	public override void Execute()
	{
	}

	public override void BeforeDestroy()
	{
	}

	public override void TearDown()
	{
	}
}
