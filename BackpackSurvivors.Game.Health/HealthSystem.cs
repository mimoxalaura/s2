using System;
using BackpackSurvivors.Game.Health.Events;
using UnityEngine;

namespace BackpackSurvivors.Game.Health;

public class HealthSystem
{
	internal delegate void TakenDamageEventHandler(object sender, DamageTakenEventArgs e);

	internal delegate void HealthMaxChangedEventHandler(object sender, HealthMaxChangedEventArgs e);

	internal delegate void HealthChangedEventHandler(object sender, HealthChangedEventArgs e);

	private float _healthMax;

	private float _health;

	private bool _canTakeDamage = true;

	private bool _canDie = true;

	private float _percentageOfMaxHealthWhenEnteringShop;

	internal event HealthChangedEventHandler OnHealthChanged;

	internal event HealthMaxChangedEventHandler OnHealthMaxChanged;

	internal event TakenDamageEventHandler OnDamaged;

	internal event EventHandler OnHealed;

	internal event EventHandler OnDead;

	public HealthSystem(float healthMax)
	{
		_healthMax = healthMax;
		_health = healthMax;
	}

	public void PreventDamageAndDying()
	{
		_canTakeDamage = false;
		_canDie = false;
	}

	public void StoreHealthWhenEnteringShop()
	{
		_percentageOfMaxHealthWhenEnteringShop = _health / _healthMax;
	}

	public void ApplyChangesInHealthMaxWhenExitingShop()
	{
		float health = _percentageOfMaxHealthWhenEnteringShop * _healthMax;
		SetHealth(health);
	}

	public float GetHealth()
	{
		return _health;
	}

	public float GetHealthMax()
	{
		return _healthMax;
	}

	public float GetHealthNormalized()
	{
		return _health / _healthMax;
	}

	public void Damage(float damage, bool shouldTriggerAudioOnDamage)
	{
		if (_canTakeDamage)
		{
			HealthChangedEventArgs e = new HealthChangedEventArgs
			{
				OldHealth = _health
			};
			_health -= damage;
			if (_health < 0f)
			{
				_health = 0f;
			}
			e.NewHealth = _health;
			if (e.HealthDidChange)
			{
				this.OnHealthChanged?.Invoke(this, e);
			}
			this.OnDamaged?.Invoke(this, new DamageTakenEventArgs(damage, shouldTriggerAudioOnDamage, _healthMax, _health));
			if (_health <= 0f)
			{
				Die();
			}
		}
	}

	public void Die()
	{
		if (_canDie)
		{
			this.OnDead?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool IsDead()
	{
		return _health <= 0f;
	}

	public void Heal(float amount)
	{
		HealthChangedEventArgs e = new HealthChangedEventArgs
		{
			OldHealth = _health
		};
		_health += amount;
		_health = Mathf.Clamp(_health, _health, _healthMax);
		e.NewHealth = _health;
		if (e.HealthDidChange)
		{
			this.OnHealthChanged?.Invoke(this, e);
		}
		this.OnHealed?.Invoke(this, EventArgs.Empty);
	}

	public void HealComplete()
	{
		HealthChangedEventArgs e = new HealthChangedEventArgs
		{
			OldHealth = _health
		};
		_health = _healthMax;
		e.NewHealth = _health;
		if (e.HealthDidChange)
		{
			this.OnHealthChanged?.Invoke(this, e);
		}
		this.OnHealed?.Invoke(this, EventArgs.Empty);
	}

	public void SetHealthMax(float healthMax, bool setHealthToMax)
	{
		HealthChangedEventArgs e = new HealthChangedEventArgs
		{
			OldHealth = _health
		};
		float healthMax2 = _healthMax;
		_healthMax = healthMax;
		if (setHealthToMax)
		{
			_health = healthMax;
		}
		e.NewHealth = _health;
		if (e.HealthDidChange)
		{
			this.OnHealthChanged?.Invoke(this, e);
		}
		HealthMaxChangedEventArgs e2 = new HealthMaxChangedEventArgs(healthMax2, healthMax);
		if (e2.HealthMaxChanged)
		{
			this.OnHealthMaxChanged?.Invoke(this, e2);
		}
	}

	public void SetHealth(float health)
	{
		HealthChangedEventArgs e = new HealthChangedEventArgs
		{
			OldHealth = _health
		};
		if (health > _healthMax)
		{
			health = _healthMax;
		}
		if (health < 0f)
		{
			health = 0f;
		}
		_health = health;
		e.NewHealth = _health;
		this.OnHealthChanged?.Invoke(this, e);
		if (health <= 0f)
		{
			Die();
		}
	}

	public static bool TryGetHealthSystem(GameObject getHealthSystemGameObject, out HealthSystem healthSystem, bool logErrors = false)
	{
		healthSystem = null;
		if (getHealthSystemGameObject != null)
		{
			if (getHealthSystemGameObject.TryGetComponent<IGetHealthSystem>(out var component))
			{
				healthSystem = component.GetHealthSystem();
				if (healthSystem != null)
				{
					return true;
				}
				if (logErrors)
				{
					Debug.LogError("Got HealthSystem from object but healthSystem is null!");
				}
				return false;
			}
			if (logErrors)
			{
				Debug.LogError($"Referenced Game Object '{getHealthSystemGameObject}' does not have a script that implements IGetHealthSystem!");
			}
			return false;
		}
		if (logErrors)
		{
			Debug.LogError("You need to assign the field 'getHealthSystemGameObject'!");
		}
		return false;
	}
}
