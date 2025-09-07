using System.Collections;
using BackpackSurvivors.Game.World;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Interactable.ByTouching;

internal class ActAfterDelayInRange : MonoBehaviour
{
	[SerializeField]
	private float _timeRequiredInRange;

	[SerializeField]
	private float _timeDropPercentageNotInRange;

	[SerializeField]
	private InteractableActor _actorToAct;

	[SerializeField]
	private Animator _inRangeVisual;

	[SerializeField]
	private bool _subtleEffect;

	[SerializeField]
	private SpriteRenderer _inRangeSpriteRenderer;

	[SerializeField]
	private Transform _completorVisual;

	[SerializeField]
	private Transform _completorVisualTargetSize;

	[SerializeField]
	private bool _canInteract = true;

	[SerializeField]
	private bool _isInRange;

	[SerializeField]
	private bool _progressFallsBack = true;

	[SerializeField]
	private bool _onceTouched;

	[SerializeField]
	private float _currentTimeSpendInRange;

	[SerializeField]
	private AudioClip _acted;

	[SerializeField]
	private GameObject[] _showOnEnter;

	[SerializeField]
	private GameObject[] _hideOnExit;

	private Vector2 _targetScale;

	private void Start()
	{
		_targetScale = new Vector2(4.939906f, 4.939906f);
		StartCoroutine(RunInRangeThread());
		StartCoroutine(RunOutOfRangeThread());
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_canInteract && collision.GetComponent<InteractingEntity>() != null)
		{
			GameObject[] showOnEnter = _showOnEnter;
			for (int i = 0; i < showOnEnter.Length; i++)
			{
				showOnEnter[i].SetActive(value: true);
			}
			_inRangeVisual.SetTrigger("Enter");
			_inRangeVisual.SetBool("Inside", value: true);
			_isInRange = true;
			_onceTouched = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (_canInteract && collision.GetComponent<InteractingEntity>() != null)
		{
			GameObject[] hideOnExit = _hideOnExit;
			for (int i = 0; i < hideOnExit.Length; i++)
			{
				hideOnExit[i].SetActive(value: false);
			}
			_inRangeVisual.SetTrigger("Exit");
			_inRangeVisual.SetBool("Inside", value: false);
			_isInRange = false;
		}
	}

	private IEnumerator RunInRangeThread()
	{
		while (_canInteract)
		{
			if (_isInRange && _onceTouched)
			{
				_currentTimeSpendInRange += Time.deltaTime + 0.02f;
				float num = _currentTimeSpendInRange / _timeRequiredInRange;
				_completorVisual.localScale = _targetScale * num;
				_inRangeSpriteRenderer.material.SetFloat("_Alpha", num);
			}
			yield return new WaitForSeconds(0.02f);
			if (_currentTimeSpendInRange > _timeRequiredInRange)
			{
				Act();
				yield return new WaitForSeconds(1.7f);
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void Act()
	{
		GameObject[] hideOnExit = _hideOnExit;
		for (int i = 0; i < hideOnExit.Length; i++)
		{
			hideOnExit[i].SetActive(value: false);
		}
		hideOnExit = _showOnEnter;
		for (int i = 0; i < hideOnExit.Length; i++)
		{
			hideOnExit[i].SetActive(value: false);
		}
		_inRangeVisual.SetBool("Subtle", _subtleEffect);
		SingletonController<AudioController>.Instance.PlaySFXClip(_acted, 1f);
		_inRangeVisual.SetTrigger("Act");
		_actorToAct.Act();
	}

	private IEnumerator RunOutOfRangeThread()
	{
		while (_canInteract)
		{
			if (!_isInRange && _onceTouched && _progressFallsBack)
			{
				_currentTimeSpendInRange -= _timeDropPercentageNotInRange * Time.deltaTime + 0.02f;
				float num = _currentTimeSpendInRange / _timeRequiredInRange;
				_completorVisual.localScale = _targetScale * num;
				_inRangeSpriteRenderer.material.SetFloat("_Alpha", num);
			}
			yield return new WaitForSeconds(0.02f);
			if (_currentTimeSpendInRange < 0f)
			{
				_currentTimeSpendInRange = 0f;
			}
		}
		while (_currentTimeSpendInRange > 0f)
		{
		}
	}
}
