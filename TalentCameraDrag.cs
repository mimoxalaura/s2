using UnityEngine;
using UnityEngine.UI;

public class TalentCameraDrag : MonoBehaviour
{
	[SerializeField]
	private float _dragSpeed = 1f;

	[SerializeField]
	private float zoomSpeed = 1f;

	[SerializeField]
	private float minZoom = 3f;

	[SerializeField]
	private float maxZoom = 10f;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private Slider _zoomSlider;

	[SerializeField]
	private Vector2 _minDragVector;

	[SerializeField]
	private Vector2 _maxDragVector;

	private Vector3 lastMousePosition;

	private bool isDragging;

	private void Start()
	{
		_zoomSlider.onValueChanged.AddListener(OnSliderValueChanged);
	}

	private void OnSliderValueChanged(float value)
	{
	}

	private void Update()
	{
		HandleDrag();
		HandleZoom();
	}

	internal void Reset()
	{
		isDragging = false;
	}

	private void HandleDrag()
	{
		if (Input.GetMouseButtonDown(0))
		{
			isDragging = true;
			lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			lastMousePosition.z = base.transform.position.z;
		}
		if (Input.GetMouseButtonUp(0))
		{
			isDragging = false;
			lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			lastMousePosition.z = base.transform.position.z;
		}
		if (isDragging)
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vector.z = base.transform.position.z;
			Vector3 vector2 = lastMousePosition - vector;
			Vector3 vector3 = base.transform.position + vector2 * _dragSpeed;
			vector3 = new Vector3(Mathf.Clamp(vector3.x, _minDragVector.x, _maxDragVector.x), Mathf.Clamp(vector3.y, _minDragVector.y, _maxDragVector.y), base.transform.position.z);
			lastMousePosition = vector;
			base.transform.position = vector3;
		}
	}

	private void HandleZoom()
	{
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis > 0f || axis < 0f)
		{
			_zoomSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
			float value = base.transform.position.z + axis * zoomSpeed;
			value = Mathf.Clamp(value, minZoom, maxZoom);
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, value);
		}
	}
}
