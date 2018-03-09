using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NaviLinkEntryArea : Navi {
    public NaviLinkEntryArea(Vector2 position, Transform owner) : base(position, owner) {
        Navigation.naviList.Add(this);
    }

    public override void split(){
    }
    public override void connect() {
        foreach (var overLap in Physics2D.OverlapCircleAll(position, 0.1f)) {
            if (overLap.GetComponent<GroundArea>()) {
                connectNaviList.Add(overLap.GetComponent<GroundArea>().getNavi(position.x));
                overLap.GetComponent<GroundArea>().getNavi(position.x).connectNaviList.Add(this);
                break;
            }
        }
    }
}