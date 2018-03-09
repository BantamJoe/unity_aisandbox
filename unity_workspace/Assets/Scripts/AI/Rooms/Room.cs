﻿using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class Room : MonoBehaviour
{
	
	// #SteveD	>>> room portal (connection between rooms) - automate/menu tool

	// #SteveD	>>> room manager showing all rooms & inhabitants

	private List<BoxCollider> m_colliders = new List<BoxCollider>();

	private List<Agent> m_inhabitants = new List<Agent>();
	public List<Agent>.Enumerator InhabitantsEnumerator { get { return m_inhabitants.GetEnumerator(); } }

	// --------------------------------------------------------------------------------

	public delegate void AgentMigration(Agent agent, Room room);
	public event AgentMigration OnAgentEnter;
	public event AgentMigration OnAgentExit;

	// --------------------------------------------------------------------------------

	protected virtual void Awake()
	{
		GetComponents(m_colliders);
		Debug.Assert(m_colliders.Count > 0, "[Room] has no colliders");
	}

	// --------------------------------------------------------------------------------

	protected virtual void OnTriggerEnter(Collider collider)
	{
		Agent agent = collider.GetComponentInParent<Agent>();
		if (agent != null && m_inhabitants.Contains(agent) == false)
		{
			m_inhabitants.Add(agent);
			if (OnAgentEnter != null)
			{
				OnAgentEnter(agent, this);
			}

			Logger.Instance.Log(GetType().ToString(), LogLevel.Info, string.Format("Agent [{0}] entered room [{1}]", agent.name, name));

#if UNITY_EDITOR
			if (OnRequestRepaint != null)
			{
				OnRequestRepaint();
			}
#endif
		}
	}

	// --------------------------------------------------------------------------------

	protected virtual void OnTriggerExit(Collider collider)
	{
		Agent agent = collider.GetComponentInParent<Agent>();
		if (agent != null && m_inhabitants.Contains(agent))
		{
			if (agent.Collider != null)
			{
				for (int i = 0; i < m_colliders.Count; ++i)
				{
					if (m_colliders[i].bounds.Contains(agent.Transform.position))
					{
						Logger.Instance.Log(GetType().ToString(), LogLevel.Info, string.Format("Agent [{0}] exit room [{1}] failed as we're still within one of it's colliders", agent.name, name));
						return;
					}
				}
			}
			
			m_inhabitants.Remove(agent);
			if (OnAgentExit != null)
			{
				OnAgentExit(agent, this);
			}

			Logger.Instance.Log(GetType().ToString(), LogLevel.Info, string.Format("Agent [{0}] exited room [{1}]", agent.name, name));

#if UNITY_EDITOR
			if (OnRequestRepaint != null)
			{
				OnRequestRepaint();
			}
#endif
		}
	}

	// --------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------

#if UNITY_EDITOR

	public delegate void RequestRepaint();
	public event RequestRepaint OnRequestRepaint;

	// --------------------------------------------------------------------------------

	protected virtual void OnDrawGizmos()
	{
		DoDrawGizmos();
	}

	// --------------------------------------------------------------------------------

	public virtual void DoDrawGizmos()
	{
		Color cachedColour = Gizmos.color;

		Color color = Color.green;
		color.a = 0.25f;
		Gizmos.color = color;

		Matrix4x4 cachedMatrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.Rotate(transform.rotation);

		for (int i = 0; i < m_colliders.Count; ++i)
		{
			Vector3 pos = transform.position;
			pos.x += m_colliders[i].center.x;
			pos.z += m_colliders[i].center.z;

			Gizmos.DrawCube(pos, new Vector3(m_colliders[i].size.x, 0.01f, m_colliders[i].size.z));
		}
		
		Gizmos.matrix = cachedMatrix;
		Gizmos.color = cachedColour;
	}

#endif // UNITY_EDITOR

}
