using UnityEngine;

public class ForeCheck : MonoBehaviour {
    private int foreBlockCount;

    public bool isForeBlock {
        get { return foreBlockCount != 0; }
    }

    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.GetComponent<Block>()) {
            foreBlockCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.GetComponent<Block>()) {
            foreBlockCount--;
        }
    }
}