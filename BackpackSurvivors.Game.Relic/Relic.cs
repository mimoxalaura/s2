using BackpackSurvivors.ScriptableObjects.Relics;
using UnityEngine;

namespace BackpackSurvivors.Game.Relic;

public class Relic : MonoBehaviour
{
	public RelicSO RelicSO;

	public RelicHandler RelicHandler { get; private set; }

	internal void Init(RelicSO relicSO)
	{
		RelicHandler = Object.Instantiate(relicSO.RelicHandler, base.transform);
		RelicSO = relicSO;
	}
}
