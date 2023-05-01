#region
using System.Collections.Generic;
using UnityEngine;
#endregion

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject objectPrefab;
    [SerializeField] int startAmount;

    readonly List<GameObject> pooledObjects = new();

    void Awake() =>

        // Clear the pooledObjects list in case it's not empty. This prevents errors when using quicker load times in the editor.
        pooledObjects.Clear();

    void Start()
    {
        ObjectPoolManager.AddExistingPool(this);
        InstantiateStartAmount();
    }

    /// <summary>
    ///     Basically a constructor.
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <param name="startAmount"></param>
    public void SetUpPool(GameObject objectPrefab, int startAmount)
    {
        this.objectPrefab = objectPrefab;
        this.startAmount  = startAmount;

        gameObject.name = objectPrefab.name + " (Pool)";
    }

    /// <summary>
    ///     Instantiates the specified number of starting objects for the pool.
    /// </summary>
    void InstantiateStartAmount()
    {
        for (int i = 0; i < startAmount; i++) { CreatePooledObject(); }
    }

    /// <summary>
    ///     Instantiates an object, adds it to the pool and makes it inactive.
    /// </summary>
    /// <returns>The object that was created.</returns>
    public GameObject CreatePooledObject()
    {
        GameObject newObject = Instantiate(objectPrefab, transform, true);
        newObject.SetActive(false);
        pooledObjects.Add(newObject);
        return newObject;
    }

    /// <summary>
    ///     Returns an object from the pool.
    /// </summary>
    /// <param name="setActive">Depicts if the object should be active on return.</param>
    /// <returns></returns>
    public GameObject GetPooledObject(bool setActive = false)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(setActive);
                return pooledObjects[i];
            }
        }

        GameObject objectToReturn = CreatePooledObject();
        objectToReturn.SetActive(setActive);

        return objectToReturn;
    }

    /// <summary>
    ///     Returns the prefab of the object this pool contains.
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledObjectPrefab() => objectPrefab;
}