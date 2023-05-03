using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponDefinition))]
public class WeaponDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var weaponDefinition = (WeaponDefinition)target;

        switch (weaponDefinition.weaponClass)
        {
            // Shows the variables for the slashing weapon type.
            case WeaponDefinition.WeaponClass.Melee:
            {
                SerializedProperty meleeSerializedProperty = serializedObject.FindProperty("animationSpeedScalar");
                EditorGUILayout.PropertyField(meleeSerializedProperty);
                meleeSerializedProperty = serializedObject.FindProperty("size");
                EditorGUILayout.PropertyField(meleeSerializedProperty);
                break;
            }

            case WeaponDefinition.WeaponClass.Ranged:
            {
                SerializedProperty rangedSerializedProperty = serializedObject.FindProperty("bulletSpeed");
                EditorGUILayout.PropertyField(rangedSerializedProperty);
                rangedSerializedProperty = serializedObject.FindProperty("bulletSize");
                EditorGUILayout.PropertyField(rangedSerializedProperty);
                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }

        // Apply any changes made to the serialized object.
        serializedObject.ApplyModifiedProperties();
    }
}