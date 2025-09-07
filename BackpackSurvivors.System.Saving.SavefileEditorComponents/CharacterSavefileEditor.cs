using BackpackSurvivors.System.Saving.SavefileEditorComponents.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.System.Saving.SavefileEditorComponents;

internal class CharacterSavefileEditor : MonoBehaviour
{
	public delegate void ActiveCharacterChangedHandler(object sender, ActiveCharacterChangedEventArgs e);

	[SerializeField]
	private TMP_InputField _characterNameInputfield;

	[SerializeField]
	private TMP_InputField _characterIdInputfield;

	[SerializeField]
	private TMP_InputField _characterLevelsInputfield;

	[SerializeField]
	private TMP_InputField _characterExperienceInputfield;

	[SerializeField]
	private Toggle _characterActiveToggle;

	public ActiveCharacterChangedHandler OnActiveCharacterChanged;

	private bool _isActive;

	public int CharacterId => int.Parse(_characterIdInputfield.text);

	public int CharacterLevels => int.Parse(_characterLevelsInputfield.text);

	public int CharacterExperience => int.Parse(_characterExperienceInputfield.text);

	public bool IsActiveCharacter => _characterActiveToggle.isOn;

	private void Start()
	{
		_characterActiveToggle.onValueChanged.AddListener(delegate(bool isActive)
		{
			SetActive(isActive, triggerEvent: true);
		});
	}

	internal void LoadCharacter(string characterName, int characterId, int characterLevels, float characterExperience, int activeCharacterId)
	{
		_characterNameInputfield.text = characterName;
		_characterIdInputfield.text = characterId.ToString();
		_characterLevelsInputfield.text = characterLevels.ToString();
		_characterExperienceInputfield.text = characterExperience.ToString();
		bool flag = CharacterId == activeCharacterId;
		_characterActiveToggle.isOn = flag;
		_isActive = flag;
	}

	public void SetActive(bool setActive, bool triggerEvent = false)
	{
		_characterActiveToggle.isOn = setActive;
		if (triggerEvent && !_isActive && setActive)
		{
			OnActiveCharacterChanged?.Invoke(this, new ActiveCharacterChangedEventArgs(CharacterId));
		}
		_isActive = setActive;
	}
}
