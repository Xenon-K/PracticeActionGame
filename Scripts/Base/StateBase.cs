using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// status type
public abstract class StateBase 
{
    /// <summary>
    /// init
    /// <param name="owner"></param>
    public abstract void Init(IStateMachineOwner owner);

    /// <summary>
    /// release resources
    public abstract void UnInit();

    /// <summary>
    /// enter state
    public abstract void Enter();

    /// <summary>
    /// end state
    public abstract void Exit();

    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void LateUpdate();
}
