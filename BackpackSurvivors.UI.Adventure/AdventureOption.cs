using BackpackSurvivors.ScriptableObjects.Adventures;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureOption : MonoBehaviour
{
	public delegate void AdventureOptionSelectedHandler(object sender, AdventureOptionSelectedeventArgs e);

	[SerializeField]
	private GameObject _highlightedObject;

	[SerializeField]
	private GameObject _selecterObject;

	[SerializeField]
	private GameObject _lockObject;

	[SerializeField]
	private TextMeshProUGUI _adventureText;

	[SerializeField]
	private Color _adventureAvailableFontColor;

	[SerializeField]
	private Color _adventureUnavailableFontColor;

	[SerializeField]
	private Color _adventureSelectedFontColor;

	private AdventureSO _adventure;

	internal AdventureSO Adventure => _adventure;

	public event AdventureOptionSelectedHandler OnAdventureOptionSelected;

	public void Init(AdventureSO adventure)
	{
		_adventure = adventure;
		_lockObject.SetActive(!adventure.Available);
		_adventureText.SetText(adventure.AdventureName);
		_adventureText.color = (adventure.Available ? _adventureAvailableFontColor : _adventureUnavailableFontColor);
		base.gameObject.name = _adventure.AdventureName;
	}

	public void SetSelect(bool selected)
	{
		_highlightedObject.SetActive(selected);
		_selecterObject.SetActive(selected);
		if (_adventure.Available)
		{
			_adventureText.color = (selected ? _adventureSelectedFontColor : _adventureAvailableFontColor);
		}
		else
		{
			_adventureText.color = _adventureUnavailableFontColor;
		}
	}

	public void SelectThisAdventure()
	{
		if (_adventure.Available)
		{
			this.OnAdventureOptionSelected?.Invoke(this, new AdventureOptionSelectedeventArgs(_adventure));
		}
	}
}
