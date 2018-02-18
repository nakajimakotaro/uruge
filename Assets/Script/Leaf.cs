using UnityEngine;

public class Leaf : ItemDefault {
    public GameObject fire;
    public bool isFire;

    protected override void OnEffect(Item dropItem) {
        if (!isFire) {
            isFire = true;
            Instantiate(fire, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.identity);
        }
    }

    protected override bool canEffect(Item dropItem) {
        return true;
    }
}