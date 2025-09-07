using BackpackSurvivors.System;
using BackpackSurvivors.System.Steam;
using UnityEngine;

namespace BackpackSurvivors.Game.MainMenu;

public class SteamRichPresenceController : MonoBehaviour
{
	[SerializeField]
	private Enums.RichPresenceOptions richPresenceOptionToDisplay;

	private void Start()
	{
		SteamController.SetRichPresence(richPresenceOptionToDisplay);
	}
}
