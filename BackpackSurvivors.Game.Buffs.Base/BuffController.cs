using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Shared.Interfaces;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs.Base;

[RequireComponent(typeof(Character))]
internal class BuffController : MonoBehaviour, IClearable, IInitializable
{
	internal delegate void BuffAddedHandler(object sender, BuffAddedEventArgs e);

	internal delegate void BuffRemovedHandler(object sender, BuffRemovedEventArgs e);

	private List<BuffHandler> _activeBuffHandlers = new List<BuffHandler>();

	private float _delayBetweenTriggerChecks = 0.5f;

	public bool IsInitialized => true;

	internal event BuffAddedHandler OnBuffAdded;

	internal event BuffRemovedHandler OnBuffRemoved;

	private void Start()
	{
		StartCoroutine(TriggerDebuffEffects());
		StartCoroutine(StartFalloutTimer());
	}

	private IEnumerator TriggerDebuffEffects()
	{
		while (base.isActiveAndEnabled)
		{
			foreach (BuffHandler activeBuffHandler in _activeBuffHandlers)
			{
				if (!activeBuffHandler.CanBeDestroyed())
				{
					activeBuffHandler.Trigger(_delayBetweenTriggerChecks);
				}
			}
			foreach (BuffHandler item in _activeBuffHandlers.Where((BuffHandler t) => t.CanBeDestroyed()).ToList())
			{
				_activeBuffHandlers.Remove(item);
			}
			yield return new WaitForSeconds(_delayBetweenTriggerChecks);
		}
	}

	private IEnumerator StartFalloutTimer()
	{
		while (base.isActiveAndEnabled)
		{
			foreach (BuffHandler activeBuffHandler in _activeBuffHandlers)
			{
				switch (activeBuffHandler.BuffSO.BuffFalloffTimeType)
				{
				case Enums.Buff.BuffFalloffTimeType.SetTime:
					if (activeBuffHandler.IsStackReadyToFallOff())
					{
						RemoveBuff(activeBuffHandler);
					}
					break;
				case Enums.Buff.BuffFalloffTimeType.AfterTrigger:
					if (activeBuffHandler.TriggerCount > 0)
					{
						RemoveBuff(activeBuffHandler);
					}
					break;
				}
			}
			yield return new WaitForSeconds(_delayBetweenTriggerChecks);
		}
	}

	public bool HasBuff(BuffSO buffSO)
	{
		return _activeBuffHandlers.Any((BuffHandler x) => x.BuffSO == buffSO);
	}

	public bool AddBuff(BuffSO buffSO, out int currentStacks, float durationMod = 1f)
	{
		currentStacks = _activeBuffHandlers.Count((BuffHandler x) => x.BuffSO == buffSO);
		BuffHandler buffHandler = _activeBuffHandlers.FirstOrDefault((BuffHandler x) => x.BuffSO == buffSO);
		if (buffHandler != null)
		{
			if (buffSO.MaxStacks <= buffHandler.BuffStacks)
			{
				return false;
			}
			buffHandler.AddBuffStack();
			this.OnBuffAdded?.Invoke(this, new BuffAddedEventArgs(buffHandler));
			return true;
		}
		BuffHandler buffHandler2 = new BuffHandler();
		buffHandler2.Init(buffSO, durationMod);
		buffHandler2.SetCharacter(GetComponent<Character>());
		_activeBuffHandlers.Add(buffHandler2);
		this.OnBuffAdded?.Invoke(this, new BuffAddedEventArgs(buffHandler2));
		return true;
	}

	public void RemoveBuff(BuffHandler buffHandler)
	{
		buffHandler.Deactivate();
		this.OnBuffRemoved?.Invoke(this, new BuffRemovedEventArgs(buffHandler));
	}

	public void RemoveBuff(BuffSO buffSO)
	{
		BuffHandler buffHandler = _activeBuffHandlers.FirstOrDefault((BuffHandler x) => x.BuffSO == buffSO);
		if (buffHandler != null)
		{
			RemoveBuff(buffHandler);
		}
	}

	internal List<BuffSO> GetBuffs()
	{
		return _activeBuffHandlers.Select((BuffHandler x) => x.BuffSO).ToList();
	}

	public void Clear()
	{
		ClearAdventure();
	}

	internal void CarrierDied()
	{
		ClearAdventure();
	}

	public void ClearAdventure()
	{
		foreach (BuffHandler activeBuffHandler in _activeBuffHandlers)
		{
			RemoveBuff(activeBuffHandler);
		}
		_activeBuffHandlers.Clear();
	}
}
