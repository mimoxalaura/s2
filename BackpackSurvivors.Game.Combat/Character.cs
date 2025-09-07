using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Debuffs;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Health;
using BackpackSurvivors.Game.Health.Events;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.DamageNumbers;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat;

public class Character : MonoBehaviour
{
	internal delegate void CharacterDamagedHandler(object sender, DamageTakenEventArgs e);

	public delegate void CharacterRevivedHandler(object sender, EventArgs e);

	internal delegate void CharacterBuffAppliedHandler(object sender, BuffAppliedEventArgs e);

	internal delegate void CharacterDebuffAppliedHandler(object sender, DebuffAppliedEventArgs e);

	internal delegate void CharacterDebuffRemovedHandler(object sender, DebuffRemovedEventArgs e);

	public delegate void DashCooldownUpdatedHandler(object sender, DashCooldownEventArgs e);

	public delegate void DashedHandler(object sender, DashCooldownEventArgs e);

	public delegate void DashReadyHandler(object sender, DashCooldownEventArgs e);

	internal delegate void KilledHandler(object sender, KilledEventArgs e);

	[SerializeField]
	internal DamageVisualizer DamageVisualizer;

	[SerializeField]
	public Transform AttackPoint;

	[SerializeField]
	public Transform NonFlippingAttackPoint;

	[SerializeField]
	private Animator _immunityBubbleAnimatorController;

	[SerializeField]
	private bool _isEnemy = true;

	[SerializeField]
	private bool _isPlayer;

	[SerializeField]
	private Emote _emote;

	[SerializeField]
	private bool _isDummy;

	[Header("Debuff")]
	[SerializeField]
	private DebuffContainer _debuffContainer;

	[Header("Buffs")]
	[SerializeField]
	private BuffController _buffController;

	private Dictionary<Enums.ItemStatType, float> _calculatedStats = new Dictionary<Enums.ItemStatType, float>();

	private Dictionary<Enums.ItemStatType, float> _buffedStats = new Dictionary<Enums.ItemStatType, float>();

	private Dictionary<Enums.DamageType, float> _buffedDamageTypes = new Dictionary<Enums.DamageType, float>();

	[Header("DEBUG")]
	public bool IsAttacking;

	public bool IsAttackingWithVisualWeapon;

	public bool IsMoving;

	private float _immuneDurationAfterTakingDamage;

	private bool _immunityBecauseOfTakenDamage;

	private bool _immunityBecauseOfDash;

	private bool _immunityBecauseOfLeavingShop;

	private bool _immunityBecauseOfBurrowing;

	private bool _immunityBecauseOfReviving;

	private bool _onDeadHasTriggered;

	private bool _canHealOffLifedrain = true;

	private const float _lifeDrainCooldownTime = 0.1f;

	public bool CanAct;

	private SpeakingController _speakingController;

	private bool _lastHitWasCrit;

	private WeaponSO _lastWeaponSO;

	private Character _lastCharacterHit;

	private Enums.DamageType _lastDamageType;

	private float _lastDamage;

	public Dictionary<Enums.ItemStatType, List<ItemStatModifier>> CalculatedStatsWithSource { get; internal set; }

	public Dictionary<Enums.DamageType, float> CalculatedDamageTypeValues { get; internal set; }

	public Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> CalculatedDamageTypeValuesWithSource { get; internal set; }

	public HealthSystem HealthSystem { get; set; }

	public List<DebuffHandler> DebuffHandlers => _debuffContainer.DebuffHandlers;

	public Guid UniqueKey { get; private set; }

	public bool IsDead
	{
		get
		{
			if (HealthSystem == null)
			{
				return false;
			}
			return HealthSystem.IsDead();
		}
	}

	public bool IsEnemy => _isEnemy;

	public bool IsPlayer => _isPlayer;

	internal Dictionary<Enums.ItemStatType, float> CalculatedStats => _calculatedStats;

	internal Dictionary<Enums.ItemStatType, float> BuffStats => _buffedStats;

	internal Dictionary<Enums.DamageType, float> BuffDamageTypes => _buffedDamageTypes;

	internal event HealthSystem.HealthChangedEventHandler OnHealthChanged;

