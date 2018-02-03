﻿using UnityEngine;

[DisallowMultipleComponent]
public class Perception_Smell : Perception
{

	public override PerceptionType PerceptionType { get { return PerceptionType.Smell; } }
	
	// --------------------------------------------------------------------------------

	public override void OnUpdate()
	{
		;
	}

	// --------------------------------------------------------------------------------

	protected override bool CanPercieve(PerceptionEvent percievedEvent)
	{
		if (percievedEvent == null)
		{
			return false;
		}
		return false;
	}

}
