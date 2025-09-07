using System;
using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Shared;

public class BaseModalUIController : MonoBehaviour, IModelUIController
{
	public delegate void CloseButtonClickedHandler(object sender, EventArgs e);

	[SerializeField]
	private Camera[] _cameras;

	[SerializeField]
	private bool _delayCameraEnable;

	[SerializeField]
	private float _cameraEnableDelay;

	[SerializeField]
	private bool _delayCameraDisable;

	[SerializeField]
	private float _cameraDisableDelay;

	public event CloseButtonClickedHandler OnCloseButtonClicked;

	public virtual void OpenUI()
	{
		if (!_delayCameraEnable)
		{
			SetCamerasEnabled(enabled: true);
		}
		else
		{
			SetCamerasDelayedEnabled(enabled: true);
		}
	}

	public virtual void CloseUI()
	{
		if (!_delayCameraDisable)
		{
			SetCamerasEnabled(enabled: false);
		}
		else
		{
			SetCamerasDelayedEnabled(enabled: false);
		}
	}

	internal void SetCamerasEnabled(bool enabled)
	{
		Camera[] cameras = _cameras;
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].gameObject.SetActive(enabled);
		}
	}

	internal void SetCamerasDelayedEnabled(bool enabled)
	{
		StartCoroutine(SetCamerasDelayedEnabledASync(enabled));
	}

	internal IEnumerator SetCamerasDelayedEnabledASync(bool enabled)
	{
		float time = (enabled ? _cameraEnableDelay : _cameraDisableDelay);
		yield return new WaitForSecondsRealtime(time);
		Camera[] cameras = _cameras;
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].gameObject.SetActive(enabled);
		}
	}

	public virtual bool CloseOnCancelInput()
	{
		return false;
	}

	public virtual bool AudioOnOpen()
	{
		return false;
	}

	public virtual bool AudioOnClose()
	{
		return false;
	}

	public void ClickedCloseButton()
	{
		this.OnCloseButtonClicked?.Invoke(this, new EventArgs());
	}

	public virtual Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.None;
	}
}
