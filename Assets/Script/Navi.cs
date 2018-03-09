using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Navi{
    public List<Navi> connectNaviList = new List<Navi>();
    public Transform owner;
    public Vector2 position;
    public Navi(Vector2 position, Transform owner){
        this.owner = owner;
        this.position = position;
    }
    public abstract void split();
    public abstract void connect();
}