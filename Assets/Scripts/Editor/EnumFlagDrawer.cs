using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{

    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        /*if (Event.current.type == EventType.Layout)
        {*/
            int buttonsIntValue = 0;

            int enumLength = _property.enumNames.Length;
            int columns = 4;
            int rows = enumLength / columns;
            bool[] buttonPressed = new bool[enumLength];
            float buttonWidth = (_position.width - EditorGUIUtility.labelWidth) / columns;
            float buttonHeight = (_position.height) / rows;
            EditorGUI.LabelField(new Rect(_position.x, _position.y, EditorGUIUtility.labelWidth, _position.height), _label);
        
            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int index = j + i * rows;
                    // Check if the button is/was pressed 
                    if ((_property.intValue & (1 << index)) == 1 << index)
                    {
                        buttonPressed[index] = true;
                    }
                    Rect buttonPos = new Rect(_position.x + EditorGUIUtility.labelWidth + buttonWidth * j, _position.y + buttonHeight * i, buttonWidth, buttonHeight);

                    buttonPressed[index] = GUI.Toggle(buttonPos, buttonPressed[index], _property.enumNames[index], "Button");

                    if (buttonPressed[index])
                        buttonsIntValue += 1 << index;
                }


            }

            if (EditorGUI.EndChangeCheck())
            {
                _property.intValue = buttonsIntValue;
            }
        //}
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 60;
    }
}
