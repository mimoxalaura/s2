using BackpackSurvivors.Game.Backpack;
using UnityEngine;

namespace BackpackSurvivors.DEBUG.Backpack;

public class DEBUG_CellCreator : MonoBehaviour
{
	[SerializeField]
	private Transform _gridParent;

	[SerializeField]
	private GameObject _cellPrefab;

	[SerializeField]
	private int _numberOfCellsToCreate;

	private void Start()
	{
		for (int i = 0; i < _numberOfCellsToCreate; i++)
		{
			Object.Instantiate(_cellPrefab, _gridParent).GetComponent<BackpackCell>().Init(i);
		}
	}
}
