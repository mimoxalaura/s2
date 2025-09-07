using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Backpack;

public class HoveredSlotInfo
{
	public static HoveredSlotInfo None => GetNoneHoveredSlotInfo();

	public int Slotid { get; private set; }

	public bool IsHoveredSlotOnRight { get; private set; }

	public bool IsHoveredSlotOnBottom { get; private set; }

	public Enums.Backpack.GridType HoveredCellGridType { get; private set; }

	private static HoveredSlotInfo GetNoneHoveredSlotInfo()
	{
		return new HoveredSlotInfo(-1, isHoveredSlotOnRight: false, isHoveredSlotOnBottom: false, Enums.Backpack.GridType.Backpack);
	}

	public HoveredSlotInfo(int slotId, bool isHoveredSlotOnRight, bool isHoveredSlotOnBottom, Enums.Backpack.GridType hoveredCellGridType)
	{
		Slotid = slotId;
		IsHoveredSlotOnRight = isHoveredSlotOnRight;
		IsHoveredSlotOnBottom = isHoveredSlotOnBottom;
		HoveredCellGridType = hoveredCellGridType;
	}

	public override bool Equals(object other)
	{
		if (other is HoveredSlotInfo)
		{
			HoveredSlotInfo hoveredSlotInfo = (HoveredSlotInfo)other;
			bool num = Slotid == hoveredSlotInfo.Slotid;
			bool flag = IsHoveredSlotOnRight == hoveredSlotInfo.IsHoveredSlotOnRight;
			bool flag2 = IsHoveredSlotOnBottom == hoveredSlotInfo.IsHoveredSlotOnBottom;
			return num && flag && flag2;
		}
		return base.Equals(other);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
