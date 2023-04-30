using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject objectPrefab;
    [SerializeField] int startAmount;

    Queue<GameObject> pooledObjects = new();

    void Awake() => gameObject.name = objectPrefab.name + " (Pool)";

    void Start()
    {
        ObjectPoolManager.AddExistingPool(this);
        InstantiateStartAmount();
    }

    void InstantiateStartAmount()
    {
        for (int i = 0; i < startAmount; i++) { CreatePooledObject(); }
    }

    public void SetUpPool(GameObject objectPrefab, int startAmount)
    {
        pooledObjects = new Queue<GameObject>();

        for (int i = 0; i < startAmount; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            pooledObjects.Enqueue(obj);
        }
    }


    GameObject CreatePooledObject()
    {
        GameObject newObject = Instantiate(objectPrefab, transform, true);
        newObject.SetActive(false);
        pooledObjects.Enqueue(newObject);
        return newObject;
    }

    public GameObject GetPooledObject(bool setActive = false)
    {
        GameObject objectToReturn = null;

        objectToReturn = pooledObjects.Count > 0 ? pooledObjects.Dequeue() : CreatePooledObject();

        objectToReturn.SetActive(setActive);
        return objectToReturn;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        pooledObjects.Enqueue(obj);
    }

    public GameObject GetPooledObjectPrefab() => objectPrefab;
}