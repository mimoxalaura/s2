using System;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwapping : MonoBehaviour
{
	[SerializeField]
	private CharacterSwapOption[] _characterSwapOptions;

	[SerializeField]
	private CharacterSwapDetailUI _characterSwapDetailUI;

	private void Start()
	{
		CharacterSwapOption[] characterSwapOptions = _characterSwapOptions;
		foreach (CharacterSwapOption characterSwapOption in characterSwapOptions)
		{
			characterSwapOption.SwapOptionInteraction.OnCharacterSwapOptionInteractionEntered += SwapOptionInteraction_OnCharacterSwapOptionInteractionEntered;
			characterSwapOption.SwapOptionInteraction.OnCharacterSwapOptionInteractionExited += SwapOptionInteraction_OnCharacterSwapOptionInteractionExited;
			if (SingletonController<GameController>.Instance.Player.BaseCharacter == characterSwapOption.SwapOptionInteraction.CharacterSO)
			{
				characterSwapOption.SwapOptionInteraction.ToggleVisibility(visible: true, Enums.Unlockable.ExtraCharacter0);
			}
		}
	}

	private void SwapOptionInteraction_OnCharacterSwapOptionInteractionExited(object sender, EventArgs e)
	{
		_characterSwapDetailUI.CloseUI();
	}

	private void SwapOptionInteraction_OnCharacterSwapOptionInteractionEntered(object sender, CharacterSwappedZoneEnteredEventArgs e)
	{
		if (!(e.CharacterSO == null))
		{
			_characterSwapDetailUI.Init(e.CharacterSO);
			_characterSwapDetailUI.OpenUI();
		}
	}

	private void OnDestroy()
	{
		CharacterSwapOption[] characterSwapOptions = _characterSwapOptions;
		foreach (CharacterSwapOption obj in characterSwapOptions)
		{
			obj.SwapOptionInteraction.OnCharacterSwapOptionInteractionEntered -= SwapOptionInteraction_OnCharacterSwapOptionInteractionEntered;
			obj.SwapOptionInteraction.OnCharacterSwapOptionInteractionExited -= SwapOptionInteraction_OnCharacterSwapOptionInteractionExited;
		}
	}

	public void SwapToCharacter(CharacterSO character)
	{
		_characterSwapOptions[character.Id - 1].SwapOptionInteraction.DoInteract();
		_characterSwapOptions[character.Id - 1].SwapOptionInteraction.ToggleVisibility(visible: false, character.Unlockable);
	}
}
