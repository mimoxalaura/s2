using System;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.World;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwapOptionInteractionNew : Interaction
{
	public delegate void OnCharacterSwapOptionInteractionEnteredHandler(object sender, CharacterSwappedZoneEnteredEventArgsNew e);

	public delegate void OnCharacterSwapOptionInteractionExitedHandler(object sender, EventArgs e);

	[SerializeField]
	private DEBUG_FeatureToggleController.Feature _feature;

	[SerializeField]
	private CharacterSO _characterSO;

	[SerializeField]
	private SpriteRenderer[] _spriteRenderers;

	[SerializeField]
	private CharacterSwapOptionNew _characterSwapOptionNew;

	public Enums.Unlockable Unlockable => _characterSwapOptionNew.Unlockable;

	public CharacterSwapOptionNew CharacterSwapOptionNew => _characterSwapOptionNew;

	public CharacterSO Character => _characterSO;

	public event OnCharacterSwapOptionInteractionEnteredHandler OnCharacterSwapOptionInteractionEntered;

	public event OnCharacterSwapOptionInteractionExitedHandler OnCharacterSwapOptionInteractionExited;

	public void ToggleVisibility(bool visible)
	{
		_characterSwapOptionNew.Interaction.CanInteract = visible;
		SpriteRenderer[] spriteRenderers = _spriteRenderers;
		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			spriteRenderers[i].enabled = visible;
		}
		if (!visible)
		{
			this.OnCharacterSwapOptionInteractionExited?.Invoke(this, new EventArgs());
		}
	}

	public override void DoStart()
	{
		base.DoStart();
		base.OnInteractionZoneEntered += CharacterSwapOptionInteraction_OnInteractionZoneEntered;
		base.OnInteractionZoneExited += CharacterSwapOptionInteraction_OnInteractionZoneExited;
	}

	private void CharacterSwapOptionInteraction_OnInteractionZoneExited(object sender, EventArgs e)
	{
		this.OnCharacterSwapOptionInteractionExited?.Invoke(this, new EventArgs());
	}

	private void CharacterSwapOptionInteraction_OnInteractionZoneEntered(object sender, EventArgs e)
	{
		this.OnCharacterSwapOptionInteractionEntered?.Invoke(this, new CharacterSwappedZoneEnteredEventArgsNew(_characterSO));
	}

	public override void DoInteract()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(_feature))
		{
			PickCharacter();
		}
	}

	public void PickCharacter()
	{
		SingletonController<CharactersController>.Instance.SwitchCharacter(_characterSO);
	}
}
