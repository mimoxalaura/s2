using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Debuffs;

internal class DebuffContainer : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRendererForDebuffs;

	[SerializeField]
	private DebuffVisualContainer DebuffVisualContainer;

	[SerializeField]
	private DebuffVisualContainer Debug;

	private List<DebuffHandler> _debuffHandlers;

	private float _delayBetweenTriggerChecks = 0.5f;

	private bool _initialized;

	private List<Enums.Debuff.DebuffType> _debuffTypesBeforeDeath;

	public List<DebuffHandler> DebuffHandlers => _debuffHandlers;

	public List<Enums.Debuff.DebuffType> DebuffsOnDeath => _debuffTypesBeforeDeath;

	internal event Character.CharacterDebuffAppliedHandler OnDebuffApplied;

	internal event Character.CharacterDebuffRemovedHandler OnDebuffRemoved;

	private void Start()
	{
		StartCoroutine(TriggerDebuffEffects());
		StartCoroutine(StartFalloutTimer());
	}

	public int GetDebuffStacks(DebuffSO debuffSO)
	{
		return _debuffHandlers.FirstOrDefault((DebuffHandler x) => x.DebuffSO == debuffSO)?.DebuffStacks ?? 0;
	}

	public void AddDebuff(DebuffHandler debuffHandler, Character character)
	{
		DebuffHandler debuffHandler2 = _debuffHandlers.FirstOrDefault((DebuffHandler x) => x.DebuffType == debuffHandler.DebuffType && !x.CanBeDestroyed);
		if (character.IsEnemy)
		{
			Enums.Debuff.DebuffType debuffType = debuffHandler.DebuffType;
			if (((Enemy)character).GetDebuffImmunities().Contains(debuffType))
			{
				return;
			}
		}
		if (debuffHandler2 == null)
		{
			_debuffHandlers.Add(debuffHandler);
			if (!(DebuffVisualContainer == null))
			{
				this.OnDebuffApplied?.Invoke(this, new DebuffAppliedEventArgs(debuffHandler.DebuffSO, character));
			}
		}
		else
		{
			debuffHandler2.AddStack();
			if (!(DebuffVisualContainer == null))
			{
				this.OnDebuffApplied?.Invoke(this, new DebuffAppliedEventArgs(debuffHandler.DebuffSO, character));
			}
		}
	}

	private IEnumerator TriggerDebuffEffects()
	{
		if (_initialized)
		{
			yield break;
		}
		_initialized = true;
		if (_debuffHandlers == null)
		{
			_debuffHandlers = new List<DebuffHandler>();
		}
		while (base.isActiveAndEnabled)
		{
			for (int num = _debuffHandlers.Count - 1; num >= 0; num--)
			{
				if (!_debuffHandlers[num].CanBeDestroyed)
				{
					_debuffHandlers[num].Trigger(_delayBetweenTriggerChecks);
				}
			}
			_debuffHandlers.RemoveAll((DebuffHandler t) => t.CanBeDestroyed);
			if (_debuffHandlers.Count > 0)
			{
				DebuffHandler debuffHandler = _debuffHandlers.OrderBy((DebuffHandler x) => x.DebuffType).FirstOrDefault();
				if (debuffHandler != null)
				{
					if (_spriteRendererForDebuffs != null)
					{
						_spriteRendererForDebuffs.color = debuffHandler.DebuffSO.DebuffColor;
					}
				}
				else if (_spriteRendererForDebuffs != null)
				{
					_spriteRendererForDebuffs.color = Color.white;
				}
			}
			yield return new WaitForSeconds(_delayBetweenTriggerChecks);
		}
	}

	private IEnumerator StartFalloutTimer()
	{
		while (base.isActiveAndEnabled)
		{
			foreach (DebuffHandler item in _debuffHandlers.Where((DebuffHandler x) => !x.CanBeDestroyed))
			{
				switch (item.DebuffFalloffTimeType)
				{
				case Enums.Debuff.DebuffFalloffTimeType.SetTime:
					if (item.TimeAlive >= item.TimeUntillFalloff)
					{
						item.Deactivate();
						_spriteRendererForDebuffs.color = Color.white;
						if (!(DebuffVisualContainer == null))
						{
							this.OnDebuffRemoved?.Invoke(this, new DebuffRemovedEventArgs(item.DebuffSO));
						}
					}
					break;
				case Enums.Debuff.DebuffFalloffTimeType.AfterTrigger:
					if (item.TriggerCount > 0)
					{
						item.Deactivate();
						_spriteRendererForDebuffs.color = Color.white;
						if (!(DebuffVisualContainer == null))
						{
							this.OnDebuffRemoved?.Invoke(this, new DebuffRemovedEventArgs(item.DebuffSO));
						}
					}
					break;
				}
			}
			yield return new WaitForSeconds(_delayBetweenTriggerChecks);
		}
	}

	internal void Reset()
	{
		if (base.isActiveAndEnabled)
		{
			StopAllCoroutines();
			if (_debuffHandlers != null)
			{
				_debuffHandlers.Clear();
			}
			DebuffVisualContainer.Reset();
			_initialized = false;
			_spriteRendererForDebuffs.color = Color.white;
			StartCoroutine(TriggerDebuffEffects());
			StartCoroutine(StartFalloutTimer());
		}
	}

	internal void CarrierDied()
	{
		StopAllCoroutines();
		if (_debuffHandlers != null)
		{
			_debuffTypesBeforeDeath = new List<Enums.Debuff.DebuffType>();
			_debuffTypesBeforeDeath.AddRange(_debuffHandlers.Select((DebuffHandler x) => x.DebuffType));
			_debuffHandlers.Clear();
		}
		_initialized = false;
		_spriteRendererForDebuffs.color = Color.white;
	}

	private void OnDestroy()
	{
		Reset();
	}

	internal void RemoveDebuff(Enums.Debuff.DebuffType debuff)
	{
		DebuffHandler debuffHandler = _debuffHandlers.FirstOrDefault((DebuffHandler x) => x.DebuffType == debuff);
		if (debuffHandler != null)
		{
			debuffHandler.Deactivate();
			this.OnDebuffRemoved?.Invoke(this, new DebuffRemovedEventArgs(debuffHandler.DebuffSO));
		}
	}
}
