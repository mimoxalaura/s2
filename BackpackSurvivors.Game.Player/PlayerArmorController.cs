using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerArmorController : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Material _defaultMaterial;

	private void Start()
	{
		_spriteRenderer.gameObject.SetActive(value: false);
	}

	public void ChangeArmorVisuals(ItemSO armor, Enums.CharacterClass characterClass)
	{
		_spriteRenderer.gameObject.SetActive(value: true);
		if (armor != null)
		{
			if (!armor.BodyMaterials.ContainsKey(characterClass))
			{
				_spriteRenderer.material = _defaultMaterial;
			}
			else
			{
				_spriteRenderer.material = armor.BodyMaterials[characterClass];
			}
		}
		else
		{
			_spriteRenderer.material = _defaultMaterial;
		}
	}
}
