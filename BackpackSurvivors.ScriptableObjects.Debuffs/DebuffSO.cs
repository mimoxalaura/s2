using BackpackSurvivors.Game.Debuffs;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Debuffs;

[CreateAssetMenu(fileName = "Debuff", menuName = "Game/Debuffs/Debuff", order = 1)]
public class DebuffSO : ScriptableObject
{
	[SerializeField]
	public DebuffEffect DebuffEffect;

	[SerializeField]
	public DebuffVisualVFX DebuffVisualPrefab;

	[SerializeField]
	public Enums.Debuff.DebuffType DebuffType;

	[SerializeField]
	public Enums.Debuff.DebuffTriggerType DebuffTriggerType;

	[SerializeField]
	public float DelayBetweenTriggers;

	[SerializeField]
	public Enums.Debuff.DebuffFalloffTimeType DebuffFalloffTimeType;

	[SerializeField]
	public float TimeUntillFalloff;

	[SerializeField]
	public Sprite Icon;

	[SerializeField]
	public Color DebuffColor;

	[SerializeField]
	public int Id;

	[SerializeField]
	public bool ResetFalloffTimeOnApplication;

	[SerializeField]
	public int MaxStacks;

	[SerializeField]
	public DamageSO DamageSO;

	[SerializeField]
	public string DebuffTextColor;
}
