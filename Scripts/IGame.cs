using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IGame : MonoBehaviour
{

	public static IGame Instance;
	public float SecondFromStart; // время с начала игры
	public int ShotsFromLastLine; // количество выстрелов с последнего подъема

	public int[] colorballscount;
	public int next_color;
	public int second_color;



	//	GameObjects
	public List<BallScriptPointer> BallList;
	public List<BallScript> BallListPointers;
	public List<ParticleSystem> particleBangList;
	public List<ParticleSystem> particlePopList;
	public GameObject WallBottom; // тригер для нижнего ряда внизу стакана
	public GameObject WallBottomCollider; // тригер для нижнего ряда внизу стакана
	public GameObject ChangeColorBox; // тригер смены цвета

	public GameObject nextColorball;
	public GameObject nextColorbox;
	private Vector2 vGravityLastSaved;
	public ScoreManager mScoreManager;

	public virtual GAMETYPE GameType ()
	{
		return GAMETYPE.NOSELECTED;
	}

	protected void Awake ()
	{
		// First we check if there are any other instances conflicting
		if (Instance != null && Instance != this) {
			// If that is the case, we destroy other instances
			Destroy (gameObject);
		}		
		// Here we save our singleton instance
		Instance = this;
		
		//Camera setup for phone screen
		Camera.main.orthographicSize = 20.5f;
		float ratio = ((float)Screen.height / (float)Screen.width);
		if (ratio > 1.64f)
			Camera.main.orthographicSize = 12.5f * ratio;



		GameSetting.Instance.IsWin = false; // победы не было
		GameSetting.Instance.IsLose = false; // поражения не было
		GameSetting.Instance.IsGame = false; // игровой процесс еще не идет

		PlayerPrefs.SetInt ("LastChapter", GameSetting.Instance.mCurrentChapterLevel.iChapter);
		PlayerPrefs.SetInt ("LastLevel", GameSetting.Instance.mCurrentChapterLevel.iLevel);

		colorballscount = new int[(int)BALLTYPE.LASTONE];
		next_color = Random.Range (0, 6);
		second_color = Random.Range (0, 6);

		BallList = new List<BallScriptPointer> ();
		particleBangList = new List<ParticleSystem> ();
		particlePopList = new List<ParticleSystem> ();

		nextColorball = gameObject.GetComponentInChildren<NextColorGun> ().gameObject;
		nextColorbox = gameObject.GetComponentInChildren<NextColorBox> ().gameObject;
		
		for (int i = 0; i < 8; i++) {
			ParticleSystem p = Instantiate (GameSetting.Instance.particleBang, Vector3.zero, Quaternion.identity) as ParticleSystem;
			p.name = "BangSystem:" + particleBangList.Count;
			particleBangList.Add (p);
		}

		for (int i = 0; i < 8; i++) {
			ParticleSystem p = Instantiate (GameSetting.Instance.particleShockWave, Vector3.zero, Quaternion.identity) as ParticleSystem;
			p.name = "PopSystem:" + particlePopList.Count;
			particlePopList.Add (p);
		}
		
		for (int i = 0; i < 40; i++) {
			Instantiate (GameSetting.Instance.BallObject, Vector3.zero, Quaternion.identity);
//			GameObject go = Instantiate (GameSetting.Instance.BallObject, Vector3.zero, Quaternion.identity) as GameObject;
//			go.SetActive (false);
		}
		
		if (WallBottom == null)
			WallBottom = GameObject.Find ("Bottom wall");

		if (WallBottomCollider == null)
			WallBottomCollider = GameObject.Find ("BottomCollider");


		if (ChangeColorBox == null)
			ChangeColorBox = GameObject.FindGameObjectWithTag ("ChangeColorBox");
	}

	// Use this for initialization
	protected void Start ()
	{

		GameSetting.Instance.googleAnalytics.LogScreen ("IGame Start() " + GameType ().ToString ());

		NewGame ();	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameSetting.Instance.IsGame) {
			SecondFromStart += Time.deltaTime;	
			Menu.Instance.UITimer.text = SecondFromStart.ToString ("0.0");
		}

		if (GameSetting.Instance.IsWin) {
			foreach (ParticleSystem p in IGame.Instance.particleBangList) {
				if (!p.IsAlive ()) {
					p.transform.position = Random.insideUnitCircle * 15f;
					p.transform.position += Vector3.back * 5;
					p.Play ();
					break;
				}
			}
			foreach (ParticleSystem p in IGame.Instance.particlePopList) {
				if (!p.IsAlive ()) {
					p.transform.position = Random.insideUnitCircle * 15f;
					p.transform.position += Vector3.back * 5;
					p.Play ();
					break;
				}
			}
		}

		if (GameSetting.Instance.IsGameFinished () && !Menu.Instance.state) {
			if (Input.GetMouseButtonDown (0)) {
				Menu.Instance.ShowMenu ();
			}
		}
	}

	protected void FixedUpdate ()
	{
		if (GameSetting.Instance.IsGame && !GameSetting.Instance.IsGamePaused ()) {
			Vector2 gravity = new Vector2 (Input.acceleration.x, -1);
			gravity.Normalize ();
			float dot = Vector2.Dot (gravity, vGravityLastSaved);

			//Debug.Log("gravity-up-date: " + dot);
			if (dot < 0.98f) {
				vGravityLastSaved = gravity;
				Physics2D.gravity = vGravityLastSaved * 30.0f;
			}
		}
	}

