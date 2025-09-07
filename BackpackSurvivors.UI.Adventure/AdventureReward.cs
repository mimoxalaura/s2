using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(MainButton))]
public class AdventureReward : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _text;

	public Image Image => _image;

	internal void SetImage(Sprite image)
	{
		_image.sprite = image;
	}

	internal void SetText(string text)
	{
		_text.SetText(text);
	}

	public void Init(bool interactable)
	{
		GetComponent<MainButton>().ToggleHoverEffects(interactable);
		GetComponent<Button>().enabled = interactable;
	}
}
