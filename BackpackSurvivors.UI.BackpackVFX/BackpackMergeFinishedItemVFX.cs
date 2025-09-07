using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.BackpackVFX;

internal class BackpackMergeFinishedItemVFX : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private float _delay = 1f;

	[SerializeField]
	private Image _image;

	private void Start()
	{
		StartCoroutine(DestroyAfterLongDelay());
	}

	private IEnumerator DestroyAfterLongDelay()
	{
		yield return new WaitForSecondsRealtime(_delay);
		Object.Destroy(base.gameObject);
	}
}
