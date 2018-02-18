using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainPassConnect : PassConnect {
    bool isProcess = false;
    private List<PassObstance> obstanceList = new List<PassObstance>();
    void Update() {
        if (isProcess) {
            return;
        }
        isProcess = true;
        List<PassConnect> list = new List<PassConnect>();
        List<PassConnect> removeList = new List<PassConnect>();
        list.Add(this);
        foreach (var obstance in obstanceList) {
            foreach (var sprit in list) {
                var passCollider = sprit.GetComponent<BoxCollider2D>();
                float left = passCollider.transform.position.x + passCollider.offset.x - passCollider.size.x;
                float right = passCollider.transform.position.x + passCollider.offset.x + passCollider.size.x;
                if (left < obstance.transform.position.x && right > obstance.transform.position.x) {
                    list = list.Concat(splitObstance(sprit, obstance)).ToList();
                    list.Remove(sprit);
                    removeList.Add(sprit);
                    break;
                }
            }
        }

        int c = passConnectList.Count;
        for(int i = 0;i < c;i++) {
            var connect = passConnectList[i];
            var near = list.Aggregate((a, b) => {
                var aCollider = a.GetComponent<BoxCollider2D>();
                var bCollider = b.GetComponent<BoxCollider2D>();
                var aScore =
                    Mathf.Abs(connect.transform.position.x - (aCollider.transform.position.x + aCollider.offset.x + aCollider.size.x)) +
                    Mathf.Abs(connect.transform.position.x - (aCollider.transform.position.x + aCollider.offset.x - aCollider.size.x));
                var bScore =
                    Mathf.Abs(connect.transform.position.x - (bCollider.transform.position.x + bCollider.offset.x + bCollider.size.x)) +
                    Mathf.Abs(connect.transform.position.x - (bCollider.transform.position.x + bCollider.offset.x - bCollider.size.x));
                return aScore < bScore ? a : b;
            });
            connect.passConnectList.Remove(this);
            connect.passConnectList.Add(near);
            near.passConnectList.Add(connect);
        }
        foreach (var rm in removeList) {
            Destroy(rm);
        }
    }

    List<PassConnect> splitObstance(PassConnect sprit, PassObstance obstance) {
        var collider = sprit.GetComponent<BoxCollider2D>();
        var obstanceCollider = obstance.GetComponent<BoxCollider2D>();
        float leftSize =
            ((obstanceCollider.transform.position.x + obstanceCollider.offset.x - obstanceCollider.size.x) -
                (collider.transform.position.x + collider.offset.x - collider.size.x)) / 2f;

        float rightSize =
            (collider.transform.position.x + collider.offset.x + collider.size.x -
                (obstanceCollider.transform.position.x + obstanceCollider.offset.x + obstanceCollider.size.x)) / 2f;

        List<PassConnect> list = new List<PassConnect>();
        if (0.25 < leftSize) {
            var left = gameObject.AddComponent<TerrainPassConnect>();
            left.isProcess = true;
            left.passConnectList = new List<PassConnect>();
            left.GetComponent<BoxCollider2D>().size = new Vector2(leftSize, 0);
            left.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
            left.transform.position = new Vector2(collider.transform.position.x + collider.offset.x - collider.size.x + leftSize, transform.position.y);
            list.Add(left);
        }
        if (0.25 < rightSize) {
            var right = gameObject.AddComponent<TerrainPassConnect>();
            right.passConnectList = new List<PassConnect>();
            right.isProcess = true;
            right.GetComponent<BoxCollider2D>().size = new Vector2(rightSize, 0);
            right.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
            right.transform.position = new Vector2(obstanceCollider.transform.position.x + obstanceCollider.offset.x + obstanceCollider.size.x + rightSize, transform.position.y);
            list.Add(right);
        }
        return list;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        var passConnect = collider.GetComponent<PassConnect>();
        if (passConnect != null) {
            passConnectList.Add(passConnect);
        }

        var obstance = collider.GetComponent<PassObstance>();
        if (obstance != null) {
            obstanceList.Add(obstance);
        }
    }
}