	internal event HealthSystem.HealthMaxChangedEventHandler OnHealthMaxChanged;

	internal event CharacterDamagedHandler OnCharacterDamaged;

	internal event CharacterBuffAppliedHandler OnBuffApplied;

	internal event CharacterDebuffAppliedHandler OnDebuffApplied;

	internal event CharacterBuffAppliedHandler OnBuffRemoved;

	internal event CharacterDebuffRemovedHandler OnDebuffRemoved;

	public event CharacterRevivedHandler OnCharacterRevived;

	internal event DashCooldownUpdatedHandler OnDashCooldownUpdated;

	internal event DashedHandler OnDashed;

	internal event EventHandler OnLoaded;

	internal event PlayerDash.DashCountChangedHandler OnDashCountChanged;

	internal event DashReadyHandler OnDashReady;

	internal event EventHandler OnDashesCountSet;

	internal event KilledHandler OnKilled;

	public int GetDebuffStacks(DebuffSO debuffSO)
	{
		return _debuffContainer.GetDebuffStacks(debuffSO);
	}

	public void RemoveDebuff(Enums.Debuff.DebuffType debuffType)
	{
		if (!(_debuffContainer == null))
		{
			_debuffContainer.RemoveDebuff(debuffType);
		}
	}

	internal virtual SpriteRenderer GetSpriteRenderer()
	{
		return null;
	}

	internal virtual bool IsRotatedToRight()
	{
		return false;
	}

	internal virtual Enums.Enemies.EnemyType GetCharacterType()
	{
		return Enums.Enemies.EnemyType.Player;
	}

	internal void SetIsPlayer()
	{
		_isPlayer = true;
	}

	public virtual void SetCanAct(bool canAct)
	{
		CanAct = canAct;
	}

	private SpeakingController GetSpeakingController()
	{
		if (_speakingController == null)
		{
			_speakingController = UnityEngine.Object.FindObjectOfType<SpeakingController>();
		}
		return _speakingController;
	}

	public void InitGuid()
	{
		UniqueKey = Guid.NewGuid();
		if (_debuffContainer != null)
		{
			_debuffContainer.OnDebuffRemoved += _debuffContainer_OnDebuffRemoved;
		}
	}

	private void _debuffContainer_OnDebuffRemoved(object sender, DebuffRemovedEventArgs e)
	{
		this.OnDebuffRemoved?.Invoke(this, e);
	}

	internal DebuffContainer GetDebuffContainer()
	{
		return _debuffContainer;
	}

	internal void Kill()
	{
		HealthSystem.Die();
	}

	internal void RaiseCharacterKilledEvent()
	{
		this.OnKilled?.Invoke(this, new KilledEventArgs(this, _lastDamage, _lastHitWasCrit, _lastWeaponSO, _lastDamageType, _lastCharacterHit));
	}

	public void RaiseCharacterRevivedEvent()
	{
		this.OnCharacterRevived?.Invoke(this, new EventArgs());
	}

	[Command("player.emote.speak", Platform.AllPlatforms, MonoTargetType.Single)]
	internal float Speak(string textToSay, Character speakingTo = null)
	{
		if (GetSpeakingController() != null)
		{
			GetSpeakingController().ToggleSpeakCanvas(active: true);
			GetSpeakingController().SetupConversationCharacters(this, speakingTo);
			return GetSpeakingController().Speak(textToSay);
		}
		return 0f;
	}

	internal void ToggleSpeakCanvas(bool active)
	{
		GetSpeakingController().ToggleSpeakCanvas(active: false);
	}

	internal void Emote(Enums.Emotes emote)
	{
		if (_emote != null)
		{
			_emote.ActEmote(emote);
			SingletonController<AudioController>.Instance.PlayEmoteAudio(emote);
		}
	}

