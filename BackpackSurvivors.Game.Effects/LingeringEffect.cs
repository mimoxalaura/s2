using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

[RequireComponent(typeof(Collider2D))]
public abstract class LingeringEffect : MonoBehaviour
{
	public delegate void LingeringEffectTriggeredHandler(object sender, LingeringEffectTriggeredEventArgs e);

	[SerializeField]
	public Enums.LingeringEffectType LingeringEffectType;

	[SerializeField]
	private float _delayBetweenEffectTriggers;

	[SerializeField]
	private int _maxEffectTriggerAmount = 1;

	[SerializeField]
	private TriggerOnStay _triggerOnStay;

	[SerializeField]
	private TriggerOnTouch _triggerOnTouch;

	[Header("Limits")]
	[SerializeField]
	private bool _shouldLimitAffectingSameCharacter;

	[SerializeField]
	private float _timeBetweenAffectingSameCharacter = 1f;

	private float _effectDuration;

	private bool _canTrigger = true;

	private int _currentTriggerAmount;

	private List<Character> _affectedCharacters = new List<Character>();

	private Enums.CharacterType _damageTargetType;

	private CombatWeapon _combatWeapon;

	public event LingeringEffectTriggeredHandler OnLingeringEffectTriggered;

	private void Start()
	{
		Activate();
		if (_triggerOnStay != null)
		{
			_triggerOnStay.OnTriggerOnStay += TriggerOnTouch_OnTriggerOnStay;
		}
		if (_triggerOnTouch != null)
		{
			_triggerOnTouch.OnTriggerOnTouch += TriggerOnTouch_OnTriggerOnTouch;
		}
	}

	private void TriggerOnTouch_OnTriggerOnTouch(object sender, TriggerOnTouchEventArgs e)
	{
		if (CanAffectCharacter(e.TriggeredOn))
		{
			LingeringEffectTriggered(e.TriggeredOn);
		}
	}

	private void TriggerOnTouch_OnTriggerOnStay(object sender, TriggerOnStayEventArgs e)
	{
		if (CanAffectCharacter(e.TriggeredOn))
		{
			LingeringEffectTriggered(e.TriggeredOn);
		}
	}

	private bool CanAffectCharacter(Character triggeredOn)
	{
		if (!_shouldLimitAffectingSameCharacter)
		{
			return true;
		}
		return !_affectedCharacters.Contains(triggeredOn);
	}

	public void Init(Character source, CombatWeapon sourceCombatWeapon)
	{
		if (source == null)
		{
			_canTrigger = false;
			return;
		}
		_damageTargetType = ((!(source.GetType() == typeof(BackpackSurvivors.Game.Player.Player))) ? Enums.CharacterType.Player : Enums.CharacterType.Enemy);
		_triggerOnStay?.Init(_damageTargetType);
		_triggerOnTouch?.Init(_damageTargetType);
		_combatWeapon = sourceCombatWeapon;
	}

	private void LingeringEffectTriggered(Character character)
	{
		if (_canTrigger && _currentTriggerAmount < _maxEffectTriggerAmount)
		{
			_currentTriggerAmount++;
			this.OnLingeringEffectTriggered?.Invoke(this, new LingeringEffectTriggeredEventArgs(character, _combatWeapon));
			StartCoroutine(StartTriggerCooldown());
			if (_shouldLimitAffectingSameCharacter)
			{
				StartCoroutine(AddAffectedCharacter(character));
			}
		}
	}

	private IEnumerator AddAffectedCharacter(Character character)
	{
		_affectedCharacters.Add(character);
		yield return new WaitForSeconds(_timeBetweenAffectingSameCharacter);
		_affectedCharacters.Remove(character);
	}

	private IEnumerator StartTriggerCooldown()
	{
		_canTrigger = false;
		yield return new WaitForSeconds(_delayBetweenEffectTriggers);
		_canTrigger = true;
	}

	public void SetDuration(float effectDuration)
	{
		_effectDuration = effectDuration;
	}

	public virtual void SetSortingOrder(int sortingOrder)
	{
		Debug.LogWarning("Sorting Order is not overridden");
	}

	public abstract int GetSortingOrder();

	private void Update()
	{
		DoUpdate();
	}

	public virtual void DoUpdate()
	{
	}

	public virtual void Activate()
	{
		StartCoroutine(StartDurationTimer());
	}

	public IEnumerator StartDurationTimer()
	{
		yield return new WaitForSeconds(_effectDuration);
		End();
	}

	public abstract void End();

	public virtual void SetScale(float size)
	{
		EffectsHelper.Scale(base.transform, size);
	}

	public virtual void SetRotation(Quaternion rotation)
	{
		base.transform.rotation = rotation;
	}

	private void OnDestroy()
	{
		this.OnLingeringEffectTriggered = null;
		EffectsHelper.DestroyLingeringEffect(this);
	}
}
