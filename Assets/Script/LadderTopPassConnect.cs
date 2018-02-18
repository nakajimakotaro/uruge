using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTopPassConnect: PassConnect {
	public PassConnect downPoint;
	void Start(){
		base.passConnectList.Add(downPoint);
	}

}