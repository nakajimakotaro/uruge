using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassConnect : MonoBehaviour{

	public Transform owner;
	public List<PassConnect> passConnectList;

	void Start(){
	}

	void OnTriggerEnter2D(Collider2D collider){
		var passConnect = collider.GetComponent<PassConnect>();
		if(passConnect != null){
			passConnectList.Add(passConnect);
		}
	}
}
