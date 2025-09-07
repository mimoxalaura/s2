using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Relic;

public class RelicRewardUI : MonoBehaviour
{
	public delegate void RelicRewardPickedHandler(object sender, RelicRewardPickedEventArgs e);

	[SerializeField]
	private GameObject _detailContainerObject;

	[SerializeField]
	private CollectionWeaponUI _collectionWeaponUIPrefab;

	[SerializeField]
	private Transform _affectedWeaponContainer;

	[SerializeField]
	private TextMeshProUGUI _affectedWeaponTitle;

	[SerializeField]
	private GameObject[] SelectedBorders;

	[SerializeField]
	private TextMeshProUGUI _titleText;

	[SerializeField]
	private TextMeshProUGUI _descriptionText;

	[SerializeField]
	private TextMeshProUGUI _rarityText;

	[SerializeField]
	private Image _backgroundImage;

	[SerializeField]
	private Image _iconBackgroundImage;

	[SerializeField]
	private Image _iconImage;

	[SerializeField]
	private Image _selectedBackgroundImage;

	[SerializeField]
	private Animator[] _selectedBorderAnimators;

	[SerializeField]
	private AudioClip _relicHoverAudio;

	[SerializeField]
	private Animator _pickedRelicAnimator;

	public Relic Relic;

	private float _animationTime = 0.3f;

	public event RelicRewardPickedHandler OnRelicRewardPicked;

	public void Init(Relic relic)
	{
		Relic = relic;
		_titleText.SetText(relic.RelicSO.Name.ToUpper());
		string originalString = TextMeshProStringHelper.HighlightItemStatKeywords(relic.RelicSO.Description, Constants.Colors.HexStrings.DefaultKeywordColor);
		originalString = TextMeshProStringHelper.HighlightTags(originalString);
		_descriptionText.SetText(originalString);
		_rarityText.SetText($"<color=#{ColorHelper.GetColorHexcodeForRarity(relic.RelicSO.Rarity)}>{relic.RelicSO.Rarity}</color>");
		_backgroundImage.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(relic.RelicSO.Rarity);
		_iconBackgroundImage.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(relic.RelicSO.Rarity);
		_iconImage.sprite = relic.RelicSO.Icon;
		List<WeaponInstance> list = new List<WeaponInstance>();
		if (relic.RelicSO.Conditions.Length != 0)
		{
			list = StatCalculator.GetWeaponsThatFitFilters(relic.RelicSO.Conditions, SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack());
		}
		foreach (Transform item in _affectedWeaponContainer)
		{
			Object.Destroy(item.gameObject);
		}
		_affectedWeaponTitle.gameObject.SetActive(list.Any());
		float cellSizeByWeaponCount = GetCellSizeByWeaponCount(list.Count);
		_affectedWeaponContainer.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSizeByWeaponCount, cellSizeByWeaponCount);
		foreach (WeaponInstance item2 in list)
		{
			Object.Instantiate(_collectionWeaponUIPrefab, _affectedWeaponContainer).Init(item2.BaseWeaponSO, unlocked: true, interactable: false);
		}
		LeanTween.scaleX(_detailContainerObject, 1f, 0.2f);
	}

	private float GetCellSizeByWeaponCount(int weaponCount)
	{
		if (weaponCount > 35)
		{
			return 28f;
		}
		if (weaponCount > 15)
		{
			return 32f;
		}
		if (weaponCount > 9)
		{
			return 48f;
		}
		if (weaponCount > 6)
		{
			return 64f;
		}
		return 72f;
	}

	public void Hovering()
	{
		ToggleSelect(selectedSelf: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_relicHoverAudio, 1f);
	}

	public void ExitHovering()
	{
		ToggleSelect(selectedSelf: false);
	}

	public void ToggleSelect(bool selectedSelf)
	{
		foreach (RelicRewardUI item in Object.FindObjectsByType<RelicRewardUI>(FindObjectsSortMode.None).ToList())
		{
			if (item == this)
			{
				if (selectedSelf)
				{
					item.ShowSelectBorder();
					item.ShowSelectedBackground();
				}
				else
				{
					item.HideSelectBorder();
					item.HideSelectedBackground();
				}
			}
			else
			{
				item.HideSelectBorder();
				item.HideSelectedBackground();
			}
		}
	}

	private void ShowSelectedBackground()
	{
		_selectedBackgroundImage.enabled = true;
	}

	private void HideSelectedBackground()
	{
		_selectedBackgroundImage.enabled = false;
	}

	public void ShowSelectBorder()
	{
		Animator[] selectedBorderAnimators = _selectedBorderAnimators;
		foreach (Animator obj in selectedBorderAnimators)
		{
			obj.enabled = true;
			obj.SetBool("Selected", value: true);
		}
	}

	public void HideSelectBorder()
	{
		Animator[] selectedBorderAnimators = _selectedBorderAnimators;
		foreach (Animator obj in selectedBorderAnimators)
		{
			obj.enabled = true;
			obj.SetBool("Selected", value: false);
		}
	}

	public void PickRelic()
	{
		if (!SingletonController<RelicsController>.Instance.RelicPickingInProgress)
		{
			_pickedRelicAnimator.SetTrigger("Picked");
			this.OnRelicRewardPicked?.Invoke(this, new RelicRewardPickedEventArgs(Relic));
		}
	}

	internal void Despawn()
	{
		StartCoroutine(DespawnAsync());
	}

	internal void Spawn()
	{
		StartCoroutine(SpawnAsync());
	}

	private IEnumerator DespawnAsync()
	{
		LeanTween.scaleX(base.gameObject, 0f, _animationTime).setIgnoreTimeScale(useUnScaledTime: true);
		yield return new WaitForSecondsRealtime(_animationTime);
		Object.Destroy(base.gameObject);
	}

	private IEnumerator SpawnAsync()
	{
		LeanTween.scaleX(base.gameObject, 1f, _animationTime).setIgnoreTimeScale(useUnScaledTime: true);
		yield return new WaitForSecondsRealtime(_animationTime);
	}
}
