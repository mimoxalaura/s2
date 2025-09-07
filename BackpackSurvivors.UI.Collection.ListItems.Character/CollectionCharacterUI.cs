using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection.ListItems.Character;

public class CollectionCharacterUI : CollectionListItemUI
{
	public delegate void OnClickHandler(object sender, CharacterCollectionSelectedEventArgs e);

	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private GameObject _lockedOverlay;

	[SerializeField]
	private TooltipTrigger _tooltip;

	private CharacterSO _character;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(CharacterSO character, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		_character = character;
		_image.sprite = character.Icon;
		_text.SetText(character.Name);
		_lockedOverlay.SetActive(!unlocked);
		_tooltip.SetContent(character.Name, character.Description);
		_tooltip.ToggleEnabled(unlocked);
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new CharacterCollectionSelectedEventArgs(_character));
		}
	}
}
