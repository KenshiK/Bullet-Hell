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

    private Dictionary<string, SerializedProperty> positiveProperties;
    // Use this for initialization
    public void OnEnable()
    {
        m_Object = new SerializedObject(target);
        positiveProperties = new Dictionary<string, SerializedProperty>();

        bulletArrays = m_Object.FindProperty("bulletArrays");
        speedChange = m_Object.FindProperty("speedChange");
        spinReversal = m_Object.FindProperty("spinReversal");

        positiveProperties.Add("bulletsPerArray", m_Object.FindProperty("bulletsPerArray"));
        positiveProperties.Add("arrayBulletSpread", m_Object.FindProperty("arrayBulletSpread"));
        positiveProperties.Add("arraySpread", m_Object.FindProperty("arraySpread"));
        positiveProperties.Add("spinSpeed",m_Object.FindProperty("spinSpeed"));
        positiveProperties.Add("maxSpinSpeed", m_Object.FindProperty("maxSpinSpeed"));
        positiveProperties.Add("fireRate", m_Object.FindProperty("fireRate"));
        positiveProperties.Add("bulletSpeed", m_Object.FindProperty("bulletSpeed"));
    }

    public override void OnInspectorGUI()
    {
        m_Object.Update();

        GUILayout.Label("Bullet Pattern Creator", EditorStyles.boldLabel);
        EditorList.Show(bulletArrays);

        GUILayout.Label("Array Bullet Spread");
        positiveProperties["arrayBulletSpread"].floatValue = EditorGUILayout.Slider(positiveProperties["arrayBulletSpread"].floatValue, 0, 360);

        GUILayout.Label("Array Spread");
        positiveProperties["arraySpread"].floatValue = EditorGUILayout.Slider(positiveProperties["arraySpread"].floatValue, 0, 360);

        EditorGUILayout.PropertyField(positiveProperties["spinSpeed"]);
        EditorGUILayout.PropertyField(speedChange);
        if(speedChange.boolValue == true)
        {
            EditorGUILayout.PropertyField(spinReversal);
            EditorGUILayout.PropertyField(positiveProperties["maxSpinSpeed"]);
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


