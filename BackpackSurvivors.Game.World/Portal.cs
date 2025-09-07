using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.Game.World;

public class Portal : Interaction
{
	[SerializeField]
	private Animator Animator;

	[SerializeField]
	private Light2D PortalLight;

	[SerializeField]
	private AudioClip OpeningAudio;

	[SerializeField]
	private AudioClip ClosingAudio;

	[SerializeField]
	private AudioSource PortalOpenAudio;

	private bool _portalIsActive;

	private bool _isReadyToActivateAgain = true;

	public override void DoStart()
	{
		base.DoStart();
		PortalOpenAudio.gameObject.SetActive(value: false);
	}

	public override void DoInRange()
	{
		base.DoInRange();
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
	}

	public override void DoInteract()
	{
		if (_portalIsActive)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	public void Open()
	{
		if (!_portalIsActive && _isReadyToActivateAgain)
		{
			_isReadyToActivateAgain = false;
			AudioSource.PlayClipAtPoint(OpeningAudio, base.transform.position);
			Animator.SetBool("IsOpen", value: true);
			Animator.SetTrigger("Open");
			_portalIsActive = true;
			PortalOpenAudio.gameObject.SetActive(value: true);
			StartCoroutine(AfterOpen(0.4f));
		}
	}

	private IEnumerator AfterOpen(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		_isReadyToActivateAgain = true;
	}

	public void Close()
	{
		if (_portalIsActive && _isReadyToActivateAgain)
		{
			_isReadyToActivateAgain = false;
			AudioSource.PlayClipAtPoint(ClosingAudio, base.transform.position);
			Animator.SetBool("IsOpen", value: false);
			Animator.SetTrigger("Close");
			_portalIsActive = false;
			PortalOpenAudio.gameObject.SetActive(value: false);
			StartCoroutine(AfterClose(1f));
		}
	}

	private IEnumerator AfterClose(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		_isReadyToActivateAgain = true;
	}

	internal bool IsOpen()
	{
		return _portalIsActive;
	}
}
