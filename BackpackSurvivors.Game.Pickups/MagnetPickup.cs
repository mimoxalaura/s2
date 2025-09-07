using System.Collections.Generic;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Pickups;

internal class MagnetPickup : Lootdrop
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	protected override void Collect()
	{
		_collected = true;
		SingletonController<GameController>.Instance.Player.DamageVisualizer.ShowTextPopup("Magnetized!", Constants.Colors.Stunned, 3f);
		List<Enums.LootType> lootTypesToMove = new List<Enums.LootType>
		{
			Enums.LootType.Coins,
			Enums.LootType.Health,
			Enums.LootType.TitanicSouls
		};
		LootDropContainer.Instance.MoveLootdropsToPlayer(fromCompletion: true, lootTypesToMove);
		Object.Destroy(base.gameObject);
	}
}
