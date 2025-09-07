using System;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Canvas))]
public class InteractionZone : MonoBehaviour
{
	public delegate void OnInteractionZoneEnteredHandler(object sender, EventArgs e);

	public delegate void OnInteractionZoneExitedHandler(object sender, EventArgs e);

	[SerializeField]
	private Interaction _interaction;

	public event OnInteractionZoneEnteredHandler OnInteractionZoneEntered;

	public event OnInteractionZoneExitedHandler OnInteractionZoneExited;

	private void Start()
	{
		GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_interaction.CanInteract && collision.GetComponent<InteractingEntity>() != null)
		{
			_interaction.DoInRange();
			this.OnInteractionZoneEntered?.Invoke(this, new EventArgs());
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (_interaction.CanInteract && collision.GetComponent<InteractingEntity>() != null)
		{
			_interaction.DoOutOfRange();
			this.OnInteractionZoneExited?.Invoke(this, new EventArgs());
		}
	}
}
