using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectPool<T> where T : IPoolable
{
    private Queue<T> unusedObjectQueue = null;

    private GameObject prefab = null;
    private Transform rootTransform;

    public async void Initialize(string _prefabPath, int _initializeCreateCount, Transform _root)
    {
        string prefabPath = Path.Combine("", _prefabPath);
        //.. TODO :: Addressable / 비동기 적용
        prefab = Resources.Load(prefabPath, typeof(GameObject)) as GameObject;
        rootTransform = _root;
        for (int i = 0; i < _initializeCreateCount; i++)
        {
            CreateObject();
        }
    }

    public ObjectPool()
    {
        unusedObjectQueue = new Queue<T>();
        unusedObjectQueue.Clear();
    }

    ~ObjectPool()
    {
        OnRelease();
    }

    public void EnqueueObject(T _object)
    {        
        _object.OnDeactivate();
        if (unusedObjectQueue != null)
        {
            unusedObjectQueue.Enqueue(_object);
        }
    }

    public T GetObject()
    {
        if (unusedObjectQueue == null)
            return default;
        T getObject;
        if (unusedObjectQueue.Count <= 0)
        {
            getObject = CreateObject();
        }
        else
        {
            getObject = unusedObjectQueue.Dequeue();
        }

        return getObject;
    }

    private T CreateObject()
    {
        GameObject newObject = GameObject.Instantiate(prefab, rootTransform);
        var componenet = newObject.GetComponent<T>();
        EnqueueObject(componenet);
        return componenet;
    }

    public void OnRelease()
    {
        unusedObjectQueue.Clear();
        unusedObjectQueue = null;
    }
}
