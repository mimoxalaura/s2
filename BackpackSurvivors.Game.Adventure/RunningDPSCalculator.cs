using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure;

internal class RunningDPSCalculator
{
	private class DamageAtTime
	{
		internal float Time;

		internal float Damage;

		public DamageAtTime(float time, float damage)
		{
			Time = time;
			Damage = damage;
		}
	}

	private List<DamageAtTime> _damageAtTimeEntries = new List<DamageAtTime>();

	internal void Clear()
	{
		_damageAtTimeEntries.Clear();
	}

	internal void AddDamage(float damage)
	{
		DamageAtTime item = new DamageAtTime(Time.time, damage);
		_damageAtTimeEntries.Add(item);
	}

	internal float GetDPSBetweenTimes(float startTime, float endTime)
	{
		float num = endTime - startTime;
		return _damageAtTimeEntries.Where((DamageAtTime dat) => dat.Time >= startTime && dat.Time <= endTime).Sum((DamageAtTime dat) => dat.Damage) / num;
	}
}
