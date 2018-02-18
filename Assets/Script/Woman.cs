using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Woman : MonoBehaviour {
    public Vector3 goPos;

    public Transform target;

    public bool IsLeft;
    private StateMachine<Woman> stateMachine;

    public bool canStand() {
        return gameObject.transform.Find("OnGroundCheck").GetComponent<OnGroundCheck>().canStand;
    }
    public List<PassConnect> onPassConnectList() {
        return gameObject.transform.Find("OnGroundCheck").GetComponent<OnGroundCheck>().onPassConnectList;
    }

    public bool IsForeBlock() {
        return gameObject.transform.Find("ForeCheck").GetComponent<ForeCheck>().isForeBlock;
    }

    private void Start() {
        stateMachine = new StateMachine<Woman>(this);
        stateMachine.addState("walk", machine => new WalkState(machine));
        stateMachine.addState("passTrace", machine => new PassTraceState(machine));
        stateMachine.addState("fall", machine => new FallState(machine));
        stateMachine.addState("ladderUp", machine => new LadderUpState(machine));
        stateMachine.addState("ladderDown", machine => new LadderDownState(machine));
        stateMachine.Play("passTrace");
        goPos = target.transform.position;
    }

    private void Update() {
        stateMachine.Update();
    }

    public void LeftFace() {
        IsLeft = true;
        var scale = gameObject.transform.localScale;
        scale.x = -1;
        gameObject.transform.localScale = scale;
    }

    public void RightFace() {
        IsLeft = false;
        var scale = gameObject.transform.localScale;
        scale.x = 1;
        gameObject.transform.localScale = scale;
    }

    public void ReverseFace() {
        if (IsLeft) {
            RightFace();
        } else {
            LeftFace();
        }
    }

    public void walk(string direciton) {
        Vector3 moveVector = new Vector3();
        if (direciton == "left") {
            moveVector.x = -2 * Time.deltaTime;
        } else if (direciton == "right") {
            moveVector.x = 2 * Time.deltaTime;
        }
        gameObject.transform.position = gameObject.transform.position + moveVector;
    }
    public void fall() {
        Vector3 moveVector = new Vector3();
        moveVector.y = 3 * Time.deltaTime;
        gameObject.transform.position = gameObject.transform.position + moveVector;
    }
    public void ladderUp() {
        Vector3 pos = new Vector3();
        pos.y = Time.deltaTime * 2;
        gameObject.transform.position = gameObject.transform.position + pos;
    }
    public void ladderDown() {
        Vector3 move = new Vector3();
        move.y = Time.deltaTime * -2;
        gameObject.transform.position = gameObject.transform.position + move;
    }
}

class PassTraceState : State<Woman> {
    private List<PassConnect> passList;
    public StateMachine<Woman> subMachine;
    private int passIndex;

    public PassTraceState(StateMachine<Woman> machine) : base(machine) { }
    public override void StateStart() {
        subMachine = new StateMachine<Woman>(owner);
        subMachine.addState("walk", machine => new WalkState(machine));
        subMachine.addState("passTrace", machine => new PassTraceState(machine));
        subMachine.addState("fall", machine => new FallState(machine));
        subMachine.addState("ladderUp", machine => new LadderUpState(machine));
        subMachine.addState("ladderDown", machine => new LadderDownState(machine));
        subMachine.Play("walk");
    }

    private void nextPassAreaMove(){
        if (owner.transform.position.x > passList[passIndex + 1].transform.position.x) {
            owner.IsLeft = true;
        } else {
            owner.IsLeft = false;
        }
        if (passIndex + 1 < passList.Count) {
            if (subMachine.nowStateName == "walk" &&
                passList[passIndex].GetComponent<LadderDownPassConnect>() &&
                passList[passIndex + 1].GetComponent<LadderTopPassConnect>()
            ) {
                subMachine.Play("ladderUp");
            }
            if (subMachine.nowStateName == "walk" &&
                passList[passIndex].GetComponent<LadderTopPassConnect>() &&
                passList[passIndex + 1].GetComponent<LadderDownPassConnect>()
            ) {
                Vector3 pos = owner.transform.position;
                pos.x = passList[passIndex].gameObject.transform.parent.transform.position.x;
                owner.transform.position = pos;
                subMachine.Play("ladderDown");
            }
        }
    }
    private void directTargetMove(){
        if (owner.transform.position.x > owner.goPos.x) {
            owner.IsLeft = true;
        } else {
            owner.IsLeft = false;
        }
    }

    public override void StateUpdate() {
        if (Time.frameCount == 2) {
            passList = new Navigation(owner.transform.position, owner.goPos).getRoute();
        }
        if (passList == null) {
            return;
        }
        foreach(var passConnect in owner.onPassConnectList()){
            int i = passList.FindIndex(x=>x==passConnect);
            if(passIndex < i){
                passIndex = i;
            }
        }
        if(passIndex + 1 < passList.Count){
            nextPassAreaMove();
        }else{
            directTargetMove();
        }

        subMachine.Update();
    }
    public override void StateEnd() { }
}

class WalkState : State<Woman> {

    public WalkState(StateMachine<Woman> machine) : base(machine) { }
    public override void StateStart() { }

    public override void StateUpdate() {
        if (owner.IsLeft) {
            owner.walk("left");
        } else {
            owner.walk("right");
        }
    }

    public override void StateEnd() { }
}

class LadderUpState : State<Woman> {
    public LadderUpState(StateMachine<Woman> machine) : base(machine) { }

    public override void StateStart() {
    }

    public override void StateUpdate() {
        if (owner.canStand()) {
            machine.Play("walk");
        }
        owner.ladderUp();
    }

    public override void StateEnd() { }
}

class LadderDownState : State<Woman> {
    private Ladder ladder;
    public LadderDownState(StateMachine<Woman> machine) : base(machine) { }

    public override void StateStart() {
    }

    public override void StateUpdate() {
        if (owner.canStand()) {
            machine.Play("walk");
        }
        owner.ladderDown();
    }

    public override void StateEnd() { }
}
class FallState : State<Woman> {
    public FallState(StateMachine<Woman> machine) : base(machine) { }

    public override void StateStart() { }

    public override void StateUpdate() {
        if (owner.canStand()) {
            machine.Play("walk");
        } else {
            owner.fall();
        }
    }

    public override void StateEnd() { }
}