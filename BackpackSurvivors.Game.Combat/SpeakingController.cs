using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Steam;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Combat;

public class SpeakingController : MonoBehaviour
{
	[SerializeField]
	private Canvas _canvas;

	[SerializeField]
	private TextMeshProUGUI _textTMP;

	[SerializeField]
	private TextMeshProUGUI _nameTMP;

	[SerializeField]
	private Image _leftCharacter;

	[SerializeField]
	private Image _rightCharacter;

	[SerializeField]
	private AudioClip _speakLetterAudio;

	private string _text;

	private static bool _mouseButtonPressed;

	private static readonly WaitUntil waitUntil = new WaitUntil(() => _mouseButtonPressed);

	private readonly Dictionary<float, WaitForSeconds> waitPool = new Dictionary<float, WaitForSeconds>();

	internal void ToggleSpeakCanvas(bool active)
	{
		_canvas.gameObject.SetActive(active);
	}

	internal void SetupConversationCharacters(Character leftCharacter, Character rightCharacter)
	{
		if (leftCharacter != null)
		{
			_nameTMP.SetText($"{GetPlayerName()}");
			_leftCharacter.gameObject.SetActive(value: true);
			if (leftCharacter.GetCharacterType() == Enums.Enemies.EnemyType.Player)
			{
				_leftCharacter.sprite = ((BackpackSurvivors.Game.Player.Player)leftCharacter).BaseCharacter.SpeakingImage;
			}
		}
		else
		{
			_leftCharacter.gameObject.SetActive(value: false);
		}
		if (rightCharacter != null)
		{
			_rightCharacter.gameObject.SetActive(value: true);
			if (rightCharacter.GetCharacterType() == Enums.Enemies.EnemyType.Player)
			{
				_rightCharacter.sprite = ((BackpackSurvivors.Game.Player.Player)leftCharacter).BaseCharacter.SpeakingImage;
				_rightCharacter.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
		}
		else
		{
			_rightCharacter.gameObject.SetActive(value: false);
		}
	}

	internal float Speak(string textToSay)
	{
		float num = 0.05f;
		float result = (float)textToSay.Length * num + 1f;
		StartCoroutine(SpeakMessage(textToSay, num, canSkipText: true, waitForButtonClick: false));
		return result;
	}

	private object GetPlayerName()
	{
		return SingletonController<SteamController>.Instance.GetSteamName().ToUpper();
	}

	public IEnumerator SpeakMessage(string message, float timeBetweenCharacters = 0.125f, bool canSkipText = true, bool waitForButtonClick = true, float timeToWaitAfterTextIsDisplayed = 1f)
	{
		_text = "";
		_textTMP.text = _text;
		message += " ";
		if (timeBetweenCharacters != 0f)
		{
			for (int i = 0; i < message.Length - 1; i++)
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_speakLetterAudio, 1f);
				if (message[i] != '<' && message[i + 1] != '#')
				{
					_text += message[i];
					_textTMP.text = _text;
					if (_mouseButtonPressed && canSkipText)
					{
						_mouseButtonPressed = false;
						_text = message;
						_textTMP.text = _text;
						break;
					}
					if (message[i] != ' ')
					{
						yield return GetPoolWait(timeBetweenCharacters);
					}
				}
				else
				{
					for (int j = i; j <= message.IndexOf('>', i); j++)
					{
						_text += message[j];
					}
					i = message.IndexOf('>', i);
				}
			}
		}
		else
		{
			_text = message;
			_textTMP.text = _text;
		}
		if (waitForButtonClick)
		{
			yield return waitUntil;
		}
		else
		{
			yield return GetPoolWait(timeToWaitAfterTextIsDisplayed);
		}
		_mouseButtonPressed = false;
	}

	private WaitForSeconds GetPoolWait(float waitTime)
	{
		if (waitPool.TryGetValue(waitTime, out var value))
		{
			return value;
		}
		value = new WaitForSeconds(waitTime);
		waitPool.Add(waitTime, value);
		return value;
	}
}
