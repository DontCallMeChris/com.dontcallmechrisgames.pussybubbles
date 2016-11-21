using UnityEngine;
using System.Collections;

public class WindowSettingsMenu : MonoBehaviour {

	UITweener uiTweener;

	// Use this for initialization
	void Start () {
		uiTweener = GetComponent<UITweener> ();
		uiTweener.PlayForward();
	}
	
	public void Close(){
		uiTweener.PlayReverse();
		uiTweener.SetOnFinished(new EventDelegate (this, "OnFinished"));
	}
	
	public void Result(){
		uiTweener.PlayReverse();
		uiTweener.SetOnFinished(new EventDelegate (this, "OnFinished"));
	}
	
	private void OnFinished(){
		Destroy (gameObject);
	}
}
