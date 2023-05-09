#region
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[CreateAssetMenu(fileName = "Data Type Container", menuName = "Containers/Create DataTypeContainer")]
public class DataTypeContainer : ScriptableObject
{
    [SerializeField] List<EnemyDataType> dataTypes = new();

    public List<EnemyDataType> DamageTypes
    {
        get => dataTypes;
        set => dataTypes = value;
    }

#if UNITY_EDITOR
    [ContextMenu("Make New Damage Type")]
    public void MakeNewDamageType()
    {
        var damageType = CreateInstance<EnemyDataType>();
        damageType.name = "New Damage Type";
        damageType.Initialise(this);
        dataTypes.Add(damageType);

        AssetDatabase.AddObjectToAsset(damageType, this);
        AssetDatabase.SaveAssets();

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(damageType);
    }

#endif

#if UNITY_EDITOR
    [ContextMenu("Delete All Damage Types")]
    public void DeleteAllDamageTypes()
    {
        for (int i = dataTypes.Count; i-- > 0;)
        {
            EnemyDataType tmp = dataTypes[i];

            dataTypes.Remove(tmp);
            Undo.DestroyObjectImmediate(tmp);
        }

        AssetDatabase.SaveAssets();
    }
#endif
}

[CustomEditor(typeof(DataTypeContainer))]
public class DataTypeContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var dataTypeContainer = (DataTypeContainer)target;

        //TODO: rename all "damage" types to data type, and redo script to support it.
        if (GUILayout.Button("Make New Damage Type", GUILayout.Height(30)))
        {
            dataTypeContainer.MakeNewDamageType();
        }

        if (GUILayout.Button("Delete All Damage Types", GUILayout.Height(30)))
        {
            dataTypeContainer.DeleteAllDamageTypes();
        }

        DrawDefaultInspector();
    }
}