using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwappingNew : MonoBehaviour
{
	[SerializeField]
	private CharacterSO _characterSO;

	[SerializeField]
	private Enums.Unlockable _unlockable;

	[SerializeField]
	private GameObject _spriteGhost;

	[SerializeField]
	private GameObject _spriteAnimation;

	[SerializeField]
	private GameObject _spriteActive;

	[SerializeField]
	private GameObject _shadow;

	[SerializeField]
	private GameObject _interaction;

	public CharacterSO CharacterSO => _characterSO;

	public Enums.Unlockable Unlockable => _unlockable;

	public void SetActiveState(bool isCurrentCharacter)
	{
		_spriteGhost.SetActive(!isCurrentCharacter);
		_spriteAnimation.SetActive(!isCurrentCharacter);
		_spriteActive.SetActive(!isCurrentCharacter);
		_interaction.SetActive(!isCurrentCharacter);
	}
}
