using System.Collections.Generic;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Interactable.ByTouching;

internal class MagnetInteractableAltar : InteractableAltar
{
	public override void Act()
	{
		_interactionsDone++;
		_animator.SetBool("Active", base._canInteract);
		SingletonController<AudioController>.Instance.PlaySFXClip(_activationAudio, 1f);
		List<Enums.LootType> lootTypesToMove = new List<Enums.LootType>
		{
			Enums.LootType.Coins,
			Enums.LootType.Health,
			Enums.LootType.TitanicSouls
		};
		LootDropContainer.Instance.MoveLootdropsToPlayer(fromCompletion: true, lootTypesToMove);
		GameObject[] hideAfterInteractionsCompleted = _hideAfterInteractionsCompleted;
		for (int i = 0; i < hideAfterInteractionsCompleted.Length; i++)
		{
			hideAfterInteractionsCompleted[i].SetActive(base._canInteract);
		}
	}
}
