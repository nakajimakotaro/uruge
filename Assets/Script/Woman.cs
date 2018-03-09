using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Woman : MonoBehaviour {
    public Vector3 goPos;

    public Transform target;
    public Ladder handLadder;
    public bool inLadderTopExitPoint;
    public bool inLadderBottomExitPoint;

    public bool IsLeft;
    private StateMachine<Woman> stateMachine;

    public bool canStand() {
        return gameObject.transform.Find("OnGroundCheck").GetComponent<OnGroundCheck>().canStand;
    }
    public List<NaviArea> onPassConnectList() {
        return gameObject.transform.Find("OnGroundCheck").GetComponent<OnGroundCheck>().onNaviAreaList;
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

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.name == "TopExitPoint"){
            inLadderTopExitPoint = true;
        }
        if(collider.name == "BottomExitPoint"){
            inLadderBottomExitPoint = true;
        }
    }
    void OnTriggerExit2D(Collider2D collider){
        if(collider.name == "TopExitPoint"){
            inLadderTopExitPoint = false;
        }
        if(collider.name == "BottomExitPoint"){
            inLadderBottomExitPoint = false;
        }
    }
    public void OnDrawGizmos(){
        foreach(var navi in onPassConnectList()){
            Vector3 pos = navi.position;
            Gizmos.DrawSphere(pos - new Vector3(0, 0, 1), 0.2f);
        }
    }
}

class PassTraceState : State<Woman> {
    private List<Navi> naviList;
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

    private void nextPassAreaMove() {
        owner.IsLeft = owner.transform.position.x > naviList[passIndex + 1].position.x;
        if (subMachine.nowStateName == "walk" &&
            naviList[passIndex + 1] is NaviLinkEntryArea &&
            naviList[passIndex + 1].owner.GetComponent<NaviLadder>() != null
        ) {
            var naviLadder = naviList[passIndex + 1].owner.GetComponent<NaviLadder>();
            var entry = naviList[passIndex + 1] as NaviLinkEntryArea;

            if (
                naviLadder.entryTop == entry &&
                Vector2.Distance(owner.transform.position, entry.position) < 0.2
            ) {
                owner.handLadder = naviLadder.owner;
                subMachine.Play("ladderDown");
            }
            if (
                naviLadder.entryBottom == entry &&
                Vector2.Distance(owner.transform.position, entry.position) < 0.2
            ) {
                owner.handLadder = naviLadder.owner;
                subMachine.Play("ladderUp");
            }
        }
    }
    private void directTargetMove() {
        if (owner.transform.position.x > owner.goPos.x) {
            owner.IsLeft = true;
        } else {
            owner.IsLeft = false;
        }
    }

    public override void StateUpdate() {
        if (Time.frameCount == 1) {
            Navigation.bake();
            naviList = Navigation.getRoute(owner.transform.position, owner.goPos);
        }
        if (naviList == null) {
            return;
        }
        if (subMachine.nowStateName == "walk") {
            foreach (var pass in owner.onPassConnectList()) {
                int i = naviList.FindIndex(x => x == pass);
                if (passIndex < i) {
                    passIndex = i;
                }
            }
        }
        if (passIndex + 1 < naviList.Count) {
            nextPassAreaMove();
        } else {
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
        Vector3 moveVector = new Vector3();
        moveVector.x = 2 * (owner.IsLeft ? -1 : 1) * Time.deltaTime;
        owner.transform.position = owner.transform.position + moveVector;
        if(!owner.canStand()){
            machine.Play("fall");
        }
    }

    public override void StateEnd() { }
}

class LadderUpState : State<Woman> {
    public LadderUpState(StateMachine<Woman> machine) : base(machine) { }

    public override void StateStart() {
        owner.transform.position = new Vector3(owner.handLadder.transform.position.x, owner.transform.position.y, owner.transform.position.z);
    }

    public override void StateUpdate() {
        if (owner.inLadderTopExitPoint) {
            machine.Play("walk");
            owner.handLadder = null;
        }
        Vector3 move = new Vector3();
        move.y = Time.deltaTime * 2;
        owner.transform.position = owner.transform.position + move;
    }

    public override void StateEnd() { }
}

class LadderDownState : State<Woman> {
    private Ladder ladder;
    public LadderDownState(StateMachine<Woman> machine) : base(machine) { }

    public override void StateStart() {
        owner.transform.position = new Vector3(owner.handLadder.transform.position.x, owner.transform.position.y, owner.transform.position.z);
    }

    public override void StateUpdate() {
        if (owner.inLadderBottomExitPoint) {
            machine.Play("walk");
            owner.handLadder = null;
        }
        Vector3 move = new Vector3();
        move.y = Time.deltaTime * -2;
        owner.transform.position = owner.transform.position + move;
    }

    public override void StateEnd() { }
}
class FallState : State<Woman> {
    public FallState(StateMachine<Woman> machine) : base(machine) { }

    public override void StateStart() { }

    public override void StateUpdate() {
        if (owner.canStand()) {
            machine.Play("walk");
        }
        Vector3 moveVector = new Vector3();
        moveVector.y = -3 * Time.deltaTime;
        owner.transform.position = owner.transform.position + moveVector;
    }

    public override void StateEnd() { }
}