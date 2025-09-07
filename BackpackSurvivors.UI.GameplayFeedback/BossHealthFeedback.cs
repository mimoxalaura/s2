using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Debuffs;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.System.Helper;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class BossHealthFeedback : MonoBehaviour
{
	[SerializeField]
	private Image _bossSkullImage;

	[SerializeField]
	private Image _bossHealthBar;

	[SerializeField]
	private Image _bossHealthBackdropBar;

	[SerializeField]
	private Image _bossHealthMask;

	[SerializeField]
	private Image _bossHealthBorder;

	[SerializeField]
	private Image _bossHealthBarBack;

	[SerializeField]
	private Image _bossHealthBarBeautifyTop;

	[SerializeField]
	private Image _bossHealthBarBeautifyBottom;

	[SerializeField]
	private TextMeshProUGUI _bossHealthText;

	[Header("Debuff")]
	[SerializeField]
	private DebuffVisualContainer _debuffVisualContainer;

	private Enemy _boss;

	private float _totalBossHealth;

	private float _prevFillAmountBack;

	private float _targetFillAmountBack;

	private List<BuffVisualUIItem> _buffVisualItems;

	private List<DebuffVisualItem> _debuffVisualItems;

	[Command("BossHealth.UI.Init", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void Init()
	{
		_bossHealthBackdropBar.color = new Color(_bossHealthBackdropBar.color.r, _bossHealthBackdropBar.color.g, _bossHealthBackdropBar.color.b, 0f);
		_bossHealthBarBack.color = new Color(_bossHealthBarBack.color.r, _bossHealthBarBack.color.g, _bossHealthBarBack.color.b, 0f);
		_bossHealthBar.color = new Color(_bossHealthBar.color.r, _bossHealthBar.color.g, _bossHealthBar.color.b, 0f);
		_bossHealthBorder.color = new Color(_bossHealthBorder.color.r, _bossHealthBorder.color.g, _bossHealthBorder.color.b, 0f);
		_bossHealthMask.color = new Color(_bossHealthMask.color.r, _bossHealthMask.color.g, _bossHealthMask.color.b, 0f);
		_bossHealthBarBeautifyTop.transform.localScale = new Vector3(0f, 0f, 0f);
		_bossHealthBarBeautifyBottom.transform.localScale = new Vector3(0f, 0f, 0f);
		_bossHealthText.color = new Color(_bossHealthText.color.r, _bossHealthText.color.g, _bossHealthText.color.b, 0f);
		_bossSkullImage.color = new Color(_bossSkullImage.color.r, _bossSkullImage.color.g, _bossSkullImage.color.b, 0f);
		_bossSkullImage.transform.localScale = new Vector3(10f, 10f, 10f);
		base.gameObject.SetActive(value: true);
		_bossHealthText.gameObject.SetActive(value: true);
	}

	internal void ShowHealthbar()
	{
		Init();
		float num = 1f;
		float time = 0.3f;
		LeanTween.value(_bossHealthBackdropBar.gameObject, delegate(float val)
		{
			LeanTweenHelper.ChangeTransparency(_bossHealthBackdropBar, val);
		}, 0f, 1f, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_bossHealthBarBack.gameObject, delegate(float val)
		{
			LeanTweenHelper.ChangeTransparency(_bossHealthBarBack, val);
		}, 0f, 1f, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_bossHealthBar.gameObject, delegate(float val)
		{
			LeanTweenHelper.ChangeTransparency(_bossHealthBar, val);
		}, 0f, 1f, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_bossHealthBorder.gameObject, delegate(float val)
		{
			LeanTweenHelper.ChangeTransparency(_bossHealthBorder, val);
		}, 0f, 1f, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_bossHealthMask.gameObject, delegate(float val)
		{
			LeanTweenHelper.ChangeTransparency(_bossHealthMask, val);
		}, 0f, 1f, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_bossHealthBarBeautifyTop.gameObject, new Vector3(1f, 1f, 1f), time).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_bossHealthBarBeautifyBottom.gameObject, new Vector3(1f, 1f, 1f), time).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_bossHealthText.gameObject, delegate(float val)
		{
			LeanTweenHelper.ChangeTransparency(_bossHealthText, val);
		}, 0f, 1f, time).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_bossSkullImage.gameObject, new Vector3(0.55f, 0.55f, 0.55f), time).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_bossSkullImage.gameObject, delegate(float val)
		{
			LeanTweenHelper.ChangeTransparency(_bossSkullImage, val);
		}, 0f, 1f, time).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void SetupBossHealth(Enemy boss, LevelSO level)
	{
		_boss = boss;
		_bossSkullImage.sprite = level.BossIconSkull;
		_totalBossHealth = _boss.HealthSystem.GetHealth();
		_bossHealthBar.fillAmount = 1f;
		_bossHealthBarBack.fillAmount = 1f;
		SetBossHealthText(_totalBossHealth);
		_boss.OnHealthChanged += Boss_OnHealthChanged;
		_boss.OnBuffApplied += _boss_OnBuffApplied;
		_buffVisualItems = new List<BuffVisualUIItem>();
		_debuffVisualContainer.SetDebuffContainer(_boss.GetDebuffContainer());
	}

	private void _boss_OnBuffApplied(object sender, BuffAppliedEventArgs e)
	{
	}

	private void SetBossHealthText(float currentBossHealth)
	{
		if (currentBossHealth >= 0f)
		{
			string sourceText = $"{_boss.BaseEnemy.Name} ({Mathf.CeilToInt(currentBossHealth)} / {Mathf.CeilToInt(_totalBossHealth)})";
			_bossHealthText.SetText(sourceText);
		}
		else
		{
			string sourceText2 = _boss.BaseEnemy.Name + " (KILLED)";
			_bossHealthText.SetText(sourceText2);
		}
	}

	private void Boss_OnHealthChanged(object sender, EventArgs e)
	{
		float health = _boss.HealthSystem.GetHealth();
		float fillAmount = health / _totalBossHealth;
		LeanTween.cancel(_bossHealthBar.gameObject);
		LeanTween.cancel(_bossHealthBarBack.gameObject);
		_bossHealthBar.fillAmount = fillAmount;
		_bossHealthBackdropBar.fillAmount = fillAmount;
		_prevFillAmountBack = _bossHealthBarBack.fillAmount;
		_targetFillAmountBack = _bossHealthBar.fillAmount;
		_bossHealthBarBack.color = Color.yellow;
		LeanTween.value(_bossHealthBarBack.gameObject, delegate(float val)
		{
			_bossHealthBarBack.fillAmount = val;
		}, _prevFillAmountBack, _targetFillAmountBack, 0.8f).setDelay(0.8f).setIgnoreTimeScale(useUnScaledTime: true);
		SetBossHealthText(health);
	}
}
