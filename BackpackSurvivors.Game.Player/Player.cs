using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Assets.Game.Revive;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Backpack.Events;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Health;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shared.Interfaces;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.Game.Player;

public class Player : Character, IInitializable
{
	[Header("Animator")]
	[SerializeField]
	private SerializableDictionaryBase<Enums.CharacterClass, PlayerVisualController> _playerVisualController;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CharacterClass, PlayerAnimatorController> _playerAnimatorController;

	[SerializeField]
	private KnockbackFeedback knockbackFeedback;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CharacterClass, GameObject> _playerSprite;

	[Header("VFX")]
	[SerializeField]
	private GameObject _lifedrainFXPrefab;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CharacterClass, KilledVfx> _killedVfx;

	[SerializeField]
	private HitStop _hitStop;

	[Header("SFX")]
	[SerializeField]
	private AudioClip _spawningAudioClip;

	[SerializeField]
	private AudioClip _revivingAudioClip;

	[SerializeField]
	private AudioClip _killedAudioClip;

	[SerializeField]
	private AudioClip _despawningAudioClip;

	[SerializeField]
	private AudioClip _dodgedAudioClip;

	[Header("Parent")]
	[SerializeField]
	private Transform _weaponAttackTransform;

	[Header("Aiming")]
	[SerializeField]
	private GameObject _playerAimParentTransform;

	[SerializeField]
	private List<Transform> _transformsToRotateOnPlayerTotation;

	[SerializeField]
	private Sprite DEBUG_ITEM;

	public float PlayerBaseSpeedModifier = 1f;

	private bool _canPlayDamagePreventedAudio = true;

	private float _damageBlockedTimer;

	private PlayerMovement _playerMovement;

	private PlayerDash _playerDash;

	private PickupRadius _moveToPlayerPickupRadius;

	private Character _lastDamageSource;

	private Coroutine _runningHealthRegenCoroutine;

	public Dictionary<Enums.PlaceableRarity, float> ShopOfferRarityChances = new Dictionary<Enums.PlaceableRarity, float>();

	[SerializeField]
	private SpriteRenderer _blackBackdropPlayerSpriteRenderer;

	[SerializeField]
	private Light2D _blackBackdropGlowLight;

	[SerializeField]
	private Light2D _blackBackdropPlayerLight;

	[SerializeField]
	private ParticleSystem _blackBackdropParticles;

