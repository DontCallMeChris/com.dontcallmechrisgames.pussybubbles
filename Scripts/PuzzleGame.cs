using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PuzzleGame : IGame {

	public ChapterData mChapterData;
	public LevelData mCurrentLevelData;
	int iNextBallColor;

	public override GAMETYPE GameType(){
		return GAMETYPE.PUZZLE;
	}

	new void Awake () {
		base.Awake ();

		iNextBallColor = 0;

		mChapterData = new ChapterData ();

		Object[] bindata = Resources.LoadAll("Levels");
		Debug.Log ("Loading map all");
		foreach(TextAsset thisone in bindata){
			Stream s = new MemoryStream(thisone.bytes);
			mChapterData.LoadDataFromMemory(s);
			if(mChapterData.ID == GameSetting.Instance.mCurrentChapterLevel.iChapter){
				break;
			}
		}
		mCurrentLevelData = mChapterData.LevelList[GameSetting.Instance.mCurrentChapterLevel.iLevel-1];

	}


	//TODO 20141202: fix when u have no ball with next color, and its check it fakefakefake

	// Use this for initialization
	new void Start () {
		base.Start ();


		foreach (LevelData.CellBall _Cell in mCurrentLevelData.CellBallList) {
			if(_Cell.type == BALLTYPE.LASTONE || _Cell.type == BALLTYPE.MOVEDOWN)
				continue;
			newBall (_Cell);
		}


		foreach (BallScriptPointer ball in BallList)
			ball.getBall().ConnectToBallsInDistance (2.87f);
		CheckSecondColor ();
	}

	public override int newcolor ()
	{
		if(mCurrentLevelData.BallsColorLine.Count == 0){
			return base.newcolor();
		}


		if(iNextBallColor == mCurrentLevelData.BallsColorLine.Count)
			iNextBallColor = 0;

		return (int)mCurrentLevelData.BallsColorLine[iNextBallColor++];
	}

//	public override Vector3 connect2wall (Vector3 v)
//	{		
//		float y = WallBottom.transform.position.y + 1.0f + 1.28f; //18.72f;
//
//		int x = (int)(v.x * 100 + 99);
//		int x1 = (x >> 7);
//		float x2 = x1 * 1.28f - 0.0f;
//		
//		return new Vector3 (x2, y, v.z);
//	}
}
