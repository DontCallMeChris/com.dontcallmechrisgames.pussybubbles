using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ClassicGame : PuzzleGame {

	public override GAMETYPE GameType(){
		return GAMETYPE.CLASSIC;
	}

//	// Use this for initialization
//	new void Start () {
//		base.Start ();
//
//		if(GameSetting.Instance.mCurrentChapterLevel.iLevel == 2)
//			GameSetting.Instance.gridY = 13;
//		else if(GameSetting.Instance.mCurrentChapterLevel.iLevel == 3)
//			GameSetting.Instance.gridY = 1;
//		else
//			GameSetting.Instance.gridY = 8;
//
//		for (int y = 0; y < GameSetting.Instance.gridY; y++) {
//			for (int x = 0; x < GameSetting.Instance.gridX; x++) {
//				Vector2 pos = new Vector2 ((x - GameSetting.Instance.gridX / 2 + 0.5f * (y % 2)) * GameSetting.Instance.spacing + 0.64f, -20.0f + GameSetting.Instance.spacing / 2 + y * 2.22f);
//				if(GameSetting.Instance.mCurrentChapterLevel.iLevel == 2 && (x+1)+(y+1) > 13)
//					continue;
//				newBall (pos, true);
//			}
//		}
//	}
}
