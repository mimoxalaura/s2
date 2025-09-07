using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerWeaponController : MonoBehaviour
{
	[SerializeField]
	private Transform _particlePosition;

	[SerializeField]
	private Transform _lineRendererPosition;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Material _defaultMaterial;

	[SerializeField]
	private Transform _weaponParentLocation;

	[SerializeField]
	private SpriteRenderer _offhandSpriteRenderer;

	[SerializeField]
	private Transform _offhandParticlePosition;

	[SerializeField]
	private Transform _offhandLineRendererPosition;

	private WeaponSO _weapon;

	private Material _weaponElementMaterial;

	private Material _attackElementMaterial;

	private IngameWeaponObject _ingameWeaponObject;

	private void Start()
	{
		EnableSpriteRenderer();
	}

	internal void SetWeaponLayer(int layerId)
	{
		_spriteRenderer.sortingLayerID = layerId;
	}

	internal void EnableSpriteRenderer()
	{
		_spriteRenderer.gameObject.SetActive(value: true);
		if (_offhandSpriteRenderer != null && _weapon != null && _weapon.DualWield)
		{
			_offhandSpriteRenderer.gameObject.SetActive(value: true);
		}
	}

	internal void DisableSpriteRenderer()
	{
		if (_offhandSpriteRenderer != null)
		{
			_offhandSpriteRenderer.gameObject.SetActive(value: false);
		}
		_spriteRenderer.gameObject.SetActive(value: false);
	}

	public void ChangeWeaponVisuals(WeaponSO baseWeapon)
	{
		_spriteRenderer.gameObject.SetActive(value: true);
		_weapon = baseWeapon;
		EquipWeapon(baseWeapon.IngameObject, _weapon.IngameImage, _weapon.IngameImageMaterial, _spriteRenderer);
		if (_offhandSpriteRenderer != null)
		{
			if (baseWeapon.DualWield)
			{
				_offhandSpriteRenderer.gameObject.SetActive(value: true);
				EquipWeapon(baseWeapon.IngameOffhandGameObject, _weapon.IngameOffhandImage, _weapon.IngameImageMaterial, _offhandSpriteRenderer);
			}
			else
			{
				_offhandSpriteRenderer.gameObject.SetActive(value: false);
			}
		}
		ChangeWeaponParticles();
		ChangeWeaponLineRenderer();
	}

	private void EquipWeapon(IngameWeaponObject ingameWeaponObject, Sprite ingameWeaponImage, Material ingameMaterial, SpriteRenderer spriteRenderer)
	{
		if (spriteRenderer == null)
		{
			return;
		}
		spriteRenderer.sprite = null;
		for (int num = spriteRenderer.transform.childCount - 1; num > 0; num--)
		{
			string text = spriteRenderer.transform.GetChild(num).name;
			if (text != "ParticlePoint" && text != "LargeMeleeWeaponMask" && text != "LineRendererPoint")
			{
				Object.Destroy(spriteRenderer.transform.GetChild(num).gameObject);
			}
		}
		if (ingameWeaponObject == null)
		{
			spriteRenderer.sprite = ingameWeaponImage;
			spriteRenderer.material = ingameMaterial;
		}
		else
		{
			_ingameWeaponObject = Object.Instantiate(ingameWeaponObject, spriteRenderer.transform);
		}
	}

	public void AttackWithIngameWeapon()
	{
		if (_ingameWeaponObject != null)
		{
			_ingameWeaponObject.AnimateAttack();
		}
	}

	private void ChangeWeaponParticles()
	{
		if (_particlePosition == null)
		{
			return;
		}
		foreach (Transform item in _particlePosition.transform)
		{
			Object.Destroy(item.gameObject);
		}
		GameObject[] particleEffectsPrefabs = _weapon.ParticleEffectsPrefabs;
		foreach (GameObject original in particleEffectsPrefabs)
		{
			if (_weapon.DualWield)
			{
				Object.Instantiate(original, _offhandParticlePosition);
			}
			Object.Instantiate(original, _particlePosition);
		}
	}

	private void ChangeWeaponLineRenderer()
	{
		if (_lineRendererPosition == null)
		{
			return;
		}
		foreach (Transform item in _lineRendererPosition.transform)
		{
			Object.Destroy(item.gameObject);
		}
		GameObject[] lineEffectsPrefabs = _weapon.LineEffectsPrefabs;
		foreach (GameObject original in lineEffectsPrefabs)
		{
			if (_weapon.DualWield)
			{
				Object.Instantiate(original, _offhandLineRendererPosition);
			}
			Object.Instantiate(original, _lineRendererPosition);
		}
	}

	public void ToggleParticles(bool active)
	{
		foreach (Transform item in _particlePosition.transform)
		{
			item.gameObject.SetActive(active);
		}
	}

	internal void ChangeWeaponElementVisuals(Enums.DamageType elementType)
	{
		if (_weapon == null)
		{
			return;
		}
		if (_weapon.Damage.BaseDamageType != elementType)
		{
			_weaponElementMaterial = MaterialHelper.GetWeaponElementTypeMaterial(elementType);
			_attackElementMaterial = MaterialHelper.GetAttackElementTypeMaterial(elementType);
			_spriteRenderer.material = _weaponElementMaterial;
			if (_weapon.DualWield && _offhandSpriteRenderer != null)
			{
				_offhandSpriteRenderer.material = _weaponElementMaterial;
			}
			return;
		}
		_weaponElementMaterial = _defaultMaterial;
		_attackElementMaterial = _defaultMaterial;
		if (_weapon.IngameImageMaterial == null)
		{
			_spriteRenderer.material = _defaultMaterial;
		}
		else
		{
			_spriteRenderer.material = _weapon.IngameImageMaterial;
		}
		if (_weapon.DualWield && _offhandSpriteRenderer != null)
		{
			_offhandSpriteRenderer.material = _weaponElementMaterial;
		}
	}

	internal void SetMask(SpriteMaskInteraction spriteMaskInteraction)
	{
		_spriteRenderer.maskInteraction = spriteMaskInteraction;
	}
}
