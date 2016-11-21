using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public enum GAMETYPE : byte
{
	NOSELECTED = 0,
	CLASSIC,
	PUZZLE,
	INFINITY
};

[Serializable]
public struct LevelResult 
{
	public bool bComplete;
	public int iScore;
	public int iStars;
	public float fTime;
	
	LevelResult(int score, float time, int stars){
		bComplete = false;
		iScore = score;
		iStars = stars;
		fTime = time;
	}
	
	public void Complete (int score, float time, int stars){
		bComplete = true;
		iScore = score;
		iStars = stars;
		fTime = time;
	}

	public void Loose (int score, float time){
		bComplete = false;
		iStars = 0;

		iScore = score;
		fTime = time;
	}
}

[Serializable]
public struct ChapterLevel
{
	public GAMETYPE mGameType;
	public int iChapter;
	public int iLevel;

	public ChapterLevel (GAMETYPE gametype, int chapter, int level){
		mGameType = gametype; 
		iChapter = chapter;
		iLevel = level;
    }
    
	public ChapterLevel (int chapter, int level){
		mGameType = GAMETYPE.NOSELECTED;
		iChapter = chapter;
		iLevel = level;
	}
	
	public override string ToString(){
		return iChapter.ToString () + "-" + iLevel.ToString ();
	}
}

public class GameSetting : MonoBehaviour {

	public static GameSetting Instance;

	public GoogleAnalyticsV3 googleAnalytics;

	//GameLogic
	public bool IsWin; // была ли победа 
	public bool IsLose; // было ли поражение 
	public bool IsGame; // идет ли игровой процесс 
	private bool IsPause; // включена ли пауза
	public float savedTimeScale;

	public string PlayerName;

	public Sprite spriteBomb;
	public Sprite spriteMoveDown;
	public Sprite spriteBasic;
	public Sprite spriteMain;
	public Sprite spriteArmor;

	public ParticleSystem particleBang;
	public ParticleSystem particleShockWave;
	public GameObject BallObject;

	public float gridX;
	public float gridY;
	public float spacing;

	public ChapterLevel mCurrentChapterLevel;
	public LevelResult mCurrentLevelResult;
	public Dictionary<ChapterLevel, LevelResult> mLevelResults;

	private string android_id;

	private float doubleclickescape;

	private bool quitGame;
	private int framebeforequit;


	public bool bToolTipOff;
	public bool ShowToolTips { 
		get { 
			return !bToolTipOff; 
		}
	}

	public void ToggleToolTips () {
		bToolTipOff = !bToolTipOff;
		PlayerPrefs.SetInt ("bToolTipOff", bToolTipOff ? 1 : 0);
	}

	public void QuitGame(){
		quitGame = true;
		framebeforequit = 0;
		Banner.Instance.Hide();
		GameSetting.Instance.googleAnalytics.StopSession();
	}

	void LateUpdate(){
		if(quitGame == true){
			if(framebeforequit > 5)
				Application.Quit ();
			else
				framebeforequit++;
		}
	}

	struct Statistics {
		/*
		 * ID
		 * Name
		 * ~G+
		 * ~FB
		 * ~TW
		 * Current game session
		 * -RunTime
		 * -Time
		 * -NewGame
		 * -Wins
		 * -Lose
		 * 
		 * Total game session
		 * -qRunProgram 
		 * -qExitProgram
		 * 
		 * */

		int iCounterApplicationLaunch; //Счетчик запуска приложения

//		Statistics(){
//			iCounterApplicationLaunch = 0;
//			LoadPrefs();
//		}

		void LoadPrefs(){
			iCounterApplicationLaunch = PlayerPrefs.GetInt ("iCounterApplicationLaunch");
		}

		void SavePrefs(){
			PlayerPrefs.SetInt ("iCounterApplicationLaunch", iCounterApplicationLaunch);
		}

		void ApplicationLaunch(){
			iCounterApplicationLaunch++;
			//googleAnalytics.LogEvent ("Application", "Launch", "Application Launched", iCounterApplicationLaunch);


		}

	

	}

