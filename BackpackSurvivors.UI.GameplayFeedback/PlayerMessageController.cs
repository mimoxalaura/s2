using System.Collections;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class PlayerMessageController : MonoBehaviour
{
	[Header("Audio")]
	[SerializeField]
	private AudioClip _defaultAudio;

	[SerializeField]
	private AudioClip _goodAudio;

	[SerializeField]
	private AudioClip _badAudio;

	[Header("Assets")]
	[SerializeField]
	private Image _iconImage;

	[SerializeField]
	private Image _glow;

	[SerializeField]
	private Image _topBorder;

	[SerializeField]
	private Image _bottomBorder;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private Image _shine1;

	[SerializeField]
	private Image _shine2;

	[Header("Animator")]
	[SerializeField]
	private Animator _animator;

	[Header("Materials")]
	[SerializeField]
	private Material _goodMaterial;

	[SerializeField]
	private Material _badMaterial;

	[SerializeField]
	private Material _defaultMaterial;

	[SerializeField]
	private Material _goodShineMaterial;

	[SerializeField]
	private Material _badShineMaterial;

	[SerializeField]
	private Material _defaultShineMaterial;

	[Command("player.message", Platform.AllPlatforms, MonoTargetType.Single)]
	public void DEBUGSHOWMESSAGE(Enums.PlayerMessageType playerMessageType)
	{
		ShowMessage("LEVEL BOSS SPAWNED", SpriteHelper.GetCurrencySprite(Enums.CurrencyType.TitanSouls), null, playerMessageType, 4f, envelop: true);
	}

	public void ShowMessage(string message, Sprite icon = null, AudioClip audioClip = null, Enums.PlayerMessageType playerMessageType = Enums.PlayerMessageType.Default, float showDuration = 4f, bool envelop = false)
	{
		if (envelop)
		{
			Object.FindObjectOfType<EnvelopController>().ShowEnvelop(showDuration + 1f);
		}
		_iconImage.sprite = icon;
		_text.SetText(message);
		switch (playerMessageType)
		{
		case Enums.PlayerMessageType.Default:
			_glow.gameObject.SetActive(value: false);
			if (audioClip != null)
			{
				StartCoroutine(PlayAfterDelay(audioClip));
			}
			if (audioClip == null)
			{
				StartCoroutine(PlayAfterDelay(_defaultAudio));
			}
			_topBorder.material = _defaultMaterial;
			_bottomBorder.material = _defaultMaterial;
			_shine1.material = _defaultShineMaterial;
			_shine2.material = _defaultShineMaterial;
			break;
		case Enums.PlayerMessageType.Good:
			_glow.gameObject.SetActive(value: true);
			if (audioClip != null)
			{
				StartCoroutine(PlayAfterDelay(audioClip));
			}
			if (audioClip == null)
			{
				StartCoroutine(PlayAfterDelay(_goodAudio));
			}
			_topBorder.material = _goodMaterial;
			_bottomBorder.material = _goodMaterial;
			_glow.material = _goodMaterial;
			_shine1.material = _goodShineMaterial;
			_shine2.material = _goodShineMaterial;
			break;
		case Enums.PlayerMessageType.Bad:
			_glow.gameObject.SetActive(value: true);
			if (audioClip != null)
			{
				StartCoroutine(PlayAfterDelay(audioClip));
			}
			if (audioClip == null)
			{
				StartCoroutine(PlayAfterDelay(_badAudio));
			}
			_topBorder.material = _badMaterial;
			_bottomBorder.material = _badMaterial;
			_glow.material = _badMaterial;
			_shine1.material = _badShineMaterial;
			_shine2.material = _badShineMaterial;
			break;
		}
		_animator.SetTrigger("Show");
		StartCoroutine(HideAfterDelay(showDuration));
	}

	private IEnumerator PlayAfterDelay(AudioClip audioClip)
	{
		yield return new WaitForSecondsRealtime(0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(audioClip, 1f);
	}

	private IEnumerator HideAfterDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		_animator.SetTrigger("Hide");
	}
}
