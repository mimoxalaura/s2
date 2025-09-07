using System;
using Sirenix.Serialization;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[Serializable]
public class MergeableIngredient : MergeablePart
{
	[OdinSerialize]
	[SerializeField]
	private int _amount = 1;

	[OdinSerialize]
	[SerializeField]
	private bool _isPrimary;

	[HideInInspector]
	public int Amount => _amount;

	[HideInInspector]
	public bool IsPrimary => _isPrimary;
}
