using System;
using System.Collections;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Stats;
using BackpackSurvivors.UI.Tooltip;
using UnityEngine;

namespace BackpackSurvivors.UI.Shared;

public class ModalUI : MonoBehaviour
{
	[SerializeField]
	private Enums.Modal.OpenDirection _openDirection;

	[SerializeField]
	private bool _bounceOnOpen;

	[SerializeField]
	private bool _playAudioOnOpen;

	[SerializeField]
	private bool _playAudioOnClose;

	[SerializeField]
	private bool _showBlackBackdropPanel = true;

	[SerializeField]
	private bool _blurBackground;

	[SerializeField]
	public bool PauseOnOpening = true;

	private ModalUiController _modalUiController;

	public bool IsOpen;

	public event Action OnAfterCloseUI;

	public event Action OnAfterOpenUI;

	public virtual void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		VisualOpen(openDirection);
		HandleBlackPanel(_showBlackBackdropPanel);
		if (_playAudioOnOpen)
		{
			SingletonController<AudioController>.Instance.PlayDefaultAudio(Enums.DefaultAudioType.UIOpen);
		}
		SingletonController<TooltipController>.Instance.Hide(null);
		StartCoroutine(DoAfterOpenUI(0.3f));
		IsOpen = true;
	}

	public virtual void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		VisualClose(openDirection);
		HandleBlackPanel(showBlackPanel: false);
		if (_playAudioOnClose)
		{
			SingletonController<AudioController>.Instance.PlayDefaultAudio(Enums.DefaultAudioType.UIClose);
		}
		SingletonController<TooltipController>.Instance.Hide(null);
		if (base.isActiveAndEnabled)
		{
			StartCoroutine(DoAfterCloseUI(0.1f));
		}
		IsOpen = false;
	}

	private void VisualOpen(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.None)
	{
		if (_openDirection == Enums.Modal.OpenDirection.UseGiven)
		{
			HandleVisualOpen(openDirection);
		}
		else
		{
			HandleVisualOpen(_openDirection);
		}
	}

	private void HandleVisualOpen(Enums.Modal.OpenDirection openDirection)
	{
		LeanTween.cancel(base.gameObject);
		switch (openDirection)
		{
		case Enums.Modal.OpenDirection.Horizontal:
			if (_bounceOnOpen)
			{
				LeanTween.scaleX(base.gameObject, 1f, 0.3f).setEaseInElastic().setEaseOutBounce()
					.setIgnoreTimeScale(useUnScaledTime: true);
			}
			else
			{
				LeanTween.scaleX(base.gameObject, 1f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
			}
			break;
		case Enums.Modal.OpenDirection.Vertical:
			if (_bounceOnOpen)
			{
				LeanTween.scaleY(base.gameObject, 1f, 0.3f).setEaseInElastic().setEaseOutBounce()
					.setIgnoreTimeScale(useUnScaledTime: true);
			}
			else
			{
				LeanTween.scaleY(base.gameObject, 1f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
			}
			break;
		case Enums.Modal.OpenDirection.Both:
			if (_bounceOnOpen)
			{
				LeanTween.scaleX(base.gameObject, 1f, 0.3f).setEaseInElastic().setEaseOutBounce()
					.setIgnoreTimeScale(useUnScaledTime: true);
				LeanTween.scaleY(base.gameObject, 1f, 0.3f).setEaseInElastic().setEaseOutBounce()
					.setIgnoreTimeScale(useUnScaledTime: true);
			}
			else
			{
				LeanTween.scaleX(base.gameObject, 1f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
				LeanTween.scaleY(base.gameObject, 1f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
			}
			break;
		case Enums.Modal.OpenDirection.None:
			LeanTween.scaleX(base.gameObject, 1f, 0f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scaleY(base.gameObject, 1f, 0f).setIgnoreTimeScale(useUnScaledTime: true);
			break;
		}
	}

	private void VisualClose(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.None)
	{
		if (_openDirection == Enums.Modal.OpenDirection.UseGiven)
		{
			HandleVisualClose(openDirection);
		}
		else
		{
			HandleVisualClose(_openDirection);
		}
	}

	private void HandleVisualClose(Enums.Modal.OpenDirection openDirection)
	{
		LeanTween.cancel(base.gameObject);
		switch (openDirection)
		{
		case Enums.Modal.OpenDirection.Horizontal:
			LeanTween.scaleX(base.gameObject, 0f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
			break;
		case Enums.Modal.OpenDirection.Vertical:
			LeanTween.scaleY(base.gameObject, 0f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
			break;
		case Enums.Modal.OpenDirection.Both:
			LeanTween.scaleX(base.gameObject, 0f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scaleY(base.gameObject, 0f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
			break;
		case Enums.Modal.OpenDirection.None:
			LeanTween.scaleX(base.gameObject, 0f, 0f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scaleY(base.gameObject, 0f, 0f).setIgnoreTimeScale(useUnScaledTime: true);
			break;
		}
	}

	internal void HandleBlackPanel(bool showBlackPanel)
	{
		if (_showBlackBackdropPanel)
		{
			if (showBlackPanel)
			{
				SingletonController<ModalUIBlackPanelController>.Instance.Show(_blurBackground);
			}
			else
			{
				SingletonController<ModalUIBlackPanelController>.Instance.Hide(_blurBackground);
			}
		}
	}

	public IEnumerator DoAfterOpenUI(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		if (base.isActiveAndEnabled)
		{
			yield return new WaitForSecondsRealtime(0.2f);
			AfterOpenUI();
			this.OnAfterOpenUI?.Invoke();
		}
	}

	public virtual void AfterOpenUI()
	{
	}

	public IEnumerator DoAfterCloseUI(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		if (base.isActiveAndEnabled)
		{
			yield return new WaitForSecondsRealtime(0.05f);
			AfterCloseUI();
			this.OnAfterCloseUI?.Invoke();
		}
	}

	public virtual void AfterCloseUI()
	{
	}

	public ModalUiController GetModalUiController()
	{
		if (_modalUiController == null)
		{
			_modalUiController = UnityEngine.Object.FindAnyObjectByType<ModalUiController>();
		}
		return _modalUiController;
	}
}
