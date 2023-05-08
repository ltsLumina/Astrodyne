// UNCOMMENT THIS BEFORE UPDATING ESSENTIALS
//#define FOR_ESSENTIALS

#if FOR_ESSENTIALS
#region
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

/// <summary>
/// An example of a data type that can be created as a child to the DataTypeContainer scriptable object.
/// This class is a simple enemy type, such as a goblin or skeleton perhaps.
/// Add whichever variables and methods you want, and use it as you wish.
/// </summary>
public class EnemyDataType : ScriptableObject
{
    [SerializeField] DataTypeContainer myDataTypeContainer;

    [SerializeField] string objectName;
    [SerializeField] int damage;
    [SerializeField] float moveSpeed;
    [SerializeField] int health;
    [SerializeField] float attackDelay;

    public DataTypeContainer MyDataTypeContainer => myDataTypeContainer;
    public string ObjectName => objectName;

    public int Damage
    {
        get => damage;
        set => damage = value;
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

    public float AttackDelay
    {
        get => attackDelay;
        set => attackDelay = value;
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
#endif