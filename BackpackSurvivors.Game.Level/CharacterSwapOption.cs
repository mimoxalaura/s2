using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwapOption : UnlockableInTown
{
	[SerializeField]
	private CharacterSwapOptionInteraction _characterSwapOptionInteraction;

	public CharacterSwapOptionInteraction SwapOptionInteraction => _characterSwapOptionInteraction;

	private void Start()
	{
		RegisterEvents();
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, RegisterCharacterSwitched);
	}

	public void RegisterCharacterSwitched()
	{
		SingletonController<CharactersController>.Instance.OnCharacterSwitched += CharactersController_OnCharacterSwitched;
	}

	private void CharactersController_OnCharacterSwitched(object sender, CharacterSwitchedEventArgs e)
	{
		_characterSwapOptionInteraction.ToggleVisibility(e.NewCharacter != _characterSwapOptionInteraction.CharacterSO, Unlockable);
	}

	private void OnDestroy()
	{
		SingletonController<CharactersController>.Instance.OnCharacterSwitched -= CharactersController_OnCharacterSwitched;
	}
}
