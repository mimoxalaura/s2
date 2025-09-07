using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Game.Events;
using BackpackSurvivors.ScriptableObjects.CraftingResource;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.CraftingResources;

internal class CraftingResourceVisualsController : MonoBehaviour
{
	[SerializeField]
	private Transform _resourceItemContainer;

	[SerializeField]
	private CraftingResourceVisualItem _craftingResourceVisualItemPrefab;

	[SerializeField]
	private List<CraftingResourceVisualItem> _craftingResourceVisualItems;

	private bool _shouldShow = true;

	private void Start()
	{
		Init();
		BindEvents();
	}

	private void BindEvents()
	{
		SingletonController<CurrencyController>.Instance.OnCraftingResourceChanged += Instance_OnCraftingResourceChanged;
	}

	private void Instance_OnCraftingResourceChanged(object sender, CraftingResourceChangedEventArgs e)
	{
		if (e.NewAmount > 0)
		{
			UpdateCraftingResourceValue(e.CraftingResource, e.NewAmount);
		}
		else
		{
			RemoveCraftingResourceVisualItem(e.CraftingResource);
		}
	}

	private void RemoveCraftingResourceVisualItem(Enums.CraftingResource craftingResource)
	{
		CraftingResourceVisualItem craftingResourceVisualItem = _craftingResourceVisualItems.FirstOrDefault((CraftingResourceVisualItem x) => x.CraftingResource == craftingResource);
		_craftingResourceVisualItems.Remove(craftingResourceVisualItem);
		if (craftingResourceVisualItem != null)
		{
			UnityEngine.Object.Destroy(craftingResourceVisualItem.gameObject);
		}
		_resourceItemContainer.GetComponent<GridLayoutGroup>().constraintCount = Math.Clamp(_craftingResourceVisualItems.Count(), 0, 3);
		if (_craftingResourceVisualItems.Count() == 0 || !_shouldShow)
		{
			_resourceItemContainer.gameObject.SetActive(value: false);
		}
	}

	private void UpdateCraftingResourceValue(Enums.CraftingResource craftingResource, int newAmount)
	{
		CraftingResourceVisualItem craftingResourceVisualItem = _craftingResourceVisualItems.FirstOrDefault((CraftingResourceVisualItem x) => x.CraftingResource == craftingResource);
		if (craftingResourceVisualItem == null)
		{
			AddCraftingResourceToGrid(craftingResource, newAmount);
		}
		else
		{
			craftingResourceVisualItem.UpdateValue(newAmount);
		}
	}

	private void UnbindEvents()
	{
		SingletonController<CurrencyController>.Instance.OnCraftingResourceChanged -= Instance_OnCraftingResourceChanged;
	}

	public void Init()
	{
		foreach (Enums.CraftingResource value in Enum.GetValues(typeof(Enums.CraftingResource)))
		{
			foreach (CraftingResourceSO craftingResource3 in GameDatabaseHelper.GetCraftingResources())
			{
				if (value == craftingResource3.CraftingResource)
				{
					int craftingResource2 = SingletonController<CurrencyController>.Instance.GetCraftingResource(craftingResource3.CraftingResource);
					if (craftingResource2 > 0)
					{
						AddCraftingResourceToGrid(craftingResource3, craftingResource2);
					}
				}
			}
		}
		if (_shouldShow)
		{
			_resourceItemContainer.gameObject.SetActive(_craftingResourceVisualItems.Any());
		}
		else
		{
			_resourceItemContainer.gameObject.SetActive(value: false);
		}
	}

	private void AddCraftingResourceToGrid(CraftingResourceSO craftingResourceSO, int amount)
	{
		CraftingResourceVisualItem craftingResourceVisualItem = UnityEngine.Object.Instantiate(_craftingResourceVisualItemPrefab, _resourceItemContainer);
		craftingResourceVisualItem.Init(craftingResourceSO, amount);
		_craftingResourceVisualItems.Add(craftingResourceVisualItem);
		_resourceItemContainer.GetComponent<GridLayoutGroup>().constraintCount = Math.Clamp(_craftingResourceVisualItems.Count(), 0, 3);
		if (_craftingResourceVisualItems.Count() == 0 || !_shouldShow)
		{
			_resourceItemContainer.gameObject.SetActive(value: false);
		}
	}

	private void AddCraftingResourceToGrid(Enums.CraftingResource craftingResource, int amount)
	{
		CraftingResourceSO craftingResourceSO = GameDatabaseHelper.GetCraftingResources().FirstOrDefault((CraftingResourceSO x) => x.CraftingResource == craftingResource);
		if (craftingResourceSO != null)
		{
			AddCraftingResourceToGrid(craftingResourceSO, amount);
		}
		_resourceItemContainer.gameObject.SetActive(_craftingResourceVisualItems.Any());
	}

	public void ChangeGridVisibility(bool visible)
	{
		_shouldShow = visible;
		_resourceItemContainer.gameObject.SetActive(_craftingResourceVisualItems.Any() && _shouldShow);
	}

	private void OnDestroy()
	{
		UnbindEvents();
	}
}
