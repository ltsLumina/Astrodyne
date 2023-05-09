// UNCOMMENT THIS BEFORE UPDATING ESSENTIALS
//#define FOR_ESSENTIALS

#if FOR_ESSENTIALS
#region
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

/// <summary>
/// A class that allows you to organize your scriptable objects in your project a little easier, allowing you to child your scriptable objects to this.
/// To use this, simply create a container (parent) object in the project view, and then through the context menu create your childed data types.
/// Refer to https://github.com/ltsLumina/Unity-Essentials/releases/tag/ver.2.0.0 for additional information or help regarding the use of this class.
/// </summary>
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
    [ContextMenu("Make New")]
    void MakeNewDamageType()
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
    [ContextMenu("Delete all")]
    void DeleteAllDamageTypes()
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
#endif