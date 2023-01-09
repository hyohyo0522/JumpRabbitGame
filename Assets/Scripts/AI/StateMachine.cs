using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class State<T>
{
    protected StateMachine<T> stateMachine;
    protected T context;

    public State() { } // »ý¼ºÀÚ

    internal void SetStateMachineAndContext(StateMachine<T> stateMachine, T context)
    {
        this.stateMachine = stateMachine;
        this.context = context;

        OnInitialized();
    }

    public virtual void OnInitialized()
    {

    }

    public virtual void OnEnter()
    {

    }

    public abstract void Update(float deltaTime);

    public virtual void OnExit()
    {

    }

}

public sealed class StateMachine<T>
{
    private T context;

    private State<T> currentState;
    public State<T> CurrentState => currentState;

    private State<T> priviousState;
    public State<T> PrivioueState => priviousState;

    private float elapsedTimeInState = 0.0f;
    public float ElapsedTimeInState => elapsedTimeInState;

    private Dictionary<System.Type, State<T>> states = new Dictionary<System.Type, State<T>>();

    public StateMachine(T context, State<T> InitialState)
    {
        this.context = context;

        AddState(InitialState);
        currentState = InitialState;
        currentState.OnEnter();
        
    }

    public void AddState(State<T> state)
    {
        state.SetStateMachineAndContext(this, context);
        states[state.GetType()] = state;

    }

    public void Update(float deltaTime)
    {
        elapsedTimeInState += deltaTime;
        currentState.Update(deltaTime);
    }

    public A ChangeState<A>() where A : State<T>
    {
        var newType = typeof(A);
        if (currentState.GetType() == newType)
        {
            return currentState as A;
        }

        if (currentState != null)
        {
            currentState.OnExit();
        }

        priviousState = currentState;
        currentState = states[newType];
        currentState.OnEnter();
        elapsedTimeInState = 0.0f;

        return currentState as A;

    }

}
