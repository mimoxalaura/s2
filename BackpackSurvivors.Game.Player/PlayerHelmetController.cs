using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerHelmetController : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private GameObject _helmetMask;

	[SerializeField]
	private Material _defaultMaterial;

	public void ChangeHelmetVisuals(ItemSO helmet, CharacterSO character)
	{
		_spriteRenderer.gameObject.SetActive(value: false);
		_helmetMask.gameObject.SetActive(value: false);
		if (helmet != null)
		{
			_spriteRenderer.material = _defaultMaterial;
			if (helmet.IngameImagesPerCharacter.ContainsKey(character.Character))
			{
				_spriteRenderer.gameObject.SetActive(value: true);
				_helmetMask.gameObject.SetActive(value: true);
				_spriteRenderer.sprite = helmet.IngameImagesPerCharacter[character.Character];
			}
		}
		else
		{
			_spriteRenderer.sprite = null;
		}
	}

	private void Start()
	{
		_spriteRenderer.gameObject.SetActive(value: false);
	}

	internal void Despawn()
	{
		_spriteRenderer.gameObject.SetActive(value: false);
	}

	internal void AfterDespawn()
	{
		_spriteRenderer.gameObject.SetActive(value: true);
	}

	internal void Spawn()
	{
		_spriteRenderer.gameObject.SetActive(value: false);
	}

	internal void AfterSpawn()
	{
		_spriteRenderer.gameObject.SetActive(value: true);
	}

	internal void SetHelmetLayer(int layerId)
	{
		_spriteRenderer.sortingLayerID = layerId;
	}
}
