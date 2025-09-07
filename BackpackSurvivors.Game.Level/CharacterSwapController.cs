using System;
using System.Linq;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwapController : MonoBehaviour
{
	[SerializeField]
	private CharacterSwappingNew[] CharacterSwappingNew;

	[SerializeField]
	private CharacterSwapDetailUINew _characterSwapDetailUI;

	[SerializeField]
	private CharacterSwapOptionInteractionNew[] _characterSwapOptions;

	public void Start()
	{
		SetupCorrectActiveCharacter();
		SetupUnlockedCharacters();
		SingletonController<CharactersController>.Instance.OnCharacterSwitched += Instance_OnCharacterSwitched;
		SingletonController<UnlocksController>.Instance.OnUnlockableUnlocked += UnlocksController_OnUnlockableUnlocked;
		CharacterSwapOptionInteractionNew[] characterSwapOptions = _characterSwapOptions;
		foreach (CharacterSwapOptionInteractionNew obj in characterSwapOptions)
		{
			obj.OnCharacterSwapOptionInteractionEntered += CharacterSwapOption_OnCharacterSwapOptionInteractionEntered;
			obj.OnCharacterSwapOptionInteractionExited += CharacterSwapOption_OnCharacterSwapOptionInteractionExited;
		}
	}

	private void SetupUnlockedCharacters()
	{
		CharacterSwapOptionInteractionNew[] characterSwapOptions = _characterSwapOptions;
		foreach (CharacterSwapOptionInteractionNew characterSwapOptionInteractionNew in characterSwapOptions)
		{
			bool visible = SingletonController<UnlocksController>.Instance.IsUnlocked(characterSwapOptionInteractionNew.Unlockable);
			characterSwapOptionInteractionNew.ToggleVisibility(visible);
		}
	}

	private void CharacterSwapOption_OnCharacterSwapOptionInteractionExited(object sender, EventArgs e)
	{
		_characterSwapDetailUI.CloseUI(Enums.Modal.OpenDirection.Horizontal);
	}

	private void CharacterSwapOption_OnCharacterSwapOptionInteractionEntered(object sender, CharacterSwappedZoneEnteredEventArgsNew e)
	{
		_characterSwapDetailUI.Init((sender as CharacterSwapOptionInteractionNew).Character);
		_characterSwapDetailUI.OpenUI(Enums.Modal.OpenDirection.Horizontal);
	}

	private void SetupCorrectActiveCharacter()
	{
		CharacterSwappingNew[] characterSwappingNew = CharacterSwappingNew;
		foreach (CharacterSwappingNew obj in characterSwappingNew)
		{
			obj.SetActiveState(obj.CharacterSO == SingletonController<CharactersController>.Instance.ActiveCharacter);
		}
	}

	private void Instance_OnCharacterSwitched(object sender, CharacterSwitchedEventArgs e)
	{
		SetupCorrectActiveCharacter();
	}

	private void UnlocksController_OnUnlockableUnlocked(object sender, UnlockableUnlockedEventArgs e)
	{
		if (!e.FromLoad)
		{
			CharacterSwapOptionInteractionNew characterSwapOptionInteractionNew = _characterSwapOptions.FirstOrDefault((CharacterSwapOptionInteractionNew x) => x.Unlockable == e.UnlockedItem.Unlockable);
			if (!(characterSwapOptionInteractionNew == null))
			{
				characterSwapOptionInteractionNew.CharacterSwapOptionNew.UnlockAnimate(reopenUI: true, focusOnElement: true);
				characterSwapOptionInteractionNew.CharacterSwapOptionNew.Interaction.CanInteract = true;
			}
		}
	}

	private void OnDestroy()
	{
		SingletonController<CharactersController>.Instance.OnCharacterSwitched -= Instance_OnCharacterSwitched;
		SingletonController<UnlocksController>.Instance.OnUnlockableUnlocked -= UnlocksController_OnUnlockableUnlocked;
		CharacterSwapOptionInteractionNew[] characterSwapOptions = _characterSwapOptions;
		foreach (CharacterSwapOptionInteractionNew obj in characterSwapOptions)
		{
			obj.OnCharacterSwapOptionInteractionEntered -= CharacterSwapOption_OnCharacterSwapOptionInteractionEntered;
			obj.OnCharacterSwapOptionInteractionExited -= CharacterSwapOption_OnCharacterSwapOptionInteractionExited;
		}
	}
}
