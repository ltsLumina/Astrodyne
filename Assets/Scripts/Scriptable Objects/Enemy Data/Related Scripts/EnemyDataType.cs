#region
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class EnemyDataType : ScriptableObject, IDamageable
{
    [SerializeField] DataTypeContainer myDataTypeContainer;

    [SerializeField] string objectName;
    [SerializeField] int damage;
    [SerializeField] float attackDelay;
    [SerializeField] float moveSpeed;
    [SerializeField] int health;

    public DataTypeContainer MyDataTypeContainer => myDataTypeContainer;
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

    public int Health
    {
        get => health;
        set => health = value;
    }


#if UNITY_EDITOR
    public void Initialise(DataTypeContainer myDataTypeContainer) => this.myDataTypeContainer = myDataTypeContainer;
#endif

#if UNITY_EDITOR
    [ContextMenu("Rename to name")]
    void Rename()
    {
        name = objectName;
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this);
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Delete this")]
    void DeleteThis()
    {
        myDataTypeContainer.DamageTypes.Remove(this);
        Undo.DestroyObjectImmediate(this);
        AssetDatabase.SaveAssets();
    }
#endif
}