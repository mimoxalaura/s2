using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.Video;

namespace BackpackSurvivors.ScriptableObjects.Unlockable;

[CreateAssetMenu(fileName = "Unlockable", menuName = "Game/Unlockables/Unlockable", order = 1)]
public class UnlockableSO : ScriptableObject
{
	public string Title;

	public Texture PreviewIcon;

	public Enums.Unlockable Unlockable;

	public string Name;

	public bool ShouldShowInShop;

	public bool FeatureUnlocked = true;

	public string Description;

	public string FullDescription;

	public int FullDescriptionValueForCalculation;

	public int UnlockPrice;

	public Enums.CurrencyType PriceCurrencyType;

	public int UnlockAvailableAmount;

	public float ScalingPriceMultiplier;

	public Sprite Icon;

	public Sprite IconForEffect;

	public bool ShowEffect;

	public VideoClip TutorialVideo;

	private void CreateData()
	{
		if (Icon != null)
		{
			PreviewIcon = Icon.texture;
		}
		Title = Name;
	}
}
