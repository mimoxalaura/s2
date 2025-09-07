using System;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.Game.World;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwapOptionInteraction : Interaction
{
	public delegate void OnCharacterSwapOptionInteractionEnteredHandler(object sender, CharacterSwappedZoneEnteredEventArgs e);

	public delegate void OnCharacterSwapOptionInteractionExitedHandler(object sender, EventArgs e);

	[SerializeField]
	private CharacterSO _characterSO;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private SpriteRenderer _spriteRendererShadow;

	[SerializeField]
	private Collider2D _collider2D;

	[SerializeField]
	private DEBUG_FeatureToggleController.Feature _feature;

	public CharacterSO CharacterSO => _characterSO;

	public DEBUG_FeatureToggleController.Feature Feature => _feature;

	public event OnCharacterSwapOptionInteractionEnteredHandler OnCharacterSwapOptionInteractionEntered;

	public event OnCharacterSwapOptionInteractionExitedHandler OnCharacterSwapOptionInteractionExited;

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
		this.OnCharacterSwapOptionInteractionEntered?.Invoke(this, new CharacterSwappedZoneEnteredEventArgs(_characterSO));
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

	public void ToggleVisibility(bool visible, Enums.Unlockable unlockable)
	{
		if (SingletonController<UnlocksController>.Instance.IsUnlocked(unlockable))
		{
			CanInteract = visible;
			_spriteRenderer.enabled = visible;
			ResetToOriginalMaterial();
			_spriteRendererShadow.enabled = visible;
			_collider2D.enabled = visible;
			if (!visible)
			{
				DoOutOfRange();
			}
		}
	}
}
