using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderDownPassConnect: PassConnect {
	public PassConnect topPoint;
	void Start(){
		base.passConnectList.Add(topPoint);
	}
}