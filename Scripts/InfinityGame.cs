using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfinityGame : IGame {

	private int level = 1;
	private float speed = 2f;
	private int reverce_bonus = 0;
	private int pair = 0;

	List<BallScriptPointer> Bottom9Balls;

	public override GAMETYPE GameType(){
		return GAMETYPE.INFINITY;
	}

	// Use this for initialization
	new void Awake (){
		base.Awake ();
		Bottom9Balls = new List<BallScriptPointer> ();
		foreach ( BoxCollider2D bx in WallBottom.GetComponentsInChildren<BoxCollider2D>()){
			bx.enabled = false;
		}
		foreach ( SpriteRenderer sr in GameObject.Find("BottomWallSprite").GetComponentsInChildren<SpriteRenderer>()){
			sr.enabled = false;
		}

		GameSetting.Instance.gridX = 9;
		GameSetting.Instance.gridY = 8;


		for (int y = 0; y < GameSetting.Instance.gridY; y++) {
			for (int x = 0; x < GameSetting.Instance.gridX; x++) {
				if(y % 3 == 2 && x % 3 == 1)
					continue;
				Vector2 pos = new Vector2 ((x - GameSetting.Instance.gridX / 2 + 0.5f * (y % 2)) * GameSetting.Instance.spacing + 0.64f, -2.56f -20.5f + GameSetting.Instance.spacing / 2 + y * 2.22f -2.22f);
				BallScriptPointer ball = newBall (pos, true);

				if(y == 0)
					Bottom9Balls.Add(ball);

				if(Random.Range(0, 1000) < 10){ //986
					ball.setColor (BALLTYPE.BOMB);
				}
				else if(Random.Range(0, 1000) < 5){ //986
					ball.setColor (BALLTYPE.MOVEDOWN);
				}
			}
		}
		
		foreach (BallScriptPointer ball in BallList)
			ball.getBall().ConnectToBallsInDistance (2.87f);

		foreach (BallScriptPointer ball in Bottom9Balls)
			if (ball.getGameObject ().activeSelf) {
				ball.getBall ().GetComponent<Rigidbody2D>().isKinematic = true;
				ball.getBall ().armor = -1;
			}
		ShotsFromLastLine = 0;
	}

	new void FixedUpdate (){
		base.FixedUpdate ();
		if(!GameSetting.Instance.IsGameFinished() && GameSetting.Instance.IsGame)
			AddLine();
	}

	new void AddLine (){
		bool bNeedToAddLine = false;
		foreach (BallScriptPointer pball in Bottom9Balls) {
			if (pball.getGameObject ().activeSelf) {
				if(pball.getPosition().y < -80.34)
					reverce_bonus = 0;
				if(reverce_bonus > 0)
					pball.getBall ().GetComponent<Rigidbody2D>().MovePosition (new Vector2(pball.getPosition().x, pball.getPosition().y - 0.001f * 10));
				else
					pball.getBall ().GetComponent<Rigidbody2D>().MovePosition (new Vector2(pball.getPosition().x, pball.getPosition().y + 0.001f * speed));

				if(pball.getPosition().y > -19.56f-2.22f)
					bNeedToAddLine = true;
			}
		}
		if(reverce_bonus > 0)
			reverce_bonus--;
		if (bNeedToAddLine)
			AddBallsInNewLine ();
	}

	public override void Turn () {
		if (!GameSetting.Instance.IsGame)
			StartGame ();
		UpdateUI ();
	}

	public void UpdateSpeed(){
//		int b = mScoreManager.iTotalBallBanged;
//		if(level > 1)
//			b -= (2 << (level - 1));
//		if (mScoreManager.iTotalBallBanged > (2 << level)) {
//			level++;
//			speed = 1 + Mathf.Sqrt(level) * 2;
//			b = mScoreManager.iTotalBallBanged;
//			b -= (2 << (level - 1));
//			Menu.Instance.SplashText ("Level " + level);
//		}
//		if(level > 1)
//			Menu.Instance.UITurnBar.value = (float) b / (2 << level - 1);
//		else
//			Menu.Instance.UITurnBar.value = (float) b / 4;

		//	1	2	3	4	5	6	7	8	9	10
		//	0	4	8	16	32	64	128	256	512	1024
		//	0	4	4	8	16	32	64	128	256	512

		//	10	15	20	25	30	35	40	45	50	55
		//	10	25	45	70	100	135	175	220	270	325


		int b = mScoreManager.iTotalBallBanged;
		if(level > 1)
			b -= ( 16 + 32 * (level - 2));
		if (mScoreManager.iTotalBallBanged > ( 16 + 32 * (level - 1) )) {
			level++;
			speed = 2 + Mathf.Sqrt(level) * 2;
			b = mScoreManager.iTotalBallBanged;
			b -= ( 16 + 32 * (level - 2));
			Menu.Instance.SplashText ("Level " + level);
		}
		if(level > 1)
			Menu.Instance.UITurnBar.value = (float) b / 32;
		else
			Menu.Instance.UITurnBar.value = (float) b / 16;

//		Debug.Log ("UpdateSpeed: b " + b + " speed" + speed + " iTotalBallBanged" + mScoreManager.iTotalBallBanged);
//		Debug.Log ("Level: " + level + " Speed: " + speed);
	}

	public override void BonusBallsBanged (int countOfBalls){
		if (countOfBalls > 2)
			reverce_bonus = 7 * countOfBalls;
	}


	void AddBallsInNewLine()
	{
		ShotsFromLastLine = 0;

		foreach (BallScriptPointer ball in Bottom9Balls)
			if (ball.getGameObject ().activeSelf) {
				ball.getBall ().GetComponent<Rigidbody2D>().isKinematic = false;
				ball.getBall().armor = 0;
			}

		Bottom9Balls.Clear ();
		pair++;
		for (int x = 0; x < GameSetting.Instance.gridX; x++) {
			if(pair % 3 == 1 && x % 3 == 1)
				continue;
			Vector2 pos = new Vector2 ((x - GameSetting.Instance.gridX / 2 + 0.5f * (pair % 2)) * GameSetting.Instance.spacing + 0.64f, -2.56f - 20.5f + GameSetting.Instance.spacing / 2 -2.22f);
			BallScriptPointer ball = newBall (pos, true);
			Bottom9Balls.Add (ball);
			ball.getBall().ConnectToBallsInDistance (2.87f);

			if(Random.Range(0, 1000) < 20){ //986
				ball.setColor (BALLTYPE.BOMB);
			}
			else if(Random.Range(0, 1000) < 30){ //986
				ball.setColor (BALLTYPE.MOVEDOWN);
			}

		}
		foreach (BallScriptPointer ball in Bottom9Balls)
			if (ball.getGameObject ().activeSelf) {
				ball.getBall ().GetComponent<Rigidbody2D>().isKinematic = true;
				ball.getBall().armor = -1;
			}
	}

	public override void LoseGame () {
		WinGame ();
	}
}