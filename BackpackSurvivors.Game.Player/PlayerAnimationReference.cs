using BackpackSurvivors.ScriptableObjects.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerAnimationReference
{
	private PlayerVisualController _playerVisualController;

	public GameObject TopMask;

	public GameObject HelmetMask;

	public SpriteRenderer PlayerSpriteRenderer;

	public GameObject DeathWeaponInGroundMask;

	public GameObject SuccessBackdrop;

	public GameObject LargeMeleeWeaponMask;

	public GameObject LevelCompletedSuccesLight;

	public GameObject LargeMeleeWeaponFeetMask;

	public GameObject PlayerShadow;

	public Transform VisualEffectsContainer;

	public Transform BuffVisualEffectsContainer;

	public GameObject DeathFlag;

	public SpriteRenderer PortalSpriteRenderer;

	public Material SpawningPortalMaterial;

	public Material DespawningPortalMaterial;

	public GameObject DustPrefab;

	public Transform DustPrefabSpawnPoint;

	public IngameWeaponObject IngameWeaponObject;

	public PlayerWeaponController PlayerLargeMeleeWeaponController => _playerVisualController.PlayerLargeMeleeWeaponController;

	public PlayerWeaponController PlayerLargeBowWeaponController => _playerVisualController.PlayerLargeBowWeaponController;

	public PlayerWeaponController PlayerLargeSpellWeaponController => _playerVisualController.PlayerLargeSpellWeaponController;

	public PlayerWeaponController PlayerLargeThrownWeaponController => _playerVisualController.PlayerLargeThrownWeaponController;

	public PlayerWeaponController PlayerLargeFireArmWeaponController => _playerVisualController.PlayerLargeFireArmWeaponController;

	public PlayerWeaponController PlayerSmallMeleeWeaponController => _playerVisualController.PlayerSmallMeleeWeaponController;

	public PlayerWeaponController PlayerSmallBowWeaponController => _playerVisualController.PlayerSmallBowWeaponController;

	public PlayerWeaponController PlayerSmallSpellWeaponController => _playerVisualController.PlayerSmallSpellWeaponController;

	public PlayerWeaponController PlayerSmallThrownWeaponController => _playerVisualController.PlayerSmallThrownWeaponController;

	public PlayerShieldController PlayerShieldController => _playerVisualController.PlayerShieldController;

	public PlayerArmorController PlayerArmorController => _playerVisualController.PlayerArmorController;

	public PlayerHelmetController PlayerHelmetController => _playerVisualController.PlayerHelmetController;

	public PlayerAnimationReference(PlayerVisualController playerVisualController)
	{
		_playerVisualController = playerVisualController;
	}
}
