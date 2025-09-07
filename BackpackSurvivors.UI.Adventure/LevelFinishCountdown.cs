using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class LevelFinishCountdown : MonoBehaviour
{
	internal delegate void CountdownCompletedHandler(object sender, EventArgs e);

	[SerializeField]
	private Image _timelineBar;

	[SerializeField]
	private Image _timelineBarBackdrop;

	[SerializeField]
	private Image _timelineBarOverlay;

	[SerializeField]
	private Image _timelineEnder;

	[SerializeField]
	private Transform _timelineEnderTarget;

	[SerializeField]
	private TextMeshProUGUI _timelineText;

	private float _timeRemaining = 10f;

	private bool _startTimer;

	internal event CountdownCompletedHandler OnCountdownCompleted;

	public void StartCountDown(float time)
	{
		_timeRemaining = time;
		_timelineText.SetText(time.ToString());
		_timelineText.gameObject.SetActive(value: true);
		_timelineBar.gameObject.SetActive(value: true);
		_timelineEnder.gameObject.SetActive(value: true);
		_timelineBarBackdrop.gameObject.SetActive(value: true);
		_timelineBarOverlay.gameObject.SetActive(value: true);
		LeanTween.scaleY(_timelineBar.gameObject, 1f, 1f);
		LeanTween.scaleY(_timelineBarBackdrop.gameObject, 1f, 1f);
		LeanTween.scaleY(_timelineBarOverlay.gameObject, 1f, 1f);
		LeanTween.scaleY(_timelineEnder.gameObject, 1f, 1f);
		LeanTween.value(_timelineBar.gameObject, unfillTimelineBar, 1f, 0f, time);
		LeanTween.moveLocalX(_timelineEnder.gameObject, _timelineEnderTarget.localPosition.x, time);
		_startTimer = true;
	}

	private void Update()
	{
		if (_startTimer)
		{
			if (_startTimer && _timeRemaining > 0f)
			{
				_timeRemaining -= Time.deltaTime;
				_timelineText.SetText($"{_timeRemaining:0.00}s");
			}
			else if (_timeRemaining <= 0f)
			{
				_startTimer = false;
				FinishCountDown();
			}
		}
	}

	private void FinishCountDown()
	{
		_timelineText.gameObject.SetActive(value: false);
		_timelineEnder.gameObject.SetActive(value: false);
		LeanTween.scaleY(_timelineBar.gameObject, 0f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scaleY(_timelineBarBackdrop.gameObject, 0f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scaleY(_timelineBarOverlay.gameObject, 0f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scaleY(_timelineEnder.gameObject, 0f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		this.OnCountdownCompleted?.Invoke(this, null);
	}

	private void unfillTimelineBar(float val)
	{
		_timelineBar.fillAmount = val;
	}
}
