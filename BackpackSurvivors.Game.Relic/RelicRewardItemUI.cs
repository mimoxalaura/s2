using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Relic;

public class RelicRewardItemUI : MonoBehaviour
{
	public delegate void RelicRewardPickedHandler(object sender, RelicRewardPickedEventArgs e);

	[SerializeField]
	private Image _backdrop;

	[SerializeField]
	private Image _overlayBorder;

	[SerializeField]
	private GameObject _iconContainer;

	[SerializeField]
	private Image _iconImage;

	[SerializeField]
	private Image _iconImageActual;

	[SerializeField]
	private Image _iconBackdropImage;

	[SerializeField]
	private GameObject _iconActualContainer;

	[SerializeField]
	private AudioClip _audioClipOnHit;

	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private TextMeshProUGUI _rarityText;

	[SerializeField]
	private Image _rarityImage;

	[SerializeField]
	private Image _rarityBorderImage;

	[SerializeField]
	private Image _rarityBorderActualImage;

	[Header("Hovering")]
	[SerializeField]
	private AudioClip _audioOnHover;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Image _iconBorder;

	[Header("Weapons")]
	[SerializeField]
	private Transform _affectedWeaponContainer;

	[SerializeField]
	private CollectionWeaponUI _collectionWeaponUIPrefab;

	private Relic _relic;

	private bool _canHighlight;

	private float _animationTime = 0.3f;

	private float _hoverAnimationTime = 0.2f;

	public event RelicRewardPickedHandler OnRelicRewardPicked;

	public void Init(Relic relic)
	{
		_relic = relic;
		_iconImage.sprite = relic.RelicSO.Icon;
		_iconImageActual.sprite = relic.RelicSO.Icon;
		_title.SetText(relic.RelicSO.Name);
		_rarityText.SetText($"<color={ColorHelper.GetColorHexcodeForRarity(relic.RelicSO.Rarity)}>{relic.RelicSO.Rarity}</color>");
		_rarityBorderImage.color = ColorHelper.GetColorForRarity(relic.RelicSO.Rarity);
		_rarityBorderActualImage.color = _rarityBorderImage.color;
		_rarityBorderImage.material = MaterialHelper.GetShopOfferBorderRarityMaterial(relic.RelicSO.Rarity);
		_rarityBorderActualImage.material = MaterialHelper.GetShopOfferBorderRarityMaterial(relic.RelicSO.Rarity);
		string originalString = TextMeshProStringHelper.HighlightItemStatKeywords(relic.RelicSO.Description, Constants.Colors.HexStrings.DefaultKeywordColor);
		originalString = TextMeshProStringHelper.HighlightTags(originalString);
		_description.SetText(originalString);
		List<WeaponInstance> list = new List<WeaponInstance>();
		if (relic.RelicSO.Conditions.Length != 0)
		{
			list = StatCalculator.GetWeaponsThatFitFilters(relic.RelicSO.Conditions, SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack());
		}
		foreach (Transform item in _affectedWeaponContainer)
		{
			Object.Destroy(item.gameObject);
		}
		float cellSizeByWeaponCount = GetCellSizeByWeaponCount(list.Count);
		_affectedWeaponContainer.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSizeByWeaponCount, cellSizeByWeaponCount);
		foreach (WeaponInstance item2 in list)
		{
			Object.Instantiate(_collectionWeaponUIPrefab, _affectedWeaponContainer).Init(item2.BaseWeaponSO, unlocked: true, interactable: false);
		}
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

	public void PickRelic()
	{
		if (!SingletonController<RelicsController>.Instance.RelicPickingInProgress)
		{
			_animator.SetTrigger("Picked");
			StartCoroutine(DestroyAfterDelay(0.5f));
			this.OnRelicRewardPicked?.Invoke(this, new RelicRewardPickedEventArgs(_relic));
		}
	}

