using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spaceship))]
public class SpaceshipEditor : Editor
{

    private SerializedObject spaceship;

    private SerializedProperty health;

    public void OnEnable()
    {
        spaceship = new SerializedObject(target);
        health = spaceship.FindProperty("_health");
    }

    public override void OnInspectorGUI()
    {
        spaceship.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(health, true);
        EditorGUILayout.EndHorizontal();

        spaceship.ApplyModifiedProperties();
    }
}
