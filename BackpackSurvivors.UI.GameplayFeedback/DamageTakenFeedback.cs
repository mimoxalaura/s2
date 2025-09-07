using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Health.Events;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.UI.Shared;
using Cinemachine;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback;

[RequireComponent(typeof(MaterialPropertyController))]
public class DamageTakenFeedback : MonoBehaviour
{
	[SerializeField]
	private CinemachineVirtualCamera _virtualCamera;

	[SerializeField]
	private FlashImage _playerHurtImage;

	[SerializeField]
	private float _shakeIntensity = 1f;

	[SerializeField]
	private float _shakeDuration = 0.3f;

	private MaterialPropertyController _materialPropertyController;

	private float _shakeTimer;

	private void Start()
	{
		_materialPropertyController = GetComponent<MaterialPropertyController>();
		SingletonController<GameController>.Instance.Player.OnHealthChanged += Player_OnHealthChanged;
		SingletonController<GameController>.Instance.Player.OnCharacterDamaged += Player_OnCharacterDamaged;
		CheckHealthState();
		_virtualCamera = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
	}

	private void Player_OnCharacterDamaged(object sender, DamageTakenEventArgs e)
	{
		CharacterDamaged();
	}

	private void Player_OnHealthChanged(object sender, HealthChangedEventArgs e)
	{
		CheckHealthState();
	}

	private void CharacterDamaged()
	{
		if (SingletonController<SettingsController>.Instance.GameplaySettingsController.FlashOnDamageTaken == Enums.FlashOnDamageTaken.Enabled)
		{
			_playerHurtImage.Flash(Color.red);
		}
		if (SingletonController<SettingsController>.Instance.VideoSettingsController.CameraShake == Enums.CameraShake.Enabled)
		{
			ShakeCamera(_shakeIntensity, _shakeDuration);
		}
		CheckHealthState();
	}

	private void ShakeCamera(float intensity, float time)
	{
		if (base.isActiveAndEnabled)
		{
			CinemachineBasicMultiChannelPerlin cinemachineComponent = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			if (cinemachineComponent != null)
			{
				cinemachineComponent.m_AmplitudeGain = intensity;
				StartCoroutine(StopShaking(time));
			}
			else
			{
				Debug.LogWarning("ShakeCamera Perlin not set");
			}
		}
	}

	private IEnumerator StopShaking(float timeDelay)
	{
		yield return new WaitForSeconds(timeDelay);
		_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
	}

	internal void CheckHealthState()
	{
		if (SingletonController<GameController>.Instance.Player.HealthPercentage < 0.25f)
		{
			EnableFeedbackVisual();
		}
		else
		{
			DisableFeedbackVisual();
		}
	}

	public void DisableFeedbackVisual()
	{
		_materialPropertyController.SetEnabled(enabled: false);
	}

	public void EnableFeedbackVisual()
	{
		_materialPropertyController.SetEnabled(enabled: true);
	}

	private void OnDestroy()
	{
		DisableFeedbackVisual();
		if (SingletonController<GameController>.Instance.Player != null)
		{
			SingletonController<GameController>.Instance.Player.OnHealthChanged -= Player_OnHealthChanged;
			SingletonController<GameController>.Instance.Player.OnCharacterDamaged -= Player_OnCharacterDamaged;
		}
	}
}
