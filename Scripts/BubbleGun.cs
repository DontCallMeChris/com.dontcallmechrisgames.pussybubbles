using UnityEngine;
using System.Collections;

public class BubbleGun : MonoBehaviour {

	private float fLastClickTime;
	private bool bStartAim;
	private Vector2 vStartPos;

	// Use this for initialization
	void Start () {
		bStartAim = false;	
	}
	
	// Update is called once per frame
	void Update () {
		if (GameSetting.Instance.IsGameFinished())
			return;
		if (GameSetting.Instance.IsGamePaused ()) {
			bStartAim = false;
			return;
		}

//		if (Input.GetMouseButtonUp (1)) {
//			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			//Camera.main.ScreenToViewportPoint (Input.mousePosition);
//			main.Main.newBall (pos, false);			
//		}


		if (fLastClickTime < (Time.time - 0.33f)) {

			if (Input.GetMouseButtonDown (0)) {
				if (IGame.Instance.ChangeColorBox.GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					IGame.Instance.ChangeColor ();
				} else {
					vStartPos = Camera.main.ScreenToViewportPoint (Input.mousePosition);
					bStartAim = true;
				}
			}
			
			if (Input.GetMouseButtonUp (0) && bStartAim) {

				
				bStartAim = false;
				Vector2 finish_pos = Camera.main.ScreenToViewportPoint (Input.mousePosition);
				Vector2 pos = new Vector2 (0.0f, 18.5f);
				Vector2 force = finish_pos - vStartPos;
				Debug.Log("Force = " + force + " sqrt = " + force.sqrMagnitude);
				if(force.sqrMagnitude > .0001f){
					BallScriptPointer pball = IGame.Instance.newBall (pos, false);
					IGame.Instance.Turn ();
					
					force *= 20000f;
					if (force.y > 0)
						force = -force;
					if (force.y < -10000f)
						force.y = -10000f;
					if (force.y > -1000f)
						force.y = -1000f;
					
					if (force.x > 8000f)
						force.x = 8000f;
					if (force.x < -8000f)
						force.x = -8000f;

					pball.getGameObject ().GetComponent<Rigidbody2D>().AddForce (force);
					
					fLastClickTime = Time.time;
				}
			}
		}
	}
}
