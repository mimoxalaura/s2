using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.ScriptableObjects.Achievement;
using BackpackSurvivors.System.Helper;
using QFSW.QC;
using Steamworks;
using UnityEngine;

namespace BackpackSurvivors.System.Steam;

internal class SteamController : SingletonController<SteamController>
{
	protected Callback<GameOverlayActivated_t> _gameOverlayActivatedCallback;

	protected Callback<UserStatsReceived_t> _userStatsReceivedCallback;

	protected Callback<UserAchievementIconFetched_t> _userAchievementIconFetchedCallback;

	private bool _userStatsReceived;

	private List<SteamAchievement> _steamAchievements = new List<SteamAchievement>();

	private const string _achievementDisplayAttributeLocalizedName = "name";

	private const string _achievementDisplayAttributeLocalizedDescription = "desc";

	private const string _achievementDisplayAttributeHidden = "hidden";

	private void Start()
	{
		base.IsInitialized = true;
		RequestCurrentStats();
		_steamAchievements = GetAllAchievements();
	}

	private void OnEnable()
	{
		try
		{
			if (SteamManager.Initialized)
			{
				_gameOverlayActivatedCallback = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
				_userStatsReceivedCallback = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
				_userAchievementIconFetchedCallback = Callback<UserAchievementIconFetched_t>.Create(OnUserAchievementIconFetched);
			}
		}
		catch (Exception)
		{
		}
	}

	internal string GetSteamName()
	{
		if (SteamManager.Initialized)
		{
			return SteamFriends.GetPersonaName();
		}
		return string.Empty;
	}

	[Command("GetAllAchievements", Platform.AllPlatforms, MonoTargetType.Single)]
	internal List<SteamAchievement> GetAllAchievements()
	{
		List<SteamAchievement> list = new List<SteamAchievement>();
		if (!_userStatsReceived)
		{
			Debug.LogWarning("Steam RequestCurrentStats() was not successful. Unable to get achievements");
			return list;
		}
		uint numAchievements = SteamUserStats.GetNumAchievements();
		for (uint num = 0u; num < numAchievements; num++)
		{
			string achievementName = SteamUserStats.GetAchievementName(num);
			SteamAchievement achievementByName = GetAchievementByName(achievementName);
			list.Add(achievementByName);
		}
		return list;
	}

	[Command("UnlockAchievement", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UnlockAchievement(Enums.AchievementEnum achievementEnum)
	{
		AchievementSO achievementSOByEnum = GameDatabaseHelper.GetAchievementSOByEnum(achievementEnum);
		if (!IsAchievementAlreadyUnlocked(achievementSOByEnum) && !IsAchievementForOtherVersion(achievementSOByEnum))
		{
			SteamUserStats.SetAchievement(achievementSOByEnum.Name);
			SteamUserStats.StoreStats();
		}
	}

	[Command("ResetStats", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void ResetStats(bool alsoResetAchievements = true)
	{
		SteamUserStats.ResetAllStats(alsoResetAchievements);
		RequestCurrentStats();
	}

	private bool IsAchievementAlreadyUnlocked(AchievementSO achievementSO)
	{
		return _steamAchievements.FirstOrDefault((SteamAchievement a) => a.Name.Equals(achievementSO.Name))?.Achieved ?? false;
	}

	private bool IsAchievementForOtherVersion(AchievementSO achievementSO)
	{
		return GameDatabase.IsDemo != achievementSO.IsDemoAchievement;
	}

	[Command("UpdateSteamStatValueInt", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UpdateSteamStat(Enums.SteamStat steamStat, int change)
	{
		SteamStatSO steamStatSOByEnum = GameDatabaseHelper.GetSteamStatSOByEnum(steamStat);
		SteamUserStats.GetStat(steamStatSOByEnum.Name, out int pData);
		SteamUserStats.SetStat(nData: pData + change, pchName: steamStatSOByEnum.Name);
		SteamUserStats.StoreStats();
	}

	[Command("UpdateSteamStatValueFloat", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UpdateSteamStat(Enums.SteamStat steamStat, float change)
	{
		SteamStatSO steamStatSOByEnum = GameDatabaseHelper.GetSteamStatSOByEnum(steamStat);
		SteamUserStats.GetStat(steamStatSOByEnum.Name, out float pData);
		SteamUserStats.SetStat(fData: pData + change, pchName: steamStatSOByEnum.Name);
		SteamUserStats.StoreStats();
	}

	private void OnUserAchievementIconFetched(UserAchievementIconFetched_t userAchievementIconFetched)
	{
		if (userAchievementIconFetched.m_nIconHandle == 0)
		{
			Debug.Log("Achievement Icon not found");
		}
		else
		{
			GetSteamImageAsTexture(userAchievementIconFetched.m_nIconHandle);
		}
	}

	private void OnUserStatsReceived(UserStatsReceived_t userStatsReceived)
	{
	}

	private SteamAchievement GetAchievementByName(string name)
	{
		SteamUserStats.GetAchievementAchievedPercent(name, out var pflPercent);
		SteamUserStats.GetAchievementAndUnlockTime(name, out var pbAchieved, out var punUnlockTime);
		string achievementDisplayAttribute = SteamUserStats.GetAchievementDisplayAttribute(name, "name");
		string achievementDisplayAttribute2 = SteamUserStats.GetAchievementDisplayAttribute(name, "desc");
		bool hidden = SteamUserStats.GetAchievementDisplayAttribute(name, "hidden") == "1";
		int achievementIcon = SteamUserStats.GetAchievementIcon(name);
		return new SteamAchievement(name, pbAchieved, pflPercent, UnixEpochToDateTime(punUnlockTime), achievementDisplayAttribute, achievementDisplayAttribute2, hidden, achievementIcon);
	}

	private DateTime UnixEpochToDateTime(double unixTimeStamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp).ToLocalTime();
	}

	private void RequestCurrentStats()
	{
		_userStatsReceived = SteamUserStats.RequestCurrentStats();
	}

	internal Texture2D GetSteamAvatar(out bool success)
	{
		try
		{
			if (SteamManager.Initialized)
			{
				int largeFriendAvatar = SteamFriends.GetLargeFriendAvatar(GetSteamId());
				if (largeFriendAvatar == -1)
				{
					success = false;
					return null;
				}
				success = true;
				return GetSteamImageAsTexture(largeFriendAvatar);
			}
			success = false;
			return null;
		}
		catch (Exception)
		{
			success = false;
			return null;
		}
	}

	private Texture2D GetSteamImageAsTexture(int image)
	{
		Texture2D texture2D = null;
		if (SteamUtils.GetImageSize(image, out var pnWidth, out var pnHeight))
		{
			byte[] array = new byte[pnWidth * pnHeight * 4];
			if (SteamUtils.GetImageRGBA(image, array, (int)(pnHeight * pnWidth * 4)))
			{
				texture2D = new Texture2D((int)pnWidth, (int)pnHeight, TextureFormat.RGBA32, mipChain: false, linear: true);
				texture2D.LoadRawTextureData(array);
				texture2D.Apply();
			}
		}
		return texture2D;
	}

	internal CSteamID GetSteamId()
	{
		if (SteamManager.Initialized)
		{
			return SteamUser.GetSteamID();
		}
		return default(CSteamID);
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			Debug.Log("Steam Overlay has been activated");
		}
		else
		{
			Debug.Log("Steam Overlay has been closed");
		}
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}

	internal static void SetRichPresence(Enums.RichPresenceOptions richPresenceOption)
	{
		SteamFriends.SetRichPresence("steam_display", $"#{richPresenceOption}");
	}
}
