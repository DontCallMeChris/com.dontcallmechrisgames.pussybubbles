using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DFS : MonoBehaviour
{

	public static DFS dfs;
	Queue<BallScriptPointer> balls;

	public void addBall (BallScriptPointer pball)
	{
		balls.Enqueue (pball);
	}

	// Use this for initialization
	void Awake ()
	{
		
		if(dfs != null && dfs != this)
		{
			Destroy(gameObject);
			return;
		}
		dfs = this;
		
		balls = new Queue<BallScriptPointer> ();
	}
	
	void LateUpdate ()
	{
		while (balls.Count > 0) {
			BallScriptPointer ball = balls.Dequeue ();
			List<BallScriptPointer> gm = new List<BallScriptPointer> ();
			if (calculateDFS (ball, gm) >= 2) {
				bool gonnablow = false;

				foreach (BallScriptPointer pball in gm){
					if (!pball.getBall ().isbasic) {
						ball.getBall ().setBang ();
						gonnablow = true;
						break;
					}
				}
				if (gonnablow){
					foreach (BallScriptPointer pball in gm) {
						ActiveAnimation.Play(pball.getGameObject ().GetComponent<Animation>(), "BallBeamSuper", AnimationOrTween.Direction.Forward);
					}
				}
			}
		}	
	}

	public int calculateDFS (BallScriptPointer pball, List<BallScriptPointer> list, bool bang = false)
	{
		int result = 0;
		
		list.Add (pball);
		
		foreach (JointRef joint in pball.GetJoints()) {
			if (joint.joint == null)
				continue;
			
			if (!(list.Contains (joint.connectedBall))) {
				if (joint.connectedBall.color == pball.color) {
					result++;
					result += calculateDFS (joint.connectedBall, list, bang);
				}
			}
		}
		
		
		if (bang) {
			if(pball.getBall().isbasic)
				IGame.Instance.bangbang (pball.getPosition ());
			else
				IGame.Instance.bangbang (pball.getPosition (), false);
			pball.getBall ().Killitself ();
		}
		
		return result;
	}
}
