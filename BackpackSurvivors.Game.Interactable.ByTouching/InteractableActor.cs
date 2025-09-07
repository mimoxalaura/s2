using UnityEngine;

namespace BackpackSurvivors.Game.Interactable.ByTouching;

internal class InteractableActor : MonoBehaviour
{
	public virtual void Act()
	{
		Debug.LogWarning("Interactable actor not overridden!");
	}
}
