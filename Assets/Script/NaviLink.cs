using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NaviLink {
    public Transform linkA;
    public Transform linkB;
    public NaviLadder owner;

    public NaviArea linkAConnect;
    public NaviArea linkBConnect;

    public NaviLink(Transform linkA, Transform linkB, NaviLadder owner){
        this.linkA = linkA;
        this.linkB = linkB;
    }

    void Update(){
    }
}
