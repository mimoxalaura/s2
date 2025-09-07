using System;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class RarityBlockedInShopRelicHandler : RelicHandler
{
	[SerializeField]
	private Enums.PlaceableRarity[] _placeableRarityToBlock;

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnPlayerLoaded -= EventController_OnPlayerDashed;
	}

	public override void Execute()
	{
		Enums.PlaceableRarity[] placeableRarityToBlock = _placeableRarityToBlock;
		foreach (Enums.PlaceableRarity key in placeableRarityToBlock)
		{
			SingletonController<GameController>.Instance.Player.ShopOfferRarityChances[key] = -10000f;
		}
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		Execute();
		SingletonController<EventController>.Instance.OnPlayerLoaded += EventController_OnPlayerDashed;
	}

	private void EventController_OnPlayerDashed(object sender, EventArgs e)
	{
		Execute();
	}

	public override void TearDown()
	{
	}
}
