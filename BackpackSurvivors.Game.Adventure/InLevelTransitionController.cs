using System;
using System.Collections;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Video;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure;

[RequireComponent(typeof(CameraEnabler))]
public class InLevelTransitionController : SingletonController<InLevelTransitionController>
{
	internal delegate void OnTransitionCompletedHandler(object sender, EventArgs e);

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Canvas _canvas;

	[SerializeField]
	private AudioClip _transitionOutAudioclip;

	[SerializeField]
	private AudioClip _transitionInAudioclip;

	[SerializeField]
	private float _transitionDuration = 1f;

	private CameraEnabler _cameraEnabler;

	internal event OnTransitionCompletedHandler OnTransitionOutCompleted;

	internal event OnTransitionCompletedHandler OnTransitionInCompleted;

	public override void AfterBaseAwake()
	{
		_cameraEnabler = GetComponent<CameraEnabler>();
	}

	public void Transition(Action actionDuringTransitionedOut = null)
	{
		_cameraEnabler.SetCamerasEnabled(enabled: true);
		_canvas.gameObject.SetActive(value: true);
		StartCoroutine(TransitionAsync(actionDuringTransitionedOut));
	}

	private IEnumerator TransitionAsync(Action actionDuringTransitionedOut = null)
	{
		_animator.SetTrigger("TransitionOut");
		SingletonController<AudioController>.Instance.PlaySFXClip(_transitionOutAudioclip, 1f, 0.2f);
		yield return new WaitForSecondsRealtime(_transitionDuration);
		this.OnTransitionOutCompleted?.Invoke(this, new EventArgs());
		if (actionDuringTransitionedOut != null)
		{
			actionDuringTransitionedOut();
			yield return new WaitForSecondsRealtime(0.2f);
		}
		_animator.SetTrigger("TransitionIn");
		SingletonController<AudioController>.Instance.PlaySFXClip(_transitionInAudioclip, 1f);
		yield return new WaitForSecondsRealtime(_transitionDuration);
		this.OnTransitionInCompleted?.Invoke(this, new EventArgs());
		_canvas.gameObject.SetActive(value: false);
		_cameraEnabler.SetCamerasEnabled(enabled: false);
	}
}