	internal void Spawn(Transform targetPositionTransform, Transform targetIconPositionTransform, bool isReroll)
	{
		StartCoroutine(SpawnAsync(targetPositionTransform, targetIconPositionTransform, isReroll));
	}

	private IEnumerator SpawnAsync(Transform targetPositionTransform, Transform targetIconPositionTransform, bool isReroll)
	{
		float num = 0.5f;
		float relicUIPushbackTime = 0.1f;
		float time = 1f;
		_iconContainer.transform.localScale = new Vector3(10f, 10f, 10f);
		_backdrop.color = new Color(255f, 255f, 255f, 0f);
		_iconImage.color = new Color(255f, 255f, 255f, 0f);
		_iconBackdropImage.color = new Color(255f, 255f, 255f, 0f);
		_affectedWeaponContainer.gameObject.SetActive(value: false);
		base.transform.position = targetPositionTransform.position;
		_iconContainer.transform.position = targetIconPositionTransform.position;
		LeanTween.value(_backdrop.gameObject, delegate(float val)
		{
			_backdrop.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_title.gameObject, delegate(float val)
		{
			_title.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_description.gameObject, delegate(float val)
		{
			_description.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_rarityText.gameObject, delegate(float val)
		{
			_rarityText.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_rarityImage.gameObject, delegate(float val)
		{
			_rarityImage.color = new Color(255f, 255f, 255f, val);
		}, 0f, 0.29f, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_iconContainer.gameObject, new Vector3(0.8f, 0.8f, 0.8f), num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_iconImage.gameObject, delegate(float val)
		{
			_iconImage.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, num / 2f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_iconBackdropImage.gameObject, delegate(float val)
		{
			_iconBackdropImage.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, num / 2f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_rarityBorderImage.gameObject, delegate(float val)
		{
			_rarityBorderImage.color = new Color(_rarityBorderImage.color.r, _rarityBorderImage.color.g, _rarityBorderImage.color.b, val);
		}, 0f, 1f, num / 2f).setIgnoreTimeScale(useUnScaledTime: true);
		yield return new WaitForSecondsRealtime(num);
		_iconContainer.SetActive(value: false);
		_iconActualContainer.SetActive(value: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_audioClipOnHit, 1f);
		_affectedWeaponContainer.gameObject.SetActive(value: true);
		LeanTween.scale(base.gameObject, new Vector3(0.8f, 0.8f, 0.8f), relicUIPushbackTime).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(base.gameObject, new Vector3(1f, 1f, 1f), relicUIPushbackTime).setDelay(relicUIPushbackTime).setIgnoreTimeScale(useUnScaledTime: true);
		_canHighlight = true;
		yield return new WaitForSecondsRealtime(_animationTime);
	}

	public void OnHover()
	{
		if (_canHighlight)
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(_audioOnHover, 1f);
			_animator.SetBool("Selected", value: true);
			LeanTween.value(_iconBorder.gameObject, delegate(float val)
			{
				_iconBorder.fillAmount = val;
			}, 0f, 1f, _hoverAnimationTime).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(base.gameObject, new Vector3(1.1f, 1.1f, 1.1f), _hoverAnimationTime).setIgnoreTimeScale(useUnScaledTime: true);
		}
	}

	public void OnExitHover()
	{
		if (_canHighlight)
		{
			_animator.SetBool("Selected", value: false);
			LeanTween.value(_iconBorder.gameObject, delegate(float val)
			{
				_iconBorder.fillAmount = val;
			}, 1f, 0f, _hoverAnimationTime).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(base.gameObject, new Vector3(1f, 1f, 1f), _hoverAnimationTime).setIgnoreTimeScale(useUnScaledTime: true);
		}
	}

	internal void Despawn(bool animateDespawn = false, float delay = 0f)
	{
		if (animateDespawn)
		{
			_animator.SetTrigger("Despawn");
		}
		StartCoroutine(DestroyAfterDelay(delay));
	}

	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		Object.Destroy(base.gameObject);
	}
}