	void Awake () {

		// First we check if there are any other instances conflicting
		if (Instance != null && Instance != this) {
			// If that is the case, we destroy other instances
			Destroy (gameObject);
			return;
		}

		DontDestroyOnLoad (this);

		// Here we save our singleton instance
		Instance = this;


		PlayerName = PlayerPrefs.GetString ("PlayerName");
		LoadLevelResults ();

		mCurrentLevelResult = new LevelResult ();
		mCurrentChapterLevel = new ChapterLevel ();

		mCurrentChapterLevel.iChapter = PlayerPrefs.GetInt ("LastChapter");
		mCurrentChapterLevel.iLevel = PlayerPrefs.GetInt ("LastLevel");

		bToolTipOff = PlayerPrefs.GetInt ("bToolTipOff") > 0;

		//mLevelResults.Add (mCurrentChapterLevel, mCurrentLevelResult);

		#if !UNITY_EDITOR
		AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
		AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
		android_id = secure.CallStatic<string> ("getString", contentResolver, "android_id");
		#endif

		int iCounterApplicationLaunch; //Счетчик запуска приложения
		iCounterApplicationLaunch = PlayerPrefs.GetInt ("iCounterApplicationLaunch");
		Debug.Log("iCounterApplicationLaunch = " + iCounterApplicationLaunch);
		if(iCounterApplicationLaunch == 0){
			switch(Application.systemLanguage.ToString()){
			case "Russian":
				Localization.language = "Русский";
				break;
			case "Ukrainian":
				Localization.language = "Українська";
				break;
			default:
				Localization.language = "English";
				break;
			}
		}
		iCounterApplicationLaunch++;
		GameSetting.Instance.googleAnalytics.LogEvent ("Statistics", "StartApplication", "iCounterApplicationLaunch", iCounterApplicationLaunch);
		PlayerPrefs.SetInt ("iCounterApplicationLaunch", iCounterApplicationLaunch);

		quitGame = false;
	}

	public string GetDeviceID(){
		#if UNITY_EDITOR
		return "UNITYEDITOR";
		#elif !UNITY_EDITOR
		return android_id;
		#endif
	}

	// Use this for initialization
	void Start () {
		googleAnalytics.StartSession();
	}

