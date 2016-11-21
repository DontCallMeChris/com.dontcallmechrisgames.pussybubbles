using UnityEngine;
using System.Collections;

public class EnterNameWindow : MonoBehaviour {

	string text;
	public UIInput uiInput;

	// Use this for initialization
	void Start () {
		uiInput = gameObject.GetComponentInChildren<UIInput> ();
		text = PlayerPrefs.GetString ("PlayerName");
		uiInput.value = text;
		GetComponent<UITweener> ().PlayForward();
	}

	public void Close(){
		GetComponent<UITweener> ().PlayReverse();
		GetComponent<UITweener> ().SetOnFinished(new EventDelegate (this, "OnFinished"));
	}

	public void Result(){
		text = uiInput.value;
		PlayerPrefs.SetString ("PlayerName", text);
		GameSetting.Instance.SavePlayerName (text);
		GetComponent<UITweener> ().PlayReverse();
		GetComponent<UITweener> ().SetOnFinished(new EventDelegate (this, "OnFinished"));
	}

	private void OnFinished(){
		Destroy (gameObject);
		Menu.Instance.ButtonsEnable ();
		if (IGame.Instance != null) {
			UILabel label = GameObject.Find ("UILabelWinnerText").GetComponent<UILabel> ();
			label.text = IGame.Instance.mScoreManager.GetWinnerText ();
		}
	}
}
