using System;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Collection;
using BackpackSurvivors.UI.Collection.ListItems.Item;
using BackpackSurvivors.UI.Collection.ListItems.Relic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureRewardUI : MonoBehaviour
{
	[SerializeField]
	private CollectionWeaponUI _adventureRewardUIWeapon;

	[SerializeField]
	private CollectionItemUI _adventureRewardUIItem;

	[SerializeField]
	private CollectionRelicUI _adventureRewardUIRelic;

	[SerializeField]
	private Image _adventureRewardUITitanicSouls;

	[SerializeField]
	private TextMeshProUGUI _adventureRewardUIText;

	internal void Init(RewardSO item)
	{
		string sourceText = string.Empty;
		switch (item.CompletionRewardType)
		{
		case Enums.RewardType.Weapon:
		{
			CollectionWeaponUI collectionWeaponUI = UnityEngine.Object.Instantiate(_adventureRewardUIWeapon, base.transform);
			collectionWeaponUI.Init((WeaponSO)item.CompletionReward, unlocked: true, interactable: true);
			collectionWeaponUI.GetComponent<MainButton>().ToggleHoverEffects(toggled: false);
			sourceText = ((WeaponSO)item.CompletionReward).Name + Environment.NewLine + item.Description;
			break;
		}
		case Enums.RewardType.Item:
		{
			CollectionItemUI collectionItemUI = UnityEngine.Object.Instantiate(_adventureRewardUIItem, base.transform);
			collectionItemUI.Init((ItemSO)item.CompletionReward, unlocked: true, interactable: true);
			collectionItemUI.GetComponent<MainButton>().ToggleHoverEffects(toggled: false);
			sourceText = ((ItemSO)item.CompletionReward).Name + Environment.NewLine + item.Description;
			break;
		}
		case Enums.RewardType.Relic:
		{
			CollectionRelicUI collectionRelicUI = UnityEngine.Object.Instantiate(_adventureRewardUIRelic, base.transform);
			collectionRelicUI.Init((RelicSO)item.CompletionReward, unlocked: true, interactable: true);
			collectionRelicUI.GetComponent<MainButton>().ToggleHoverEffects(toggled: false);
			sourceText = ((RelicSO)item.CompletionReward).Name + Environment.NewLine + item.Description;
			break;
		}
		case Enums.RewardType.TitanicSouls:
		{
			UnityEngine.Object.Instantiate(_adventureRewardUITitanicSouls, base.transform);
			float rewardMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetRewardMultiplierFromHellfire(SingletonController<AdventureController>.Instance.ActiveAdventure);
			int num = (int)((float)item.Amount * rewardMultiplierFromHellfire);
			sourceText = $"{num} Titanic Souls";
			break;
		}
		}
		TextMeshProUGUI textMeshProUGUI = UnityEngine.Object.Instantiate(_adventureRewardUIText, base.transform);
		textMeshProUGUI.SetText(sourceText);
		textMeshProUGUI.fontSize = 16f;
	}
}
