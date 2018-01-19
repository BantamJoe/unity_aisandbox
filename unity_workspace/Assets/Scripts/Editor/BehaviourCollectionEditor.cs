﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AIBehaviourCollection), true)]
public class BehaviourCollectionEditor : Editor
{

	private AIBehaviourCollection m_collection = null;
	
	// --------------------------------------------------------------------------------

	public void OnEnable()
	{
		m_collection = target as AIBehaviourCollection;
	}

	// --------------------------------------------------------------------------------

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		DrawDefaultInspector();

		if (m_collection != null)
		{
			// cache current behaviour Id
			AIBehaviourId activeBehaviourId = AIBehaviourId.None;
			if (m_collection.Editor_CurrentBehaviour != null)
			{
				activeBehaviourId = m_collection.Editor_CurrentBehaviour.BehaviourId;
			}

			if (m_collection.Editor_Behaviours.Count > 0)
			{
				GUILayout.Space(6);
				GUILayout.Label("Behaviours:", EditorStyles.boldLabel);

				// cache current background colour
				Color cachedColor = GUI.backgroundColor;
				
				// create a label style for behaviours
				GUIStyle behaviourTextFieldStyle = new GUIStyle(GUI.skin.textField)
				{
					fontStyle = FontStyle.Italic,
				};

				//EditorGUI.BeginDisabledGroup(true);

				// list of all behaviours
				foreach (var behaviour in m_collection.Editor_Behaviours.Values)
				{
					// set content colour if this is our active behaviour
					if (behaviour.BehaviourId == activeBehaviourId)
					{
						GUI.backgroundColor = Color.green;
					}
					EditorGUILayout.TextField(behaviour.BehaviourId.ToString(), behaviourTextFieldStyle);
					
					// reset background colour
					GUI.backgroundColor = cachedColor;
				}

				//EditorGUI.EndDisabledGroup();
			}

			GUILayout.Space(6);

			// disable buttons if we're in not playing
			EditorGUI.BeginDisabledGroup(Application.isPlaying == false);
			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Reset Current Behaviour"))
			{
				m_collection.Editor_ResetCurrentBehaviour();
			}
			if (GUILayout.Button("Hard Reset"))
			{
				m_collection.Editor_ResetCollection();
			}

			GUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
		}

		serializedObject.ApplyModifiedProperties();
	}

}