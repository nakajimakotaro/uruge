using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OnGroundCheck : MonoBehaviour {
    private int onGroundCount;
    public List<PassConnect> onPassConnectList = new List<PassConnect>();

    public bool canStand {
        get { return onGroundCount != 0; }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("ground")) {
            onGroundCount++;
        }
        if(collider.GetComponent<PassConnect>()){
            onPassConnectList.Add(collider.GetComponent<PassConnect>());
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("ground")) {
            onGroundCount--;
        }
        if(collider.GetComponent<PassConnect>()){
            onPassConnectList.Remove(collider.GetComponent<PassConnect>());
        }
    }
}