using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Core;

internal class GameDatabase : SingletonController<GameDatabase>
{
	[SerializeField]
	internal bool _isDemo;

	[SerializeField]
	internal string _demoAppId;

	[SerializeField]
	private GameDatabaseSO _gameDatabaseSO;

	[SerializeField]
	private GameDatabaseSO _demoGameDatabaseSO;

	internal GameDatabaseSO GameDatabaseSO => GetCorrectGameDatabaseSO();

	internal static bool IsDemo => SingletonController<GameDatabase>.Instance._isDemo;

	private void Start()
	{
		base.IsInitialized = true;
	}

	private GameDatabaseSO GetCorrectGameDatabaseSO()
	{
		if (_isDemo)
		{
			return _demoGameDatabaseSO;
		}
		return _gameDatabaseSO;
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}

	internal string GetDemoAppId()
	{
		return _demoAppId;
	}
}
