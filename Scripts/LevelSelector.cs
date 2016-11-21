using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LevelSelector : MonoBehaviour {

	public static LevelSelector Instance;

	public GameObject ObjectChapter;


	public GameObject UIGridLevel;
	public List<GameObject> LevelButtonList;

	public GameObject UIGridChapter;
	public List<GameObject> ChapterButtonList;
	public List<ChapterData> ChapterList;
	public Dictionary<GameObject, int> ChapterButtonDictionary;
	public Dictionary<GameObject, ChapterCache> ChapterCacheDictionaty;
	public Dictionary<int, ChapterData> ChapterDataDictionary;

	UICenterOnChild UICenterOnChapter;

	public struct ChapterCache {
		public UILabel NAME;
		public GameObject NEW;
		public UISprite ICON;

		public void Setup(GameObject parent){
			foreach (UILabel label in parent.GetComponentsInChildren<UILabel>()) {
				if(label.gameObject.name == "UILabelNew")
					NEW = label.gameObject;
				if(label.gameObject.name == "UILabelName")
					NAME = label;
			}
			foreach (UISprite sprite in parent.GetComponentsInChildren<UISprite>()) {
				if(sprite.gameObject.name == "UISpriteIcon")
					ICON = sprite;
			}
		}

	}

	void Awake () {
		// First we check if there are any other instances conflicting
		if (Instance != null && Instance != this) {
			// If that is the case, we destroy other instances
			Destroy (gameObject);
			return;
		}
		// Here we save our singleton instance
		Instance = this;

		ChapterButtonList = new List<GameObject> ();
		ChapterCacheDictionaty = new Dictionary<GameObject, ChapterCache> ();
		LevelButtonList = new List<GameObject>();
	}

	// Use this for initialization
	void Start () {


		UIGridLevel = GameObject.Find ("UIGridLevel");
		int i = 0;
		foreach (UILabel LevelButton in UIGridLevel.GetComponentsInChildren<UILabel>()) {
			i++;
			LevelButtonList.Add(LevelButton.gameObject);
			LevelButton.gameObject.name = "Level"+i.ToString();
			LevelButton.text = i.ToString();
		}


		UIGridChapter = GameObject.Find ("UIGridChapter");
		i = 0;
		foreach (BoxCollider ChapterButton in UIGridChapter.GetComponentsInChildren<BoxCollider>()) {
			i++;
			ChapterButtonList.Add(ChapterButton.gameObject);
			ChapterButton.gameObject.name = "Chapter"+i.ToString();

			ChapterCache chaptercache = new ChapterCache();
			chaptercache.Setup(ChapterButton.gameObject);
			ChapterCacheDictionaty.Add(ChapterButton.gameObject, chaptercache);
		}


		if (Application.loadedLevel != 0 || GameSetting.Instance.mCurrentChapterLevel.mGameType == GAMETYPE.NOSELECTED)
			HideLevelSelector ();
		else 
			ShowLevelSelector ();

		UICenterOnChapter = UIGridChapter.GetComponent<UICenterOnChild> ();
		UICenterOnChapter.onFinished += UpdateChapterInCenter;
	}

	void UpdateChapterInCenter(){
		int chapter;
		if (ChapterButtonDictionary.TryGetValue (UICenterOnChapter.centeredObject, out chapter)) {
			GameSetting.Instance.mCurrentChapterLevel.iChapter = chapter;
			LoadChapter (chapter);
		} else {
			Debug.Log ("ChapterButtonDictionary.TryGetValue false in UpdateChapterInCenter");
		}
	}

	public void ShowLevelSelector () {
		gameObject.SetActive (true);

		ChapterButtonDictionary = new Dictionary<GameObject, int> ();
		ChapterList = new List<ChapterData>();
		ChapterDataDictionary = new Dictionary<int, ChapterData> ();

		foreach(GameObject ChapterButton in ChapterButtonList)
			ChapterButton.transform.parent = UIGridChapter.transform;

		Object[] bindata = Resources.LoadAll("Levels");
		Debug.Log ("Loading map all");
		foreach(TextAsset thisone in bindata){
			Debug.Log("Loading map name" + thisone.name);
			Debug.Log("Loading map text" + thisone.text);
			Stream s = new MemoryStream(thisone.bytes);
			//ChapterData cd = GameSetting.Instance.LoadChapter(s);
			ChapterData cd = new ChapterData();
			cd.LoadDataFromMemory(s);
//			if(cd.GameType == GameSetting.Instance.mCurrentChapterLevel.mGameType){
				ChapterList.Add(cd);
				ChapterDataDictionary.Add(cd.ID, cd);
//			}
		}

		ChapterList = ChapterList.OrderBy (Chapter => Chapter.ID).ToList ();
		
		while (ChapterList.Count > ChapterButtonList.Count) {

			GameObject newButton = NGUITools.AddChild(UIGridChapter, ObjectChapter);
			ChapterButtonList.Add(newButton);	
			newButton.name = "Chapter" + ChapterButtonList.Count.ToString();

			ChapterCache chaptercache = new ChapterCache();
			chaptercache.Setup(newButton);
			ChapterCacheDictionaty.Add(newButton, chaptercache);

			UIGridChapter.GetComponent<UIGrid>().Reposition();
		}

		int i = 0;
		foreach (ChapterData mChapterData in ChapterList) {
			
			ChapterButtonList[i].SetActive (true);
			ChapterCache chaptercache;
			if(!ChapterCacheDictionaty.TryGetValue (ChapterButtonList[i], out chaptercache))
				Debug.Log("ChapterCacheDictionaty.TryGetValue die");
			chaptercache.NAME.text = mChapterData.Name;

			ChapterLevel mChapterLevel = new ChapterLevel(mChapterData.GameType, mChapterData.ID, 1);
			if(GameSetting.Instance.mLevelResults.ContainsKey(mChapterLevel))
				chaptercache.NEW.SetActive(false);
			else
				chaptercache.NEW.SetActive(true);

			if(mChapterLevel.iChapter == GameSetting.Instance.mCurrentChapterLevel.iChapter){
				Debug.Log("CenterOn please");
				UICenterOnChapter.CenterOn (ChapterButtonList[i].transform);
			}

			ChapterButtonDictionary.Add(ChapterButtonList[i], mChapterData.ID);
			i++;
		}

		while (i < ChapterButtonList.Count) {
			ChapterButtonList[i].SetActive (false);
			ChapterButtonList[i].transform.parent = this.transform;
			i++;
		}

		//UIGridChapter.GetComponent<UIGrid>().Reposition();
		//UICenterOnChapter.Recenter ();

		UpdateChapterInCenter ();
	}


	public void HideLevelSelector () {
		gameObject.SetActive (false);
	}

	private void LoadChapter (int chapter){
		ChapterData mChapterData;
		if (!ChapterDataDictionary.TryGetValue (chapter, out mChapterData)) {
			Debug.Log ("ChapterDataDictionary.TryGetValue (chapter, out mChapterData) in LoadChapter ("+chapter+")");
		}

		GameSetting.Instance.mCurrentChapterLevel.mGameType = mChapterData.GameType;

		while (mChapterData.LevelList.Count > LevelButtonList.Count) {

			GameObject newButton = NGUITools.AddChild(UIGridLevel, LevelButtonList[0]);
			LevelButtonList.Add(newButton);	
			newButton.name = "Level" + LevelButtonList.Count.ToString();
			newButton.GetComponent<UILabel>().text = LevelButtonList.Count.ToString();
			UIGridLevel.GetComponent<UIGrid>().Reposition();

		}


		int i = 0;
		bool nextlevel = false;
		foreach (GameObject LevelButton in LevelButtonList) {
			i++;
			if(i > mChapterData.LevelList.Count){
				LevelButton.SetActive (false);
			}
			else{
				LevelButton.SetActive (true);
				LevelButton.GetComponentInChildren<Stars>().SetStars(0);
				LevelButton.GetComponent<UIButton>().isEnabled = false;
				GameSetting.Instance.mCurrentChapterLevel.iLevel = i;

				if(GameSetting.Instance.mCurrentChapterLevel.iLevel == 1 || nextlevel){
	                LevelButton.GetComponent<UIButton>().isEnabled = true;
					nextlevel = false;

					if(GameSetting.Instance.mLevelResults.ContainsKey(GameSetting.Instance.mCurrentChapterLevel)){
						LevelResult mLevelResult;
						if(!GameSetting.Instance.mLevelResults.TryGetValue(GameSetting.Instance.mCurrentChapterLevel, out mLevelResult))
							Debug.Log ("GameSetting.Instance.mLevelResults.TryGetValue(GameSetting.Instance.mCurrentChapterLevel, out mLevelResult) in LoadChapter ("+chapter+")");
						if(mLevelResult.bComplete){
							nextlevel = true;
							LevelButton.GetComponentInChildren<Stars>().SetStars(mLevelResult.iStars);
						}
					}
				}
			}
		}
		GameSetting.Instance.mCurrentChapterLevel.iLevel = 0;
		UIGridLevel.GetComponent<UIGrid>().Reposition();
	}

	public void SelectLevel(GameObject level)
	{
		Debug.Log ("Level " + level.GetComponent<UILabel>().text);
		GameSetting.Instance.mCurrentChapterLevel.iLevel = System.Convert.ToInt32(level.GetComponent<UILabel> ().text); 
		Time.timeScale = 1f;
		Menu.Instance.HideMenu (Menu.MENUACTION.MA_LOADNEWLEVEL);
	}

}
