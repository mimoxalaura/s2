using BackpackSurvivors.ScriptableObjects.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerShieldController : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private SpriteRenderer _offhandspriteRenderer;

	[SerializeField]
	private SpriteMask _spriteMask;

	[SerializeField]
	private Material _defaultMaterial;

	private ItemSO _shield;

	public void ChangeShieldVisuals(ItemSO baseShield)
	{
		_offhandspriteRenderer.gameObject.SetActive(value: false);
		_spriteRenderer.gameObject.SetActive(value: true);
		if (baseShield != null)
		{
			_shield = baseShield;
			_spriteRenderer.material = _defaultMaterial;
			_spriteRenderer.sprite = _shield.IngameImage;
			_spriteMask.sprite = _shield.IngameImage;
		}
	}

	internal void DisableSpriteRenderer()
	{
		_spriteRenderer.gameObject.SetActive(value: false);
	}

	internal void EnableSpriteRenderer()
	{
		_spriteRenderer.gameObject.SetActive(value: true);
	}
}
