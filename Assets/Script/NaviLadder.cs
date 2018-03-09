using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NaviLadder : MonoBehaviour{
    public Ladder owner;
    public Transform topPos;
    public Transform bottomPos;

    public NaviLinkEntryArea entryTop;
    public NaviLinkEntryArea entryBottom;

    NaviLink naviLink;
    void Awake(){
        entryTop = new NaviLinkEntryArea(topPos.position, this.transform);
        entryBottom = new NaviLinkEntryArea(bottomPos.position, this.transform);

        entryTop.connectNaviList.Add(entryBottom);
        entryBottom.connectNaviList.Add(entryTop);
    }
}