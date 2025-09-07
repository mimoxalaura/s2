using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Buffs;

[CreateAssetMenu(fileName = "Buff", menuName = "Game/Buffs/Buff", order = 1)]
public class BuffSO : ScriptableObject
{
	[SerializeField]
	public BuffEffect BuffEffect;

	[SerializeField]
	public BuffVisualEffectOnPlayer[] BuffVisualOnPlayerPrefabs;

	[SerializeField]
	public Enums.Buff.BuffTriggerType BuffTriggerType;

	[SerializeField]
	public float DelayBetweenTriggers;

	[SerializeField]
	public Enums.Buff.BuffFalloffTimeType BuffFalloffTimeType;

	[SerializeField]
	public float TimeUntillFalloff;

	[SerializeField]
	public int MaxStacks;

	[SerializeField]
	public Color BuffColor;

	[SerializeField]
	public Material BuffMaterial;

	[SerializeField]
	public Sprite Icon;

	[SerializeField]
	public string Name;

	[SerializeField]
	public string Description;

	[SerializeField]
	public int Id;

	[SerializeField]
	public bool ResetFalloffTimeOnApplication;

	[SerializeField]
	public AudioClip AudioOnTrigger;

	[SerializeField]
	public AudioClip AudioOnFallOff;

	[SerializeField]
	public float VisualPriority;

	[SerializeField]
	public bool TooltipShowsDuration = true;

	[SerializeField]
	public bool ShowTextPopupOnGain = true;

	[SerializeField]
	public bool ShowTextPopupOnFirstGain;
}
