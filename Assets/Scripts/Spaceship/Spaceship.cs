using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spaceship : MonoBehaviour {

    #region PrivateData
    private int _health;
    #endregion

    #region PublicData
    public int Health {
        get { return _health; }
        set { _health = Mathf.Max(0, value); }
    }
    #endregion

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ApplyDamage()
    {

    }
}

[CustomEditor(typeof(Spaceship))]
public class SpaceshipEditor : Editor
{

    Spaceship spaceship;

    public void OnEnable()
    {
        spaceship = target as Spaceship;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Health"));
        spaceship.Health = EditorGUILayout.IntField(spaceship.Health);
        EditorGUILayout.EndHorizontal();
    }
}
