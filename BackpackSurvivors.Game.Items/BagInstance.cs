using BackpackSurvivors.ScriptableObjects.Items;

namespace BackpackSurvivors.Game.Items;

public class BagInstance : BaseItemInstance
{
	public string Name => BagSO.Name;

	public BagSO BagSO { get; private set; }

	public BagInstance(BagSO bagSO)
	{
		BagSO = bagSO;
		SetBaseItemInstance(bagSO);
	}

	public override bool Equals(object other)
	{
		if (other is BagInstance)
		{
			return ((BagInstance)other).Guid == base.Guid;
		}
		return base.Equals(other);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
