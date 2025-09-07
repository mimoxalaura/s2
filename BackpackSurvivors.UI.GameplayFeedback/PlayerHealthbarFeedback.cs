using System;
using System.Collections;
using System.Linq;
using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Debuffs;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Health.Events;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using QFSW.QC;
using TMPro;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class PlayerHealthbarFeedback : MonoBehaviour
{
	[Header("Base")]
	[SerializeField]
	private Image _characterImage;

	[SerializeField]
	private SceneReference[] _scenesToShowExperienceIn;

	[SerializeField]
	private GameObject _experienceBarContainer;

	[Header("Healthbar")]
	[SerializeField]
	private Image _sliderBar;

	[SerializeField]
	private Image _sliderBarBack;

	[SerializeField]
	private TextMeshProUGUI _healthText;

	[SerializeField]
	private float _chipSpeed;

	[Header("Experience")]
	[SerializeField]
	private Image _experienceSliderBar;

	[SerializeField]
	private TextMeshProUGUI _experienceText;

	[SerializeField]
	private TextMeshProUGUI _levelText;

	[Header("Buffs")]
	[SerializeField]
	private BuffVisualUIContainer _buffVisualUIContainer;

	[Header("Debuff")]
	[SerializeField]
	private DebuffVisualContainer _debuffVisualContainer;

	[Header("Damage numbers")]
	[SerializeField]
	private TextMeshProUGUI _damageTakenNumber;

	[SerializeField]
	private TextMeshProUGUI _healthGainedNumber;

	private Player _player;

	private TimeBasedWaveController _waveController;

	private float _lerpHealthBarTimer;

	private int _healthGained;

	private int _healthLost;

	private void Start()
	{
		RegisterEvents();
		Init();
	}

	private void Init()
	{
		_player = SingletonController<GameController>.Instance.Player;
		_buffVisualUIContainer.SetBuffController(_player.GetComponent<BuffController>());
		SetExperienceBarVisible();
	}

	private void SetExperienceBarVisible()
	{
		Scene scene = SceneManager.GetActiveScene();
		bool active = _scenesToShowExperienceIn.Any((SceneReference sc) => sc.ScenePath == scene.path);
		_experienceBarContainer.SetActive(active);
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, RegisterCharactersControllerInitialized);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameControllerInitialized);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<EventController>.Instance, RegisterEventControllerInitialized);
	}

	private void RegisterEventControllerInitialized()
	{
		SingletonController<EventController>.Instance.OnPlayerHealthChanged += EventController_OnPlayerHealthChanged;
		SingletonController<EventController>.Instance.OnPlayerHealthMaxChanged += EventController_OnPlayerHealthMaxChanged;
	}

	private void EventController_OnPlayerHealthMaxChanged(object sender, HealthMaxChangedEventArgs e)
	{
		if (e.HealthMaxChanged)
		{
			UpdateHealthVisuals(Mathf.CeilToInt(_player.HealthSystem.GetHealth()), (int)_player.HealthSystem.GetHealthMax(), 0f);
		}
	}

	private void EventController_OnPlayerHealthChanged(object sender, HealthChangedEventArgs e)
	{
		float num = 0f;
		if (e.HealthDidChange)
		{
			num = e.NewHealth - e.OldHealth;
			if (num > 0f)
			{
				ShowHealthGained(num);
			}
			else
			{
				ShowHealthLost(num);
			}
		}
		UpdateHealthVisuals(Mathf.CeilToInt(_player.HealthSystem.GetHealth()), (int)_player.HealthSystem.GetHealthMax(), num);
	}

	[Command("PlayerHealthUI.Gained", Platform.AllPlatforms, MonoTargetType.Single)]
	private void ShowHealthGained(float health)
	{
		if (base.isActiveAndEnabled)
		{
			_healthGained = (int)health;
			StopAllCoroutines();
			_damageTakenNumber.gameObject.SetActive(value: false);
			_healthGainedNumber.SetText($"<color={ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase)}>+{_healthGained}</color>");
			_healthGainedNumber.color = new Color(255f, 255f, 255f, 0f);
			_healthGainedNumber.gameObject.SetActive(value: true);
			StartCoroutine(ShowHealthGainedAsync());
		}
	}

	[Command("PlayerHealthUI.Lost", Platform.AllPlatforms, MonoTargetType.Single)]
	private void ShowHealthLost(float health)
	{
		if (base.isActiveAndEnabled)
		{
			_healthLost = (int)health;
			StopAllCoroutines();
			_healthGainedNumber.gameObject.SetActive(value: false);
			_damageTakenNumber.SetText($"<color={ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase)}>{_healthLost}</color>");
			_damageTakenNumber.color = new Color(255f, 255f, 255f, 0f);
			_damageTakenNumber.gameObject.SetActive(value: true);
			StartCoroutine(ShowHealthLostAsync());
		}
	}

	private IEnumerator ShowHealthGainedAsync()
	{
		LeanTween.scale(_healthGainedNumber.gameObject, Vector3.one, 0.3f);
		LeanTween.value(_healthGainedNumber.gameObject, delegate(float val)
		{
			_healthGainedNumber.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, 0.3f);
		LeanTween.value(_healthGainedNumber.gameObject, delegate(float val)
		{
			_healthGainedNumber.color = new Color(255f, 255f, 255f, val);
		}, 1f, 0f, 1f).setDelay(0.3f);
		yield return new WaitForSeconds(1.3f);
		_healthGainedNumber.gameObject.SetActive(value: false);
	}

	private IEnumerator ShowHealthLostAsync()
	{
		LeanTween.scale(_damageTakenNumber.gameObject, Vector3.one, 0.3f);
		LeanTween.value(_damageTakenNumber.gameObject, delegate(float val)
		{
			_damageTakenNumber.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, 0.3f);
		LeanTween.value(_damageTakenNumber.gameObject, delegate(float val)
		{
			_damageTakenNumber.color = new Color(255f, 255f, 255f, val);
		}, 1f, 0f, 1f).setDelay(0.3f);
		yield return new WaitForSeconds(1.3f);
		_damageTakenNumber.gameObject.SetActive(value: false);
	}

	public void RegisterCharactersControllerInitialized()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<GameController>.Instance.Player, RegisterPlayerInitialized);
		UpdateCharacterVisuals(SingletonController<CharactersController>.Instance.ActiveCharacter);
	}

	private void RegisterPlayerInitialized()
	{
		SingletonController<CharactersController>.Instance.OnCharacterLoaded += CharactersController_OnCharacterLoaded;
		SingletonController<CharactersController>.Instance.OnCharacterSwitched += CharactersController_OnCharacterSwitched;
		SingletonController<CharactersController>.Instance.OnExperienceGained += CharactersController_OnExperienceGained;
		UpdateExperienceVisuals(SingletonController<CharactersController>.Instance.CurrentExperience, SingletonController<CharactersController>.Instance.ExperienceToNext, SingletonController<CharactersController>.Instance.CurrentLevel);
	}

	public void RegisterSaveGameControllerInitialized()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		UpdateExperienceVisuals(SingletonController<CharactersController>.Instance.CurrentExperience, SingletonController<CharactersController>.Instance.ExperienceToNext, SingletonController<CharactersController>.Instance.CurrentLevel);
	}

	private void CharactersController_OnCharacterSwitched(object sender, CharacterSwitchedEventArgs e)
	{
		_player = SingletonController<GameController>.Instance.Player;
		UpdateHealthVisuals(_player.HealthSystem.GetHealth(), _player.HealthSystem.GetHealthMax(), 0f);
		UpdateExperienceVisuals(SingletonController<CharactersController>.Instance.CurrentExperience, SingletonController<CharactersController>.Instance.ExperienceToNext, SingletonController<CharactersController>.Instance.CurrentLevel);
		UpdateCharacterVisuals(SingletonController<CharactersController>.Instance.ActiveCharacter);
	}

	private void CharactersController_OnCharacterLoaded(object sender, CharacterSwitchedEventArgs e)
	{
		_player = SingletonController<GameController>.Instance.Player;
		UpdateExperienceVisuals(SingletonController<CharactersController>.Instance.CurrentExperience, SingletonController<CharactersController>.Instance.ExperienceToNext, SingletonController<CharactersController>.Instance.CurrentLevel);
	}

	private void CharactersController_OnExperienceGained(object sender, ExperienceGainedEventArgs e)
	{
		UpdateExperienceVisuals(e.CurrentExp, e.MaxExpTillLevel, e.CurrentLevel);
	}

	public void UpdateCharacterVisuals(CharacterSO characterSO)
	{
		_characterImage.sprite = characterSO.Face;
	}

	public void UpdateHealthVisuals(float currentHP, float maxHP, float diff)
	{
		bool fillingHealth = false;
		bool droppingHealth = false;
		float num = 0f;
		float num2 = 0f;
		LeanTween.cancel(_sliderBar.gameObject);
		LeanTween.cancel(_sliderBarBack.gameObject);
		if (diff < 0f)
		{
			_ = fillingHealth;
			droppingHealth = true;
			_sliderBar.fillAmount = currentHP / maxHP;
			num = _sliderBarBack.fillAmount;
			num2 = _sliderBar.fillAmount;
			_sliderBarBack.color = Color.yellow;
			LeanTween.value(_sliderBarBack.gameObject, delegate(float val)
			{
				_sliderBarBack.fillAmount = val;
			}, num, num2, 0.8f).setOnComplete((Action)delegate
			{
				droppingHealth = false;
			}).setDelay(0.8f);
		}
		else
		{
			_ = droppingHealth;
			fillingHealth = true;
			_sliderBarBack.color = Color.green;
			num = _sliderBar.fillAmount;
			num2 = currentHP / maxHP;
			_sliderBarBack.fillAmount = num2;
			LeanTween.value(_sliderBar.gameObject, delegate(float val)
			{
				_sliderBar.fillAmount = val;
			}, num, num2, 0.8f).setOnComplete((Action)delegate
			{
				fillingHealth = false;
			}).setDelay(0.8f);
		}
		_healthText.SetText($"{(int)currentHP}/{(int)maxHP}");
	}

	private void UpdateExperienceVisuals(float currentExp, float maxExp, int currentLevel)
	{
		_experienceSliderBar.fillAmount = currentExp / maxExp;
		if (GameDatabase.IsDemo && currentLevel >= SingletonController<GameDatabase>.Instance.GameDatabaseSO.MaxLevel)
		{
			_experienceText.SetText($"(demo) max level {SingletonController<GameDatabase>.Instance.GameDatabaseSO.MaxLevel}");
		}
		else
		{
			_experienceText.SetText($"{(int)currentExp}/{(int)maxExp} exp");
		}
		_levelText.SetText($"lv. {currentLevel}");
	}

	private void OnDestroy()
	{
		SingletonController<CharactersController>.Instance.OnCharacterLoaded -= CharactersController_OnCharacterLoaded;
		SingletonController<CharactersController>.Instance.OnCharacterSwitched -= CharactersController_OnCharacterSwitched;
		SingletonController<CharactersController>.Instance.OnExperienceGained -= CharactersController_OnExperienceGained;
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded -= SaveGameController_OnSaveGameLoaded;
		SingletonController<EventController>.Instance.OnPlayerHealthChanged -= EventController_OnPlayerHealthChanged;
		SingletonController<EventController>.Instance.OnPlayerHealthMaxChanged -= EventController_OnPlayerHealthMaxChanged;
	}
}
