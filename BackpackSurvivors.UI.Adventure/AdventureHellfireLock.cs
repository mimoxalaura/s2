using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureHellfireLock : MonoBehaviour
{
	[SerializeField]
	private Image _line;

	[SerializeField]
	private Image _bar;

	[SerializeField]
	private Image _lock;

	public void SetLock(bool locked)
	{
		if (_line != null)
		{
			_line.gameObject.SetActive(locked);
		}
		if (_line != null)
		{
			_bar.gameObject.SetActive(locked);
		}
		if (_line != null)
		{
			_lock.gameObject.SetActive(locked);
		}
	}
}
