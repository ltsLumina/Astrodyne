#region
using UnityEngine;
#endregion

[DefaultExecutionOrder(-2)]
public sealed class Singleton<T> : MonoBehaviour where T : Component
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No object of type " + typeof(T).FullName + " was found.");
                return null;
            }

            return instance;
        }
    }

    void Awake()
    {
        if (instance == null) instance = this as T;
        else Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}

public class SingletonPersistent<T> : MonoBehaviour where T : Component
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No object of type " + typeof(T).FullName + " was found.");
                return null;
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject); //TODO: Returns a warning??
        }
        else { Destroy(gameObject); }
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}