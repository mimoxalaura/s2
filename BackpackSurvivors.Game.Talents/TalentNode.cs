using UnityEngine;

namespace BackpackSurvivors.Game.Talents;

public class TalentNode : MonoBehaviour
{
	public virtual int GetId()
	{
		return -1;
	}

	public virtual bool IsKeystone()
	{
		return false;
	}

	public virtual bool IsActive()
	{
		return false;
	}

	public virtual void Activate(bool animate = true)
	{
	}

	public virtual void Deactivate()
	{
	}

	public virtual void SetHighlightInSearch(bool highlight)
	{
	}

	public virtual void Init(bool debug = true)
	{
	}

	internal virtual bool FitsSearch(string searchTerm)
	{
		return false;
	}
}
