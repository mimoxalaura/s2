using UnityEngine;

namespace BackpackSurvivors.Game.Level;

internal class WorldBounds : MonoBehaviour
{
	internal float LeftX { get; private set; }

	internal float RightX { get; private set; }

	internal float TopY { get; private set; }

	internal float BottomY { get; private set; }

	private void Start()
	{
		InitBounds();
	}

	public Vector2 MovePositionWithinWorldBounds(Vector2 position)
	{
		float x = Mathf.Min(Mathf.Max(position.x, LeftX), RightX);
		float a = Mathf.Min(position.y, TopY);
		a = Mathf.Max(a, BottomY);
		return new Vector2(x, a);
	}

	private void InitBounds()
	{
		LeftX = base.transform.position.x - base.transform.localScale.x / 2f;
		RightX = base.transform.position.x + base.transform.localScale.x / 2f;
		TopY = base.transform.position.y + base.transform.localScale.y / 2f;
		BottomY = base.transform.position.y - base.transform.localScale.y / 2f;
	}
}