//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define
//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define
//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define
//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define
//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define

	public virtual bool InPull (BallScriptPointer pball, List<BallScriptPointer> list)
	{
		bool result = false;
		list.Add (pball);
		
		foreach (JointRef joint in pball.GetJoints()) {
			if (joint.joint == null)
				continue;
			
			if (!(list.Contains (joint.connectedBall))) {
				if (joint.connectedBall.getGameObject ().GetComponent<Rigidbody2D> ().isKinematic)
					return true;
				result = InPull (joint.connectedBall, list);
				if (result)
					return result;
			}
		}
		
		return result;
	}

	public virtual Vector3 connect2wall (Vector3 v)
	{
		// 18
		// 	0 	-10.88	-9.6		17	15		8	6	4	2	0	
		//	1	-8.32	-7.04		13	11		7	5	3	1	0
		//	2	-5.76	-4.48		9	7
		//	3	-3.2	-1.92		5	3
		//	4	-0.64	0.64		1	1
		//	5	1.92	3.2
		//	6	4.48	5.76
		//	7	7.04	8.32
		//	8	9.6		10.88
		
		float y = WallBottom.transform.position.y + 1.0f + 1.28f; //18.72f;

		return new Vector3 (Mathf.Round (v.x / 0.64f) * 0.64f, y, v.z);
	}

	public void TryRemoveJoints ()
	{
		foreach (BallScriptPointer pball in BallList)
			pball.RemoveJoints ();
	}

	public void boombomb (Vector3 v)
	{
		foreach (BallScriptPointer pball in BallList) {
			if (Vector3.Distance (v, pball.getPosition ()) < 3.84f)
				pball.getBall ().setBang (0);
		}
		BonusBallsBanged (7);
		Menu.Instance.SplashText ("BOOM!!!", true);
	}

	//Возвращает неактивный экземпляр пузыря или создает новый
	BallScriptPointer GetFreeBall ()
	{
		foreach (BallScriptPointer pball in BallList) {
			if (!pball.getGameObject ().activeSelf) {
				if (pball.getBall ().isbang) {
					//Debug.LogError("FAIL BALL " + pball.getBall().name + " pball.getBall().isbang ");
					pball.getBall ().Refresh ();
					continue;
				}

				return pball;
			}
		}

		GameObject go = Instantiate (GameSetting.Instance.BallObject, Vector3.zero, Quaternion.identity) as GameObject;
//		go.SetActive (false);
		go = Instantiate (GameSetting.Instance.BallObject, Vector3.zero, Quaternion.identity) as GameObject;
//		go.SetActive (false);
		go = Instantiate (GameSetting.Instance.BallObject, Vector3.zero, Quaternion.identity) as GameObject;
		return (go.GetComponent ("BallScript") as BallScript).BallScriptPtr;
	}
	
	public BallScriptPointer newBall (Vector3 Position, bool isbasic = false)
	{
		BallScriptPointer pball = GetFreeBall ();
		pball.setPosition (Position);

		pball.getBall ().Refresh ();
		pball.getBall ().isbasic = isbasic ? true : false;
		pball.getBall ().armor = 0;
		pball.getBall ().GetComponent<Rigidbody2D> ().isKinematic = false;
		if (isbasic)
			pball.setColor (Random.Range (0, (int)BALLTYPE.LASTCOLOR));
		else
			pball.setColor (random_color ());
		
		pball.getGameObject ().SetActive (true);
		
		return pball;
	}

	public BallScriptPointer newBall (LevelData.CellBall _Cell)
	{
		BallScriptPointer pball = GetFreeBall ();
		pball.setPosition (_Cell.position);

		pball.getBall ().isbasic = _Cell.basic;
		pball.getBall ().armor = _Cell.armor;
		pball.getBall ().GetComponent<Rigidbody2D> ().isKinematic = _Cell.frozen;
		pball.setColor (_Cell.type);

		pball.getGameObject ().SetActive (true);
		
		return pball;

		//pball.getBall ().isbasic = isbasic ? true : false;
	}
	
	public void bangbang (Vector3 pos, bool isbasic = true)
	{
		bool boom = false;
		if (isbasic == true) {
			foreach (ParticleSystem p in particleBangList) {
				if (!p.IsAlive ()) {
					p.transform.position = pos + Vector3.back * 5;
					p.Play ();
					boom = true;
					break;
				}
			}
			if (!boom) {
				ParticleSystem p = Instantiate (GameSetting.Instance.particleBang, pos, Quaternion.identity) as ParticleSystem;
				particleBangList.Add (p);
			}
		}
		else{
			foreach (ParticleSystem p in particlePopList) {
				if (!p.IsAlive ()) {
					p.transform.position = pos + Vector3.back * 5;
					p.Play ();
					boom = true;
					break;
				}
			}
			if (!boom) {
				ParticleSystem p = Instantiate (GameSetting.Instance.particleBang, pos, Quaternion.identity) as ParticleSystem;
				particlePopList.Add (p);
			}
		}
	}
	
	public virtual void AddLine ()
	{
		ShotsFromLastLine = 0;
		
		foreach (BallScriptPointer pball in BallList)
			if (pball.getGameObject ().activeSelf)
				pball.Move (new Vector3 (0.0f, 2.56f, 0.0f));
		WallBottom.transform.position += new Vector3 (0.0f, 2.56f, 0.0f);
		//WallBottom.rigidbody2D.MovePosition (WallBottom.rigidbody2D.position + new Vector3 (0.0f, 2.56f, 0.0f))
//		Camera.main.transform.position += new Vector3 (0.0f, 1.28f, 0.0f);
		
//		checksomeobject ();
	}


	
