using BackpackSurvivors.Game.Buffs.Base;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Buffs;

public class BuffVisualEffectOnPlayer : MonoBehaviour
{
	public BuffHandler BuffHandler;

	public bool CanBeDestroyed;

	public void Init(BuffHandler buffHandler)
	{
		BuffHandler = buffHandler;
	}
}
