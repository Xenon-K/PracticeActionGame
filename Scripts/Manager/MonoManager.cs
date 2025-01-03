using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoManager : SingleMomoBase<MonoManager>
{
    //update task collection
    public Action updatAction;
    //fixedUpdate task collection
    public Action fixedUpdatAction;
    //lateUpdate task collection
    public Action lateUpdatAction;

    /// <summary>
    /// Add update task
    /// <param name="task"></param>
    public void AddUpdateAction(Action task)
    {
        updatAction += task;
    }

    /// <summary>
    /// Remove update task
    /// <param name="task"></param>
    public void RemoveUpdateAction(Action task)
    {
        updatAction -= task;
    }

    /// <summary>
    /// Add fixedUpdate task
    /// <param name="task"></param>
    public void AddFixedUpdateAction(Action task)
    {
        fixedUpdatAction += task;
    }

    /// <summary>
    /// Remove fixedUpdate task
    /// <param name="task"></param>
    public void RemoveFixedUpdateAction(Action task)
    {
        fixedUpdatAction -= task;
    }

    /// <summary>
    /// Add lateUpdate task
    /// <param name="task"></param>
    public void AddLateUpdateAction(Action task)
    {
        lateUpdatAction += task;
    }

    /// <summary>
    /// Remove lateUpdate task
    /// <param name="task"></param>
    public void RemoveLateUpdateAction(Action task)
    {
        lateUpdatAction -= task;
    }

    void Update()
    {
        updatAction?.Invoke();
    }

    void FixedUpdate()
    {
        fixedUpdatAction?.Invoke();
    }
     void LateUpdate()
    {
        lateUpdatAction?.Invoke();
    }
}
