﻿using UnityEngine;

[DisallowMultipleComponent]
public class Perception_Vision : Perception
{
	
	[SerializeField, Range(1.0f, 180.0f)]
	private float m_horizontalFieldOfView = 65.0f;

	[SerializeField, Range(1.0f, 180.0f)]
	private float m_verticalFieldOfView = 15.0f;
	
	private Transform m_transform = null;

	// --------------------------------------------------------------------------------

	public override PerceptionType PerceptionType { get { return PerceptionType.Vision; } }
	
	// --------------------------------------------------------------------------------

	protected override void OnAwake()
	{
		m_transform = GetComponent<Transform>();
	}

	// --------------------------------------------------------------------------------

	public override void OnUpdate()
	{
		;
	}

	// --------------------------------------------------------------------------------

	protected override bool CanPercieve(PercievedEvent percievedEvent)
	{
		// validate
		if (percievedEvent == null || m_transform == false)
		{
			return false;
		}

		// #SteveD >>> range check can be moved to calling code otherwise it will be duplicated on every perception
		// #SteveD >>> modifiers on each perception could extend/reduce the range on the Perception trigger
		// #SteveD >>> move range back to trigger from percieved action

		// points of interest
		Vector3 eyePosition = m_transform.position;
		Vector3 eventPosition = percievedEvent.Actor == null ?
			percievedEvent.Location :
			percievedEvent.Actor.Transform.position;

		// vector to event
		Vector3 toEvent = eventPosition - eyePosition;
		// distance squared to event
		float toEventDistanceSquared = toEvent.sqrMagnitude;

		// fail if out of range
		float eventRangeSquared = percievedEvent.Range * percievedEvent.Range;
		if (toEventDistanceSquared > eventRangeSquared)
		{
			return false;
		}

		// fail if out of horizontal FOV
		Vector3 toEventHorizontal = toEvent;
		toEventHorizontal.y = 0.0f;
		if (Vector3.Angle(m_transform.forward, toEventHorizontal) > m_horizontalFieldOfView * 0.5f)
		{
			return false;
		}

		// fail if out of vertical FOV
		Vector3 toEventVertical = toEvent;
		toEventVertical.x = 0.0f;
		toEventVertical.z = 0.0f;
		if (Vector3.Angle(m_transform.forward, toEventVertical) > m_verticalFieldOfView * 0.5f)
		{
			return false;
		}
		
		// #SteveD >>> raycast to target
		//		>>> exclude target
		//		>>> exclude owner (always)
		//		>>> if we have 0 collisions, we have a clear path to the target event/object

		return false;
	}
	
	// --------------------------------------------------------------------------------

	protected virtual void OnDrawGizmos()
	{
		// #SteveD >>> represent view cone
	}

}