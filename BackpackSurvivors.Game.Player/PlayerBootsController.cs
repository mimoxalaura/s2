using BackpackSurvivors.ScriptableObjects.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerBootsController : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	public void ChangeBootsVisuals(ItemSO boots)
	{
		if (boots != null)
		{
			_spriteRenderer.material = boots.IngameImageMaterial;
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

	internal void SetBootsLayer(int layerId)
	{
		_spriteRenderer.sortingLayerID = layerId;
	}
}
