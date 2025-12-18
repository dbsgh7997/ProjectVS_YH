using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<Type, ObjectPool<IPoolable>> poolDict = null;

    public PoolManager()
    {
        poolDict = new Dictionary<Type, ObjectPool<IPoolable>>();
        poolDict.Clear();
    }

    ~PoolManager()
    {
        poolDict.Clear();
        poolDict = null;
    }

    public void RegisterObjectPool<T>(ObjectPool<IPoolable> _pool) where T : IPoolable
    {
        Type type = typeof(T);
        if (poolDict.ContainsKey(type))
        {
            throw new Exception("Duplication Key");
        }

        poolDict.Add(type, _pool);
    }

    public void RemoveObjectPool<T>() where T : IPoolable
    {
        Type type = typeof(T);
        if (!poolDict.ContainsKey(type))
        {
            throw new KeyNotFoundException();
        }

        var pool = poolDict[type];
        pool.OnRelease();

        poolDict.Remove(type);
    }

    public ObjectPool<IPoolable> GetObjectPool<T>() where T : IPoolable
    {
        poolDict.TryGetValue(typeof(T), out var pool);
        if (pool == null)
        {
            //.. FIXME? :: throw¸¦ ´øÁú±î?
            pool = new ObjectPool<IPoolable>();
            RegisterObjectPool<T>(pool);
        }

        return pool;
    }
}
