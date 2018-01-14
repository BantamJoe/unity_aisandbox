﻿using UnityEngine;

public class Behaviour_Evade : Behaviour
{

	private static readonly Vector3 k_vectorUp = new Vector3(0.0f, 1.0f, 0.0f);

	// --------------------------------------------------------------------------------

	public override BehaviourId BehaviourId { get { return BehaviourId.Evade; } }
	public override GoalFlags AchievedGoals { get { return GoalFlags.Escape; } }
	public override GoalFlags PrerequisiteGoals { get { return GoalFlags.None; } }

	// --------------------------------------------------------------------------------

	[SerializeField]
	private float m_triggerDistance = 5.0f;
	
	[SerializeField] 
	private float m_successDistance = 6.0f;

	[SerializeField]
	private float m_minimumAngleForMovement = 45.0f;

	[SerializeField]
	private float m_isFacingAwayAngle = 5.0f;

	[SerializeField]
	private float m_toTurnAngleScalar = 0.1f;

	// --------------------------------------------------------------------------------

	private Agent m_cachedTarget = null;
	private Transform m_cachedTargetTransform = null;

	private bool m_evading = false;
	private float m_triggerDistanceSquared = 0.0f;
	private float m_successDistanceSquared = 0.0f;
	private float m_toTargetSquared = float.MaxValue;

	// --------------------------------------------------------------------------------

	protected override void OnValidate()
	{
		m_triggerDistanceSquared = m_triggerDistance * m_triggerDistance;
		m_successDistanceSquared = m_successDistance * m_successDistance;
	}

	// --------------------------------------------------------------------------------

	public override void OnStart(Agent owner, WorkingMemory workingMemory)
	{
		base.OnStart(owner, workingMemory);

		m_triggerDistanceSquared = m_triggerDistance * m_triggerDistance;
		m_successDistanceSquared = m_successDistance * m_successDistance;
	}

	// --------------------------------------------------------------------------------

	public override void OnEnter()
	{
		if (m_workingMemory != null)
		{
			m_workingMemory.SortTargets();

			m_cachedTarget = m_workingMemory.GetHighestPriorityTarget();
			if (m_cachedTarget != null)
			{
				m_cachedTargetTransform = m_cachedTarget.transform;
			}
		}
	}

	// --------------------------------------------------------------------------------

	public override void OnExit()
	{
		;
	}

	// --------------------------------------------------------------------------------

	public override void OnUpdate()
	{
		if (m_owner == null)
		{
			Debug.LogError("[Behaviour_Pursue] Unable to update >>> owner is null\n");
			return;
		}

		if (m_ownerTransform == null)
		{
			Debug.LogError("[Behaviour_Pursue] Unable to update >>> owner transform is null\n");
			return;
		}

		if (m_cachedTarget == null)
		{
			Debug.LogError("[Behaviour_Pursue] Unable to update >>> cached target is null\n");
			return;
		}

		if (m_cachedTargetTransform == null)
		{
			Debug.LogError("[Behaviour_Pursue] Unable to update >>> cached target transform is null\n");
			return;
		}

		AgentController controller = m_owner.AgentController;
		if (controller == null)
		{
			Debug.LogError("[Behaviour_Pursue] Unable to update >>> owner AgentController is null\n");
			return;
		}

		// vector away from target
		Vector3 toTarget = m_cachedTargetTransform.position - m_ownerTransform.position;
		m_toTargetSquared = toTarget.sqrMagnitude;

		if (m_evading || m_toTargetSquared <= m_triggerDistanceSquared)
		{
			// evade
			float escapeAngle = Vector3.SignedAngle(-m_ownerTransform.forward, toTarget, k_vectorUp);
			float absEscapeAngle = Mathf.Abs(escapeAngle);

			// rotate away from target
			if (absEscapeAngle >= m_isFacingAwayAngle)
			{
				controller.Rotate(escapeAngle * m_toTurnAngleScalar);
			}

			// move away from target
			if (absEscapeAngle <= m_minimumAngleForMovement)
			{
				controller.MoveForward(1.0f);
			}

			// deactivate if reached success distance
			m_evading = m_toTargetSquared <= m_successDistanceSquared;
		}
	}

	// --------------------------------------------------------------------------------

	public override bool IsGoalAchieved()
	{
		return m_toTargetSquared >= m_successDistanceSquared;
	}

	// --------------------------------------------------------------------------------

	public override bool IsGoalInvalid()
	{
		return m_owner == null ||
			m_ownerTransform == null ||
			m_owner.AgentController == null ||
			m_cachedTarget == null ||
			m_cachedTargetTransform == null;
	}

}
