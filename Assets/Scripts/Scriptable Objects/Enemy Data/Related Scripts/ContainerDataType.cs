#region
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[CreateAssetMenu(fileName = "Container Data Type", menuName = "Containers/Create ContainerDataType")]
public class ContainerDataType : ScriptableObject
{
    [SerializeField] List<EnemyDataType> _damageTypes = new ();

    public List<EnemyDataType> DamageTypes
    {
        get => _damageTypes;
        set => _damageTypes = value;
    }

#if UNITY_EDITOR
    [ContextMenu("Make New")]
    void MakeNewDamageType()
    {
        var damageType = CreateInstance<EnemyDataType>();
        damageType.name = "New Damage Type";
        damageType.Initialise(this);
        _damageTypes.Add(damageType);

        AssetDatabase.AddObjectToAsset(damageType, this);
        AssetDatabase.SaveAssets();

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(damageType);
    }

#endif

#if UNITY_EDITOR
    [ContextMenu("Delete all")]
    void DeleteAll()
    {
        for (int i = _damageTypes.Count; i-- > 0;)
        {
            EnemyDataType tmp = _damageTypes[i];

            _damageTypes.Remove(tmp);
            Undo.DestroyObjectImmediate(tmp);
        }

        AssetDatabase.SaveAssets();
    }

#endif
}