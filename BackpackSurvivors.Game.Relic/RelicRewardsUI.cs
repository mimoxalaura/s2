using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Relic;

public class RelicRewardsUI : ModalUI
{
	public delegate void RelicRewardPickedHandler(object sender, RelicRewardPickedEventArgs e);

	public delegate void RerollPressedHandler(object sender, EventArgs e);

	[SerializeField]
	private RelicRewardItemUI _relicRewardItemUIPrefab;

	[SerializeField]
	private Transform _relicRewardUIContainer;

	[SerializeField]
	private GameObject _bovehBackgroundVfx;

	[SerializeField]
	private Image _blackBackgroundImage;

	[SerializeField]
	private Image _relicSelectedFadeOut;

	[SerializeField]
	private TextMeshProUGUI _rerollButtonText;

	[SerializeField]
	private Button _rerollButton;

	[SerializeField]
	private Transform[] _relicPositions;

	[SerializeField]
	private Transform[] _relicIconPositions;

	[SerializeField]
	private Animator _titleAnimator;

	[SerializeField]
	private GameObject _buttonContainer;

	[SerializeField]
	private GameObject _vfxContainer;

	[SerializeField]
	private GameObject _vfxContainer2;

	[SerializeField]
	private GameObject _rerollButtonSelectedHighlight;

	[SerializeField]
	private Image _artImage1;

	[SerializeField]
	private Image _artImage2;

	private List<RelicRewardItemUI> _availableRelicRewards;

	private float _durationPerRelic = 0.7f;

	private float _durationPerRelicOnReroll = 0.5f;

	public event RelicRewardPickedHandler OnRelicRewardPicked;

