#region
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class EnemyDataType : ScriptableObject, IDamageable
{
    [SerializeField] ContainerDataType _myContainer;

    [SerializeField] string _name;
    [SerializeField] int _damage;
    [SerializeField] float _moveSpeed;
    [SerializeField] int _health;
    [SerializeField] float _attackDelay;

    public ContainerDataType MyContainer => _myContainer;
    public string Name => _name;

    public int Damage
    {
        get => _damage;
        set => _damage = value;
    }

    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }

    public int Health
    {
        get => _health;
        set => _health = value;
    }

    public float AttackDelay
    {
        get => _attackDelay;
        set => _attackDelay = value;
    }

#if UNITY_EDITOR
    public void Initialise(ContainerDataType myContainer) { _myContainer = myContainer; }
#endif

#if UNITY_EDITOR
    [ContextMenu("Rename to name")]
    void Rename()
    {
        name = _name;
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this);
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Delete this")]
    void DeleteThis()
    {
        _myContainer.DamageTypes.Remove(this);
        Undo.DestroyObjectImmediate(this);
        AssetDatabase.SaveAssets();
    }
#endif
}