using System;
using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Interactable.ByTouching;

internal class InteractableLever : InteractableActor
{
	internal delegate void LeverPulledEventHandler(object sender, EventArgs e);

	[SerializeField]
	private Animator _leverAnimator;

	[SerializeField]
	private AudioClip _opened;

	[SerializeField]
	private float _delayToRemove;

	[SerializeField]
	private float _fadeTime;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private GameObject _parentObject;

	[SerializeField]
	private Collider2D _collider2d;

	internal event LeverPulledEventHandler OnLeverPulled;

	public override void Act()
	{
		SingletonController<AudioController>.Instance.PlaySFXClip(_opened, 1f);
		_leverAnimator.SetBool("Opened", value: true);
		this.OnLeverPulled?.Invoke(this, new EventArgs());
		StartCoroutine(RemoveAfterDelay());
	}

	private IEnumerator RemoveAfterDelay()
	{
		yield return new WaitForSeconds(0.4f);
		yield return new WaitForSeconds(_delayToRemove);
		LeanTween.value(_spriteRenderer.gameObject, FadeOutLever, 1f, 0f, _fadeTime);
		_collider2d.enabled = false;
		yield return new WaitForSeconds(_fadeTime);
		UnityEngine.Object.Destroy(base.gameObject);
		UnityEngine.Object.Destroy(_parentObject);
	}

	private void FadeOutLever(float val)
	{
		_spriteRenderer.color = new Color(1f, 1f, 1f, val);
	}
}
