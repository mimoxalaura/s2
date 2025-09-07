using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Backpack.Highlighting;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Video;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.Merging;

[RequireComponent(typeof(CameraEnabler))]
internal class StarLineController : SingletonController<StarLineController>
{
	[SerializeField]
	private Vector2 _graphScale = new Vector2(1f, 1f);

	[SerializeField]
	private Canvas _canvas;

	[Header("Lines")]
	[SerializeField]
	private Transform _starredLineContainer;

	[SerializeField]
	private StarredItemLine _starredItemLinePrefab;

	[SerializeField]
	private Camera _starLineCamera;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _activeStarBoostAudio;

	private List<MergableItemLine> _starredItemLines = new List<MergableItemLine>();

	private CameraEnabler _cameraEnabler;

	private void Start()
	{
		base.IsInitialized = true;
	}

	public override void AfterBaseAwake()
	{
		_cameraEnabler = GetComponent<CameraEnabler>();
	}

	internal void SetCamerasEnabled(bool enabled)
	{
		_cameraEnabler.SetCamerasEnabled(enabled);
	}

	internal void ClearHighlights()
	{
		foreach (BaseDraggable item in from x in Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None)
			where x.Owner == Enums.Backpack.DraggableOwner.Player
			select x)
		{
			item.SetHighlight(highlight: false, effectIsPositive: true);
		}
	}

	internal void HighlightAffectedWeaponsAndItems(BaseDraggable draggable, bool animate, bool effectIsPositive)
	{
		ClearHighlights();
		HighlightDraggables(GetDraggablesToHighlight(draggable), animate, effectIsPositive);
	}

	internal void ClearStarredLines()
	{
		_starredItemLines.Clear();
		for (int num = _starredLineContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(_starredLineContainer.GetChild(num).gameObject);
		}
	}

	internal void DrawLinesToAffectedWeaponsAndItems(BaseDraggable draggable, bool effectIsPositive)
	{
		ClearStarredLines();
		List<BaseDraggable> draggablesToHighlight = GetDraggablesToHighlight(draggable);
		GenerateStarredLinesToDraggables(draggable, draggablesToHighlight, effectIsPositive);
	}

	private void GenerateStarredLinesToDraggables(BaseDraggable draggable, List<BaseDraggable> draggables, bool effectIsPositive)
	{
		foreach (BaseDraggable draggable2 in draggables)
		{
			MakeStarredLine(draggable.transform.position, draggable2.transform.position, effectIsPositive);
		}
	}

	private static void HighlightDraggables(List<BaseDraggable> draggables, bool animate, bool effectIsPositive)
	{
		if (animate)
		{
			foreach (BaseDraggable draggable in draggables)
			{
				draggable.AnimateForEffect();
			}
		}
		foreach (BaseDraggable draggable2 in draggables)
		{
			draggable2.SetHighlight(highlight: true, effectIsPositive);
		}
	}

	private List<BaseDraggable> GetDraggablesToHighlight(BaseDraggable draggable)
	{
		List<WeaponInstance> weaponsAffectedByBaseDraggable = GetWeaponsAffectedByBaseDraggable(draggable);
		List<BaseDraggable> list = new List<BaseDraggable>();
		list.AddRange(GetDraggablesFromWeaponInstances(weaponsAffectedByBaseDraggable));
		return list;
	}

	private List<BaseDraggable> GetDraggablesFromWeaponInstances(List<WeaponInstance> weaponsAffectedByCurrentDraggable)
	{
		List<BaseDraggable> list = new List<BaseDraggable>();
		List<BaseDraggable> source = (from x in Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None)
			where x.Owner == Enums.Backpack.DraggableOwner.Player && x.BaseItemSO.ItemType == Enums.PlaceableType.Weapon
			select x).ToList();
		foreach (WeaponInstance weaponAffectedByCurrentDraggable in weaponsAffectedByCurrentDraggable)
		{
			BaseDraggable baseDraggable = source.FirstOrDefault((BaseDraggable x) => ((DraggableWeapon)x).WeaponInstance.Guid == weaponAffectedByCurrentDraggable.Guid);
			if (baseDraggable != null)
			{
				list.Add(baseDraggable);
			}
		}
		return list;
	}

	private List<WeaponInstance> GetWeaponsAffectedByBaseDraggable(BaseDraggable draggable)
	{
		return draggable.DraggableType switch
		{
			Enums.Backpack.DraggableType.Bag => new List<WeaponInstance>(), 
			Enums.Backpack.DraggableType.Item => GetWeaponsAffectedByItem((DraggableItem)draggable), 
			Enums.Backpack.DraggableType.Weapon => GetWeaponsAffectedByWeapon((DraggableWeapon)draggable), 
			_ => new List<WeaponInstance>(), 
		};
	}

	private List<WeaponInstance> GetWeaponsAffectedByItem(DraggableItem draggable)
	{
		List<WeaponInstance> list = new List<WeaponInstance>();
		foreach (int currentStarSlotid in draggable.CurrentStarSlotids)
		{
			WeaponInstance weaponFromSlot = SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage().GetWeaponFromSlot(currentStarSlotid);
			if (weaponFromSlot != null && !list.Contains(weaponFromSlot) && draggable.ItemInstance.CanAffectWeaponInstance(weaponFromSlot))
			{
				list.Add(weaponFromSlot);
			}
		}
		return list;
	}

	private List<WeaponInstance> GetWeaponsAffectedByWeapon(DraggableWeapon draggable)
	{
		List<WeaponInstance> list = new List<WeaponInstance>();
		foreach (WeaponInstance weapon in SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack())
		{
			foreach (WeaponStatModifier weaponModifier in weapon.WeaponModifiers)
			{
				if (weaponModifier.Source != null && weaponModifier.Source.ConnectedWeapon != null && weaponModifier.Source.ConnectedWeapon == draggable.WeaponInstance && !list.Any((WeaponInstance x) => x.Guid == weapon.Guid))
				{
					list.Add(weapon);
				}
			}
			foreach (DamageModifier damageModifier in weapon.DamageModifiers)
			{
				if (damageModifier.Source != null && damageModifier.Source.ConnectedWeapon != null && damageModifier.Source.ConnectedWeapon == draggable.WeaponInstance && !list.Any((WeaponInstance x) => x.Guid == weapon.Guid))
				{
					list.Add(weapon);
				}
			}
		}
		return list;
	}

	private void MakeStarredLine(Vector2 startPos, Vector2 endPos, bool effectIsPositive)
	{
		StarredItemLine starredItemLine = Object.Instantiate(_starredItemLinePrefab, _starredLineContainer);
		Vector2 startPos2 = startPos * GetScaleMod();
		Vector2 vector = endPos * GetScaleMod();
		starredItemLine.Init(startPos2, vector, effectIsPositive);
	}

	private float GetScaleMod()
	{
		return 1f / _canvas.transform.localScale.y;
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
