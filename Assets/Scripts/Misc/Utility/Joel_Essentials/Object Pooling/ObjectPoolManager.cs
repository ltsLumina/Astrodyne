using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPoolManager
{
    private static List<ObjectPool> objectPools = new List<ObjectPool>();

    private static Transform objectPoolParent = null;
    private static Transform ObjectPoolParent
    {
        get
        {
            if (objectPoolParent == null)
            {
                objectPoolParent = new GameObject("--- Object Pools ---").transform;
            }

            return objectPoolParent;
        }
    }

    /// <summary>
    /// Adds an existing pool to the list of object pools.
    /// </summary>
    /// <param name="objectPool"></param>
    public static void AddExistingPool(ObjectPool objectPool)
    {
        objectPools.Add(objectPool);
        objectPool.transform.parent = ObjectPoolParent;
    }

    /// <summary>
    /// Creates a new object pool as a new gameobject.
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <param name="startAmount"></param>
    /// <returns>The pool that was created.</returns>
    public static ObjectPool CreateNewPool(GameObject objectPrefab, int startAmount = 20)
    {
        ObjectPool newObjectPool = new GameObject().AddComponent<ObjectPool>();
        newObjectPool.SetUpPool(objectPrefab, startAmount);

        return newObjectPool;
    }

    /// <summary>
    /// Returns the pool containing the specified object prefab.
    /// Creates and returns a new pool if none is found.
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <returns></returns>
    public static ObjectPool FindObjectPool(GameObject objectPrefab)
    {
        foreach (ObjectPool objectPool in objectPools)
        {
            if (objectPool.GetPooledObjectPrefab() == objectPrefab)
            {
                return objectPool;
            }
        }

        Debug.LogWarning("That object is NOT yet pooled! Creating a new pool...");
        return CreateNewPool(objectPrefab, 20);
    }
}