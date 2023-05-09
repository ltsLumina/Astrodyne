#region
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class ScriptableData : ScriptableObject
{
    [SerializeField] ScriptableDataContainer myDataContainer;

    [SerializeField] string objectName;
    [SerializeField] int damage;
    [SerializeField] float attackDelay;
    [SerializeField] float moveSpeed;

    public ScriptableDataContainer MyDataContainer => myDataContainer;
    public string ObjectName => objectName;

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public float AttackDelay
    {
        get => attackDelay;
        set => attackDelay = value;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }


#if UNITY_EDITOR
    public void Initialise(ScriptableDataContainer myScriptableDataContainer) => myDataContainer = myScriptableDataContainer;
#endif

#if UNITY_EDITOR
    [ContextMenu("Rename to name")]
    public void Rename()
    {
        name = objectName;
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this);
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Delete this")]
    public void Delete()
    {
        myDataContainer.BlankScriptableData.Remove(this);
        Undo.DestroyObjectImmediate(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

[CustomEditor(typeof(ScriptableData))]
public class ScriptableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var scriptableData = (ScriptableData)target;

        if (GUILayout.Button("Rename Selected Data Type", GUILayout.Height(30)))
        {
            scriptableData.Rename();
        }

        if (GUILayout.Button("Delete Selected Data Types", GUILayout.Height(30)))
        {
            scriptableData.Delete();
        }

        DrawDefaultInspector();
    }
}