	internal virtual bool Damage(float damage, bool wasCrit, GameObject sourceForKnockback, float knockbackPower, WeaponSO weaponSource, Enums.DamageType damageType, Character damageSource = null)
	{
		if (!base.isActiveAndEnabled)
		{
			return false;
		}
		if (!CanTakeDamage())
		{
			return false;
		}
		bool num = this is BackpackSurvivors.Game.Player.Player;
		bool flag = !(weaponSource == null) && weaponSource.ShouldTriggerAudioOnDamage;
		bool shouldTriggerAudioOnDamage = num || flag;
		_lastHitWasCrit = wasCrit;
		_lastWeaponSO = weaponSource;
		_lastCharacterHit = damageSource;
		_lastDamageType = damageType;
		_lastDamage = damage;
		HealthSystem.Damage(damage, shouldTriggerAudioOnDamage);
		if (num && damage > float.Epsilon)
		{
			SetImmunityForDuration(Enums.ImmunitySource.DamageTaken, _immuneDurationAfterTakingDamage);
		}
		return true;
	}

	internal virtual Enums.Debuff.DebuffType[] GetDebuffImmunities()
	{
		return new Enums.Debuff.DebuffType[0];
	}

	internal void ResetDebuffs()
	{
		_debuffContainer.Reset();
	}

	internal virtual void AnimateSuicide()
	{
	}

	internal void ResetBuffs()
	{
	}

	internal float GetCalculatedStat(Enums.ItemStatType statType)
	{
		if (_calculatedStats.ContainsKey(statType))
		{
			float num = _calculatedStats[statType];
			if (statType == Enums.ItemStatType.WeaponCapacity && num <= 0f)
			{
				return 1f;
			}
			return num;
		}
		return 0f;
	}

	internal float GetBuffedStat(Enums.ItemStatType statType)
	{
		if (_buffedStats.ContainsKey(statType))
		{
			return _buffedStats[statType];
		}
		return 0f;
	}

	internal float GetBuffedDamageType(Enums.DamageType damageType)
	{
		if (_buffedDamageTypes.ContainsKey(damageType))
		{
			return _buffedDamageTypes[damageType];
		}
		return 0f;
	}

	internal void SetCalculatedStat(Enums.ItemStatType type, float value)
	{
		_calculatedStats[type] = value;
	}

	internal void AddBuffedStat(Enums.ItemStatType type, float value)
	{
		if (!_buffedStats.ContainsKey(type))
		{
			_buffedStats.Add(type, 0f);
		}
		_buffedStats[type] += value;
	}

	internal void AddBuffedDamageType(Enums.DamageType type, float value)
	{
		if (!_buffedDamageTypes.ContainsKey(type))
		{
			_buffedDamageTypes.Add(type, 0f);
		}
		_buffedDamageTypes[type] += value;
	}

	internal void RemoveBuffedStat(Enums.ItemStatType type, float value)
	{
		if (_buffedStats.ContainsKey(type))
		{
			_buffedStats[type] -= value;
			if (_buffedStats[type] <= 0f)
			{
				_buffedStats[type] = 0f;
			}
		}
	}

	internal void InitCalculatedStats(Dictionary<Enums.ItemStatType, float> calculatedStats)
	{
		_calculatedStats = calculatedStats;
	}

	internal void InitBuffedStats()
	{
		_buffedStats = new Dictionary<Enums.ItemStatType, float>();
	}

	internal virtual void AddDebuffAfterDelay(DebuffHandler debuffHandler, Character sourceCharacter, float delay)
	{
		StartCoroutine(AddDebuffAfterDelayAsync(debuffHandler, sourceCharacter, delay));
	}

	private IEnumerator AddDebuffAfterDelayAsync(DebuffHandler debuffHandler, Character sourceCharacter, float delay)
	{
		yield return new WaitForSeconds(delay);
		AddDebuff(debuffHandler, sourceCharacter);
	}

	internal virtual void AddDebuff(DebuffHandler debuffHandler, Character sourceCharacter)
	{
		if (!(_debuffContainer == null))
		{
			debuffHandler.SetCharacters(this, sourceCharacter);
			_debuffContainer.AddDebuff(debuffHandler, this);
			this.OnDebuffApplied?.Invoke(this, new DebuffAppliedEventArgs(debuffHandler.DebuffSO, this));
		}
	}

	internal virtual void RemoveDebuff(DebuffHandler debuffHandler)
	{
		this.OnDebuffRemoved?.Invoke(this, new DebuffRemovedEventArgs(debuffHandler.DebuffSO));
	}

	internal virtual bool HasBuff(BuffSO buffSO)
	{
		return _buffController.HasBuff(buffSO);
	}

