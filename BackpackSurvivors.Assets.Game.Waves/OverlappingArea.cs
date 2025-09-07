namespace BackpackSurvivors.Assets.Game.Waves;

internal class OverlappingArea
{
	public float MinX { get; private set; }

	public float MaxX { get; private set; }

	public float MinY { get; private set; }

	public float MaxY { get; private set; }

	public bool IsValid { get; private set; }

	public OverlappingArea(float minX, float maxX, float minY, float maxY, bool isValid = true)
	{
		MinX = minX;
		MaxX = maxX;
		MinY = minY;
		MaxY = maxY;
		IsValid = isValid;
	}

	public float GetAreaSize()
	{
		float num = MaxX - MinX;
		float num2 = MaxY - MinY;
		return num * num2;
	}
}
