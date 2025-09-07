using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Achievement;

[CreateAssetMenu(fileName = "SteamStat", menuName = "Game/SteamStat", order = 3)]
internal class SteamStatSO : ScriptableObject
{
	[SerializeField]
	public Enums.SteamStat SteamStat;

	[SerializeField]
	public string Name;

	private void RenameFile()
	{
	}
}
