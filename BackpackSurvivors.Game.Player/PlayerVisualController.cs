using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

[RequireComponent(typeof(Animator))]
public class PlayerVisualController : MonoBehaviour
{
	[SerializeField]
	private PlayerWeaponController _playerLargeMeleeWeaponController;

	[SerializeField]
	private PlayerWeaponController _playerLargeBowWeaponController;

	[SerializeField]
	private PlayerWeaponController _playerLargeSpellWeaponController;

	[SerializeField]
	private PlayerWeaponController _playerLargeThrownWeaponController;

	[SerializeField]
	private PlayerWeaponController _playerLargeFireArmWeaponController;

	[SerializeField]
	private PlayerWeaponController _playerSmallMeleeWeaponController;

	[SerializeField]
	private PlayerWeaponController _playerSmallBowWeaponController;

	[SerializeField]
	private PlayerWeaponController _playerSmallSpellWeaponController;

	[SerializeField]
	private PlayerWeaponController _playerSmallThrownWeaponController;

	[SerializeField]
	private PlayerShieldController _playerShieldController;

	[SerializeField]
	private PlayerArmorController _playerArmorController;

	[SerializeField]
	private PlayerHelmetController _playerHelmetController;

	[SerializeField]
	private PlayerGlovesController _playerGlovesController;

	[SerializeField]
	private PlayerBootsController _playerBootsController;

	[SerializeField]
	private Transform _visualEffectsContainer;

	[SerializeField]
	private GameObject _visualOnHitPrefab;

	[SerializeField]
	private Transform[] _visualOnHitPositions;

	[SerializeField]
	private SpriteRenderer _showItem;

	[SerializeField]
	private SpriteRenderer _showItemVfx;

	private WeaponInstance _activeWeapon;

	private ItemSO _activeShield;

	private ItemSO _activeArmor;

	private ItemSO _activeHelmet;

	private ItemSO _activeGloves;

	private ItemSO _activeBoots;

	private Dictionary<Enums.WeaponAnimationSize, Dictionary<Enums.WeaponAnimationType, PlayerWeaponController>> _playerWeaponControllers;

	public PlayerWeaponController PlayerLargeMeleeWeaponController => _playerLargeMeleeWeaponController;

	public PlayerWeaponController PlayerLargeBowWeaponController => _playerLargeBowWeaponController;

	public PlayerWeaponController PlayerLargeSpellWeaponController => _playerLargeSpellWeaponController;

	public PlayerWeaponController PlayerLargeThrownWeaponController => _playerLargeThrownWeaponController;

	public PlayerWeaponController PlayerLargeFireArmWeaponController => _playerLargeFireArmWeaponController;

	public PlayerWeaponController PlayerSmallMeleeWeaponController => _playerSmallMeleeWeaponController;

	public PlayerWeaponController PlayerSmallBowWeaponController => _playerSmallBowWeaponController;

	public PlayerWeaponController PlayerSmallSpellWeaponController => _playerSmallSpellWeaponController;

	public PlayerWeaponController PlayerSmallThrownWeaponController => _playerSmallThrownWeaponController;

	public PlayerShieldController PlayerShieldController => _playerShieldController;

	public PlayerArmorController PlayerArmorController => _playerArmorController;

	public PlayerHelmetController PlayerHelmetController => _playerHelmetController;

	public PlayerGlovesController PlayerGlovesController => _playerGlovesController;

	public PlayerBootsController PlayerBootsController => _playerBootsController;

	public bool HasShield => _activeShield != null;

	public bool HasHelmet => _activeHelmet != null;

	public bool HasGloves => _activeGloves != null;

	public bool HasBoots => _activeBoots != null;

	public bool HasWeapon => _activeWeapon != null;

	public WeaponInstance ActiveWeapon => _activeWeapon;

	public Enums.WeaponAnimationSize WeaponAnimationSize
	{
		get
		{
			if (!HasWeapon)
			{
				return Enums.WeaponAnimationSize.None;
			}
			return _activeWeapon.BaseWeaponSO.WeaponAnimationSize;
		}
	}

	public Enums.WeaponAnimationType WeaponAnimationType
	{
		get
		{
			if (!HasWeapon)
			{
				return Enums.WeaponAnimationType.None;
			}
			return _activeWeapon.BaseWeaponSO.WeaponAnimationType;
		}
	}

