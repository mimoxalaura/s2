using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CollectionChest : UnlockableInTown
{
	private float _speed = 1f;

	private Vector2 _pointA;

	[SerializeField]
	private GameObject _propToMoveUpAndDown;

	[SerializeField]
	private GameObject[] _gameObjectsToDisable;

	[SerializeField]
	private SpriteRenderer[] _spriteRenderersToDisable;

	[SerializeField]
	private Animator[] _animatorsToDisable;

	public override void ToggleLockState(bool unlocked, bool animate, bool focusOnUnlock, bool reopenUI)
	{
		base.ToggleLockState(unlocked, animate, focusOnUnlock, reopenUI);
		StartCoroutine(delayedToggleLockState(unlocked, animate));
	}

	private IEnumerator delayedToggleLockState(bool unlocked, bool animate)
	{
		yield return null;
		if (animate)
		{
			yield return new WaitForSeconds(1.7f);
		}
		SpriteRenderer[] spriteRenderersToDisable = _spriteRenderersToDisable;
		for (int i = 0; i < spriteRenderersToDisable.Length; i++)
		{
			spriteRenderersToDisable[i].enabled = false;
		}
		GameObject[] gameObjectsToDisable = _gameObjectsToDisable;
		for (int i = 0; i < gameObjectsToDisable.Length; i++)
		{
			gameObjectsToDisable[i].SetActive(value: false);
		}
		Animator[] animatorsToDisable = _animatorsToDisable;
		for (int i = 0; i < animatorsToDisable.Length; i++)
		{
			animatorsToDisable[i].enabled = false;
		}
		if (unlocked)
		{
			Vector2 vector = base.transform.position;
			_pointA = vector + new Vector2(0f, -0.1f);
			LeanTween.moveY(_propToMoveUpAndDown, _pointA.y, _speed).setEaseInOutCirc().setLoopPingPong();
		}
	}
}