	void OnDestroy(){
		googleAnalytics.StopSession ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Menu)) {
			Debug.Log ("KeyCode.Menu");
			//Application.OpenURL ("market://details?q=pname:com.dontcallmechrisgames.pussybubbles/");

			if (IGame.Instance != null && !(GameSetting.Instance.IsGameFinished())) {
				TogglePause ();

			}
		}

		if (doubleclickescape != 0 && doubleclickescape < Time.time - 0.4f)
			doubleclickescape = 0;

		if (Input.GetKeyDown (KeyCode.Escape)) {

			if(Menu.Instance.state){

				if(doubleclickescape != 0){
					doubleclickescape = 0;
					Menu.Instance.moExit();
				}
				else{
					doubleclickescape = Time.time;
				}
			}
			else{
				if (IGame.Instance != null && !(GameSetting.Instance.IsGameFinished())) {
					TogglePause ();
					Debug.Log ("KeyCode.Escape");
				}
			}
			Debug.Log ("KeyCode.Escape " + doubleclickescape);
			//Menu.Instance.UILoading.SetActive(!Menu.Instance.UILoading.activeSelf);
		}
	}

	void OnLevelWasLoaded(int level) {

		if (Instance != this) {
			Debug.Log ("WTF MENU??? " + level);
			return;
		}
		
		Debug.Log ("LEVELLOADED " + level);

		UnPauseGame ();		
	}



	/***********************
	 * 
	 * 		PAUSE GAME
	 * 
	 ***********************/

	private void PauseGame ()
	{
		if (!IsPause) {
			IsPause = true;

			Menu.Instance.TogglePause();
			savedTimeScale = Time.timeScale;
			Time.timeScale = 0;
//			AudioListener.pause = true;
			Banner.Instance.Show();
		}
	}
	
	public void UnPauseGame ()
	{
		if (IsPause) {
			IsPause = false;

			Menu.Instance.TogglePause();
			Time.timeScale = GameSetting.Instance.savedTimeScale;
//			AudioListener.pause = false;

			Banner.Instance.Hide();
		}
	}
	
	public void TogglePause () {
		if (GameSetting.Instance.IsPause) {
			Menu.Instance.HideMenu();
		}
		else
			PauseGame ();
	}
	

	
	public bool IsGamePaused ()
	{
		return IsPause?true:false;
//		return (Time.timeScale == 0);
	}
	
	void OnApplicationPause (bool pause)
	{
		if (IsGamePaused ()) {
//			AudioListener.pause = true;
		}
	}

	public bool IsGameFinished () {
		if (GameSetting.Instance.IsWin || GameSetting.Instance.IsLose)
			return true;
		return false;
    }
    




	
	public void SendScore () {
		GameObject.Find ("Button Save Result").GetComponent<UIButton>().isEnabled = false;
		GameObject.Find ("Button Change Name").GetComponent<UIButton>().isEnabled = false;
		GameObject.Find ("Button Save Result").GetComponentInChildren<UILabel>().text = "Sending...";
		StartCoroutine (IGame.Instance.mScoreManager.postScore (GameSetting.Instance.PlayerName));
	}

	public void ChangePlayerName () {
		GameObject.Find ("Button Save Result").GetComponent<UIButton>().isEnabled = false;
		GameObject.Find ("Button Change Name").GetComponent<UIButton>().isEnabled = false;
		Menu.Instance.ShowWindowEnterName ();
	}

	public void SavePlayerName (string PlayerName) {
		this.PlayerName = PlayerName;
		PlayerPrefs.SetString ("PlayerName", PlayerName);
	}

	public void SaveLevelResults (){
		BinaryWriter Writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "//.BubbleRockLevelResult"));
		
		Writer.Write ("BubbleRockLevelResult");
		Writer.Write (2);

		foreach (KeyValuePair<ChapterLevel, LevelResult> kvp in mLevelResults) {
			Writer.Write("LR");
			Writer.Write((int)kvp.Key.mGameType);
			Writer.Write(kvp.Key.iChapter);
			Writer.Write(kvp.Key.iLevel);
			
			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream ms = new MemoryStream();
			bf.Serialize(ms, kvp.Value);
			
            Writer.Write(ms.ToArray());
        }
        
        Writer.Write("End");
        
        // Writer raw data
        Writer.Flush();
        Writer.Close();
	}


	public void LoadLevelResults(){

		mLevelResults = new Dictionary<ChapterLevel, LevelResult>();

		if (!File.Exists (Application.persistentDataPath + "//.BubbleRockLevelResult"))
			return;

		BinaryReader Reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "//.BubbleRockLevelResult"));
		
		string title = Reader.ReadString ();
		if (title != "BubbleRockLevelResult") {
			Debug.Log ("Bad title");
			return;
		}
		int version = Reader.ReadInt32();
		
		if (version != 2) {
			Debug.Log ("Bad version");
			return;
		}


		while (true) {
			string name = Reader.ReadString();
			if(name == "End")
				break;
			if(name != "LR"){
				Debug.Log ("Bad data? LevelResult");
				return;
			}
			GAMETYPE gametype = (GAMETYPE)Reader.ReadInt32();
			int chapter = Reader.ReadInt32();
			int level = Reader.ReadInt32();
			Debug.Log ( "GAMETYPE: " + gametype + " Chapter: " + chapter + " Level: " + level);

			ChapterLevel mChapterLevel = new ChapterLevel(gametype, chapter, level);
			
			BinaryFormatter bf = new BinaryFormatter();
			LevelResult mLevelResult = (LevelResult)bf.Deserialize(Reader.BaseStream);
			mLevelResults.Add(mChapterLevel, mLevelResult);        
        }

        Reader.Close();
	}
