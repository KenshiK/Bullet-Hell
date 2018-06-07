using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(BulletPattern))]
public class BulletPatternEditor : Editor {

    private SerializedObject m_Object;

    private SerializedProperty bulletArrays;
    private SerializedProperty speedChange;
    private SerializedProperty spinReversal;
    private SerializedProperty spinSpeed;
    private SerializedProperty maxSpinSpeed;
    private SerializedProperty direction;
    private SerializedProperty easeMode;
    private SerializedProperty aimAtPlayer;
    private SerializedProperty straightShot;
    private SerializedProperty offset;

    private Dictionary<string, SerializedProperty> positiveProperties;
    // Use this for initialization
    public void OnEnable()
    {
        m_Object = new SerializedObject(target);
        positiveProperties = new Dictionary<string, SerializedProperty>();

        bulletArrays = m_Object.FindProperty("bulletArrays");
        speedChange = m_Object.FindProperty("speedChange");
        spinReversal = m_Object.FindProperty("spinReversal");
        direction = m_Object.FindProperty("direction");
        easeMode = m_Object.FindProperty("easeMode");
        aimAtPlayer = m_Object.FindProperty("aimAtPlayer");
        offset = m_Object.FindProperty("offset");
        straightShot = m_Object.FindProperty("straightShot");
        positiveProperties.Add("timeToLive", m_Object.FindProperty("timeToLive"));
        positiveProperties.Add("bulletsPerArray", m_Object.FindProperty("bulletsPerArray"));
        positiveProperties.Add("origin", m_Object.FindProperty("origin"));
        positiveProperties.Add("arrayBulletSpread", m_Object.FindProperty("arrayBulletSpread"));
        positiveProperties.Add("arraySpread", m_Object.FindProperty("arraySpread"));
        positiveProperties.Add("spinSpeed",m_Object.FindProperty("spinSpeed"));
        positiveProperties.Add("timeToLerp", m_Object.FindProperty("timeToLerp"));
        positiveProperties.Add("maxSpinSpeed", m_Object.FindProperty("maxSpinSpeed"));
        positiveProperties.Add("fireRate", m_Object.FindProperty("fireRate"));
        positiveProperties.Add("bulletSpeed", m_Object.FindProperty("bulletSpeed"));
        positiveProperties.Add("cycles", m_Object.FindProperty("cycles"));
        positiveProperties.Add("spawnRadius", m_Object.FindProperty("spawnRadius"));
    }

    public override void OnInspectorGUI()
    {
        m_Object.Update();
        GUILayout.Label("Bullet Pattern Creator", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(positiveProperties["timeToLive"]);
        EditorGUILayout.PropertyField(direction);
        EditorList.Show(bulletArrays);

        EditorGUILayout.PropertyField(positiveProperties["bulletsPerArray"]);

        EditorGUILayout.PropertyField(offset);
        GUILayout.Label("Origin");
        positiveProperties["origin"].floatValue = EditorGUILayout.Slider(positiveProperties["origin"].floatValue, 0, 360);
        GUILayout.Label("Spawn Radius");
        positiveProperties["spawnRadius"].floatValue = EditorGUILayout.Slider(positiveProperties["spawnRadius"].floatValue, 0, 100);

        GUILayout.Label("Array Bullet Spread");
        positiveProperties["arrayBulletSpread"].floatValue = EditorGUILayout.Slider(positiveProperties["arrayBulletSpread"].floatValue, 0, 360);

        GUILayout.Label("Array Spread");
        positiveProperties["arraySpread"].floatValue = EditorGUILayout.Slider(positiveProperties["arraySpread"].floatValue, 0, 360);


        EditorGUILayout.PropertyField(straightShot);
        EditorGUILayout.PropertyField(positiveProperties["spinSpeed"]);

        
        EditorGUILayout.PropertyField(speedChange);
        if (positiveProperties["spinSpeed"].floatValue == 0 && speedChange.boolValue == false)
        {
            EditorGUILayout.PropertyField(aimAtPlayer);
        }
        if (speedChange.boolValue == true)
        {
            EditorGUILayout.PropertyField(positiveProperties["timeToLerp"]);
            EditorGUILayout.PropertyField(easeMode);
            EditorGUILayout.PropertyField(positiveProperties["maxSpinSpeed"]);
            EditorGUILayout.PropertyField(spinReversal);
            if (spinReversal.boolValue == true)
            {
                EditorGUILayout.PropertyField(positiveProperties["cycles"]);
            }
        }
        
        EditorGUILayout.PropertyField(positiveProperties["fireRate"]);

        EditorGUILayout.PropertyField(positiveProperties["bulletSpeed"]);

        CheckPositiveProperties();
        m_Object.ApplyModifiedProperties();
    }
    
    private void CheckPositiveProperties()
    {
        foreach(KeyValuePair<string, SerializedProperty> prop in positiveProperties)
        {
            if(prop.Key == "maxSpinSpeed")
            {
                if(prop.Value.floatValue < positiveProperties["spinSpeed"].floatValue)
                {
                    prop.Value.floatValue = positiveProperties["spinSpeed"].floatValue;
                }
            }
            else if(prop.Value.propertyType == SerializedPropertyType.Integer)
            {
                if(prop.Value.intValue < 0)
                {
                    prop.Value.intValue = 0;
                }
            }
            else if(prop.Value.propertyType == SerializedPropertyType.Float)
            {
                if (prop.Value.floatValue < 0)
                {
                    prop.Value.floatValue = 0;
                }
            }
        }
    }
}


