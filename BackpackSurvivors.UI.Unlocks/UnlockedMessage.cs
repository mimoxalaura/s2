using System.Collections;
using BackpackSurvivors.System;
using QFSW.QC;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Unlocks;

public class UnlockedMessage : MonoBehaviour
{
	[SerializeField]
	private Image _warningImage;

	[SerializeField]
	private Image _backdropImage;

	[SerializeField]
	private Image _bottomBarImage;

	[SerializeField]
	private Image _bottomBarLight;

	[SerializeField]
	private Image _raycastBlock;

	[SerializeField]
	private AudioClip _warningAudioClip;

	[SerializeField]
	private AudioClip _noVisualAudioClip;

	[Command("unlockable.showMessage", Platform.AllPlatforms, MonoTargetType.Single)]
	public void Show(Enums.Unlockable unlockable)
	{
		float num = 1f;
		float num2 = 3f;
		float time = 2f;
		float num3 = 1f;
		float time2 = 1.5f;
		float num4 = 200f;
		float to = 105f;
		float to2 = 752f;
		float num5 = 50f;
		float num6 = 0.5f;
		_raycastBlock.gameObject.SetActive(value: true);
		_warningImage.material.SetFloat("_FadeAmount", num6);
		_warningImage.material.SetFloat("_Glow", num5);
		_backdropImage.color = new Color(_backdropImage.color.r, _backdropImage.color.g, _backdropImage.color.b, 0f);
		_bottomBarImage.transform.localPosition = new Vector3(_bottomBarImage.transform.localPosition.x, num4);
		_bottomBarLight.transform.localPosition = new Vector3(-725f, _bottomBarLight.transform.localPosition.y);
		SingletonController<AudioController>.Instance.PlaySFXClip(_warningAudioClip, 1f);
		LeanTween.value(_backdropImage.gameObject, UpdateBackdropFade, 0f, 1f, num);
		LeanTween.value(_bottomBarImage.gameObject, UpdateBottomBarFade, 0f, 1f, num2).setDelay(num);
		LeanTween.moveLocalY(_bottomBarImage.gameObject, to, num).setDelay(num);
		LeanTween.moveLocalX(_bottomBarLight.gameObject, to2, time2).setDelay(num2 + num);
		LeanTween.value(_warningImage.gameObject, UpdateFadeAmount, num6, -0.1f, num2).setDelay(num);
		LeanTween.value(_warningImage.gameObject, UpdateGlow, num5, 0f, num2).setDelay(num);
		float num7 = 2f + num2;
		LeanTween.value(_warningImage.gameObject, UpdateFadeAmount, -0.1f, 1f, time).setDelay(num7);
		LeanTween.value(_warningImage.gameObject, UpdateGlow, 0f, 100f, time).setDelay(num7);
		LeanTween.value(_backdropImage.gameObject, UpdateBackdropFade, 1f, 0f, num3).setDelay(num7 + num3);
		LeanTween.value(_bottomBarImage.gameObject, UpdateBottomBarFade, 1f, 0f, num3 / 2f).setDelay(num7 + num3);
		LeanTween.moveLocalY(_bottomBarImage.gameObject, num4, num3).setDelay(num7 + num3);
		StartCoroutine(DisableRaycastBlocker(num7 + num3));
	}

	public void ShowWithoutMessage()
	{
		SingletonController<AudioController>.Instance.PlaySFXClip(_noVisualAudioClip, 1f);
	}

	private IEnumerator DisableRaycastBlocker(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		_raycastBlock.gameObject.SetActive(value: false);
	}

	private void UpdateFadeAmount(float val)
	{
		_warningImage.material.SetFloat("_FadeAmount", val);
	}

	private void UpdateGlow(float val)
	{
		_warningImage.material.SetFloat("_Glow", val);
	}

	private void UpdateBackdropFade(float val)
	{
		_backdropImage.color = new Color(_backdropImage.color.r, _backdropImage.color.g, _backdropImage.color.b, val);
	}

	private void UpdateBottomBarFade(float val)
	{
		_bottomBarImage.color = new Color(_backdropImage.color.r, _backdropImage.color.g, _backdropImage.color.b, val);
	}
}
