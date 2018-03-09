using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundArea : MonoBehaviour {

    public NaviArea originNaviArea;
    public List<NaviArea> naviAreaList = new List<NaviArea>();
    public string type {
        get {
            return "ground";
        }
    }

    public NaviArea getNavi(float x) {
        foreach (var naviArea in naviAreaList) {
            if (
                naviArea.position.x - naviArea.extents.x < x &&
                naviArea.position.x + naviArea.extents.x > x) {
                return naviArea;
            }
        }
        return null;
    }

    void Awake() {
        var navi = new NaviArea(GetComponent<BoxCollider2D>().bounds.extents, transform.position, this);
        originNaviArea = navi;
        naviAreaList.Add(navi);
        Navigation.naviList.Add(navi);
    }
    public void debugView() {
    }
    public void OnDrawGizmos(){
        foreach (var navi in naviAreaList) {
            var a = new Vector3(navi.position.x - navi.extents.x, navi.position.y - navi.extents.y, -2);
            var b = new Vector3(navi.position.x - navi.extents.x, navi.position.y + navi.extents.y, -2);
            var c = new Vector3(navi.position.x + navi.extents.x, navi.position.y + navi.extents.y, -2);
            var d = new Vector3(navi.position.x + navi.extents.x, navi.position.y - navi.extents.y, -2);
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
    }
}