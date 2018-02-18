using UnityEngine;

public class BackGournd : MonoBehaviour {
    private void Update() {
        var pos = GameObject.Find("Main Camera").transform.position;
        gameObject.transform.position = new Vector3(pos.x, pos.y, 0);
    }
}