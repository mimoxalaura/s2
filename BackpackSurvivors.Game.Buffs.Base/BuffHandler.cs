using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs.Base;

public class BuffHandler
{
	private float _lastTrigger;

	private int _deactivatedBuffStacks;

	private Character _buffedCharacter;

	private List<float> _buffStackStartTimes = new List<float>();

	public float TimeUntillFalloff { get; private set; }

	public int BuffStacks { get; private set; }

	public int TriggerCount { get; private set; }

	public BuffSO BuffSO { get; private set; }

	public void Init(BuffSO buffSO, float durationMod = 1f)
	{
		BuffSO = buffSO;
		TimeUntillFalloff = buffSO.TimeUntillFalloff * durationMod;
		BuffSO.BuffEffect.Init(buffSO);
		AddBuffStack();
	}

	public bool CanBeDestroyed()
	{
		bool num = BuffStacks > 0;
		bool flag = _deactivatedBuffStacks >= BuffStacks;
		return num && flag;
	}

	public bool IsStackReadyToFallOff()
	{
		if (_buffStackStartTimes.Count == 0)
		{
			return false;
		}
		float num = _buffStackStartTimes.Min();
		return Time.time - num > TimeUntillFalloff;
	}

	public void AddBuffStack()
	{
		BuffStacks++;
		_buffStackStartTimes.Add(Time.time);
	}

	public void Deactivate()
	{
		BuffSO.BuffEffect.OnFallOff(_buffedCharacter);
		_deactivatedBuffStacks++;
		RemoveOldestStack();
	}

	public void Trigger(float tickTime)
	{
		if (CanTrigger())
		{
			TriggerCount++;
			BuffSO.BuffEffect.Trigger(_buffedCharacter);
		}
	}

	private void RemoveOldestStack()
	{
		float item = _buffStackStartTimes.Min();
		_buffStackStartTimes.Remove(item);
	}

	private bool CanTrigger()
	{
		if (BuffSO.BuffTriggerType == Enums.Buff.BuffTriggerType.Once)
		{
			return TriggerCount < BuffStacks;
		}
		if (Time.time - _lastTrigger > BuffSO.DelayBetweenTriggers)
		{
			_lastTrigger = Time.time;
			return true;
		}
		return false;
	}

	internal void SetCharacter(Character buffedCharacter)
	{
		_buffedCharacter = buffedCharacter;
	}
}
