using BackpackSurvivors.Game.Adventure;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Adventures;

[CreateAssetMenu(fileName = "Adventure Effect", menuName = "Game/Adventures/AdventureEffect", order = 2)]
public class AdventureEffectSO : ScriptableObject
{
	[SerializeReference]
	public AdventureEffectController AdventureEffectController;

	[SerializeReference]
	public Sprite Icon;

	[SerializeReference]
	public string Name;

	[SerializeReference]
	public string Description;

	[SerializeReference]
	public int ActiveFromHellfireLevel;
}
