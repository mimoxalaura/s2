using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Saving;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure;

internal class RunningDPSLogger : MonoBehaviour
{
	private class DPSLogEntry
	{
		internal float Time;

		internal float DPS;

		internal float TotalEnemyHealth;
	}

	[SerializeField]
	private float _logEveryXSeconds;

	[SerializeField]
	private float _logDPSOfLastXSeconds;

	private bool _shouldLog;

	private List<DPSLogEntry> _dpsLogs = new List<DPSLogEntry>();

	private TimeBasedLevelController _levelController;

	internal void SetShouldLog(bool shouldLog)
	{
		_shouldLog = shouldLog;
		if (_shouldLog)
		{
			StartCoroutine(LogDPS());
		}
	}

	[Command("DPS.WriteLogsToFile", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void WriteDPSLogsToFile()
	{
		SetShouldLog(shouldLog: true);
		_levelController = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
		if (_levelController != null)
		{
			_levelController.OnLevelCompleted += TimeBasedLevelController_OnLevelCompleted;
		}
	}

	private void TimeBasedLevelController_OnLevelCompleted(object sender, EventArgs e)
	{
		SaveDPSLogsToFile();
		_levelController.OnLevelCompleted -= TimeBasedLevelController_OnLevelCompleted;
	}

	[Command("DPS.SaveLogsToFile", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void SaveDPSLogsToFile()
	{
		if (_dpsLogs != null && _dpsLogs.Count != 0)
		{
			string text = string.Join(",", _dpsLogs.Select((DPSLogEntry dl) => $"{dl.Time}"));
			string text2 = string.Join(",", _dpsLogs.Select((DPSLogEntry dl) => $"{dl.DPS:0.0}"));
			string text3 = string.Join(",", _dpsLogs.Select((DPSLogEntry dl) => $"{dl.TotalEnemyHealth:0}"));
			string levelName = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>().CurrentLevel.LevelName;
			string contents = levelName + Environment.NewLine + text + Environment.NewLine + text2 + Environment.NewLine + text3 + Environment.NewLine;
			File.WriteAllText(SaveFileController.GetFilePath("DPS Log " + levelName + " " + DateTime.Now.ToShortDateString().Replace("/", "-") + "-" + DateTime.Now.ToShortTimeString().Replace(":", ".") + ".txt"), contents);
		}
	}

	private IEnumerator LogDPS()
	{
		while (_shouldLog)
		{
			float dPSLastXSeconds = SingletonController<WeaponDamageAndDPSController>.Instance.GetDPSLastXSeconds(_logDPSOfLastXSeconds);
			float totalEnemyHealth = SingletonController<EnemyController>.Instance.TotalEnemyHealth;
			AddDPSLogEntry(dPSLastXSeconds, totalEnemyHealth);
			yield return new WaitForSeconds(_logEveryXSeconds);
		}
	}

	private void AddDPSLogEntry(float dps, float totalEnemyHealth)
	{
		DPSLogEntry item = new DPSLogEntry
		{
			Time = Time.time,
			DPS = dps,
			TotalEnemyHealth = totalEnemyHealth
		};
		_dpsLogs.Add(item);
	}

	private void OnDestroy()
	{
		if (_levelController != null)
		{
			_levelController.OnLevelCompleted -= TimeBasedLevelController_OnLevelCompleted;
		}
	}
}