	private void Awake()
	{
		_playerWeaponControllers = new Dictionary<Enums.WeaponAnimationSize, Dictionary<Enums.WeaponAnimationType, PlayerWeaponController>>();
		_playerWeaponControllers.Add(Enums.WeaponAnimationSize.Small, new Dictionary<Enums.WeaponAnimationType, PlayerWeaponController>());
		_playerWeaponControllers.Add(Enums.WeaponAnimationSize.Large, new Dictionary<Enums.WeaponAnimationType, PlayerWeaponController>());
		_playerWeaponControllers[Enums.WeaponAnimationSize.Small].Add(Enums.WeaponAnimationType.Melee, _playerSmallMeleeWeaponController);
		_playerWeaponControllers[Enums.WeaponAnimationSize.Small].Add(Enums.WeaponAnimationType.Bow, _playerSmallBowWeaponController);
		_playerWeaponControllers[Enums.WeaponAnimationSize.Small].Add(Enums.WeaponAnimationType.Spell, _playerSmallSpellWeaponController);
		_playerWeaponControllers[Enums.WeaponAnimationSize.Small].Add(Enums.WeaponAnimationType.Thrown, _playerSmallThrownWeaponController);
		_playerWeaponControllers[Enums.WeaponAnimationSize.Large].Add(Enums.WeaponAnimationType.Melee, _playerLargeMeleeWeaponController);
		_playerWeaponControllers[Enums.WeaponAnimationSize.Large].Add(Enums.WeaponAnimationType.Bow, _playerLargeBowWeaponController);
		_playerWeaponControllers[Enums.WeaponAnimationSize.Large].Add(Enums.WeaponAnimationType.Spell, _playerLargeSpellWeaponController);
		_playerWeaponControllers[Enums.WeaponAnimationSize.Large].Add(Enums.WeaponAnimationType.Thrown, _playerLargeThrownWeaponController);
		_playerWeaponControllers[Enums.WeaponAnimationSize.Large].Add(Enums.WeaponAnimationType.Firearm, _playerLargeFireArmWeaponController);
	}

	public void SetWeaponLayer(int layerId)
	{
		foreach (Enums.WeaponAnimationSize key in _playerWeaponControllers.Keys)
		{
			foreach (Enums.WeaponAnimationType key2 in _playerWeaponControllers[key].Keys)
			{
				_playerWeaponControllers[key][key2].SetWeaponLayer(layerId);
			}
		}
	}

	public void SetHelmetLayer(int layerId)
	{
		_playerHelmetController.SetHelmetLayer(layerId);
	}

	public void SetGlovesLayer(int layerId)
	{
		if (!(_playerGlovesController == null))
		{
			_playerGlovesController.SetGlovesLayer(layerId);
		}
	}

	public void SetBootsLayer(int layerId)
	{
		if (!(_playerBootsController == null))
		{
			_playerBootsController.SetBootsLayer(layerId);
		}
	}

	public void ChangeWeaponVisuals(WeaponInstance weaponInstance)
	{
		ResetWeapon();
		if (_activeWeapon != null)
		{
			_playerWeaponControllers[_activeWeapon.BaseWeaponSO.WeaponAnimationSize][_activeWeapon.BaseWeaponSO.WeaponAnimationType].DisableSpriteRenderer();
		}
		_activeWeapon = weaponInstance;
		if (HasWeapon)
		{
			_playerWeaponControllers[_activeWeapon.BaseWeaponSO.WeaponAnimationSize][_activeWeapon.BaseWeaponSO.WeaponAnimationType].DisableSpriteRenderer();
			_playerWeaponControllers[_activeWeapon.BaseWeaponSO.WeaponAnimationSize][_activeWeapon.BaseWeaponSO.WeaponAnimationType].ChangeWeaponVisuals(_activeWeapon.BaseWeaponSO);
			_playerWeaponControllers[_activeWeapon.BaseWeaponSO.WeaponAnimationSize][_activeWeapon.BaseWeaponSO.WeaponAnimationType].ChangeWeaponElementVisuals(_activeWeapon.DamageInstance.CalculatedDamageType);
		}
	}

	public void ChangeItemVisualsOnShow(Sprite sprite)
	{
		_showItem.gameObject.SetActive(value: true);
		_showItem.sprite = sprite;
		_showItemVfx.gameObject.SetActive(value: true);
		_showItemVfx.sprite = sprite;
	}

	public void HideItemVisuals()
	{
		_showItem.gameObject.SetActive(value: false);
	}

