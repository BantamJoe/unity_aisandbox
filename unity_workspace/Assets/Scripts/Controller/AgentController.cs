﻿using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AgentController : MonoBehaviour
{

	// movement
	[SerializeField]
	private float m_moveForwardSpeed = 10.0f;
	[SerializeField]
	private float m_moveBackwardSpeed = 7.0f;
	[SerializeField]
	private float m_moveSidewaysSpeed = 6.0f;
	private Vector3 m_movementStep = Vector3.zero;

	// rotation
	[SerializeField]
	private float m_rotateSpeed = 360.0f;
	private float m_rotationStep = 0.0f;

	// movement acceleration, deceleration
	[SerializeField]
	private float m_movementAccelerationFactor = 1.0f;
	[SerializeField]
	private float m_movementDecelerationFactor = 1.0f;
	private float m_movementAcceleration = 0.0f;

	// rotation acceleration, deceleration
	[SerializeField]
	private float m_rotationAccelerationFactor = 1.0f;
	[SerializeField]
	private float m_rotationDecelerationFactor = 1.0f;
	private float m_rotationAcceleration = 0.0f;

	// --------------------------------------------------------------------------------

	// cached variables
	private Transform m_transform = null;
	private CharacterController m_characterController = null;

	// --------------------------------------------------------------------------------

	protected virtual void Awake()
	{
		m_transform = GetComponent<Transform>();
		m_characterController = GetComponent<CharacterController>();
	}

	// --------------------------------------------------------------------------------

	protected virtual void Update()
	{
		if (m_characterController != null)
		{
			// movement acceleration
			if (m_movementStep.sqrMagnitude > 0.0f)
			{
				if (m_movementAcceleration < 1.0f)
				{
					m_movementAcceleration = Mathf.Clamp(m_movementAcceleration + Time.deltaTime * m_movementAccelerationFactor, 0.0f, 1.0f);
				}
			}
			else
			{
				// #SteveD >>> Deceleration is immediate as m_movementStep is reset every frame. Requires a cached version to be used
				// when no input is received
				if (m_movementAcceleration > 0.0f)
				{
					m_movementAcceleration = Mathf.Clamp(m_movementAcceleration - Time.deltaTime * m_movementDecelerationFactor, 0.0f, 1.0f);
				}
			}

			// move
			m_characterController.Move(m_movementStep * m_movementAcceleration);
			m_movementStep.Set(0.0f, 0.0f, 0.0f);

			// rotation acceleration
			if (m_rotationStep != 0.0f)
			{
				if (m_rotationAcceleration < 1.0f)
				{
					m_rotationAcceleration = Mathf.Clamp(m_rotationAcceleration + Time.deltaTime * m_rotationAccelerationFactor, 0.0f, 1.0f);
				}
			}
			else
			{
				// #SteveD >>> Deceleration is immediate as m_rotationStep is reset every frame. Requires a cached version to be used
				// when no input is received
				if (m_rotationAcceleration > 0.0f)
				{
					m_rotationAcceleration = Mathf.Clamp(m_rotationAcceleration - Time.deltaTime * m_rotationDecelerationFactor, 0.0f, 1.0f);
				}
			}

			// rotate
			m_transform.Rotate(0.0f, m_rotationStep * m_rotationAcceleration, 0.0f);
			m_rotationStep = 0.0f;
		}
	}

	// --------------------------------------------------------------------------------

	public void MoveForward(float value)
	{
		if (value > 0.0f)
		{
			m_movementStep += m_transform.forward * value * m_moveForwardSpeed * Time.deltaTime;
		}
		else
		{
			m_movementStep += m_transform.forward * value * m_moveBackwardSpeed * Time.deltaTime;
		}
	}

	// --------------------------------------------------------------------------------

	public void MoveSideways(float value)
	{
		m_movementStep += m_transform.right * value * m_moveSidewaysSpeed * Time.deltaTime;
	}

	// --------------------------------------------------------------------------------

	public void Rotate(float value)
	{
		m_rotationStep += value * m_rotateSpeed * Time.deltaTime;
	}

	// --------------------------------------------------------------------------------

	protected virtual void OnDrawGizmos()
	{
		if (m_transform == null)
		{
			return;
		}

		Color originalColor = Gizmos.color;

		// forward, z
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.forward * 2.0f);

		// right, x
		Gizmos.color = Color.red;
		Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.right * 2.0f);

		// up, y
		Gizmos.color = Color.green;
		Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.up * 2.0f);

		Gizmos.color = originalColor;
	}

}
