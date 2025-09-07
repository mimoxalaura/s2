using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

internal class RaycastColliderHelper : MonoBehaviour
{
	[SerializeField]
	private Collider2D _collider;

	[SerializeField]
	private LayerMask _layerMaskToTriggerOn;

	private ContactFilter2D _contactFilter;

	internal Collider2D Collider2D => _collider;

	internal bool GetHitTargets(out List<Character> charactersHit)
	{
		charactersHit = new List<Character>();
		_contactFilter = default(ContactFilter2D);
		_contactFilter.SetLayerMask(_layerMaskToTriggerOn);
		_contactFilter.useLayerMask = true;
		List<Collider2D> list = new List<Collider2D>();
		if (_collider.OverlapCollider(_contactFilter, list) > 0)
		{
			foreach (Collider2D item in list)
			{
				Character component = item.gameObject.GetComponent<Character>();
				if (!(component == null))
				{
					charactersHit.Add(component);
				}
			}
			return true;
		}
		return false;
	}
}
