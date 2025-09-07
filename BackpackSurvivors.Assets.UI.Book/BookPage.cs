using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class BookPage : MonoBehaviour
{
	[SerializeField]
	private Transform _contentLeftContainer;

	protected Transform ContentLeftContainer => _contentLeftContainer;

	internal virtual void InitPage()
	{
	}
}
