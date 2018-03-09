using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnGroundCheck : MonoBehaviour {
    public List<NaviArea> onNaviAreaList = new List<NaviArea>();
    public List<Collider2D> colliderList = new List<Collider2D>();

    public bool canStand {
        get { return onNaviAreaList.Count != 0; }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        colliderList.Add(collider);
    }

    private void OnTriggerExit2D(Collider2D collider) {
        colliderList.Remove(collider);
    }

    private void Update() {
        onNaviAreaList.Clear();
        foreach (var collider in colliderList) {
            if (collider.GetComponent<GroundArea>() == null) {
                continue;
            }

            var navi = collider.GetComponent<GroundArea>().getNavi(transform.position.x);
            if (navi == null) {
                continue;
            }
            onNaviAreaList.Add(navi);

        }
    }
}