using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMonoBaseEnemy<T> : MonoBehaviour where T : SingleMonoBaseEnemy<T>
{
    public static T INSTANCE;

    protected virtual void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = (T)this;
        }
        else
        {
            // Do nothing; allow multiple instances
        }
    }

    protected virtual void OnDestroy()
    {
        if (INSTANCE == this)
        {
            INSTANCE = null;
        }
    }
}
