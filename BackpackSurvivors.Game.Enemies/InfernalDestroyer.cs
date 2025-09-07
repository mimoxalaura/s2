using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

public class InfernalDestroyer : Enemy
{
	[SerializeField]
	private SpriteRenderer[] _bossSpriteRenderers;

	private Dictionary<SpriteRenderer, string> _bossSpriteRendererLayers;

	private void Awake()
	{
		GetSpriteRenderer().color = new Color(255f, 255f, 255f, 0f);
		SetCanAct(canAct: false);
		Initialize();
		StoreSpriteRenderLayers();
	}

	public override void SetCanAct(bool canAct)
	{
		base.SetCanAct(canAct);
	}

	public override void ResetToDefaultVisualState()
	{
		GetSpriteRenderer().color = new Color(255f, 255f, 255f, 1f);
	}

	private void StoreSpriteRenderLayers()
	{
		_bossSpriteRendererLayers = new Dictionary<SpriteRenderer, string>();
		SpriteRenderer[] bossSpriteRenderers = _bossSpriteRenderers;
		foreach (SpriteRenderer spriteRenderer in bossSpriteRenderers)
		{
			_bossSpriteRendererLayers.Add(spriteRenderer, spriteRenderer.sortingLayerName);
		}
	}

	internal void RestoreSpriteRenderLayers()
	{
		SpriteRenderer[] bossSpriteRenderers = _bossSpriteRenderers;
		foreach (SpriteRenderer spriteRenderer in bossSpriteRenderers)
		{
			spriteRenderer.sortingLayerName = _bossSpriteRendererLayers[spriteRenderer];
		}
	}

	internal void LowerAllSpriteRenderLayers()
	{
		SpriteRenderer[] bossSpriteRenderers = _bossSpriteRenderers;
		for (int i = 0; i < bossSpriteRenderers.Length; i++)
		{
			bossSpriteRenderers[i].sortingLayerName = "Props";
		}
	}

	internal override void BeforeSpawning()
	{
		base.BeforeSpawning();
	}

	internal override void AfterSpawning()
	{
		base.AfterSpawning();
		RestoreSpriteRenderLayers();
	}
}
