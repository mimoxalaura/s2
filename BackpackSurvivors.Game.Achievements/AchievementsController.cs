using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Steam;
using BackpackSurvivors.UI.Achievements;
using BackpackSurvivors.UI.Shared;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Achievements;

internal class AchievementsController : BaseSingletonModalUIController<AchievementsController>
{
	[SerializeField]
	private AchievementsUI _achievementUI;

	[SerializeField]
	private Canvas _canvas;

	private List<SteamAchievement> _steamAchievements;

	private void Start()
	{
		base.IsInitialized = true;
		LoadAchievements();
	}

	private void LoadAchievements()
	{
		_steamAchievements = SingletonController<SteamController>.Instance.GetAllAchievements();
	}

	public override bool CloseOnCancelInput()
	{
		return true;
	}

	[Command("achievements.open", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		base.OpenUI();
		_achievementUI.Init(from x in _steamAchievements
			orderby x.PercentAchieved descending, x.UnlockTime
			select x);
		_achievementUI.OpenUI();
	}

	public override void CloseUI()
	{
		base.CloseUI();
		_achievementUI.CloseUI();
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Achievements;
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool AudioOnClose()
	{
		return true;
	}
}
