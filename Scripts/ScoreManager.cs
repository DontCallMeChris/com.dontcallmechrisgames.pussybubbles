using UnityEngine;
using System.Collections;

public class ScoreManager {

	private string secretKey;
	private string addScoreUrl;
	private string highscoreUrl;




	//SCORE
	private int iCurrentScore;
	private int iTotalScore;
	private int iScoreBonusLine;
	private int iScoreBonusBangPerSecond;
	public int iTotalBallBanged;



	public ScoreManager(){
		secretKey = "__________________";
		addScoreUrl = "http://www.rizen.pp.ua/bubbles/__________.php";
		highscoreUrl = "http://www.rizen.pp.ua/bubbles/";

		iCurrentScore = 0;
		iTotalScore = 0;
		iScoreBonusBangPerSecond = 0;
		iScoreBonusLine = 0;
		iTotalBallBanged = 0;
	}

	public void BallHadBeenBanged(){
		iTotalBallBanged++;
	}

	public void AddBangedBallsInCombo(int countOfBalls){
		int iBonusScore = countOfBalls * 15;
		
		if (countOfBalls > 2) {
			iBonusScore += 2 << (countOfBalls - 2);
		}
		
		if(iBonusScore > 2048)
			iBonusScore = 2048;
		
		iCurrentScore += iBonusScore;

		IGame.Instance.BonusBallsBanged (countOfBalls);
		Menu.Instance.SetScore (iCurrentScore);
	}

	public void Calculate(){
		iTotalScore += iCurrentScore;

//		while (WallBottom.transform.position.y < 18.5f) { //20.5
//			iScoreBonusLine += 64;
//			AddLine ();
//		}



        
        
        /*iTotalScore += iScoreBonusLine;

		iScoreBonusBangPerSecond += (int) (1024 * iTotalBallBanged / IGame.Instance.SecondFromStart);
		iTotalScore += iScoreBonusBangPerSecond;*/
	}


	public string GetWinnerText(){
		string result =  "[555555][b]We have a winner here![/b] \n" +
			"Your score is [b][FF6666]" + iTotalScore + "[-][/b]\n"/* +
				"[FF6666]" + iCurrentScore + "[-]+[00CC00]" + iScoreBonusLine + "[-]+[6666FF]" + iScoreBonusBangPerSecond + "[-]\n[-]"*/;
		if (GameSetting.Instance.PlayerName != "")
			result += "[555555]Congratulations, [00CC00][b]" + GameSetting.Instance.PlayerName + "[/b][-]![-]";
		else
			result += "[555555]Congratulations![-]";
		return result;
	}

	public int GetStars(){
		int iStars = 3;
		if (iScoreBonusLine < 1024)
			iStars--;
		if(iTotalBallBanged / IGame.Instance.SecondFromStart < 0.36f)
			iStars--;
		return iStars;
	}

	public int GetTotalScore(){
		return iTotalScore;
	}


			
	public IEnumerator postScore(string name) {

		Debug.Log("postScore");
		int score = iTotalScore;
		string hash = Md5Sum(name + score + GameSetting.Instance.GetDeviceID() + GameSetting.Instance.mCurrentChapterLevel.ToString() + secretKey);		
		string highscore_url = addScoreUrl + "?name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash + "&levelid=" + GameSetting.Instance.mCurrentChapterLevel.ToString() + "&deviceid=" + GameSetting.Instance.GetDeviceID();

		Debug.Log(highscore_url);
		
		WWW hs_post = new WWW(highscore_url);

		yield return hs_post;

		if (hs_post.error != null) {
			Debug.Log ("There was an error posting the high score: " + hs_post.error);
		} else {
			if (hs_post.text != null) {
				Debug.Log ("There was an text posting the high score: " + hs_post.text);

			}
		}
	}
	
	IEnumerator getScores() {
		
		//gameObject.guiText.text = "Loading Scores";
		
		WWW hs_get = new WWW (highscoreUrl);
		
		yield return hs_get; 

		if (hs_get.error.Length > 0) {
			Debug.Log ("There was an error getting the high score: " + hs_get.error);
		} else {
		//	gameObject.guiText.text = hs_get.data;			
		}
	}

	public string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
}


