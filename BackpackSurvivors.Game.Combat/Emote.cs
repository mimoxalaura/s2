using BackpackSurvivors.Game.Core;
using BackpackSurvivors.System;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat;

public class Emote : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Transform _start;

	[SerializeField]
	private Transform _end;

	[Command("player.emote.emote", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void ActEmote(Enums.Emotes emote)
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.EmoteIcons.ContainsKey(emote))
		{
			Sprite sprite = SingletonController<GameDatabase>.Instance.GameDatabaseSO.EmoteIcons[emote];
			_spriteRenderer.sprite = sprite;
			LeanTween.cancel(_spriteRenderer.gameObject);
			_spriteRenderer.color = Color.white;
			_spriteRenderer.transform.localScale = Vector3.zero;
			LeanTween.moveY(_spriteRenderer.gameObject, _start.position.y, 0f);
			LeanTween.scale(_spriteRenderer.gameObject, Vector3.one, 1f).setEaseInOutElastic();
			LeanTween.moveY(_spriteRenderer.gameObject, _end.position.y, 2f);
			LeanTween.value(base.gameObject, delegate(float val)
			{
				_spriteRenderer.color = new Color(255f, 255f, 255f, val);
			}, 1f, 0f, 2f).setDelay(0.5f);
		}
	}
}
