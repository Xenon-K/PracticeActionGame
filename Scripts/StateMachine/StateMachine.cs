using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// owner tag
public interface IStateMachineOwner { }

/// <summary>
/// Finite State Machine
public class StateMachine 
{
    //current state
    private StateBase currentState;

    //Whether to include the current state
    public bool HasState { get => currentState != null;}

    //owner
    private IStateMachineOwner owner;

    //status dictionary
    private Dictionary<Type, StateBase> stateDic = new Dictionary<Type, StateBase>();

    public StateMachine(IStateMachineOwner owner) 
    {
        Init(owner);
    }

    //init
    public void Init(IStateMachineOwner owner)
    { 
        this.owner = owner;
    }

    /// <summary>
    /// enter state
    /// <typeparam name="T">Status Type</typeparam>
    /// /// <typeparam name="reLoadState">Whether to refresh the status</typeparam>
    public void EnterState<T>(bool reLoadState = false) where T : StateBase, new()
    {
        //If the status is the same, no switching is performed
        if (HasState && currentState.GetType() == typeof(T) && !reLoadState)  
        {
            return;
        }

        #region end current state
        if (HasState)
        {
            ExitCurrentState();
        }
        #endregion

        #region enter new state
        currentState = LoadState<T>();
        EnterCurrentState();
        #endregion
    }

    /// <summary>
    /// Load or create a new state
    /// <typeparam name="T">status type</typeparam>
    /// <returns></returns>
    private StateBase LoadState<T>() where T : StateBase, new()
    { 
        //get status type
        Type stateType = typeof(T);

        //if not a state in dictionary
        if (!stateDic.TryGetValue(stateType, out StateBase state))
        {
            //Create a new state and save it to the dictionary
            state = new T();
            state.Init(owner);
            stateDic.Add(stateType, state);
        }

        return state;
    }

    private void EnterCurrentState()
    {
        currentState.Enter();
        MonoManager.INSTANCE.AddUpdateAction(currentState.Update);
        MonoManager.INSTANCE.AddFixedUpdateAction(currentState.FixedUpdate);
        MonoManager.INSTANCE.AddLateUpdateAction(currentState.LateUpdate);
    }
    private void ExitCurrentState()
    {
        currentState.Exit();
        MonoManager.INSTANCE.RemoveUpdateAction(currentState.Update);
        MonoManager.INSTANCE.RemoveFixedUpdateAction(currentState.FixedUpdate);
        MonoManager.INSTANCE.RemoveLateUpdateAction(currentState.LateUpdate);
    }

    /// <summary>
    /// refresh state machine
    public void Clear()
    {
        ExitCurrentState(); 
        currentState = null;
        foreach (var item in stateDic.Values)
        {
            item.UnInit();
        }
        stateDic.Clear();
    }

    /// <summary>
    /// Get the current state
    /// <typeparam name="T">State type</typeparam>
    /// <returns>The current state cast to the specified type</returns>
    public T GetCurrentState<T>() where T : StateBase
    {
        return currentState as T;
    }
}
