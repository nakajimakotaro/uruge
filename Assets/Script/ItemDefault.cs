using UnityEngine;

public class ItemDefault : Item {
    private GameObject sprite;

    public new void Start() {
        base.Start();
        sprite = gameObject.transform.Find("New Sprite").gameObject;
    }

    protected override void OnPointerEnter() {
    }

    protected override void OnPointerStay() {
    }

    protected override void OnPointerExit() {
    }

    protected override void OnHaveStart() {
    }

    protected override void OnHaveStay() {
        var angle = Mathf.Sin(Time.frameCount / 15f) * 30;
        sprite.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void OnHaveEnd() {
        sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    protected override void OnEffect(Item dropItem) {
    }

    protected override bool canEffect(Item dropItem) {
        return true;
    }
}