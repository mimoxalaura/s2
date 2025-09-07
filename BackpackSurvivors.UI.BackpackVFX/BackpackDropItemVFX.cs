using System.Collections;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.BackpackVFX;

internal class BackpackDropItemVFX : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private float _delay = 1f;

	[SerializeField]
	private Image _image;

	private void Start()
	{
		StartCoroutine(DestroyAfterLongDelay());
	}

	internal void Init(BaseItemSO item, Image image, float animationDelayMod = 0.7f, bool fromMerge = false)
	{
		Material vfxRarityMaterial = MaterialHelper.GetVfxRarityMaterial(item.ItemRarity);
		if (fromMerge)
		{
			vfxRarityMaterial = MaterialHelper.GetVfxRarityMaterial(Enums.PlaceableRarity.Epic);
		}
		_image.material = vfxRarityMaterial;
		image.material = vfxRarityMaterial;
		Vector2 itemSizeWithoutStars = item.ItemSize.GetItemSizeWithoutStars();
		_image.transform.localScale = new Vector3(itemSizeWithoutStars.x, itemSizeWithoutStars.y, 1f);
		_animator.SetTrigger("Drop");
		LeanTween.value(base.gameObject, 1f, 0f, _delay * animationDelayMod).setIgnoreTimeScale(useUnScaledTime: true).setOnUpdate(delegate(float fade)
		{
			image.color = new Color(255f, 255f, 255f, fade);
		});
		StartCoroutine(DestroyAfterDelay());
	}

	private IEnumerator DestroyAfterDelay()
	{
		yield return new WaitForSecondsRealtime(_delay * 2f);
		Object.Destroy(base.gameObject);
	}

	private IEnumerator DestroyAfterLongDelay()
	{
		yield return new WaitForSecondsRealtime(5f);
		Object.Destroy(base.gameObject);
	}
}
