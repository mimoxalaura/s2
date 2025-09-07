using UnityEngine;

namespace IndieImpulseAssets;

public class DemoSlide : MonoBehaviour
{
	public GameObject[] Effects;

	private int index;

	private void Start()
	{
		for (int i = 1; i < Effects.Length; i++)
		{
			Effects[i].SetActive(value: false);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ChangeEffect();
			index = (index + 1) % Effects.Length;
		}
	}

	private void ChangeEffect()
	{
		Effects[index].SetActive(value: false);
		Effects[(index + 1) % Effects.Length].SetActive(value: true);
	}
}
