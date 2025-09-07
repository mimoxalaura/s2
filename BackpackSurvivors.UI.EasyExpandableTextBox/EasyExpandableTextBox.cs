using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.EasyExpandableTextBox;

public class EasyExpandableTextBox : MonoBehaviour
{
	public TextMeshProUGUI textTMP;

	public Sprite boxSprite;

	public TMP_FontAsset font;

	[TextArea]
	public string text;

	public int fontSize = 72;

	public bool autoSize;

	public Color color = Color.black;

	public TextAlignmentOptions alignment;

	public int autoSizeMin = 12;

	public int autoSizeMax = 72;

	[SerializeField]
	public FontStyles style;

	public bool settings;

	public int left;

	public int right;

	public int top;

	public int bottom;

	public TextAnchor boxAlignment;

	public AudioSource audioSource;

	[SerializeField]
	public List<AudioClip> typingSounds;

	private static bool mouseButtonPressed;

	private static readonly WaitUntil waitUntil = new WaitUntil(() => mouseButtonPressed);

	private readonly Dictionary<float, WaitForSeconds> waitPool = new Dictionary<float, WaitForSeconds>();

	public void Awake()
	{
		textTMP = GetComponentInChildren<TextMeshProUGUI>();
		audioSource = GetComponent<AudioSource>();
	}

	public void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			mouseButtonPressed = true;
		}
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

	public IEnumerator EasyMessage(string message, float timeBetweenCharacters = 0.125f, bool canSkipText = true, bool waitForButtonClick = true, float timeToWaitAfterTextIsDisplayed = 1f)
	{
		text = "";
		textTMP.text = text;
		message += " ";
		if (timeBetweenCharacters != 0f)
		{
			for (int i = 0; i < message.Length - 1; i++)
			{
				if (message[i] != '<' && message[i + 1] != '#')
				{
					text += message[i];
					textTMP.text = text;
					if (mouseButtonPressed && canSkipText)
					{
						mouseButtonPressed = false;
						text = message;
						textTMP.text = text;
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
						text += message[j];
					}
					i = message.IndexOf('>', i);
				}
			}
		}
		else
		{
			text = message;
			textTMP.text = text;
		}
		if (waitForButtonClick)
		{
			yield return waitUntil;
		}
		else
		{
			yield return GetPoolWait(timeToWaitAfterTextIsDisplayed);
		}
		mouseButtonPressed = false;
	}
}