	internal PlayerAnimatorController PlayerAnimatorController => _playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character];

	public PlayerVisualController PlayerVisualController => _playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character];

	public Dictionary<Enums.ItemStatType, float> BaseStats { get; internal set; }

	public Dictionary<Enums.DamageType, float> BaseDamageTypeValues { get; internal set; }

	public Character LastDamageSource => _lastDamageSource;

	public CharacterSO BaseCharacter { get; private set; }

	public float HealthPercentage
	{
		get
		{
			if (base.HealthSystem == null)
			{
				return 1f;
			}
			return base.HealthSystem.GetHealth() / base.HealthSystem.GetHealthMax();
		}
	}

	public Transform WeaponAttackTransform => _weaponAttackTransform;

	public bool IsInitialized { get; private set; }

	public int TotalDashCount => GetPlayerDash().TotalDashes;

	public int CurrentDashCount => GetPlayerDash().CurrentDashes;

	public Transform Transform => base.transform;

	internal override bool IsRotatedToRight()
	{
		return _playerMovement.IsRotatedToRight;
	}

	internal override SpriteRenderer GetSpriteRenderer()
	{
		return _playerSprite[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].GetComponent<SpriteRenderer>();
	}

	public void SetBeingKnockedBack(bool isBeingKnockedBack)
	{
		_playerMovement.IsBeingKnockedback = isBeingKnockedBack;
	}

	private void Awake()
	{
		RegisterComponents();
		foreach (KeyValuePair<Enums.CharacterClass, PlayerAnimatorController> item in _playerAnimatorController)
		{
			item.Value.InitializeStates(new PlayerAnimationReference(_playerVisualController[item.Key]));
			item.Value.GetComponent<SpriteRenderer>().enabled = false;
			_playerVisualController[item.Key].PlayerLargeMeleeWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerSmallSpellWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerSmallBowWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerSmallThrownWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerLargeMeleeWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerLargeSpellWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerLargeBowWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerLargeThrownWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerLargeFireArmWeaponController.gameObject.SetActive(value: false);
			_playerVisualController[item.Key].PlayerShieldController.gameObject.SetActive(value: false);
		}
		SetIsPlayer();
	}

	private void CharactersController_Initialized()
	{
		SingletonController<CharactersController>.Instance.OnCharacterLoaded += CharactersController_OnCharacterLoaded;
		SingletonController<CharactersController>.Instance.OnCharacterSwitched += CharactersController_OnCharacterSwitched;
		Initialize();
	}

	private void CharactersController_OnCharacterSwitched(object sender, CharacterSwitchedEventArgs e)
	{
		foreach (KeyValuePair<Enums.CharacterClass, GameObject> item in _playerSprite)
		{
			if (item.Key != e.NewCharacter.Character)
			{
				item.Value.SetActive(value: false);
			}
			else
			{
				item.Value.SetActive(value: true);
			}
		}
		LoadCharacter(e.NewCharacter);
		Spawn(teleportIn: false);
	}

	private void CharactersController_OnCharacterLoaded(object sender, CharacterSwitchedEventArgs e)
	{
		LoadCharacter(e.NewCharacter);
	}

	private void Start()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, CharactersController_Initialized);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<EventController>.Instance, EventController_Initialized);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<UnlocksController>.Instance, UnlocksController_Initialized);
	}

	private void EventController_Initialized()
	{
		SingletonController<EventController>.Instance.RegisterPlayer(this);
	}

	private void UnlocksController_Initialized()
	{
		SingletonController<UnlocksController>.Instance.OnUnlockableUnlocked += UnlocksController_OnUnlockableUnlocked;
	}

	private void UnlocksController_OnUnlockableUnlocked(object sender, UnlockableUnlockedEventArgs e)
	{
		if (e.UnlockedItem.Unlockable == Enums.Unlockable.ExtraDashes)
		{
			UpdateDashes();
		}
	}

	private void UpdateDashes()
	{
		if (!BaseStats.ContainsKey(Enums.ItemStatType.ExtraDash))
		{
			BaseStats.Add(Enums.ItemStatType.ExtraDash, 0f);
		}
		BaseStats[Enums.ItemStatType.ExtraDash]++;
	}

	private void Initialize()
	{
		LoadCharacter(SingletonController<CharactersController>.Instance.ActiveCharacter);
		HandlePlayerAnimations();
		RegisterEvents();
		SetupMovement();
		InitBuffedStats();
		_playerDash.UpdateTotalDashCount();
		IsInitialized = true;
		RefreshPlayerAiming();
	}

	private void Update()
	{
		if (IsInitialized)
		{
			HandlePlayerAnimations();
			_damageBlockedTimer += Time.deltaTime;
		}
	}

	internal DebuffContainer GetPlayerDebuffContainer()
	{
		return GetDebuffContainer();
	}

	internal bool TryGetBaseStat(Enums.ItemStatType statType, out float baseValue)
	{
		if (BaseStats.TryGetValue(statType, out var value))
		{
			baseValue = value;
			return true;
		}
		baseValue = 0f;
		return false;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		_playerAnimatorController[Enums.CharacterClass.Warrior].OnPlayerAnimationCompleted -= PlayerAnimatorController_OnPlayerAnimationCompleted;
		_playerAnimatorController[Enums.CharacterClass.Necromancer].OnPlayerAnimationCompleted -= PlayerAnimatorController_OnPlayerAnimationCompleted;
		_playerAnimatorController[Enums.CharacterClass.Mage].OnPlayerAnimationCompleted -= PlayerAnimatorController_OnPlayerAnimationCompleted;
		SingletonController<BackpackController>.Instance.OnDraggableDropped -= BackpackController_OnDraggableDropped;
		SingletonController<BackpackController>.Instance.OnDraggableSold -= BackpackController_OnDraggableSold;
		GetPlayerMovement().OnPlayerAimUpdated -= PlayerMovement_OnPlayerAimUpdated;
		GetPlayerMovement().OnPlayerRotationUpdated -= Player_OnPlayerRotationUpdated;
		SingletonController<CharactersController>.Instance.OnCharacterLoaded -= CharactersController_OnCharacterLoaded;
		SingletonController<CharactersController>.Instance.OnCharacterSwitched -= CharactersController_OnCharacterSwitched;
		SingletonController<EventController>.Instance.UnregisterPlayer(this);
	}

	private void RegisterComponents()
	{
		PickupRadius[] componentsInChildren = GetComponentsInChildren<PickupRadius>();
		_moveToPlayerPickupRadius = componentsInChildren.First((PickupRadius p) => p.Type == PickupRadius.PickupRadiusType.MoveToPlayer);
	}

	public Dictionary<Enums.ItemStatType, List<ItemStatModifier>> GetComplexBaseStats()
	{
		Dictionary<Enums.ItemStatType, List<ItemStatModifier>> dictionary = new Dictionary<Enums.ItemStatType, List<ItemStatModifier>>();
		foreach (KeyValuePair<Enums.ItemStatType, float> baseStat in SingletonController<GameController>.Instance.Player.BaseStats)
		{
			dictionary.Add(baseStat.Key, new List<ItemStatModifier>
			{
				new ItemStatModifier
				{
					CalculatedBonus = baseStat.Value,
					ItemStatType = baseStat.Key,
					Source = null,
					ItemModifierFilter = null
				}
			});
		}
		return dictionary;
	}

	private void SetupMovement()
	{
		GetPlayerMovement().SetMovementSpeed(BaseCharacter.BaseMovementSpeed * PlayerBaseSpeedModifier);
	}

	public void LoadCharacter(CharacterSO character)
	{
		BaseCharacter = character;
		ShopOfferRarityChances = new Dictionary<Enums.PlaceableRarity, float>();
		foreach (KeyValuePair<Enums.PlaceableRarity, float> shopOfferRarityChance in BaseCharacter.ShopOfferRarityChances)
		{
			ShopOfferRarityChances.Add(shopOfferRarityChance.Key, shopOfferRarityChance.Value);
		}
		BaseStats = StatCalculator.TransformSerializeDictionaryBaseToDictionary(character.StartingStats.StatValues);
		ApplyUnlockedStats(BaseStats);
		BaseDamageTypeValues = StatCalculator.TransformSerializeDictionaryBaseToDictionary(character.StartingStats.DamageTypeValues);
		InitCalculatedStats(BaseStats);
		base.CalculatedDamageTypeValues = BaseDamageTypeValues;
		InitializeHealthSystem();
		TriggerOnLoadedEvent(null);
	}

	private void ApplyUnlockedStats(Dictionary<Enums.ItemStatType, float> baseStats)
	{
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ExtraDashes);
		if (!baseStats.ContainsKey(Enums.ItemStatType.ExtraDash))
		{
			baseStats.Add(Enums.ItemStatType.ExtraDash, 0f);
		}
		baseStats[Enums.ItemStatType.ExtraDash] += unlockedCount;
	}

	public void UpdateStats(Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats, bool healHealth, bool refreshDashes = true)
	{
		base.CalculatedStatsWithSource = playerStats;
		InitCalculatedStats(ConvertToSimpleList(playerStats));
		float calculatedStat = GetCalculatedStat(Enums.ItemStatType.Health);
		calculatedStat = Mathf.Clamp(calculatedStat, 1f, calculatedStat);
		base.HealthSystem.SetHealthMax(calculatedStat, healHealth);
		GetPlayerMovement().SetMovementSpeed(BaseCharacter.BaseMovementSpeed * (GetCalculatedStat(Enums.ItemStatType.SpeedPercentage) + GetBuffedStat(Enums.ItemStatType.SpeedPercentage)) * PlayerBaseSpeedModifier);
		_moveToPlayerPickupRadius.UpdatePickupRadius(GetCalculatedStat(Enums.ItemStatType.PickupRadiusPercentage));
		if (refreshDashes)
		{
			_playerDash.UpdateTotalDashCount();
		}
	}

	public void RefreshMovementSpeed()
	{
		GetPlayerMovement().SetMovementSpeed(BaseCharacter.BaseMovementSpeed * (GetCalculatedStat(Enums.ItemStatType.SpeedPercentage) + GetBuffedStat(Enums.ItemStatType.SpeedPercentage)) * PlayerBaseSpeedModifier);
	}

	private Dictionary<Enums.ItemStatType, float> ConvertToSimpleList(Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats)
	{
		Dictionary<Enums.ItemStatType, float> dictionary = new Dictionary<Enums.ItemStatType, float>();
		foreach (KeyValuePair<Enums.ItemStatType, List<ItemStatModifier>> playerStat in playerStats)
		{
			dictionary.Add(playerStat.Key, playerStat.Value.Sum((ItemStatModifier x) => x.CalculatedBonus));
		}
		return dictionary;
	}

	private Dictionary<Enums.DamageType, float> ConvertToSimpleList(Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> damageTypeValues)
	{
		Dictionary<Enums.DamageType, float> dictionary = new Dictionary<Enums.DamageType, float>();
		foreach (KeyValuePair<Enums.DamageType, List<DamageTypeValueModifier>> damageTypeValue in damageTypeValues)
		{
			dictionary.Add(damageTypeValue.Key, damageTypeValue.Value.Sum((DamageTypeValueModifier x) => x.CalculatedBonus));
		}
		return dictionary;
	}

	public void UpdateDamageTypeValues(Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> damageTypeValues)
	{
		base.CalculatedDamageTypeValuesWithSource = damageTypeValues;
		base.CalculatedDamageTypeValues = ConvertToSimpleList(damageTypeValues);
		foreach (KeyValuePair<Enums.DamageType, float> buffDamageType in base.BuffDamageTypes)
		{
			float buffedDamageType = GetBuffedDamageType(buffDamageType.Key);
			base.CalculatedDamageTypeValues[buffDamageType.Key] += buffedDamageType;
		}
	}

	private void BackpackController_OnDraggableDropped(object sender, DraggableDroppedEventArgs e)
	{
		RefreshPlayer();
	}

	private void BackpackController_OnDraggableSold(object sender, EventArgs e)
	{
		RefreshPlayer();
	}

	private void RefreshPlayer()
	{
		ResetVisuals();
		SingletonController<GameController>.Instance.Player.SwitchToIdleAnimation();
		RefreshVisuals(returnToIdle: true);
		RefreshPlayerAiming();
	}

	private void RefreshPlayerAiming()
	{
		bool active = SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack().Any((WeaponInstance x) => x.BaseAttackType == Enums.AttackTargetingType.AttackClosestEnemy);
		_playerAimParentTransform.SetActive(active);
	}

	internal void RefreshVisuals(bool returnToIdle = false)
	{
		IsAttackingWithVisualWeapon = false;
		StartCoroutine(RefreshVisualsAsync(returnToIdle));
	}

	private IEnumerator RefreshVisualsAsync(bool returnToIdle = false)
	{
		yield return new WaitForSecondsRealtime(0.1f);
		SetupVisualForWeaponFromBackpack();
		SetupVisualForShieldFromBackpack();
		SetupVisualForArmorFromBackpack();
		SetupVisualForHelmetFromBackpack();
		SetupVisualForGlovesFromBackpack();
		SetupVisualForBootsFromBackpack();
		if (returnToIdle)
		{
			SingletonController<GameController>.Instance.Player.SwitchToIdleAnimation();
		}
	}

	public void ResetVisuals()
	{
		if (!(SingletonController<CharactersController>.Instance.ActiveCharacter == null))
		{
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].gameObject.SetActive(value: true);
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ResetWeapon();
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ResetShield();
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ResetHelmet(this);
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ResetArmor();
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ResetGloves();
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ResetBoots();
		}
	}

	[Command("player.animation.succes", Platform.AllPlatforms, MonoTargetType.Single)]
	public void ShowSuccesEffect()
	{
		SetCanAct(canAct: false);
		ShowPlayerBackdrop();
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowSuccesAnimation();
	}

	private void ShowPlayerBackdrop()
	{
		_playerSprite[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("UI");
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetWeaponLayer(SortingLayer.NameToID("UI"));
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetHelmetLayer(SortingLayer.NameToID("UI"));
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetBootsLayer(SortingLayer.NameToID("UI"));
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetGlovesLayer(SortingLayer.NameToID("UI"));
		_blackBackdropPlayerSpriteRenderer.gameObject.SetActive(value: true);
		LeanTween.value(_blackBackdropPlayerSpriteRenderer.gameObject, delegate(float val)
		{
			_blackBackdropPlayerSpriteRenderer.color = new Color(0f, 0f, 0f, val);
		}, 0f, 1f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		_blackBackdropGlowLight.gameObject.SetActive(value: true);
		_blackBackdropPlayerLight.gameObject.SetActive(value: true);
		ParticleSystem.EmissionModule emission = _blackBackdropParticles.emission;
		emission.rateOverTime = 100f;
		_blackBackdropParticles.gameObject.SetActive(value: true);
	}

	public void EndSuccesEffect()
	{
		_playerSprite[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Player");
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetWeaponLayer(SortingLayer.NameToID("Player"));
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetHelmetLayer(SortingLayer.NameToID("Player"));
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetBootsLayer(SortingLayer.NameToID("Player"));
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetGlovesLayer(SortingLayer.NameToID("Player"));
		ParticleSystem.EmissionModule emission = _blackBackdropParticles.emission;
		emission.rateOverTime = 0f;
		LeanTween.value(_blackBackdropPlayerSpriteRenderer.gameObject, delegate(float val)
		{
			_blackBackdropPlayerSpriteRenderer.color = new Color(0f, 0f, 0f, val);
		}, 0f, 1f, 1f).setIgnoreTimeScale(useUnScaledTime: true).setOnComplete((Action)delegate
		{
			_blackBackdropPlayerSpriteRenderer.gameObject.SetActive(value: false);
			_blackBackdropGlowLight.gameObject.SetActive(value: false);
			_blackBackdropPlayerLight.gameObject.SetActive(value: false);
			_blackBackdropParticles.gameObject.SetActive(value: false);
		});
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowIdleAnimation();
	}

	public void ShowItemEffect()
	{
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowShowItemAnimation();
	}

	public void ShowItemEffect(Sprite sprite, bool playAudio)
	{
		SetCanAct(canAct: false);
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeItemVisualsOnShow(sprite);
		ChangeWeapon(null);
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeShieldVisuals(null);
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowIdleAnimation();
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowShowItemAnimation();
		if (playAudio)
		{
			SingletonController<AudioController>.Instance.PlayShowItemSFX();
		}
		StartCoroutine(HideItemEffect());
	}

	private IEnumerator HideItemEffect()
	{
		yield return new WaitForSeconds(3f);
		SetCanAct(canAct: true);
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].HideItemVisuals();
	}

	public void SetupVisualForWeaponFromBackpack()
	{
		WeaponInstance rarestWeaponInBackpack = SingletonController<BackpackController>.Instance.GetRarestWeaponInBackpack();
		ChangeWeapon(rarestWeaponInBackpack);
	}

	public void SetupVisualForArmorFromBackpack()
	{
		ItemInstance rarestArmorInBackpack = SingletonController<BackpackController>.Instance.GetRarestArmorInBackpack();
		if (rarestArmorInBackpack != null)
		{
			ChangeArmor(rarestArmorInBackpack.ItemSO);
		}
	}

	public void SetupVisualForHelmetFromBackpack()
	{
		ItemInstance rarestHelmetInBackpack = SingletonController<BackpackController>.Instance.GetRarestHelmetInBackpack();
		if (rarestHelmetInBackpack != null)
		{
			ChangeHelmet(rarestHelmetInBackpack.ItemSO);
		}
	}

	public void SetupVisualForGlovesFromBackpack()
	{
		ItemInstance rarestGlovesInBackpack = SingletonController<BackpackController>.Instance.GetRarestGlovesInBackpack();
		if (rarestGlovesInBackpack != null)
		{
			ChangeGloves(rarestGlovesInBackpack.ItemSO);
		}
	}

	public void SetupVisualForBootsFromBackpack()
	{
		ItemInstance rarestBootsInBackpack = SingletonController<BackpackController>.Instance.GetRarestBootsInBackpack();
		if (rarestBootsInBackpack != null)
		{
			ChangeBoots(rarestBootsInBackpack.ItemSO);
		}
	}

	public void SetupVisualForShieldFromBackpack()
	{
		ItemInstance rarestShieldInBackpack = SingletonController<BackpackController>.Instance.GetRarestShieldInBackpack();
		if (rarestShieldInBackpack != null)
		{
			ChangeShield(rarestShieldInBackpack.ItemSO);
		}
	}

	public void InitializeHealthSystem()
	{
		if (base.HealthSystem != null)
		{
			RemoveHealthEvents();
		}
		base.HealthSystem = new HealthSystem(GetCalculatedStat(Enums.ItemStatType.Health));
		InitHealthEvents();
		InitHealthData(0.5f);
		UpdateHealthRegen();
	}

	public void UpdateHealthRegen()
	{
		try
		{
			if (base.isActiveAndEnabled)
			{
				if (_runningHealthRegenCoroutine != null)
				{
					StopCoroutine(_runningHealthRegenCoroutine);
				}
				_runningHealthRegenCoroutine = StartCoroutine(HealthRegenAsync());
			}
		}
		catch (Exception)
		{
		}
	}

	private IEnumerator HealthRegenAsync()
	{
		while (!base.IsDead)
		{
			yield return new WaitForSeconds(5f);
			if (!base.IsDead)
			{
				float calculatedStat = GetCalculatedStat(Enums.ItemStatType.HealthRegeneration);
				calculatedStat = Mathf.Clamp(calculatedStat, 0f, calculatedStat);
				base.HealthSystem.Heal(calculatedStat);
			}
		}
	}

	private void RegisterEvents()
	{
		SingletonController<BackpackController>.Instance.OnDraggableDropped += BackpackController_OnDraggableDropped;
		SingletonController<BackpackController>.Instance.OnDraggableSold += BackpackController_OnDraggableSold;
		_playerAnimatorController[Enums.CharacterClass.Warrior].OnPlayerAnimationCompleted += PlayerAnimatorController_OnPlayerAnimationCompleted;
		_playerAnimatorController[Enums.CharacterClass.Necromancer].OnPlayerAnimationCompleted += PlayerAnimatorController_OnPlayerAnimationCompleted;
		_playerAnimatorController[Enums.CharacterClass.Mage].OnPlayerAnimationCompleted += PlayerAnimatorController_OnPlayerAnimationCompleted;
		GetPlayerMovement().OnPlayerAimUpdated += PlayerMovement_OnPlayerAimUpdated;
		GetPlayerMovement().OnPlayerRotationUpdated += Player_OnPlayerRotationUpdated;
		InitDashEvents();
		RegisterShopControllerEvents();
	}

	internal void InitDashEvents()
	{
		GetPlayerDash().OnDashCooldownUpdated += _playerDash_OnDashCooldownUpdated;
		GetPlayerDash().OnDashed += _playerDash_OnDashed;
		GetPlayerDash().OnDashCountChanged += _playerDash_OnDashCountChanged;
		GetPlayerDash().OnDashReady += _playerDash_OnDashReady;
		GetPlayerDash().OnDashesCountSet += Player_OnDashesCountSet;
	}

	private void Player_OnDashesCountSet(object sender, EventArgs e)
	{
		TriggerDashesCountSet(e);
	}

	private void _playerDash_OnDashed(object sender, DashCooldownEventArgs e)
	{
		TriggerDashEvent(e);
	}

	private void _playerDash_OnDashCooldownUpdated(object sender, DashCooldownEventArgs e)
	{
		TriggerDashCooldownEvent(e);
	}

	private void _playerDash_OnDashCountChanged(object sender, DashCooldownEventArgs e)
	{
		TriggerDashCountChangedEvent(e);
	}

	private void _playerDash_OnDashReady(object sender, DashCooldownEventArgs e)
	{
		TriggerDashReadyEvent(e);
	}

	private void RegisterShopControllerEvents()
	{
		ShopController controllerByType = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		if (controllerByType != null)
		{
			controllerByType.OnShopClosed += ShopController_OnShopClosed;
			controllerByType.OnShopOpened += ShopController_OnShopOpened;
		}
	}

	private void ShopController_OnShopClosed(object sender, ShopClosedEventArgs e)
	{
		if (e.ContinueToAdventure)
		{
			base.HealthSystem.ApplyChangesInHealthMaxWhenExitingShop();
			UpdateHealthRegen();
		}
	}

	private void ShopController_OnShopOpened()
	{
		base.HealthSystem.StoreHealthWhenEnteringShop();
	}

	private void Player_OnPlayerRotationUpdated(float rotation)
	{
		foreach (Transform item in _transformsToRotateOnPlayerTotation)
		{
			item.localScale = new Vector3(rotation, 1f, 1f);
		}
	}

	private void PlayerMovement_OnPlayerAimUpdated(Vector2 aim)
	{
		_ = aim == Vector2.zero;
	}

	private void PlayerAnimatorController_OnPlayerAnimationCompleted(object sender, PlayerAnimationCompletedEventArgs e)
	{
		if (e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Spawning)
		{
			FinishSpawn();
		}
		if (e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeBowWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallBowWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeMeleeWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallMeleeWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeSpellWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallSpellWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeThrownWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallThrownWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeFireArmWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackNoWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeBowWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallBowWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeMeleeWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallMeleeWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeSpellWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallSpellWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeThrownWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallThrownWeapon || e.PlayerAnimationState == Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackNoWeapon)
		{
			IsAttacking = false;
			IsAttackingWithVisualWeapon = false;
		}
	}

	private void HandlePlayerAnimations()
	{
		if (!(GetComponent<PlayerMovement>() == null))
		{
			IsMoving = GetPlayerMovement().IsMoving;
		}
	}

	private PlayerMovement GetPlayerMovement()
	{
		if (_playerMovement == null)
		{
			_playerMovement = GetComponent<PlayerMovement>();
		}
		return _playerMovement;
	}

	internal PlayerDash GetPlayerDash()
	{
		if (_playerDash == null)
		{
			_playerDash = GetComponent<PlayerDash>();
		}
		return _playerDash;
	}

	private void ChangeWeapon(WeaponInstance weapon)
	{
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].gameObject.SetActive(value: true);
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeWeaponVisuals(weapon);
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeWeapon(weapon);
	}

	[Command("player.shield.change", Platform.AllPlatforms, MonoTargetType.Single)]
	public void SetShieldForVisual(int itemId)
	{
		ChangeShield(GameDatabaseHelper.GetItems().FirstOrDefault((ItemSO x) => x.Id == itemId && x.ItemSubtype == Enums.PlaceableItemSubtype.Shield));
	}

	private void ChangeShield(ItemSO shield)
	{
		if (!(shield == null))
		{
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeShieldVisuals(shield);
		}
	}

	[Command("player.armor.change", Platform.AllPlatforms, MonoTargetType.Single)]
	public void EquipArmor(int itemId)
	{
		ChangeArmor(GameDatabaseHelper.GetItems().FirstOrDefault((ItemSO x) => x.Id == itemId && x.ItemSubtype == Enums.PlaceableItemSubtype.BodyArmor));
	}

	private void ChangeArmor(ItemSO armor)
	{
		if (armor == null)
		{
			Debug.LogWarning("Armor not found!");
		}
		else
		{
			_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeArmorVisuals(armor);
		}
	}

	[Command("player.helmet.change", Platform.AllPlatforms, MonoTargetType.Single)]
	public void EquipHelmet(int itemId)
	{
		ChangeHelmet(GameDatabaseHelper.GetItems().FirstOrDefault((ItemSO x) => x.Id == itemId && x.ItemSubtype == Enums.PlaceableItemSubtype.Headwear));
	}

	private void ChangeHelmet(ItemSO armor)
	{
		if (armor == null)
		{
			Debug.LogWarning("Helmet not found!");
		}
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeHelmetVisuals(armor, this);
	}

	[Command("player.gloves.change", Platform.AllPlatforms, MonoTargetType.Single)]
	public void EquipGloves(int itemId)
	{
		ChangeGloves(GameDatabaseHelper.GetItems().FirstOrDefault((ItemSO x) => x.Id == itemId && x.ItemSubtype == Enums.PlaceableItemSubtype.Gloves));
	}

	private void ChangeGloves(ItemSO gloves)
	{
		if (gloves == null)
		{
			Debug.LogWarning("Gloves not found!");
		}
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeGlovesVisuals(gloves);
	}

	[Command("player.boots.change", Platform.AllPlatforms, MonoTargetType.Single)]
	public void EquipBoots(int itemId)
	{
		ChangeBoots(GameDatabaseHelper.GetItems().FirstOrDefault((ItemSO x) => x.Id == itemId && x.ItemSubtype == Enums.PlaceableItemSubtype.Boots));
	}

	private void ChangeBoots(ItemSO boots)
	{
		if (boots == null)
		{
			Debug.LogWarning("Gloves not found!");
		}
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ChangeBootsVisuals(boots);
	}

	[Command("player.animation.dead", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void CharacterDied()
	{
		bool num = SingletonController<ReviveController>.Instance.AvailableRevives > 0;
		_killedVfx[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowVfx();
		SingletonController<AudioController>.Instance.PlaySFXClip(_killedAudioClip, 1f);
		if (!num)
		{
			base.CharacterDied();
			PlayDeathAudio();
			RaiseCharacterKilledEvent();
			SingletonController<GameController>.Instance.PlayerDied();
			return;
		}
		Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
		foreach (Enemy obj in array)
		{
			obj.EnemyMovement.StopMovement();
			obj.EnemyMovement.PreventMovement();
			obj.Knockback(Transform, 0.5f);
		}
		SetImmunityForDuration(Enums.ImmunitySource.DamageTaken, 3f);
		PlayDeathAudio();
		SingletonController<GameController>.Instance.PlayerDiedButHasRevives();
	}

	public override void CharacterDamaged(float damageTaken, bool shouldTriggerAudioOnDamage)
	{
		base.CharacterDamaged(damageTaken, shouldTriggerAudioOnDamage);
		if (damageTaken > 0f)
		{
			if (shouldTriggerAudioOnDamage)
			{
				if (damageTaken * 0.1f > base.HealthSystem.GetHealth())
				{
					_hitStop.Stop(0.1f);
					PlayHitAudio();
				}
				else
				{
					PlayWeakHitAudio();
				}
			}
			PlayHitAnimation();
		}
		else if (_damageBlockedTimer > 0.5f)
		{
			_damageBlockedTimer = 0f;
			PlayDamagePreventedAudio();
			PlayDamageBlockedAnimation();
			DamageVisualizer.ShowTextPopup("Blocked", Constants.Colors.PlayerBlockedDamageColor);
		}
	}

	internal void PreventDamageAndDying()
	{
		base.HealthSystem.PreventDamageAndDying();
	}

	internal void SwitchToIdleAnimation()
	{
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ForceAnimationReset();
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowIdleAnimation();
	}

	internal void SwitchToSuccesAnimation()
	{
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowSuccesAnimation();
	}

	public void StopAllMovement()
	{
		_playerMovement.StopAllMovement();
		IsMoving = false;
	}

	private void PlayDamagePreventedAudio()
	{
		if (_canPlayDamagePreventedAudio && BaseCharacter.OnDamagePreventedAudio.Length != 0)
		{
			StartCoroutine(DisableDamagePreventedAudio(1f));
			int num = UnityEngine.Random.Range(0, BaseCharacter.OnDamagePreventedAudio.Length - 1);
			SingletonController<AudioController>.Instance.PlaySFXClip(BaseCharacter.OnDamagePreventedAudio[num], 0.5f);
		}
	}

	private IEnumerator DisableDamagePreventedAudio(float duration)
	{
		_canPlayDamagePreventedAudio = false;
		yield return new WaitForSeconds(duration);
		_canPlayDamagePreventedAudio = true;
	}

	private void PlayHitAudio()
	{
		if (BaseCharacter.OnHitAudio.Length != 0)
		{
			int num = UnityEngine.Random.Range(0, BaseCharacter.OnHitAudio.Length);
			SingletonController<AudioController>.Instance.PlaySFXClip(BaseCharacter.OnHitAudio[num], 1f, 0f, AudioController.GetPitchVariation());
			if (BaseCharacter.OnHitDamageAudio.Length != 0)
			{
				UnityEngine.Random.Range(0, BaseCharacter.OnHitDamageAudio.Length);
				SingletonController<AudioController>.Instance.PlaySFXClip(BaseCharacter.OnHitDamageAudio[num], 1f);
			}
		}
	}

	private void PlayWeakHitAudio()
	{
		if (BaseCharacter.OnWeakHitAudio.Length != 0)
		{
			int num = UnityEngine.Random.Range(0, BaseCharacter.OnWeakHitAudio.Length);
			SingletonController<AudioController>.Instance.PlaySFXClip(BaseCharacter.OnWeakHitAudio[num], 0.5f);
			if (BaseCharacter.OnHitDamageAudio.Length != 0)
			{
				UnityEngine.Random.Range(0, BaseCharacter.OnHitDamageAudio.Length);
				SingletonController<AudioController>.Instance.PlaySFXClip(BaseCharacter.OnHitDamageAudio[num], 0.5f);
			}
		}
	}

	private void PlayDeathAudio()
	{
		SingletonController<AudioController>.Instance.PlaySFXClip(BaseCharacter.OnDeathAudio, 1f);
	}

	[Command("player.kill", Platform.AllPlatforms, MonoTargetType.Single)]
	public void DEBUG_Kill()
	{
		SetCanAct(canAct: false);
		base.HealthSystem.Damage(999999f, shouldTriggerAudioOnDamage: true);
	}

	[Command("player.animation.dohit", Platform.AllPlatforms, MonoTargetType.Single)]
	public void PlayHitAnimation()
	{
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowHitAnimation();
	}

	[Command("player.animation.damageBlocked", Platform.AllPlatforms, MonoTargetType.Single)]
	public void PlayDamageBlockedAnimation()
	{
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowDamageBlockedEffect();
	}

	[Command("player.animation.Revive", Platform.AllPlatforms, MonoTargetType.Single)]
	public void Revive()
	{
		ResetDeathHasTriggered();
		Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
		foreach (Enemy obj in array)
		{
			obj.EnemyMovement.PreventMovement();
			obj.EnemyMovement.StopMovement();
			obj.SetCanAct(canAct: false);
		}
		_playerMovement.LockPosition();
		StopMovement();
		SetCanAct(canAct: false);
		ContinueMovement();
		SetImmunityForDuration(Enums.ImmunitySource.Reviving, 4f);
		if (base.HealthSystem != null)
		{
			base.HealthSystem.HealComplete();
		}
		_playerSprite[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetActive(value: true);
		RefreshVisuals();
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowReviveAnimation();
		SingletonController<AudioController>.Instance.PlaySFXClip(_revivingAudioClip, 1f);
		UpdateHealthRegen();
		StartCoroutine(ReviveAsync());
	}

	private IEnumerator ReviveAsync()
	{
		yield return new WaitForSeconds(1f);
		DamageVisualizer.ShowTextPopup("Revived!", Constants.Colors.PositiveEffectColor, 2f);
		yield return new WaitForSeconds(1f);
		Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
		foreach (Enemy enemy in array)
		{
			enemy.EnemyMovement.StopMovement();
			if (Vector2.Distance(enemy.transform.position, base.transform.position) < 8f)
			{
				enemy.Knockback(Transform, 0.5f);
				Enums.DamageType damageType = Enums.DamageType.Holy;
				switch (BaseCharacter.Character)
				{
				case Enums.CharacterClass.Warrior:
					damageType = Enums.DamageType.Holy;
					break;
				case Enums.CharacterClass.Mage:
					damageType = Enums.DamageType.Fire;
					break;
				case Enums.CharacterClass.Rogue:
					damageType = Enums.DamageType.Poison;
					break;
				case Enums.CharacterClass.Necromancer:
					damageType = Enums.DamageType.Cold;
					break;
				}
				enemy.Damage(100f, wasCrit: true, base.gameObject, 0f, null, damageType);
			}
		}
		yield return new WaitForSeconds(1f);
		SwitchToIdleAnimation();
		SetCanAct(canAct: true);
		StopMoveToPosition();
		RaiseCharacterRevivedEvent();
		_playerMovement.UnlockPosition();
		yield return new WaitForSeconds(1f);
		array = UnityEngine.Object.FindObjectsOfType<Enemy>();
		foreach (Enemy obj in array)
		{
			obj.EnemyMovement.AllowMovement();
			obj.SetCanAct(canAct: true);
		}
	}

	[Command("player.animation.Spawn", Platform.AllPlatforms, MonoTargetType.Single)]
	public void Spawn(bool teleportIn = true)
	{
		SingletonController<InputController>.Instance.CanCancel = false;
		StartCoroutine(EnableCancelInputAfterDelay(1f));
		if (base.HealthSystem != null)
		{
			base.HealthSystem.HealComplete();
			SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: true);
		}
		_playerSprite[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].SetActive(value: true);
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ResetWeapon();
		if (teleportIn)
		{
			_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowSpawnAnimation();
			SingletonController<AudioController>.Instance.PlaySFXClip(_spawningAudioClip, 0.5f);
		}
		else
		{
			_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowIdleAnimation();
			FinishSpawn();
		}
		_playerDash.UpdateTotalDashCount();
	}

	private IEnumerator EnableCancelInputAfterDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		SingletonController<InputController>.Instance.CanCancel = true;
	}

	private void FinishSpawn()
	{
		if (SingletonController<SceneChangeController>.Instance.CurrentSceneName.Contains("4. Town"))
		{
			SetCanAct(canAct: true);
		}
		RefreshVisuals();
	}

	[Command("player.animation.Despawn", Platform.AllPlatforms, MonoTargetType.Single)]
	public void Despawn()
	{
		SetCanAct(canAct: false);
		_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowDespawnAnimation();
		SingletonController<AudioController>.Instance.PlaySFXClip(_despawningAudioClip, 1f);
	}

	public override void OnLifeDrain()
	{
		base.OnLifeDrain();
		_playerVisualController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowVisual(_lifedrainFXPrefab, 1, 0f, 1f);
	}

	internal override bool Damage(float damage, bool wasCrit, GameObject source, float knockbackPower, WeaponSO weaponSource, Enums.DamageType damageType, Character damageSource = null)
	{
		_lastDamageSource = damageSource;
		bool result = base.Damage(damage, wasCrit, source, knockbackPower, weaponSource, damageType, damageSource);
		DamageVisualizer.ShowDamagePopup(damage, wasCrit, Enums.CharacterType.Player, damageType);
		if (knockbackPower > 0f)
		{
			Knockback(source.transform, knockbackPower);
		}
		return result;
	}

	internal void SetCanDash(bool canDash)
	{
		GetPlayerDash().SetCanDash(canDash);
	}

	public void Knockback(Transform sourceOfKnockback, float knockbackPower)
	{
		knockbackFeedback.PlayFeedback(sourceOfKnockback, knockbackPower, 0f);
	}

	internal void MoveToPosition(Transform infrontOfGatePosition)
	{
		_playerMovement.MoveToPosition(infrontOfGatePosition);
	}

	internal void ContinueMovement()
	{
		_playerMovement.ContinueMovement();
	}

	internal void StopMovement()
	{
		_playerMovement.StopAllMovement();
	}

	internal void MoveToPositionInstant(Transform infrontOfGatePosition)
	{
		base.transform.position = infrontOfGatePosition.position;
	}

	internal void StopMoveToPosition()
	{
		_playerMovement.StopMoveToPosition();
	}

	internal bool CheckForDodge()
	{
		return RandomHelper.GetRollSuccess(GetCalculatedStat(Enums.ItemStatType.DodgePercentage));
	}

	internal void Dodged()
	{
		DamageVisualizer.ShowTextPopup("Dodged!", Constants.Colors.PlayerDodgedAttackColor);
		SingletonController<AudioController>.Instance.PlaySFXClip(_dodgedAudioClip, 1f, 0.2f, 2f);
	}
}
