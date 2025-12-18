using UnityEngine;
using System.Threading;

public abstract class Singleton<T> where T : class, new()
{
    private static T instance;

    public static T getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }

    protected Singleton()
    {

    }
}
