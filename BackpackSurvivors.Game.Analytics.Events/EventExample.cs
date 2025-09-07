using Unity.Services.Analytics;

namespace BackpackSurvivors.Game.Analytics.Events;

internal class EventExample : Event
{
	public string CustomString
	{
		set
		{
			SetParameter("customString", value);
		}
	}

	public int CustomInt
	{
		set
		{
			SetParameter("customInt", value);
		}
	}

	public float CustomFloat
	{
		set
		{
			SetParameter("customFloat", value);
		}
	}

	public bool CustomBool
	{
		set
		{
			SetParameter("customBool", value);
		}
	}

	public EventExample()
		: base("eventExample")
	{
	}
}