	internal virtual void AddBuff(BuffSO buffSO)
	{
		float durationMod = 1f;
		if (GetCharacterType() == Enums.Enemies.EnemyType.Player)
		{
			durationMod = GetCalculatedStat(Enums.ItemStatType.BuffDuration) + 1f;
		}
		if (_buffController.AddBuff(buffSO, out var currentStacks, durationMod))
		{
			if (buffSO.ShowTextPopupOnFirstGain && currentStacks == 0)
			{
				DamageVisualizer.ShowTextPopup(buffSO.Name, Constants.Colors.PositiveEffectColor, 2f);
			}
			else if (buffSO.ShowTextPopupOnGain)
			{
				DamageVisualizer.ShowTextPopup(buffSO.Name, Constants.Colors.PositiveEffectColor, 2f);
			}
			this.OnBuffApplied?.Invoke(this, new BuffAppliedEventArgs(buffSO));
		}
	}

	internal virtual void RemoveBuff(BuffHandler buffHandler)
	{
		_buffController.RemoveBuff(buffHandler);
		this.OnBuffRemoved?.Invoke(this, new BuffAppliedEventArgs(buffHandler.BuffSO));
	}

	internal virtual void RemoveBuff(BuffSO buffSO)
	{
		_buffController.RemoveBuff(buffSO);
		this.OnBuffRemoved?.Invoke(this, new BuffAppliedEventArgs(buffSO));
	}

	internal List<BuffSO> GetBuffs()
	{
		return _buffController.GetBuffs();
	}

	internal void InitHealthData(float immuneDuration)
	{
		_immuneDurationAfterTakingDamage = immuneDuration;
	}

	internal void InitHealthEvents()
	{
		HealthSystem.OnDamaged += HealthSystem_OnDamaged;
		HealthSystem.OnDead += HealthSystem_OnDead;
		HealthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
		HealthSystem.OnHealthMaxChanged += HealthSystem_OnHealthMaxChanged;
	}

	private void HealthSystem_OnHealthMaxChanged(object sender, HealthMaxChangedEventArgs e)
	{
		this.OnHealthMaxChanged?.Invoke(this, e);
	}

	private void HealthSystem_OnHealthChanged(object sender, HealthChangedEventArgs e)
	{
		this.OnHealthChanged?.Invoke(this, e);
	}

	internal void RemoveHealthEvents()
	{
		HealthSystem.OnDamaged -= HealthSystem_OnDamaged;
		HealthSystem.OnDead -= HealthSystem_OnDead;
		HealthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
		HealthSystem.OnHealthMaxChanged -= HealthSystem_OnHealthMaxChanged;
	}

	private void HealthSystem_OnDead(object sender, EventArgs e)
	{
		if (!_onDeadHasTriggered)
		{
			_onDeadHasTriggered = true;
			_immunityBecauseOfTakenDamage = true;
			if (_buffController != null)
			{
				_buffController.CarrierDied();
			}
			if (_debuffContainer != null)
			{
				_debuffContainer.CarrierDied();
			}
			CharacterDied();
		}
	}

	internal void ResetDeathHasTriggered()
	{
		_onDeadHasTriggered = false;
	}

	private void HealthSystem_OnDamaged(object sender, DamageTakenEventArgs e)
	{
		CharacterDamaged(e.DamageDealt, e.ShouldTriggerAudioOnDamage);
		this.OnCharacterDamaged?.Invoke(this, e);
	}

	public bool CanTakeDamage()
	{
		return !_isDummy && !_immunityBecauseOfTakenDamage && !_immunityBecauseOfDash && !_immunityBecauseOfLeavingShop && !_immunityBecauseOfBurrowing && !IsDead && !_immunityBecauseOfReviving;
	}

	internal void SetImmunityForDuration(Enums.ImmunitySource source, float duration)
	{
		StartCoroutine(SetImmunity(source, duration));
	}