	public void ResetWeapon()
	{
		_activeWeapon = null;
		foreach (PlayerWeaponController value in _playerWeaponControllers[Enums.WeaponAnimationSize.Small].Values)
		{
			value.DisableSpriteRenderer();
		}
		foreach (PlayerWeaponController value2 in _playerWeaponControllers[Enums.WeaponAnimationSize.Large].Values)
		{
			value2.DisableSpriteRenderer();
		}
	}

	public void ResetShield()
	{
		_activeShield = null;
		_playerShieldController.gameObject.SetActive(value: false);
	}

	public void ResetHelmet(Player player)
	{
		_activeHelmet = null;
		_playerHelmetController.ChangeHelmetVisuals(null, player.BaseCharacter);
	}

	public void ResetGloves()
	{
	}

	public void ResetBoots()
	{
	}

	public void ResetArmor()
	{
		_activeArmor = null;
		if (SingletonController<GameController>.Instance.Player.BaseCharacter == null)
		{
			_playerArmorController.ChangeArmorVisuals(null, Enums.CharacterClass.Warrior);
		}
		else
		{
			_playerArmorController.ChangeArmorVisuals(null, SingletonController<GameController>.Instance.Player.BaseCharacter.Character);
		}
	}

	public void ChangeShieldVisuals(ItemSO shield)
	{
		_activeShield = shield;
		_playerShieldController.ChangeShieldVisuals(shield);
		if (shield != null)
		{
			if (_activeWeapon == null || (_activeWeapon.BaseWeaponSO.WeaponAnimationSize == Enums.WeaponAnimationSize.Small && _activeWeapon.BaseWeaponSO.WeaponAnimationType == Enums.WeaponAnimationType.Bow) || _activeWeapon.BaseWeaponSO.WeaponAnimationType == Enums.WeaponAnimationType.Melee || _activeWeapon.BaseWeaponSO.WeaponAnimationType == Enums.WeaponAnimationType.Thrown || _activeWeapon.BaseWeaponSO.WeaponAnimationType == Enums.WeaponAnimationType.Spell)
			{
				_playerShieldController.gameObject.SetActive(value: true);
				return;
			}
			_activeShield = null;
			_playerShieldController.ChangeShieldVisuals(null);
			_playerShieldController.gameObject.SetActive(value: false);
		}
		else
		{
			_activeShield = null;
			_playerShieldController.ChangeShieldVisuals(null);
			_playerShieldController.gameObject.SetActive(value: false);
		}
	}

	public void ChangeArmorVisuals(ItemSO armor)
	{
		_activeArmor = armor;
		_playerArmorController.ChangeArmorVisuals(_activeArmor, SingletonController<GameController>.Instance.Player.BaseCharacter.Character);
	}

	public void ChangeHelmetVisuals(ItemSO helmet, Player player)
	{
		_activeHelmet = helmet;
		_playerHelmetController.ChangeHelmetVisuals(_activeHelmet, player.BaseCharacter);
	}

	public void ChangeGlovesVisuals(ItemSO gloves)
	{
	}

	public void ChangeBootsVisuals(ItemSO boots)
	{
	}

	internal void ShowVisual(GameObject visualOnPlayerPrefab, int amount, float delayPerVisual, float fallOffTime, Transform overridePositionTransform = null)
	{
		StartCoroutine(VisualBuffOnPlayerFallOff(amount, delayPerVisual, fallOffTime, visualOnPlayerPrefab, overridePositionTransform));
	}

	private IEnumerator VisualBuffOnPlayerFallOff(int amount, float delayPerVisual, float fallOffTime, GameObject visualOnPlayerPrefab, Transform overridePositionTransform = null)
	{
		for (int i = 0; i < amount; i++)
		{
			GameObject visualOnPlayerInstance = ((!(overridePositionTransform != null)) ? Object.Instantiate(visualOnPlayerPrefab, _visualEffectsContainer) : Object.Instantiate(visualOnPlayerPrefab, overridePositionTransform));
			StartCoroutine(DestroyAfterDelay(fallOffTime, visualOnPlayerInstance));
			yield return new WaitForSeconds(delayPerVisual);
		}
	}

	private IEnumerator DestroyAfterDelay(float fallOffTime, GameObject visualOnPlayerInstance)
	{
		yield return new WaitForSeconds(fallOffTime);
		Object.Destroy(visualOnPlayerInstance.gameObject);
	}

	public void ShowDamageBlockedEffect()
	{
		int num = Random.Range(0, _visualOnHitPositions.Length - 1);
		ShowVisual(_visualOnHitPrefab, 1, 0f, 0.5f, _visualOnHitPositions[num]);
	}
}
