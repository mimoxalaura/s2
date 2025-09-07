using BackpackSurvivors.System;
using BackpackSurvivors.System.Video;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Shared;

[RequireComponent(typeof(CameraEnabler))]
public class ModalUIBlackPanelController : SingletonController<ModalUIBlackPanelController>
{
	[SerializeField]
	private Image _blackBackpanel;

	[SerializeField]
	private GameObject _blurVolume;

	[SerializeField]
	private Camera _blackBackdropCamera;

	private CameraEnabler _cameraEnabler;

	public override void AfterBaseAwake()
	{
		_cameraEnabler = GetComponent<CameraEnabler>();
	}

	public void Show(bool blurBackground)
	{
		if (!(_blackBackpanel == null))
		{
			_cameraEnabler.SetCamerasEnabled(enabled: true);
			_blackBackpanel.gameObject.SetActive(value: true);
			_blurVolume.SetActive(blurBackground);
			_blackBackdropCamera.enabled = true;
		}
	}

	public void Hide(bool blurBackground)
	{
		if (!(_blackBackpanel == null))
		{
			_cameraEnabler.SetCamerasEnabled(enabled: false);
			_blackBackpanel.gameObject.SetActive(value: false);
			_blurVolume.SetActive(value: false);
			_blackBackdropCamera.enabled = false;
		}
	}
}