	public event RerollPressedHandler OnRerollClicked;

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.OpenUI(openDirection);
		SingletonController<InputController>.Instance.OnAcceptHandler += Instance_OnAcceptHandler;
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.CloseUI(openDirection);
		SingletonController<InputController>.Instance.CanCancel = true;
		SingletonController<InputController>.Instance.OnAcceptHandler -= Instance_OnAcceptHandler;
	}

	private void Instance_OnAcceptHandler(object sender, EventArgs e)
	{
		RerollRelics();
	}

	public override void AfterOpenUI()
	{
		base.AfterOpenUI();
		SetRerollButtonState();
		_bovehBackgroundVfx.SetActive(value: true);
	}

	public override void AfterCloseUI()
	{
		base.AfterCloseUI();
		_bovehBackgroundVfx.SetActive(value: false);
	}

	public void Init(List<Relic> relics)
	{
		StartCoroutine(InitAsync(relics));
	}

	public void RerollToGivenList(List<Relic> relics)
	{
		StartCoroutine(SpawnRelicsAsync(relics, isReroll: true));
	}

	private IEnumerator InitAsync(List<Relic> relics)
	{
		float delayForTitle = 1f;
		float time = 1f;
		_rerollButton.interactable = false;
		_blackBackgroundImage.color = new Color(0f, 0f, 0f, 0f);
		_artImage1.color = new Color(255f, 255f, 255f, 0f);
		_artImage2.color = new Color(255f, 255f, 255f, 0f);
		_vfxContainer.transform.localScale = Vector3.zero;
		_vfxContainer.SetActive(value: true);
		_vfxContainer2.transform.localScale = Vector3.zero;
		_vfxContainer2.SetActive(value: true);
		_buttonContainer.transform.localScale = Vector3.zero;
		LeanTween.value(_blackBackgroundImage.gameObject, delegate(float val)
		{
			_blackBackgroundImage.color = new Color(0f, 0f, 0f, val);
		}, 0f, 0.93f, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_artImage1.gameObject, delegate(float val)
		{
			_artImage1.color = new Color(255f, 255f, 255f, val);
		}, 0f, 0.05f, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_artImage2.gameObject, delegate(float val)
		{
			_artImage2.color = new Color(255f, 255f, 255f, val);
		}, 0f, 0.05f, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_vfxContainer, new Vector3(20f, 20f, 20f), time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_vfxContainer2, new Vector3(20f, 20f, 20f), time).setIgnoreTimeScale(useUnScaledTime: true);
		yield return new WaitForSecondsRealtime(time);
		_titleAnimator.SetTrigger("Show");
		yield return new WaitForSecondsRealtime(delayForTitle);
		StartCoroutine(SpawnRelicsAsync(relics, isReroll: false));
		yield return new WaitForSecondsRealtime(_durationPerRelic * (float)relics.Count);
		LeanTween.scale(_buttonContainer, Vector3.one, 0.3f).setEaseOutElastic().setIgnoreTimeScale(useUnScaledTime: true);
		_rerollButton.interactable = SingletonController<RelicsController>.Instance.RemainingRerolls > 0;
	}

	private IEnumerator SpawnRelicsAsync(List<Relic> relics, bool isReroll)
	{
		int index = 0;
		RemoveRelics(instant: true);
		_rerollButton.interactable = false;
		_availableRelicRewards = new List<RelicRewardItemUI>();
		foreach (Relic relic in relics)
		{
			RelicRewardItemUI relicRewardItemUI = UnityEngine.Object.Instantiate(_relicRewardItemUIPrefab, _relicRewardUIContainer);
			relicRewardItemUI.Init(relic);
			relicRewardItemUI.OnRelicRewardPicked += RelicRewardUI_OnRelicRewardPicked;
			relicRewardItemUI.Spawn(_relicPositions[index], _relicIconPositions[index], isReroll);
			index++;
			_availableRelicRewards.Add(relicRewardItemUI);
			if (isReroll)
			{
				yield return new WaitForSecondsRealtime(_durationPerRelicOnReroll);
			}
			else
			{
				yield return new WaitForSecondsRealtime(_durationPerRelic);
			}
		}
		_rerollButton.interactable = SingletonController<RelicsController>.Instance.RemainingRerolls > 0;
	}

	private void RelicRewardUI_OnRelicRewardPicked(object sender, RelicRewardPickedEventArgs e)
	{
		_rerollButton.interactable = false;
		_titleAnimator.SetTrigger("Hide");
		RemoveRelics(instant: false, animateDespawn: true);
		this.OnRelicRewardPicked?.Invoke(this, new RelicRewardPickedEventArgs(e.Relic));
	}

	public void RerollRelics()
	{
		if (_rerollButton.interactable && SingletonController<RelicsController>.Instance.RemainingRerolls > 0)
		{
			SingletonController<RelicsController>.Instance.RemainingRerolls--;
			_rerollButtonSelectedHighlight.SetActive(value: false);
			SetRerollButtonState();
			this.OnRerollClicked?.Invoke(this, new EventArgs());
		}
	}

	internal void SetRerollButtonState()
	{
		_rerollButtonText.SetText(SetRerollText());
		_rerollButton.interactable = SingletonController<RelicsController>.Instance.RemainingRerolls > 0;
		_rerollButtonText.color = (_rerollButton.interactable ? new Color(254f, 217f, 135f, 1f) : new Color(123f, 123f, 123f, 1f));
	}

	private string SetRerollText()
	{
		return $"{SingletonController<RelicsController>.Instance.RemainingRerolls}";
	}

	internal void RemoveRelics(bool instant, bool animateDespawn = false, float delay = 2f)
	{
		if (instant)
		{
			for (int num = _relicRewardUIContainer.childCount - 1; num >= 0; num--)
			{
				_relicRewardUIContainer.GetChild(num).GetComponent<RelicRewardItemUI>().Despawn();
			}
			return;
		}
		LeanTween.value(_relicSelectedFadeOut.gameObject, delegate(float val)
		{
			_relicSelectedFadeOut.fillAmount = val;
		}, 0f, 1f, 1f).setDelay(1f).setIgnoreTimeScale(useUnScaledTime: true);
		for (int num2 = 0; num2 < _relicRewardUIContainer.childCount; num2++)
		{
			_relicRewardUIContainer.GetChild(num2).GetComponent<RelicRewardItemUI>().Despawn(animateDespawn, delay);
		}
		LeanTween.value(_relicSelectedFadeOut.gameObject, delegate(float val)
		{
			_relicSelectedFadeOut.fillAmount = val;
		}, 1f, 0f, 1f).setDelay(3f).setIgnoreTimeScale(useUnScaledTime: true);
	}
}
