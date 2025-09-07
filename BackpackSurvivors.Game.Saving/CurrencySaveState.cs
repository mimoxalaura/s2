using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class CurrencySaveState : BaseSaveState
{
	public Dictionary<Enums.CurrencyType, int> MetaProgressionCurrencies { get; private set; }

	public Dictionary<Enums.CraftingResource, int> CraftingResources { get; private set; }

	public CurrencySaveState()
	{
		Init();
	}

	public void ValidateAndFix()
	{
		if (MetaProgressionCurrencies == null)
		{
			MetaProgressionCurrencies = new Dictionary<Enums.CurrencyType, int>();
			MetaProgressionCurrencies.Add(Enums.CurrencyType.TitanSouls, 0);
		}
		if (CraftingResources != null)
		{
			return;
		}
		CraftingResources = new Dictionary<Enums.CraftingResource, int>();
		foreach (Enums.CraftingResource value in Enum.GetValues(typeof(Enums.CraftingResource)))
		{
			CraftingResources.Add(value, 0);
		}
	}

	public void Init()
	{
		MetaProgressionCurrencies = new Dictionary<Enums.CurrencyType, int>();
		MetaProgressionCurrencies.Add(Enums.CurrencyType.TitanSouls, 0);
		CraftingResources = new Dictionary<Enums.CraftingResource, int>();
		foreach (Enums.CraftingResource value in Enum.GetValues(typeof(Enums.CraftingResource)))
		{
			CraftingResources.Add(value, 0);
		}
	}

	public void SetState(Dictionary<Enums.CurrencyType, int> metaProgressionCurrencies, Dictionary<Enums.CraftingResource, int> craftingResources)
	{
		MetaProgressionCurrencies = metaProgressionCurrencies;
		CraftingResources = craftingResources;
	}

	public int GetCurrencyValue(Enums.CurrencyType currencyType)
	{
		if (!MetaProgressionCurrencies.ContainsKey(currencyType))
		{
			MetaProgressionCurrencies.Add(currencyType, 0);
		}
		return MetaProgressionCurrencies[currencyType];
	}

	public int GetCraftingResourceValue(Enums.CraftingResource craftingResource)
	{
		if (!CraftingResources.ContainsKey(craftingResource))
		{
			CraftingResources.Add(craftingResource, 0);
		}
		return CraftingResources[craftingResource];
	}

	public override bool HasData()
	{
		if (MetaProgressionCurrencies == null || !MetaProgressionCurrencies.Any((KeyValuePair<Enums.CurrencyType, int> x) => x.Value > 0))
		{
			if (CraftingResources != null)
			{
				return CraftingResources.Any((KeyValuePair<Enums.CraftingResource, int> x) => x.Value > 0);
			}
			return false;
		}
		return true;
	}
}
