using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <typeparam name="T"></typeparam>
public class SingleMomoBase<T> : MonoBehaviour where T : SingleMomoBase<T>
{
    public static T INSTANCE;

    protected virtual void Awake()
    {
        if (INSTANCE != null) 
        {
            Debug.LogError(this + "not instance mode");
        }
        INSTANCE = (T)this;
    }

    protected virtual void OnDestroy()
    {
        Destroy();         
    }

    /// <summary>
    public void Destroy()
    {
        INSTANCE = null;
    }
}
