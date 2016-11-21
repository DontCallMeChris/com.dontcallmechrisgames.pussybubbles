using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Looser : MonoBehaviour {
	bool on;
	bool needcheck;
	BallScriptPointer pball;

	void Start(){
		on = true;
		needcheck = false;
		pball = null;
	}

	void OnTriggerStay2D(Collider2D other) {
		if (on && other.tag == "Ball") {
			pball = (other.gameObject.GetComponent("BallScript") as BallScript).BallScriptPtr;
			needcheck = true;
		}
	}

	void LateUpdate (){
		if (needcheck) {
#if UNITY_EDITOR
			Debug.Log("Looser checking");
#endif
			if (IGame.Instance.InPull (pball, new List<BallScriptPointer> ())) {
				IGame.Instance.LoseGame ();
				on = false;
            }
			needcheck = false;
        }
	}
}
