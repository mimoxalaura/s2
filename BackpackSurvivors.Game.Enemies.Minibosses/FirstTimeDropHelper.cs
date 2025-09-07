using System.Collections;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Minibosses;

public class FirstTimeDropHelper : MonoBehaviour
{
	[SerializeField]
	private SoftPopupHelper _softPopupHelper;

	[SerializeField]
	private AudioClip _audioClip;

	[Header("DEBUG")]
	[SerializeField]
	private RewardSO[] _debugRewards;

	private RewardSO[] _rewardsToDropOnce;

	private bool _itemsShouldBeAddedToFutureAdventures;

	private string _rewardDescription;

	public void Init(RewardSO[] rewardSOs, bool itemsShouldBeAddedToFutureAdventures, string rewardDescription)
	{
		_rewardsToDropOnce = rewardSOs;
		_itemsShouldBeAddedToFutureAdventures = itemsShouldBeAddedToFutureAdventures;
		_rewardDescription = rewardDescription;
	}

	public void HandleDrops()
	{
		SingletonController<GameController>.Instance.IsShowingOneTimeRewards = true;
		if (_rewardsToDropOnce != null && _rewardsToDropOnce.Any())
		{
			StartCoroutine(HandleDropsAsync(_rewardsToDropOnce, _itemsShouldBeAddedToFutureAdventures, _rewardDescription));
		}
		else
		{
			SingletonController<GameController>.Instance.IsShowingOneTimeRewards = false;
		}
	}

	private IEnumerator HandleDropsAsync(RewardSO[] rewards, bool itemsShouldBeAddedToFutureAdventures, string description)
	{
		try
		{
			BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
			yield return new WaitForSeconds(1f);
			foreach (RewardSO reward in rewards)
			{
				if (reward.CompletionRewardType == Enums.RewardType.TitanicSouls)
				{
					Sprite sprite = SpriteHelper.GetCurrencySprite(Enums.CurrencyType.TitanSouls);
					_softPopupHelper.ShowInformationUI($"Received {reward.Amount} Titan Souls", sprite, description, Enums.SoftPopupType.FirstKill);
					yield return new WaitForSeconds(0.5f);
					player.ShowItemEffect(sprite, playAudio: true);
					yield return new WaitForSeconds(0.5f);
					SingletonController<AudioController>.Instance.PlaySFXClip(_audioClip, 1f);
					SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.TitanSouls, reward.Amount, Enums.CurrencySource.Reward);
					yield return new WaitForSeconds(3f);
				}
				else if (reward.CompletionRewardType == Enums.RewardType.Item)
				{
					ItemSO itemReward = reward.CompletionReward as ItemSO;
					Sprite sprite = itemReward.IngameImage;
					_softPopupHelper.ShowInformationUI("Received [<color=#" + ColorHelper.GetColorHexcodeForRarity(itemReward.ItemRarity) + ">" + itemReward.Name + "</color>]", sprite, description, Enums.SoftPopupType.FirstKill);
					yield return new WaitForSeconds(0.5f);
					player.ShowItemEffect(sprite, playAudio: true);
					yield return new WaitForSeconds(0.5f);
					SingletonController<AudioController>.Instance.PlaySFXClip(_audioClip, 1f);
					SingletonController<BackpackController>.Instance.OpenUI();
					SingletonController<BackpackController>.Instance.AddItemToStorage(itemReward, showVfx: false, fromMerge: false, updateLines: false);
					SingletonController<BackpackController>.Instance.CloseUI();
					if (itemsShouldBeAddedToFutureAdventures)
					{
						SingletonController<UnlockedEquipmentController>.Instance.AddItemReward(reward);
					}
					yield return new WaitForSeconds(3f);
				}
				else if (reward.CompletionRewardType == Enums.RewardType.Weapon)
				{
					WeaponSO weaponReward = reward.CompletionReward as WeaponSO;
					Sprite sprite = weaponReward.IngameImage;
					_softPopupHelper.ShowInformationUI("Received [<color=#" + ColorHelper.GetColorHexcodeForRarity(weaponReward.ItemRarity) + ">" + weaponReward.Name + "</color>]", sprite, description, Enums.SoftPopupType.FirstKill);
					yield return new WaitForSeconds(0.5f);
					player.ShowItemEffect(sprite, playAudio: true);
					yield return new WaitForSeconds(0.5f);
					SingletonController<AudioController>.Instance.PlaySFXClip(_audioClip, 1f);
					SingletonController<BackpackController>.Instance.OpenUI();
					SingletonController<BackpackController>.Instance.AddWeaponToStorage(weaponReward, showVfx: false, fromMerge: false, updateLines: false);
					SingletonController<BackpackController>.Instance.CloseUI();
					if (itemsShouldBeAddedToFutureAdventures)
					{
						SingletonController<UnlockedEquipmentController>.Instance.AddWeaponReward(reward);
					}
					yield return new WaitForSeconds(3f);
				}
				else if (reward.CompletionRewardType == Enums.RewardType.Bag)
				{
					BagSO bagReward = reward.CompletionReward as BagSO;
					Sprite sprite = bagReward.IngameImage;
					_softPopupHelper.ShowInformationUI("Received [<color=#" + ColorHelper.GetColorHexcodeForRarity(bagReward.ItemRarity) + ">" + bagReward.Name + "</color>]", sprite, description, Enums.SoftPopupType.FirstKill);
					yield return new WaitForSeconds(0.5f);
					player.ShowItemEffect(sprite, playAudio: true);
					yield return new WaitForSeconds(0.5f);
					SingletonController<AudioController>.Instance.PlaySFXClip(_audioClip, 1f);
					SingletonController<BackpackController>.Instance.OpenUI();
					SingletonController<BackpackController>.Instance.AddBagToStorage(bagReward);
					SingletonController<BackpackController>.Instance.CloseUI();
					if (itemsShouldBeAddedToFutureAdventures)
					{
						SingletonController<UnlockedEquipmentController>.Instance.AddWeaponReward(reward);
					}
					yield return new WaitForSeconds(3f);
				}
			}
			SingletonController<SaveGameController>.Instance.SaveProgression();
			SingletonController<GameController>.Instance.IsShowingOneTimeRewards = false;
			yield return new WaitForSeconds(0.5f);
		}
		finally
		{
			if (SingletonController<GameController>.Instance.IsShowingOneTimeRewards)
			{
				Debug.LogWarning("GameController.Instance.IsShowingOneTimeRewards was NOT set in FirstTimeDropHelper.HandleDropsAsync");
			}
			SingletonController<GameController>.Instance.IsShowingOneTimeRewards = false;
		}
	}
}