//	private void checksomeobject ()
//	{
//		foreach (GameObject go in someobject)
//			if (go.transform.position.y > 30.0f)
//				Destroy (go);
//	}



	
	
	
	int ColorBallsCount_getamount ()
	{
		int result = 0;
		for (int i = 0; i < (int)BALLTYPE.LASTCOLOR; i++)
			if (colorballscount [i] > 0)
				++result;
		return result;
	}
	
	int ColorBallsCount_getcolor (int r)
	{
		int result = 0;
		for (; result < (int)BALLTYPE.LASTCOLOR; result++) {
			if (colorballscount [result] > 0)
				r--;
			if (r < 0)
				return result;
		}
		return result;
	}
	
	public virtual int newcolor ()
	{
		if (!GameSetting.Instance.IsGame)
			return Random.Range (0, 6);
		else {
			//ColorBallsCount_update ();
			return ColorBallsCount_getcolor (Random.Range (0, ColorBallsCount_getamount ()));
		}
	}
	
	public void CheckSecondColor ()
	{
		//ColorBallsCount_update ();
		if (colorballscount [second_color] == 0) {
			second_color = newcolor ();
			nextColorbox.GetComponent<Renderer> ().material.color = get_color (second_color);
		}
		
		if (colorballscount [next_color] == 0) {
			next_color = newcolor ();
			nextColorball.GetComponent<Renderer> ().material.color = get_color (next_color);
		}
	}
	
	int random_color ()
	{
		int c = next_color;
		next_color = second_color;
		second_color = newcolor (); 
		nextColorball.GetComponent<Renderer> ().material.color = get_color (next_color);
		nextColorbox.GetComponent<Renderer> ().material.color = get_color (second_color);

		return c;
	}
	
	public Color get_color (BALLTYPE i)
	{
		switch (i) {
		case BALLTYPE.RED:
			return new Color (0.9f, 0.1f, 0.1f, 1.0f);
		case BALLTYPE.GREEN:
			return new Color (0.1f, 0.9f, 0.1f, 1.0f);
		case BALLTYPE.BLUE:
			return new Color (0.1f, 0.1f, 0.9f, 1.0f);
		case BALLTYPE.YELLOW:
			return Color.yellow;
		case BALLTYPE.CYAN:
			return Color.cyan;
		case BALLTYPE.MAGENTA:
			return Color.magenta;
		case BALLTYPE.BOMB:
			return Color.white;
		case BALLTYPE.MOVEDOWN:
			return Color.white;
		case BALLTYPE.LASTONE:
			return new Color (1, 1, 1, 0.5f);
		default:
			return Color.white;
		}
	}
	
	public Color get_color (int i)
	{
		return get_color ((BALLTYPE)i);
	}
	
	public void ChangeColor ()
	{
		int b = next_color;
		next_color = second_color;
		second_color = b;
		nextColorball.GetComponent<Renderer> ().material.color = get_color (next_color);
		nextColorbox.GetComponent<Renderer> ().material.color = get_color (second_color);
	}


