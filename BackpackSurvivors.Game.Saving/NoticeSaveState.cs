namespace BackpackSurvivors.Game.Saving;

internal class NoticeSaveState : BaseSaveState
{
	public int MaxNoticeNumberSeen;

	public override bool HasData()
	{
		return false;
	}
}
