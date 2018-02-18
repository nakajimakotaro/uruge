using UnityEngine;

public abstract class State<T> {
    public StateMachine<T> machine;

    public State(StateMachine<T> machine) {
        this.machine = machine;
    }

    public T owner {
        get { return machine.owner; }
    }

    public virtual void StateStart() {
    }

    public virtual void StateUpdate() {
    }

    public virtual void StateEnd() {
    }

    public virtual void OnCollisionEnter2D(Collision2D c) {
    }

    public virtual void OnCollisionStay2D(Collision2D c) {
    }

    public virtual void OnCollisionExit2D(Collision2D c) {
    }

    public virtual void OnTriggerEnter2D(Collider2D c) {
    }

    public virtual void OnTriggerStay2D(Collider2D c) {
    }

    public virtual void OnTriggerExit2D(Collider2D c) {
    }
}

public class NoneState<T> : State<T> {
    public NoneState(StateMachine<T> machine) : base(machine) {
    }

    public override void StateStart() {
    }

    public override void StateUpdate() {
    }

    public override void StateEnd() {
    }
}