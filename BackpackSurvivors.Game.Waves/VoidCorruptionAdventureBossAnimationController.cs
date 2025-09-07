using System;
using System.Collections;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.GameplayFeedback;
using Cinemachine;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

internal class VoidCorruptionAdventureBossAnimationController : AdventureBossAnimationController
{
	[SerializeField]
	private AudioClip _textImpactAudioclip;

	[SerializeField]
	private CinemachineVirtualCamera _virtualCamera;

	private Enemy _enemy;

	private void Instance_OnCancelHandler(object sender, EventArgs e)
	{
		Complete();
	}

	public override void Animate(Action onCancelOrFinish, Enemy enemy)
	{
		_enemy = enemy;
		SingletonController<GameController>.Instance.CanPauseGame = false;
		SingletonController<GameController>.Instance.CanOpenMenu = false;
		SingletonController<InputController>.Instance.OnCancelHandler += Instance_OnCancelHandler;
		base.Animate(onCancelOrFinish, enemy);
		StartCoroutine(AnimateAsync(enemy));
	}

	public override void Complete()
	{
		StopAllCoroutines();
		SingletonController<InputController>.Instance.OnCancelHandler -= Instance_OnCancelHandler;
		FocusOnPlayer();
		UnityEngine.Object.FindObjectOfType<EnvelopController>().SetEnvelopInvisible();
		base.BossNameImage.SetActive(value: false);
		_enemy.ResetToDefaultVisualState();
		SingletonController<GameController>.Instance.CanPauseGame = true;
		SingletonController<GameController>.Instance.CanOpenMenu = true;
		SingletonController<InputController>.Instance.SwitchToIngameActionMap(storeCurrentInputMap: false);
		base.Complete();
	}

	private IEnumerator AnimateAsync(Enemy enemy)
	{
		FocusOnBoss();
		UnityEngine.Object.FindObjectOfType<EnvelopController>().SetEnvelopVisible();
		ShakeCamera(1f);
		enemy.ShowSpawnAnimation();
		yield return new WaitForSeconds(3f);
		enemy.SpawnAudio();
		yield return new WaitForSeconds(4f);
		StopShaking();
		base.BossNameImage.SetActive(value: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_textImpactAudioclip, 1f);
		yield return new WaitForSeconds(5f);
		Complete();
	}

	private void ShakeCamera(float intensity)
	{
		_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
	}

	public void StopShaking()
	{
		_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
	}
}
