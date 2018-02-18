using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate State<T> StateGenerator<T>(StateMachine<T> machine);

public class StateMachine<T> {
    private readonly Dictionary<string, StateGenerator<T>> stateHash = new Dictionary<string, StateGenerator<T>>();

    private State<T> currentState;
    private State<T> nextState;
    public T owner;
    public string nowStateName = "none";
    UnityEvent changeEvent = new UnityEvent();

    public StateMachine(T target) {
        currentState = new NoneState<T>(this);
        owner = target;
    }


    public void addState(string name, StateGenerator<T> generator) {
        stateHash.Add(name, generator);
    }

    public virtual void Update() {
        if (nextState != null) {
            if (currentState != null) {
                currentState.StateEnd();
            }

            currentState = nextState;
            currentState.StateStart();
            nextState = null;
            
        }

        if (currentState != null) {
            currentState.StateUpdate();
        }
    }

    public void Play(string name) {
        nextState = stateHash[name](this);
        nowStateName = name;
        changeEvent.Invoke();
    }
}