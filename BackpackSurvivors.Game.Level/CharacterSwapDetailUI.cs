using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwapDetailUI : ModalUI
{
	[SerializeField]
	private TextMeshProUGUI _characterTitleText;

	public void Init(CharacterSO characterSO)
	{
		_characterTitleText.SetText(characterSO.Name);
	}
}
