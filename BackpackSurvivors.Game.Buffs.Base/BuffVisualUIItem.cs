using System;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Buffs.Base;

public class BuffVisualUIItem : MonoBehaviour
{
	[SerializeField]
	private Image _backDropImage;

	[SerializeField]
	private Image _foreDropImage;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private TextMeshProUGUI _stackText;

	[SerializeField]
	private BuffTooltipTrigger _tooltipTrigger;

	private BuffSO _buffSO;

	private BuffHandler _buffHandler;

	private float _startTime;

	private float _currentTime;

	private int _stacks;

	internal int Stacks => _stacks;

	public int BuffId => _buffSO.Id;

	private void Start()
	{
		if (!_buffSO.TooltipShowsDuration)
		{
			_text.SetText(string.Empty);
		}
	}

	private void Update()
	{
		_currentTime -= Time.deltaTime;
		_foreDropImage.fillAmount = _currentTime / _startTime;
		if (_buffSO.TooltipShowsDuration)
		{
			_text.SetText(((int)Math.Ceiling(_currentTime)).ToString());
		}
		_tooltipTrigger.SetRemainingTime(_currentTime);
	}

	public void SetBuff(BuffHandler buffHandler)
	{
		_buffSO = buffHandler.BuffSO;
		_buffHandler = buffHandler;
		_backDropImage.sprite = _buffSO.Icon;
		_foreDropImage.sprite = _buffSO.Icon;
		_startTime = _buffHandler.TimeUntillFalloff;
		_currentTime = _buffHandler.TimeUntillFalloff;
		_tooltipTrigger.SetBuff(_buffSO);
		_stacks = 1;
	}

	public void UpdateStackCountStack(int stackCount)
	{
		if (stackCount <= 1)
		{
			_stackText.SetText(string.Empty);
			return;
		}
		_stacks = stackCount;
		_stackText.SetText($"<size=12>x</size>{stackCount}");
	}

	public void AddStack()
	{
		_currentTime = _buffHandler.TimeUntillFalloff;
		_stacks++;
		UpdateStackCountStack(_stacks);
	}

	public void RemoveStack()
	{
		_stacks--;
		UpdateStackCountStack(_stacks);
	}
}