//
//	public ChapterData LoadChapter(Stream s){
//
//		gridX = 9;
//		gridY = 8;
//
//		//		TextAsset asset = Resources.Load("lvl.bytes") as TextAsset;
//		//		Stream s = new MemoryStream(asset.bytes);
//		//		BinaryReader Reader = new BinaryReader (s);
//		ChapterData result = new ChapterData();
//		BinaryReader Reader = new BinaryReader (s);
//		
//		string title = Reader.ReadString ();
//		if (title != "BubbleRockMap") {
//			Debug.Log ("Bad title");
//			return null;
//		}
//		int version = Reader.ReadInt32();
//		
//		int i = 0;
//		
//		switch (version) {
//			
//		case 2:
//			
//			result.GameType = (GAMETYPE)Reader.ReadInt32();
//			result.ID = Reader.ReadInt32();
//			int k = Reader.ReadInt32();
//			if(k>0){
//				result.IDsNeedToBeFinishedForUnlock = new int[k];
//				for(int j = 0; j < k; j++)
//					result.IDsNeedToBeFinishedForUnlock[j] = Reader.ReadInt32();
//			}
//			result.Name = Reader.ReadString();
//			result.Description = Reader.ReadString();
//			k = Reader.ReadInt32();
//			if(k == 1)
//				result.IconName = Reader.ReadString();
//			else if(k == 2)
//				result.IconTexture = Reader.ReadBytes(0);
//			
//			
//			result.LevelList = new List<LevelData>();
//			
//			i = 0;
//			while (true) {
//				string name = Reader.ReadString();
//				if(name == "End")
//					break;
//				if(name != "Level"){
//					Debug.Log ("Bad data? Level" + " i " + i + " name " + name);
//					return null;
//				}
//				int level = Reader.ReadInt32();
//				Debug.Log ("Level: " + level);
//				if(level != i)
//					Debug.Log ("Level number corrupt? level " + level + " i " + i);
//				
//				LevelData levelData = new LevelData();
//				foreach (LevelData.CellBall cell in levelData.CellBallList) {
//					cell.type = (BALLTYPE) Reader.ReadInt32();
//					cell.basic = Reader.ReadBoolean();
//
//					cell.armor = 0; Reader.ReadBoolean(); //Reader.ReadInt32();
//					//					Debug.Log ("Level: " + level + " cell " + cell.type);
//				}
//				result.LevelList.Add(levelData);
//				i++;
//				//				Debug.Log ("Level: " + level + " done");
//			}
//			break;
//
//
//			/**************************************************************************/
//
//		case 3:
//			
//			result.GameType = (GAMETYPE)Reader.ReadInt32();
//			result.ID = Reader.ReadInt32();
//			k = Reader.ReadInt32();
//			if(k>0){
//				result.IDsNeedToBeFinishedForUnlock = new int[k];
//				for(int j = 0; j < k; j++)
//					result.IDsNeedToBeFinishedForUnlock[j] = Reader.ReadInt32();
//			}
//			result.Name = Reader.ReadString();
//			result.Description = Reader.ReadString();
//			k = Reader.ReadInt32();
//			if(k == 1)
//				result.IconName = Reader.ReadString();
//			else if(k == 2)
//				result.IconTexture = Reader.ReadBytes(0);
//			
//			
//			result.LevelList = new List<LevelData>();
//			
//			i = 0;
//			while (true) {
//				string name = Reader.ReadString();
//				if(name == "End" || name == "Object")
//					break;
//				if(name != "Level"){
//					Debug.Log ("Bad data? Level");
//					return null;
//				}
//				int level = Reader.ReadInt32();
//				Debug.Log ("Level: " + level);
//				if(level != i)
//					Debug.Log ("Level number corrupt? level " + level + " i " + i);
//				
//				LevelData levelData = new LevelData();
//				levelData.CellBallList.Clear();
//				int j = 0;
//				while(j < 9*13){
//					j++;
//					
//					levelData.CellBallList.Add(new LevelData.CellBall(
//						(BALLTYPE) Reader.ReadInt32(),
//						Reader.ReadBoolean(),
//						Reader.ReadInt32()));
//					//					Debug.Log ("Level: " + level + " cell " + cell.type);
//				}
//				result.LevelList.Add(levelData);
//				i++;
//				Debug.Log ("Level: " + level + " done");
//			}
//			break;
//					
//		default:
//			Debug.Log ("Bad version");
//			break;
//		}
//		
//		Reader.Close();
//		
//		return result;
//	}

	/*
	 * 
	 * 
	 * 
	 * */


}
