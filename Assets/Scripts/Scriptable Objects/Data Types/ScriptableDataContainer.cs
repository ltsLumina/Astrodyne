#region
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[CreateAssetMenu(fileName = "Scriptable Data Container", menuName = "Containers/Create ScriptableDataContainer")]
public class ScriptableDataContainer : ScriptableObject
{
    [SerializeField] DataType dataType;
    [SerializeField] List<ScriptableData> blankScriptableData = new();
    [SerializeField] List<EnemyData> enemyScriptableData = new();

    public List<ScriptableData> BlankScriptableData => blankScriptableData;
    public List<EnemyData> EnemyScriptableData => enemyScriptableData;

#if UNITY_EDITOR
    [ContextMenu("Create Scriptable Data")]
    public void CreateScriptableData()
    {
        switch (dataType)
        {
            case DataType.Enemy:
                var enemyData = CreateInstance<EnemyData>();
                enemyData.name = "New Enemy Data";
                enemyData.Initialise(this);
                EnemyScriptableData.Add(enemyData);

                AssetDatabase.AddObjectToAsset(enemyData, this);
                AssetDatabase.SaveAssets();

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(enemyData);
                break;


            case DataType.Blank:
                var scriptableData = CreateInstance<ScriptableData>();
                scriptableData.name = "New Scriptable Data";
                scriptableData.Initialise(this);
                blankScriptableData.Add(scriptableData);

                AssetDatabase.AddObjectToAsset(scriptableData, this);
                AssetDatabase.SaveAssets();

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(scriptableData);
                break;
        }
    }

#endif

#if UNITY_EDITOR
    [ContextMenu("Delete All Selected Scriptable Data")]
    public void DeleteAllScriptableData()
    {
        switch (dataType)
        {
            case DataType.Enemy:
                for (int i = enemyScriptableData.Count; i-- > 0;)
                {
                    EnemyData tmp = enemyScriptableData[i];

                    enemyScriptableData.Remove(tmp);
                    Undo.DestroyObjectImmediate(tmp);
                }
                break;

            case DataType.Blank:
                for (int i = blankScriptableData.Count; i-- > 0;)
                {
                    ScriptableData tmp = blankScriptableData[i];

                    blankScriptableData.Remove(tmp);
                    Undo.DestroyObjectImmediate(tmp);
                }
                break;
        }

        AssetDatabase.SaveAssets();
    }
#endif

    enum DataType
    {
        Enemy,
        Blank
    }
}

[CustomEditor(typeof(ScriptableDataContainer))]
public class DataTypeContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var dataTypeContainer = (ScriptableDataContainer)target;

        //TODO: rename all "damage" types to data type, and redo script to support it.
        if (GUILayout.Button("Create Selected Data Type", GUILayout.Height(30)))
        {
            dataTypeContainer.CreateScriptableData();
        }

        if (GUILayout.Button("Delete Selected Data Types", GUILayout.Height(30)))
        {
            dataTypeContainer.DeleteAllScriptableData();
        }

        DrawDefaultInspector();
    }
}