using System.Collections;
using BackpackSurvivors.ScriptableObjects.Loot;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Interactable.ByTouching;

internal class InteractableRewardChest : InteractableActor
{
	[SerializeField]
	private Animator _chestAnimator;

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

	[SerializeField]
	private LootBag _lootBag;

	[SerializeField]
	private LootBagSO _lootBagSO;

	private void Start()
	{
		_lootBag.Init(_lootBagSO);
	}

	public override void Act()
	{
		SingletonController<AudioController>.Instance.PlaySFXClip(_opened, 1f);
		_chestAnimator.SetBool("Opened", value: true);
		StartCoroutine(RemoveAfterDelay());
	}

	private IEnumerator RemoveAfterDelay()
	{
		yield return new WaitForSeconds(0.4f);
		_lootBag.TryDrop(base.transform, 1f);
		yield return new WaitForSeconds(_delayToRemove);
		LeanTween.value(_spriteRenderer.gameObject, FadeOutRewardChest, 1f, 0f, _fadeTime);
		_collider2d.enabled = false;
		yield return new WaitForSeconds(_fadeTime);
		Object.Destroy(base.gameObject);
		Object.Destroy(_parentObject);
	}

	private void FadeOutRewardChest(float val)
	{
		_spriteRenderer.color = new Color(1f, 1f, 1f, val);
	}
}
