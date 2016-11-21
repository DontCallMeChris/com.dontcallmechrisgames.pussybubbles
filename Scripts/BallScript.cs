using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallScript : MonoBehaviour
{
	public bool isbang;
	public bool isbasic;
	public float bangtime;
	public BallScriptPointer BallScriptPtr;
	public float STRx;
	public float STRy;
	public int armor;

	public void setBang (float time = -1)
	{
		if (isbang && (time != 0 || BallScriptPtr.color == BALLTYPE.BOMB || BallScriptPtr.color == BALLTYPE.MOVEDOWN))
			return;
		if (time == -1)
			time = Time.time;
		isbang = true;
		bangtime = time;
		if (BallScriptPtr.color == BALLTYPE.BOMB)
			IGame.Instance.boombomb (BallScriptPtr.getPosition ());
	}

	public void Refresh ()
	{
		transform.localScale = Vector3.one;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		GetComponent<Rigidbody2D>().isKinematic = false;
		BallScriptPtr.RemoveAllJoints ();
		
		isbang = false;
		isbasic = false;
		GetComponent<Rigidbody2D>().isKinematic = false;
		bangtime = 0f;
		transform.localScale = Vector3.one;
        armor = 0;
    }
    
	// Use this for initialization
	void Awake ()
	{
		isbang = false;
		bangtime = 0f;
		BallScriptPtr = new BallScriptPointer (this);
		IGame.Instance.BallList.Add (BallScriptPtr);
		gameObject.name = "Ball:" + IGame.Instance.BallList.Count;
		gameObject.SetActive (false);
	}

	// Use this for initialization
	void Start ()
	{
	}

	void OnDisable(){
		Refresh ();
	}

	void OnEnable(){
		IGame.Instance.colorballscount [(int)BallScriptPtr.color]++;
	}
    
    void OnDestroy(){
		IGame.Instance.BallList.Remove (BallScriptPtr);
	}

	public void Killitself ()
	{
		if (IGame.Instance != null) {
			if (armor == 0) {

				foreach (JointRef joint in BallScriptPtr.GetJoints()) {
					if (joint.joint == null)
						continue;
					
					foreach (JointRef joint_backward in joint.connectedBall.GetJoints()) {
						if (joint_backward.joint == null)
							continue;
						if (joint_backward.connectedBall == BallScriptPtr) {
							joint_backward.joint.GetComponent<Rigidbody2D>().WakeUp ();
							joint_backward.Remove ();
						}
					}
					joint.Remove ();
				}


				IGame.Instance.mScoreManager.BallHadBeenBanged();

				IGame.Instance.colorballscount [(int)BallScriptPtr.color]--;
				if (IGame.Instance.colorballscount [(int)BallScriptPtr.color] < 0)
					Debug.Log ("colorballscount[" + BallScriptPtr.color + "] < 0");

				gameObject.SetActive (false);
			} else {
				armor--;
				BallScriptPtr.setColor(BallScriptPtr.color);
				isbang = false;
				bangtime = 0f;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate ()
	{
		if (GameSetting.Instance.IsGameFinished ())
			return;

		if (isbang && (bangtime < Time.time - 1.5f)) {
			List<BallScriptPointer> gm = new List<BallScriptPointer> ();
			IGame.Instance.mScoreManager.AddBangedBallsInCombo (DFS.dfs.calculateDFS (BallScriptPtr, gm, true));
			IGame.Instance.TryRemoveJoints ();
			IGame.Instance.CheckWin ();
		}

		if (BallScriptPtr.getPosition ().y < -100.0f){
			Debug.LogError("FAIL BALL " + name + " BallScriptPtr.getPosition ().y < -100.0f ");
			Debug.Break();
			setBang (0);
		}

	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (GetComponent<Rigidbody2D>().isKinematic)
			return;
		if (other.gameObject == IGame.Instance.WallBottomCollider) {
			BallScriptPtr.setPosition (IGame.Instance.connect2wall (BallScriptPtr.getPosition ()));
			GetComponent<Rigidbody2D>().isKinematic = true;
			ConnectToBallsInDistance ();
			return;
		}
	}

	public void ConnectToBallsInDistance (float distance = 3.1f)
	{


//		Debug.Log (Time.time + ": " + ball.getGameObject().name + ": " + ball.getPosition ().ToString());

//		Vector3 vector = new Vector3(2.56f, 0, 0);
		foreach (BallScriptPointer pball in IGame.Instance.BallList) {
			if (pball == BallScriptPtr)
				continue;
			if (!pball.getGameObject ().activeSelf)
				continue;

//			Debug.Log (Time.time + ": " + ball.getGameObject().name + ": From:" + ball.getPosition () + 
////			           " To:" + (ball.getPosition () + vector).ToString() +
//			           "To: " + pball.getGameObject().name +
//			           " " + (pball.getPosition()).ToString() +
//			           " Distance:" + Vector2.Distance(pball.getPosition(), ball.getPosition ()).ToString()
//			           );
			if (Vector2.Distance (pball.getPosition (), BallScriptPtr.getPosition ()) < distance) {
				bool getthatshit = false;
				foreach (JointRef jointref in BallScriptPtr.GetJoints()) {
					if (jointref.connectedBall.getGameObject () == pball.getGameObject ()) {
						getthatshit = true;
						break;
					}
				}
				if (!getthatshit) {
//					Debug.Log (Time.time + ": " + pball.getGameObject ().name + " Adding kinematic connect ");
					SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D> ();
					joint.connectedBody = pball.getGameObject ().GetComponent<Rigidbody2D>();
					joint.distance = 2.56f;
					joint.frequency = 0;
					JointRef newjointref = BallScriptPtr.AddJoint (joint);
					if (BallScriptPtr.color == BALLTYPE.BOMB) {
						if (!newjointref.connectedBall.getBall ().isbasic) {
							BallScriptPtr.getBall ().setBang (0);
						}
					}
					
					if (BallScriptPtr.color == BALLTYPE.MOVEDOWN && armor >= 0) {
						if (!newjointref.connectedBall.getBall ().isbasic) {
							BallScriptPtr.getBall ().setBang (0);
                            IGame.Instance.BonusBallsBanged (15);
						}
					}
					DFS.dfs.addBall (BallScriptPtr);
				}

				getthatshit = false;
				foreach (JointRef jointref in pball.GetJoints()) {
					if (jointref.connectedBall.getGameObject () == BallScriptPtr.getGameObject ()) {
						getthatshit = true;
						break;
					}
				}
				if (!getthatshit) {
//					Debug.Log (Time.time + ": " + pball.getGameObject ().name + " Adding kinematic connect BACK");
					SpringJoint2D joint = pball.getGameObject ().AddComponent<SpringJoint2D> ();
					joint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
					joint.distance = 2.56f;
					joint.frequency = 0;
					JointRef newjointref = pball.AddJoint (joint);
					if (pball.color == BALLTYPE.BOMB) {
						if (!newjointref.connectedBall.getBall ().isbasic) {
							pball.getBall ().setBang (0);
						}
					}
					
					if (pball.color == BALLTYPE.MOVEDOWN && armor >= 0) {
						if (!newjointref.connectedBall.getBall ().isbasic) {
							pball.getBall ().setBang (0);
                            IGame.Instance.BonusBallsBanged (15);
						}
					}
					DFS.dfs.addBall (pball);
				}
			}
		}
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		// Check if more then 6 connected balls, 
		// not allow to create more
		if (BallScriptPtr.GetJointsCount () > 6) {
#if UNITY_EDITOR
			Debug.Log ("too many connects!!!");
#endif
			GetComponent<Renderer>().material.color = Color.black;
			return;
		}


		// Creating new joint to the other ball
		if (coll.gameObject.tag == "Ball") {

			foreach (JointRef jointref in BallScriptPtr.GetJoints()) {
				if (jointref.connectedBall.getGameObject () == coll.gameObject) {
#if UNITY_EDITOR
					Debug.Log ("it connected!!!");
#endif

					return;
				}
			}

			//Debug.Log ("New joint");
			SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D> ();
			joint.connectedBody = coll.gameObject.GetComponent<Rigidbody2D>();
			joint.distance = 2.56f;
			joint.frequency = 0;

//			if(GameSetting.Instance.IsGame){
				//ContactPoint2D contact = coll.contacts[0];
				//IGame.Instance.bangbang (contact.point);
				//Instantiate (GameSetting.Instance.particleShockWave, contact.point, Quaternion.identity);
				//TODO: azazazaza tut iskri ot udarov 
//			}

			JointRef newjointref = BallScriptPtr.AddJoint (joint);

			if (BallScriptPtr.color == BALLTYPE.BOMB && armor >= 0) {
				if (!newjointref.connectedBall.getBall ().isbasic) {
					setBang (0);
				}
			}

			if (BallScriptPtr.color == BALLTYPE.MOVEDOWN && armor >= 0) {
				if (!newjointref.connectedBall.getBall ().isbasic) {
					setBang (0);
					IGame.Instance.BonusBallsBanged (15);
				}
			}

			DFS.dfs.addBall (BallScriptPtr);

			ConnectToBallsInDistance (2.87f);
		}

		//		if (coll.gameObject.tag == "Wheel") {
		//			
		//			//Debug.Log ("New joint");
		//			//HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D> ();
		//			//joint.connectedBody = coll.gameObject.rigidbody2D;
		//
		//
		//			transform.position.Set(coll.gameObject.transform.parent.position.x, coll.gameObject.transform.parent.position.y+2.56f, coll.gameObject.transform.parent.position.z);
		//
		//
		//			transform.RotateAround(coll.gameObject.transform.parent.position, Vector3.forward, coll.gameObject.transform.localRotation.z);
		//
		//			gameObject.rigidbody2D.isKinematic = true;
		//
		//
		//		}
	}
}