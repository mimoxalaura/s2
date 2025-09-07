using BackpackSurvivors.System;
using UnityEngine;

public class LocalizationController : SingletonController<LocalizationController>
{
	private void Start()
	{
		base.IsInitialized = true;
	}

	public string Translate(GameObject keyContainer)
	{
		Localization_SOURCE localization_SOURCE = Object.FindObjectOfType<Localization_SOURCE>();
		Localization_KEY component = keyContainer.GetComponent<Localization_KEY>();
		return localization_SOURCE.Lang_ReturnText(component.KeyID);
	}

	public string Translate(string key)
	{
		return Object.FindObjectOfType<Localization_SOURCE>().Lang_ReturnText(key);
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
