using UnityEngine;

namespace BackpackSurvivors.UI.Adventure;

public class MoveAlongPath : MonoBehaviour
{
	public bool getTransformsFromParent;

	public bool deactivateTransforms = true;

	public bool reversePath;

	public GameObject parentTransform;

	public Transform[] transforms;

	public Vector3[] path;

	public bool randomizePath;

	public Vector3 randomizeMin;

	public Vector3 randomizeMax;

	public GameObject toAnimate;

	public float startingTime;

	public float speed = 6f;

	public float speedRandomness;

	private LTSpline visualizePath;

	public LeanTweenType loopType;

	public LeanTweenType easeType;

	public void SetPath(Transform pathParent)
	{
		speed += Random.Range(0f - speedRandomness, speedRandomness);
		if (getTransformsFromParent)
		{
			transforms = pathParent.GetComponentsInChildren<Transform>();
		}
		path = new Vector3[transforms.Length - 1];
		if (reversePath)
		{
			for (int num = transforms.Length - 1; num > 1; num--)
			{
				path[path.Length - num] = transforms[num].position;
			}
		}
		else
		{
			for (int i = 1; i < transforms.Length; i++)
			{
				path[i - 1] = transforms[i].position;
			}
		}
		if (randomizePath)
		{
			for (int j = 0; j < path.Length; j++)
			{
				if (path[j] != toAnimate.transform.position)
				{
					path[j] += new Vector3(Random.Range(randomizeMin.x, randomizeMax.x), Random.Range(randomizeMin.y, randomizeMax.y), Random.Range(randomizeMin.z, randomizeMax.z));
				}
			}
		}
		if (deactivateTransforms)
		{
			pathParent.gameObject.SetActive(value: false);
		}
		visualizePath = new LTSpline(path);
		toAnimate.transform.position = path[0];
		LeanTween.moveSpline(toAnimate, path, speed).setOrientToPath2d(doesOrient2d: true).setSpeed(speed)
			.setEase(easeType)
			.setLoopType(loopType)
			.setDelay(startingTime)
			.setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (visualizePath != null)
		{
			visualizePath.gizmoDraw();
		}
	}
}