	private IEnumerator SetImmunity(Enums.ImmunitySource source, float duration)
	{
		switch (source)
		{
		case Enums.ImmunitySource.Dash:
			_immunityBecauseOfDash = true;
			break;
		case Enums.ImmunitySource.DamageTaken:
			_immunityBecauseOfTakenDamage = true;
			break;
		case Enums.ImmunitySource.LeavingShop:
			StartCoroutine(SetImmunityBecauseOfLeavingShopForDuration(duration));
			break;
		case Enums.ImmunitySource.Burrowed:
			_immunityBecauseOfBurrowing = true;
			break;
		case Enums.ImmunitySource.Reviving:
			_immunityBecauseOfReviving = true;
			break;
		}
		yield return new WaitForSeconds(duration);
		switch (source)
		{
		case Enums.ImmunitySource.Dash:
			_immunityBecauseOfDash = false;
			break;
		case Enums.ImmunitySource.DamageTaken:
			_immunityBecauseOfTakenDamage = false;
			break;
		case Enums.ImmunitySource.Burrowed:
			_immunityBecauseOfBurrowing = false;
			break;
		case Enums.ImmunitySource.Reviving:
			_immunityBecauseOfReviving = false;
			break;
		}
	}

	private IEnumerator SetImmunityBecauseOfLeavingShopForDuration(float duration)
	{
		_immunityBecauseOfLeavingShop = true;
		_immunityBubbleAnimatorController?.SetBool("Spawned", value: true);
		float seconds = duration * 0.8f;
		float remainingDuration = duration * 0.2f;
		yield return new WaitForSeconds(seconds);
		_immunityBubbleAnimatorController?.SetBool("DroppingSoon", value: true);
		yield return new WaitForSeconds(remainingDuration);
		_immunityBubbleAnimatorController?.SetBool("Spawned", value: false);
		_immunityBubbleAnimatorController?.SetBool("DroppingSoon", value: false);
		_immunityBecauseOfLeavingShop = false;
	}

	public virtual void CharacterDied()
	{
	}

	public virtual void CharacterDamaged(float damageTaken, bool shouldTriggerAudioOnDamage)
	{
	}

	public override bool Equals(object other)
	{
		if (!(other is Character))
		{
			return false;
		}
		return UniqueKey == ((Character)other).UniqueKey;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	internal void HandleLifeDrain(float damage, float healthDrainFromWeapon)
	{
		float num = _calculatedStats[Enums.ItemStatType.LifeDrainPercentage] + healthDrainFromWeapon;
		if (num != 0f)
		{
			float num2 = num * damage;
			if (num2 > 0f)
			{
				HealthSystem.Heal(num2);
				OnLifeDrain();
			}
			else
			{
				HealthSystem.Damage(Math.Abs(num2), shouldTriggerAudioOnDamage: true);
			}
		}
	}

	public virtual void OnLifeDrain()
	{
	}

	internal void TriggerDashesCountSet(EventArgs e)
	{
		this.OnDashesCountSet?.Invoke(this, e);
	}

	internal void TriggerDashEvent(DashCooldownEventArgs e)
	{
		this.OnDashed?.Invoke(this, e);
	}

	internal void TriggerOnLoadedEvent(EventArgs e)
	{
		this.OnLoaded?.Invoke(this, e);
	}

	internal void TriggerDashCooldownEvent(DashCooldownEventArgs e)
	{
		this.OnDashCooldownUpdated?.Invoke(this, e);
	}

	internal void TriggerDashCountChangedEvent(DashCooldownEventArgs e)
	{
		this.OnDashCountChanged?.Invoke(this, e);
	}

	internal void TriggerDashReadyEvent(DashCooldownEventArgs e)
	{
		this.OnDashReady?.Invoke(this, e);
	}

	internal void HandleBrotatoLifeDrain(float lifedrainChance)
	{
		if (_canHealOffLifedrain && RandomHelper.GetRollSuccess(lifedrainChance))
		{
			HealthSystem.Heal(HealthSystem.GetHealthMax() * 0.01f);
			OnLifeDrain();
			StartCoroutine(DisableLifedrainForDuration(0.1f));
		}
	}

	private IEnumerator DisableLifedrainForDuration(float duration)
	{
		_canHealOffLifedrain = false;
		yield return new WaitForSeconds(duration);
		_canHealOffLifedrain = true;
	}

	internal virtual void TriggerDebuffsAddedEvent(List<Enums.Debuff.DebuffType> debuffsCaused)
	{
	}
}
