using BackpackSurvivors.Game.Backpack;

namespace BackpackSurvivors.Game.Items.Merging;

public class MergeLineSet
{
	internal BaseDraggable StartDraggable;

	internal BaseDraggable EndDraggable;

	internal MergeLineSet(BaseDraggable startDraggable, BaseDraggable endDraggable)
	{
		StartDraggable = startDraggable;
		EndDraggable = endDraggable;
	}
}
