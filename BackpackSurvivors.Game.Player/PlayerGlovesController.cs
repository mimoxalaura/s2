using BackpackSurvivors.ScriptableObjects.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerGlovesController : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	public void ChangeGlovesVisuals(ItemSO gloves)
	{
		if (gloves != null)
		{
			_spriteRenderer.material = gloves.IngameImageMaterial;
			_spriteRenderer.gameObject.SetActive(value: true);
		}
		else
		{
			_spriteRenderer.gameObject.SetActive(value: false);
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

	internal void SetGlovesLayer(int layerId)
	{
		_spriteRenderer.sortingLayerID = layerId;
	}
}
