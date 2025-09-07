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

internal class InfernalDestroyerAdventureBossAnimationController : AdventureBossAnimationController
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
		StartCoroutine(AnimateAsync((InfernalDestroyer)enemy));
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

	private IEnumerator AnimateAsync(InfernalDestroyer enemy)
	{
		FocusOnBoss();
		UnityEngine.Object.FindObjectOfType<EnvelopController>().SetEnvelopVisible();
		ShakeCamera(1f);
		enemy.BeforeSpawning();
		enemy.Animator.SetBool("Hidden", value: false);
		enemy.LowerAllSpriteRenderLayers();
		enemy.ShowSpawnAnimation();
		yield return new WaitForSeconds(5f);
		enemy.RestoreSpriteRenderLayers();
		enemy.SpawnAudio();
		yield return new WaitForSeconds(2f);
		StopShaking();
		base.BossNameImage.SetActive(value: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_textImpactAudioclip, 1f);
		yield return new WaitForSeconds(5f);
		enemy.AfterSpawning();
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
