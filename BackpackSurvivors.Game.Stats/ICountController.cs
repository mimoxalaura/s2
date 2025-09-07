using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Stats;

public interface ICountController
{
	int GetPlaceableTypeCount(Enums.PlaceableType placeableType);

	int GetPlaceableTagCount(Enums.PlaceableTag placeableTag);

	int GetPlaceableRarityCount(Enums.PlaceableRarity placeableRarity);

	void UpdateCounts();
}
