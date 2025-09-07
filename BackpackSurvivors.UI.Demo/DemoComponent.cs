using BackpackSurvivors.Game.Core;
using UnityEngine;

namespace BackpackSurvivors.UI.Demo;

public class DemoComponent : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(GameDatabase.IsDemo);
	}
}
