using UnityEditor;
using UnityEngine;

public static class EditorList
{

    public static void Show(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel += 1;
        if (list.isExpanded)
        {
            EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty prop = list.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(prop, true);
                if (prop.isArray)
                {
                    EditorList.Show(prop);
                }
            }
        }
        EditorGUI.indentLevel -= 1;
    }
}
