using UnityEngine;
using System.Collections;

public class LoadingGame : MonoBehaviour {
	public GameObject gameobject;

	void Start () {
		switch (GameSetting.Instance.mCurrentChapterLevel.mGameType) {
		case GAMETYPE.CLASSIC:
			gameobject.AddComponent<ClassicGame>();
			break;
		case GAMETYPE.PUZZLE:
			gameobject.AddComponent<PuzzleGame>();
			break;
		case GAMETYPE.INFINITY:
			gameobject.AddComponent<InfinityGame>();
			break;
		default:
			Debug.Log ("Unknown game type " + GameSetting.Instance.mCurrentChapterLevel.mGameType);
			Application.LoadLevel (0);
			break;
		}

		Destroy (this.gameObject);
	}
}
