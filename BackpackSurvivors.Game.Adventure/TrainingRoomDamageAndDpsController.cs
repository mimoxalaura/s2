using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.UI.Shop;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure;

public class TrainingRoomDamageAndDpsController : MonoBehaviour, IDamageExposer
{
	[SerializeField]
	private TrainingRoomDPSAndDamageUI _trainingRoomDPSAndDamageUI;

	public float TotalDamage;

	public Dictionary<float, float> DPSDamage;

	private float nextUpdate;

	private float nextUpdateTime = 0.1f;

	private int timeToKeepInStack = 5;

	private void Start()
	{
		Reset();
	}

	private void Update()
	{
		if (Time.time >= nextUpdate)
		{
			nextUpdate = (float)Mathf.FloorToInt(Time.time) + nextUpdateTime;
			DPSDamage = DPSDamage.Where((KeyValuePair<float, float> kvp) => kvp.Key > Time.time - (float)timeToKeepInStack).ToDictionary((KeyValuePair<float, float> x) => x.Key, (KeyValuePair<float, float> x) => x.Value);
			_trainingRoomDPSAndDamageUI.UpdateStats(TotalDamage, CalculateDPS());
		}
	}

	public void AddDamage(float damage)
	{
		float time = Time.time;
		if (!DPSDamage.ContainsKey(time))
		{
			DPSDamage.Add(time, 0f);
		}
		DPSDamage[time] += damage;
		TotalDamage += damage;
	}

	private float CalculateDPS()
	{
		if (DPSDamage == null || !DPSDamage.Any())
		{
			return 0f;
		}
		return DPSDamage.Sum((KeyValuePair<float, float> x) => x.Value) / (float)timeToKeepInStack;
	}

	public void Reset()
	{
		DPSDamage = new Dictionary<float, float>();
		TotalDamage = 0f;
	}
}
