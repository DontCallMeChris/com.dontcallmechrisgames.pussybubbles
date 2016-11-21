using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
	public static Menu Instance;

	public enum MENUOPTIONS : int
	{
		MO_EXIT = 0,
		MO_RESTART,
		MO_UNPAUSE,
		MO_NEWGAME_CLASSIC,
		MO_NEWGAME_PUZZLE,
		MO_NEWGAME_INFINITY,
		MO_LASTMENUOPTIONS
	}

	public enum MENUACTION : int
	{
		MA_NONE = 0,
		MA_QUIT,
		MA_EXIT2MENU,
		MA_BACK2MENU,
		MA_LOADNEWLEVEL,
		MA_RESTARTLEVEL,
		MA_LASTMENUACTION
	}

	//ButtonCACHE
	public class Button
	{
		public GameObject gameObject;
		public UILabel uiLabel;
		public UISprite uiSprite;
		public UILocalize uiLocalize;

		public Button (GameObject gameObject,
		       UILabel uiLabel,
		       UISprite uiSprite)
		{
			this.gameObject = gameObject;
			this.uiLabel = uiLabel;
			this.uiLocalize = uiLabel.GetComponent<UILocalize>();
			this.uiSprite = uiSprite;
		}

		public void Localize (string key){
			uiLocalize.key = key;
			uiLocalize.value = Localization.Get (key);
		}
	}

	private MENUACTION LastMenuAction;
	public bool[] menuoption;
	public List<GameObject> ButtonsList;
	public List<Button> ButtonsCache;
	public GameObject UIWindow;
	public GameObject UILoading;
	public GameObject UIInMenu;
	public GameObject UIPanelInGame;
	public GameObject UIPanelPause;
	public GameObject UIPanelWinner;
	public GameObject UIPanelLooser;
	public GameObject UIPanelMenu;
	public GameObject UIRoot;

	public bool state { get; private set;}

	//UI PREFABS
	public GameObject prefabUIPanelWindowEnterPlayerName;
	public GameObject prefabUIPanelWindowSettings;

	//UI
	GameObject WindowEnterName;
	GameObject WindowSettings;
	public GameObject ButtonSettings;
	UITweener uiButtonSettingsTweener;
	BoxCollider uiButtonSettinsBoxCollider;

	public GameObject ButtonNEW777;

	//UI
	public UILabel UILabelPauseText;
	public UISlider UITurnBar;
	public UILabel UITimer;

	//UI SCORE BAR NA-NA
	public UILabel UIScore;
	public UILabel UIScore2AddText;
	private int iScoreOnScreen;
	private int iScoreSet;
	private int iScoreSetLast;
	private float fScoreAddCurrentTime;

	//UI SPLASH ON DA MIDDLE OF DA SCREEN BOOOOOOOOM!!!
	public UILabel UISplashText;

	void Awake ()
	{
		state = false;

		UILoading.SetActive (true);

		// First we check if there are any other instances conflicting
		if (Instance != null && Instance != this) {
			// If that is the case, we destroy other instances
			Destroy (gameObject);
			return;
		}

		DontDestroyOnLoad (this);

		// Here we save our singleton instance
		Instance = this;

		menuoption = new bool[(int)MENUOPTIONS.MO_LASTMENUOPTIONS];
		ButtonsCache = new List<Button> ();

		iScoreOnScreen = 0;
		iScoreSet = 0;
	}

	void Start ()
	{
		for (int i = 0; i < (int)MENUOPTIONS.MO_LASTMENUOPTIONS; i++) {
			ButtonsList [i].SetActive (true);
			UISprite uiSprite = ButtonsList [i].GetComponentInChildren<UISprite> ();
			UILabel uiLabel = ButtonsList [i].GetComponentInChildren<UILabel> ();
			//uiLabel.text = "cached";
			ButtonsCache.Add (new Button (ButtonsList [i].gameObject, uiLabel, uiSprite));
		}


		GameSetting.Instance.mCurrentChapterLevel.mGameType = GAMETYPE.NOSELECTED;

		UIInMenu.SetActive (true);
		UIPanelInGame.SetActive (false);
		UIPanelPause.SetActive (false);
		UIPanelWinner.SetActive (false);
		UIPanelLooser.SetActive (false);
//		ShowMenu ();

		Application.LoadLevel (1);
	}

	void OnLevelWasLoaded (int level)
	{

		if (Instance != this) {
			Debug.Log ("WTF MENU??? " + level);
			return;
		}

		UILoading.SetActive (false);

		if (level <= 1) {
			GameSetting.Instance.mCurrentChapterLevel.mGameType = GAMETYPE.NOSELECTED;
			gameObject.SetActive (true);
            
			UIInMenu.SetActive (true);
			UIPanelInGame.SetActive (false);
			UIPanelPause.SetActive (false);
			UIPanelWinner.SetActive (false);
			UIPanelLooser.SetActive (false);

			ShowMenu ();

			GameSetting.Instance.googleAnalytics.LogScreen ("MainMenu Screen: " + level);

		} else {
			UIInMenu.SetActive (false);

			UIPanelInGame.SetActive (true);
			UIPanelPause.SetActive (false);
			UIPanelWinner.SetActive (false);
			UIPanelLooser.SetActive (false);

			SplashText(Localization.Get("Ready"), false);
			SplashText(Localization.Get("Ready"), true);

		}

	}

	void FixedUpdate ()
	{
		if (GameSetting.Instance.IsGame) {
			if (!GameSetting.Instance.IsGamePaused () && !IfScoreUpdatingFinished ()) {
				UpdateScoreStep ();
			}
		}
	}

	/***********************
	 * 
	 * 		create WINDOWS
	 * 
	 ***********************/

	public void ShowWindowEnterName(){
		ButtonsDisable ();
		if(WindowEnterName == null)
			WindowEnterName = NGUITools.AddChild(UIRoot, prefabUIPanelWindowEnterPlayerName);
	}


	public void ButtonsDisable(){
		foreach (GameObject button in ButtonsList) {
			button.GetComponent<UIButton>().isEnabled = false;
		}
	}

	public void ButtonsEnable(){
		foreach (GameObject button in ButtonsList) {
			button.GetComponent<UIButton>().isEnabled = true;
		}
		GameObject.Find ("Button Save Result").GetComponent<UIButton>().isEnabled = true;
		GameObject.Find ("Button Change Name").GetComponent<UIButton>().isEnabled = true;
	}

	
	/***********************
	 * 
	 * 		PAUSE GAME
	 * 
	 ***********************/

	public void ShowMenu ()
	{
		state = true;
		int j = 0;

		ToggleButtonSettings ();


		for (int i = 0; i < (int)MENUOPTIONS.MO_LASTMENUOPTIONS; i++) {
			menuoption [i] = false;
		}

		if (IGame.Instance != null) {

			if (GameSetting.Instance.IsGamePaused ()) {
				menuoption [(int)MENUOPTIONS.MO_UNPAUSE] = true;
				ButtonsCache [(int)MENUOPTIONS.MO_UNPAUSE].Localize("Continue");
			}
			if (GameSetting.Instance.IsWin) {
				menuoption [(int)MENUOPTIONS.MO_UNPAUSE] = true;
				if (IGame.Instance.GameType () == GAMETYPE.PUZZLE)
					ButtonsCache [(int)MENUOPTIONS.MO_UNPAUSE].Localize("NextPuzzle");
				else if (IGame.Instance.GameType () == GAMETYPE.CLASSIC)
					ButtonsCache [(int)MENUOPTIONS.MO_UNPAUSE].Localize("NewLevel");
				else if (IGame.Instance.GameType () == GAMETYPE.INFINITY)
					menuoption [(int)MENUOPTIONS.MO_UNPAUSE] = false;

			}

			menuoption [(int)MENUOPTIONS.MO_RESTART] = true;
			if (GameSetting.Instance.IsGameFinished ())
				ButtonsCache [(int)MENUOPTIONS.MO_RESTART].Localize("TryAgain");
			else
				ButtonsCache [(int)MENUOPTIONS.MO_RESTART].Localize("Restart");

			menuoption [(int)MENUOPTIONS.MO_EXIT] = true;
			ButtonsCache [(int)MENUOPTIONS.MO_EXIT].Localize("Back2Menu");
		}

		if (IGame.Instance == null) {
			menuoption [(int)MENUOPTIONS.MO_EXIT] = true;
			if (GameSetting.Instance.mCurrentChapterLevel.mGameType == GAMETYPE.NOSELECTED) {

				ButtonsCache [(int)MENUOPTIONS.MO_EXIT].Localize("Quit");

				//menuoption [(int)MENUOPTIONS.MO_NEWGAME_CLASSIC] = true;
				//ButtonsCache [(int)MENUOPTIONS.MO_NEWGAME_CLASSIC].Localize("Classic");
				//menuoption [(int)MENUOPTIONS.MO_NEWGAME_PUZZLE] = true;
				//ButtonsCache [(int)MENUOPTIONS.MO_NEWGAME_PUZZLE].Localize("Puzzle");
				menuoption [(int)MENUOPTIONS.MO_NEWGAME_INFINITY] = true;
				ButtonsCache [(int)MENUOPTIONS.MO_NEWGAME_INFINITY].Localize("Infinity");
			} else {
				ButtonsCache [(int)MENUOPTIONS.MO_EXIT].Localize("Back");
			}
		}

		for (int i = 0; i < (int)MENUOPTIONS.MO_LASTMENUOPTIONS; i++) {
			//ButtonsList [i].SetActive (menuoption [i]);
			// same as below
			ButtonsCache [i].gameObject.SetActive (menuoption [i]);
			// ;)
			if (menuoption [i])
				j++;
		}

		UIWindow.GetComponent<UISprite> ().SetDimensions (225, (4 + 84 * j));
		UIWindow.GetComponent<UIGrid> ().Reposition ();
		UIWindow.GetComponent<UIAnchor> ().pixelOffset = new Vector2 (-112f, (4 + 84 * j) / 2);
		UIWindow.GetComponent<UIAnchor> ().enabled = true;

		ActiveAnimation.Play (GetComponent<Animation>(), "Window - Forward", AnimationOrTween.Direction.Reverse);
	}

	public void HideMenu (MENUACTION menuaction = MENUACTION.MA_NONE)
	{
		state = false;
		LastMenuAction = menuaction;

		ToggleButtonSettings (false);
		
		ActiveAnimation.Play (GetComponent<Animation>(), "Window - Forward", AnimationOrTween.Direction.Forward).onFinished.Add (new EventDelegate (this, "MenuActionAfterAnimationFinished"));
	}

	private void ToggleButtonSettings(bool toggle = true){
		if (uiButtonSettinsBoxCollider == null)
			uiButtonSettinsBoxCollider = ButtonSettings.GetComponent<BoxCollider> ();
		if (uiButtonSettingsTweener == null)
			uiButtonSettingsTweener = ButtonSettings.GetComponent<UITweener> ();

		uiButtonSettinsBoxCollider.enabled = toggle;
		if(toggle)
			uiButtonSettingsTweener.PlayForward();
		else
			uiButtonSettingsTweener.PlayReverse();

		//ToggleButtonNEW777 (toggle);
	}

	private void ToggleButtonNEW777(bool toggle = true){
		ButtonNEW777.SetActive(toggle);
		//TODO:MAKE THIS NEW PROHHH BUTTON
	}


	/***********************
	 * 
	 * 		MENU INVOKE SCRIPTS
	 * 
	 ***********************/

	private void MenuActionAfterAnimationFinished ()
	{
		
		switch (LastMenuAction) {
		case MENUACTION.MA_QUIT:
			GameSetting.Instance.QuitGame();
			break;
		case MENUACTION.MA_LOADNEWLEVEL:
			Banner.Instance.Hide();
			UILoading.SetActive (true);
			HideLevelSelector ();
			Application.LoadLevel (2);
			break;
		case MENUACTION.MA_EXIT2MENU:
			Banner.Instance.Hide();
			Banner.Instance.ShowInterstitial();

			UILoading.SetActive (true);
			Application.LoadLevel (1);
			break;
		case MENUACTION.MA_BACK2MENU:
			Banner.Instance.Hide();
			GameSetting.Instance.mCurrentChapterLevel.mGameType = GAMETYPE.NOSELECTED;
			HideLevelSelector ();
			Menu.Instance.ShowMenu ();
			Banner.Instance.Show();
			break;
		case MENUACTION.MA_RESTARTLEVEL:
			Banner.Instance.Hide();
			Banner.Instance.ShowInterstitial();
			UILoading.SetActive (true);
			Application.LoadLevel (2);
			break;
		default:
			PauseAnimationDone ();
			break;
		}
		
		LastMenuAction = MENUACTION.MA_NONE;
	}

	private void PauseAnimationDone ()
	{
		if (GameSetting.Instance.IsGamePaused())
			GameSetting.Instance.UnPauseGame ();
		if (Application.loadedLevel <= 1)
			Menu.Instance.ShowMenu ();
	}

	private void PauseGame ()
	{
		UISplashText.GetComponent<ActiveAnimation> ().enabled = false;
		
		Menu.Instance.UIPanelPause.SetActive (true);

		Menu.Instance.UILabelPauseText.text = "";

		if (GameSetting.Instance.mLevelResults.ContainsKey (GameSetting.Instance.mCurrentChapterLevel)) {
			LevelResult mLevelResult;
			if (GameSetting.Instance.mLevelResults.TryGetValue (GameSetting.Instance.mCurrentChapterLevel, out mLevelResult)) {
				//Menu.Instance.UILabelPauseText.text = "Your best result is [b]" + mLevelResult.iScore + "[/b] in " + mLevelResult.fTime.ToString ("0") + " second";

				Menu.Instance.UILabelPauseText.text = string.Format(Localization.Get("YourBestResult"), mLevelResult.iScore, mLevelResult.fTime.ToString ("0"));
				//Menu.Instance.UILabelPauseText.text += Application.systemLanguage.ToString();
			}
			else{
				Menu.Instance.UILabelPauseText.text = string.Format(Localization.Get("NoBestResult"));
			}
#if !UNITY_EDITOR
			AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
			AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
			AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
			string android_id = secure.CallStatic<string> ("getString", contentResolver, "android_id");
			//Menu.Instance.UILabelPauseText.text += " id:" + android_id;
#endif
		}


		Menu.Instance.ShowMenu ();

	}
	
	private void UnPauseGame ()
	{
		UISplashText.GetComponent<ActiveAnimation> ().enabled = true;

		Menu.Instance.UIPanelPause.SetActive (false);
	}

	public void TogglePause ()
	{
		if (GameSetting.Instance.IsGamePaused()) {
			PauseGame ();
		} else
			UnPauseGame ();
	}
	
	/***********************
	 * 
	 * 		BUTTONS SCRIPTS
	 * 
	 ***********************/

	public void moExit ()
	{
		Time.timeScale = 1;
		if (Application.loadedLevel > 1) {
			HideMenu (MENUACTION.MA_EXIT2MENU);
		} else if (GameSetting.Instance.mCurrentChapterLevel.mGameType != GAMETYPE.NOSELECTED) {
			HideMenu (MENUACTION.MA_BACK2MENU);
		} else {
			HideMenu (MENUACTION.MA_QUIT);
		}
	}

	public void moRestart ()
	{
		HideMenu (MENUACTION.MA_RESTARTLEVEL);
	}

	public void moUnpause ()
	{
		if (GameSetting.Instance.IsWin) {
			GameSetting.Instance.mCurrentChapterLevel.iLevel++;
			HideMenu (MENUACTION.MA_LOADNEWLEVEL);
		} else
			HideMenu ();
	}

	public void moNewClassic ()
	{
		GameSetting.Instance.mCurrentChapterLevel.mGameType = GAMETYPE.CLASSIC;
		ShowLevelSelector ();
		HideMenu ();
	}

	public void moNewPuzzle ()
	{
		GameSetting.Instance.mCurrentChapterLevel.mGameType = GAMETYPE.PUZZLE;
		ShowLevelSelector ();
		HideMenu ();
	}

	public void moNewInfinity ()
	{
		GameSetting.Instance.mCurrentChapterLevel.mGameType = GAMETYPE.INFINITY;
		GameSetting.Instance.mCurrentChapterLevel.iChapter = 8000;
		GameSetting.Instance.mCurrentChapterLevel.iLevel = 1;
		HideMenu (MENUACTION.MA_LOADNEWLEVEL);
	}

	public void moSettings ()
	{
		if(WindowSettings == null)
			WindowSettings = NGUITools.AddChild(UIRoot, prefabUIPanelWindowSettings);
	}

	/***********************
	 * 
	 * 		LEVEL SELECTOR SCRIPTS
	 * 
	 ***********************/

	private void ShowLevelSelector ()
	{
		LevelSelector.Instance.ShowLevelSelector ();
	}

	private void HideLevelSelector ()
	{
		LevelSelector.Instance.HideLevelSelector ();
	}

	/***********************
	 * 
	 * 		MENU ELEMENTAL MAGIk SCRIPTS
	 * 
	 ***********************/

	public void SplashText (string text, bool boom = false)
	{
		UISplashText.GetComponent<Animation>().Stop ();
		UISplashText.text = text;
		if (boom)
			ActiveAnimation.Play (UISplashText.GetComponent<Animation>(), "splashboom", AnimationOrTween.Direction.Forward);
		else
			ActiveAnimation.Play (UISplashText.GetComponent<Animation>(), "splash", AnimationOrTween.Direction.Forward);
	}

	public void SplashText (int i)
	{
//		UISplashText.animation.Stop ();
//		UISplashText.text = "+" + i.ToString();
//		if (i < 64)
//			return;
//		if (i < 128)
//			ActiveAnimation.Play (UISplashText.animation, "splash", AnimationOrTween.Direction.Forward);
//		if (i < 2048)
//			ActiveAnimation.Play (UISplashText.animation, "splashboom", AnimationOrTween.Direction.Forward);
//		else 
//			ActiveAnimation.Play (UISplashText.animation, "splashmegaboom", AnimationOrTween.Direction.Forward);
	}

	/***********************
	 * 
	 * 		MENU SCORE SCORE SCORE SCORE SCORE SCORE MAGIk SCRIPTS
	 * 
	 ***********************/

	public void SetScore (int score)
	{
		fScoreAddCurrentTime = 0f;
		iScoreSet = score;
		UIScore2AddText.text = "+" + (iScoreSet - iScoreOnScreen).ToString ();
		SplashText (iScoreSet - iScoreSetLast);
//		Debug.Log("fScoreAddCurrentTime>" + fScoreAddCurrentTime + " iScoreOnScreen>" + iScoreOnScreen + " StartTime>" + Time.time);
	}

	public void RefreshUI(){
		UIScore2AddText.text = "";
		UIScore.text = Localization.Get("Score") + ": 0";
		UITurnBar.value = 0;
		UITimer.text = "0.0";
		iScoreOnScreen = 0;
		iScoreSet = 0;
		iScoreSetLast = 0;
    }

	private bool IfScoreUpdatingFinished ()
	{
		if (iScoreOnScreen == iScoreSet) {
			return true;
		}
		return false;
	}

	private void UpdateScoreStep ()
	{
		fScoreAddCurrentTime += Time.fixedDeltaTime;

		if (fScoreAddCurrentTime < 1.4f) {
			iScoreOnScreen = (int)(iScoreSetLast + (iScoreSet - iScoreSetLast) * (fScoreAddCurrentTime/1.4f));
			//Debug.Log("fScoreAddCurrentTime>" + fScoreAddCurrentTime + " iScoreOnScreen>" + iScoreOnScreen);
		} else {
			iScoreOnScreen = iScoreSet;
			iScoreSetLast = iScoreSet;
			UIScore2AddText.text = "";
			//Debug.Log("fScoreAddCurrentTime>" + fScoreAddCurrentTime + " iScoreOnScreen>" + iScoreOnScreen + " FinishTime>" + Time.time);
		}
		UpdateScoreOnScreen ();
	}

	private void UpdateScoreOnScreen ()
	{
		UIScore.text = Localization.Get("Score") + ": " + iScoreOnScreen.ToString ();

	}
}