//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define
//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define
//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define
//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define
//	#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define#define

	public virtual void Turn ()
	{
		if (!GameSetting.Instance.IsGame)
			StartGame ();
		ShotsFromLastLine++;
		if (ShotsFromLastLine > 5)
			AddLine ();
		UpdateUI ();
	}

	public virtual void BonusBallsBanged (int countOfBalls)
	{
		ShotsFromLastLine--;
		if (countOfBalls > 2) {
			ShotsFromLastLine--;
		}
		if (ShotsFromLastLine < 0 || countOfBalls > 3)
			ShotsFromLastLine = 0;
	}

	public void UpdateUI ()
	{
		if (IGame.Instance.GameType () == GAMETYPE.INFINITY) {
			((InfinityGame)IGame.Instance).UpdateSpeed ();
		} else
			Menu.Instance.UITurnBar.value = (float)ShotsFromLastLine / 5.0f;
		if (GameSetting.Instance.IsGame)
			CheckSecondColor ();
	}

	private void NewGame ()
	{
		Menu.Instance.RefreshUI ();
		mScoreManager = new ScoreManager ();
		GameSetting.Instance.mCurrentLevelResult = new LevelResult ();
		GameSetting.Instance.IsWin = false;
		GameSetting.Instance.IsLose = false;
		GameSetting.Instance.IsGame = false; 
		SecondFromStart = 0.0f;
		ShotsFromLastLine = 0;

		random_color ();
		random_color ();

	}

	public virtual void ReloadLvl ()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void StartGame ()
	{
		GameSetting.Instance.IsGame = true;
		Menu.Instance.SplashText (Localization.Get("Go"));

	}

	public void CheckWin ()
	{

		if (GameSetting.Instance.IsGameFinished ()) {
			Debug.Log ("game is finished, u cant try to win twice");
			return;
		}

		UpdateUI ();
		foreach (BallScriptPointer pball in BallList)
			if (pball.getGameObject ().activeSelf)
				return;		
		WinGame ();
	}

	public virtual void WinGame ()
	{
		if (GameSetting.Instance.IsGameFinished ()) {
			Debug.Log ("game is finished, u cant win twice");
			return;
		}
		/*
		 * Show Banner 
		 */
		Banner.Instance.ShowInterstitial();

		GameSetting.Instance.googleAnalytics.LogScreen ("IGame Start() " + GameType ().ToString () + " WinGame ()");

		GameSetting.Instance.IsWin = true;
		GameSetting.Instance.IsLose = false;
		GameSetting.Instance.IsGame = false;

		mScoreManager.Calculate ();

		Menu.Instance.UIPanelInGame.SetActive (false);
		Menu.Instance.UIPanelWinner.SetActive (true);
		/*GameObject.Find ("Button Save Result").GetComponent<UIButton> ().isEnabled = true;
		GameObject.Find ("Button Save Result").GetComponentInChildren<UILabel> ().text = "Send score";
		GameObject.Find ("Button Change Name").GetComponent<UIButton> ().isEnabled = true;*/

		UILabel label = GameObject.Find ("UILabelWinnerText").GetComponent<UILabel> ();
		//label.text = mScoreManager.GetWinnerText ();
		label.text = mScoreManager.GetTotalScore().ToString();

		if (GameSetting.Instance.PlayerName == "")
			Menu.Instance.ShowWindowEnterName ();


//		Menu.Instance.ShowMenu();

		int iStars = mScoreManager.GetStars ();

		GameSetting.Instance.mCurrentLevelResult.Complete (mScoreManager.GetTotalScore (), SecondFromStart, iStars);
		if (GameSetting.Instance.mLevelResults.ContainsKey (GameSetting.Instance.mCurrentChapterLevel)) {
			LevelResult mLevelResult;
			if (GameSetting.Instance.mLevelResults.TryGetValue (GameSetting.Instance.mCurrentChapterLevel, out mLevelResult)) {
				GameSetting.Instance.mCurrentLevelResult.iStars = System.Math.Max (GameSetting.Instance.mCurrentLevelResult.iStars, mLevelResult.iStars);
				if (GameSetting.Instance.mCurrentLevelResult.bComplete == mLevelResult.bComplete) {
					if (GameSetting.Instance.mCurrentLevelResult.iScore == mLevelResult.iScore)
						GameSetting.Instance.mCurrentLevelResult.fTime = System.Math.Min (GameSetting.Instance.mCurrentLevelResult.fTime, mLevelResult.fTime);
					else if (GameSetting.Instance.mCurrentLevelResult.iScore < mLevelResult.iScore) {
						GameSetting.Instance.mCurrentLevelResult.iScore = mLevelResult.iScore;
						GameSetting.Instance.mCurrentLevelResult.fTime = mLevelResult.fTime;
					}
				} else if (!GameSetting.Instance.mCurrentLevelResult.bComplete) {
					GameSetting.Instance.mCurrentLevelResult.iScore = mLevelResult.iScore;
					GameSetting.Instance.mCurrentLevelResult.fTime = mLevelResult.fTime;
				}
				GameSetting.Instance.mLevelResults.Remove (GameSetting.Instance.mCurrentChapterLevel);
				GameSetting.Instance.mLevelResults.Add (GameSetting.Instance.mCurrentChapterLevel, GameSetting.Instance.mCurrentLevelResult);
			}
		} else {
			GameSetting.Instance.mLevelResults.Add (GameSetting.Instance.mCurrentChapterLevel, GameSetting.Instance.mCurrentLevelResult);
		}
		GameSetting.Instance.SaveLevelResults ();

		/*
		 * Sending score to server
		 */
		StartCoroutine (mScoreManager.postScore (GameSetting.Instance.PlayerName));

		GameSetting.Instance.googleAnalytics.LogEvent ("IGame Start() " + GameType ().ToString (), "WinGame", "Score", mScoreManager.GetTotalScore());
	}

	public virtual void LoseGame ()
	{
		if (GameSetting.Instance.IsGameFinished ()) {
			Debug.Log ("game is finished, u cant loose twice");
			return;
		}
		GameSetting.Instance.IsWin = false;
		GameSetting.Instance.IsLose = true;
		GameSetting.Instance.IsGame = false;
		Menu.Instance.UIPanelInGame.SetActive (false);
		Menu.Instance.UIPanelLooser.SetActive (true);

		foreach (BallScriptPointer pball in BallList)
			pball.getBall ().GetComponent<Rigidbody2D> ().isKinematic = true;



		GameSetting.Instance.mCurrentLevelResult.Loose (mScoreManager.GetTotalScore (), SecondFromStart);
		if (GameSetting.Instance.mLevelResults.ContainsKey (GameSetting.Instance.mCurrentChapterLevel)) {
			LevelResult mLevelResult;
			if (GameSetting.Instance.mLevelResults.TryGetValue (GameSetting.Instance.mCurrentChapterLevel, out mLevelResult)) {
				if (GameSetting.Instance.mCurrentLevelResult.bComplete == mLevelResult.bComplete) {
					if (GameSetting.Instance.mCurrentLevelResult.iScore == mLevelResult.iScore)
						GameSetting.Instance.mCurrentLevelResult.fTime = System.Math.Min (GameSetting.Instance.mCurrentLevelResult.fTime, mLevelResult.fTime);
					else if (GameSetting.Instance.mCurrentLevelResult.iScore < mLevelResult.iScore) {
						GameSetting.Instance.mCurrentLevelResult.iScore = mLevelResult.iScore;
						GameSetting.Instance.mCurrentLevelResult.fTime = mLevelResult.fTime;
					}
				} else if (!GameSetting.Instance.mCurrentLevelResult.bComplete) {
					GameSetting.Instance.mCurrentLevelResult.iScore = mLevelResult.iScore;
					GameSetting.Instance.mCurrentLevelResult.fTime = mLevelResult.fTime;
				}
				GameSetting.Instance.mLevelResults.Remove (GameSetting.Instance.mCurrentChapterLevel);
				GameSetting.Instance.mLevelResults.Add (GameSetting.Instance.mCurrentChapterLevel, GameSetting.Instance.mCurrentLevelResult);
			}
		} else {
			GameSetting.Instance.mLevelResults.Add (GameSetting.Instance.mCurrentChapterLevel, GameSetting.Instance.mCurrentLevelResult);
		}
		GameSetting.Instance.SaveLevelResults ();

		Menu.Instance.ShowMenu ();

	}

	void OnDestroy() {
		GameSetting.Instance.googleAnalytics.LogEvent ("IGame OnDestroy() " + GameType ().ToString (), "ExitGame", "TimeInGame", (long)SecondFromStart);
	}

}
