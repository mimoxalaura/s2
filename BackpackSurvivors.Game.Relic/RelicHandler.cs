using UnityEngine;

namespace BackpackSurvivors.Game.Relic;

public class RelicHandler : MonoBehaviour
{
	public Relic Relic { get; private set; }

	public virtual void Setup(Relic relic)
	{
		Relic = relic;
	}

	public virtual void TearDown()
	{
	}

	public virtual void Execute()
	{
	}

	public virtual void BeforeDestroy()
	{
	}

	private void OnDestroy()
	{
		BeforeDestroy();
	}
}
