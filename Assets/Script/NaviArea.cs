using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NaviArea : Navi {
    public Vector2 extents;

    private GroundArea groundArea {
        get {
            return owner.GetComponent<GroundArea>();
        }
    }
    public NaviArea(Vector2 extents, Vector2 position, GroundArea owner) : base(position, owner.transform) {
        this.extents = extents;
    }
    public override void connect() {
        foreach (var collider in Physics2D.OverlapBoxAll(position, owner.GetComponent<BoxCollider2D>().size, 0)) {
            if (collider.GetComponent<GroundArea>() != null) {
                NaviArea nearArea = null;
                foreach (var naviArea in collider.GetComponent<GroundArea>().naviAreaList) {
                    if (nearArea == null) {
                        nearArea = naviArea;
                        continue;
                    }
                    if (Vector3.Distance(nearArea.position, owner.transform.position) > Vector3.Distance(naviArea.position, owner.transform.position)) {
                        nearArea = naviArea;
                    }
                }
                connectNaviList.Add(nearArea);
            }
        }
    }
    public override void split() {
        foreach (var collider in Physics2D.OverlapBoxAll(position, owner.GetComponent<BoxCollider2D>().size, 0)) {
            if (collider.GetComponent<PassObstance>() == null) {
                continue;
            }
            var splitPos = collider.transform.position;
            NaviArea splitNaviArea = groundArea.getNavi(splitPos.x);
            //TODO ??
            if(splitNaviArea == null){
                continue;
            }

            var asize = new Vector2((splitPos.x - (splitNaviArea.position.x - splitNaviArea.extents.x)) / 2, splitNaviArea.extents.y);
            var apos = new Vector2(splitNaviArea.position.x - splitNaviArea.extents.x + asize.x, splitNaviArea.position.y);
            var aArea = new NaviArea(asize, apos, groundArea);
            if (asize.x > 0) {
                groundArea.naviAreaList.Add(aArea);
            }
            var bsize = new Vector2(((splitNaviArea.position.x + splitNaviArea.extents.x) - splitPos.x) / 2, splitNaviArea.extents.y);
            var bpos = new Vector2(splitNaviArea.position.x - splitNaviArea.extents.x + asize.x, splitNaviArea.position.y);
            var bArea = new NaviArea(bsize, bpos, groundArea);
            if (bsize.x > 0) {
                groundArea.naviAreaList.Add(bArea);
            }
            groundArea.naviAreaList.Remove(splitNaviArea);
        }
        groundArea.debugView();
    }
}