using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure;

public class WeaponDamageAndDPSController : SingletonController<WeaponDamageAndDPSController>
{
	[SerializeField]
	private RunningDPSLogger _runningDPSLogger;

	private Dictionary<int, WeaponSOAndStats> _weaponSOAndStats = new Dictionary<int, WeaponSOAndStats>();

	private List<IDamageExposer> _damageExposers = new List<IDamageExposer>();

	private readonly RunningDPSCalculator _runningDPSCalculator = new RunningDPSCalculator();

	private void Start()
	{
		base.IsInitialized = true;
	}

	public void AddDamageExposer(IDamageExposer damageExposer)
	{
		_damageExposers.Add(damageExposer);
	}

	public WeaponSOAndStats GetStatsOnWeapon(WeaponSO weaponSO)
	{
		if (_weaponSOAndStats.ContainsKey(weaponSO.Id))
		{
			return _weaponSOAndStats[weaponSO.Id];
		}
		return null;
	}

	public void AddDamage(WeaponSO weaponSO, float damage)
	{
		if (!(weaponSO != null) || !weaponSO.LogDamage)
		{
			return;
		}
		if (!_weaponSOAndStats.ContainsKey(weaponSO.Id))
		{
			AddWeaponSO(weaponSO);
		}
		_weaponSOAndStats[weaponSO.Id].AddDamage(damage);
		_runningDPSCalculator.AddDamage(damage);
		foreach (IDamageExposer damageExposer in _damageExposers)
		{
			damageExposer.AddDamage(damage);
		}
	}

	public void AddTimeActive(WeaponSO weaponSO, float timeActive)
	{
		if (!_weaponSOAndStats.ContainsKey(weaponSO.Id))
		{
			AddWeaponSO(weaponSO);
		}
		_weaponSOAndStats[weaponSO.Id].AddTimeActive(timeActive);
	}

	private void AddWeaponSO(WeaponSO weaponSO)
	{
		_weaponSOAndStats.Add(weaponSO.Id, new WeaponSOAndStats
		{
			WeaponSO = weaponSO
		});
	}

	public float GetDPSLastXSeconds(float lastXSeconds)
	{
		float startTime = Time.time - lastXSeconds;
		float time = Time.time;
		return _runningDPSCalculator.GetDPSBetweenTimes(startTime, time);
	}

	public List<WeaponSOAndStats> GetAll()
	{
		return _weaponSOAndStats.Values.OrderBy((WeaponSOAndStats x) => x.Damage).ToList();
	}

	public override void Clear()
	{
		ResetStats();
	}

	public override void ClearAdventure()
	{
		ResetStats();
	}

	private void ResetStats()
	{
		_weaponSOAndStats.Clear();
		_damageExposers.Clear();
		_runningDPSCalculator.Clear();
	}
}
