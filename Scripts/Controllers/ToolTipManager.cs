using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ToolTipManager : MonoBehaviour {

	struct ToolTip {

		public GameObject gameObject;
		public enumToolTipsList tt;

		public ToolTip(GameObject gameObject, enumToolTipsList tt){
			this.gameObject = gameObject;
			this.tt = tt;
		}
	}

	float dt;

	Queue<ToolTip> ToolTipsOrder;

	public static ToolTipManager Instance;

	public GUIToolTipController guiToolTipController;

	public void RegToolTip(ToolTipObject g){
			newToolTip (g.gameObject, 
		    	        g.tt);
		Debug.Log ("Tooltip registered: " + g.tt);
	}

	public bool[] b;

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

		ToolTipsOrder = new Queue<ToolTip>();

		b = new bool[(int)enumToolTipsList._LastOne];
		for(enumToolTipsList tt = enumToolTipsList._FirstOne; tt < enumToolTipsList._LastOne; tt++){
			b[(int)tt] = PlayerPrefs.GetInt(tt.ToString()) > 0;
		}
	}

	
	// Update is called once per frame
	void Update () {
		if(GameSetting.Instance.ShowToolTips){
			dt += Time.deltaTime;
			if(dt > 1.5f){
				dt = 0;
				if(guiToolTipController.gameObject.activeSelf){

				}
				else{
					if(ToolTipsOrder.Count > 0){
						ToolTip tt = ToolTipsOrder.Dequeue();
						if(!b[(int)tt.tt]){
							if(tt.gameObject != null && tt.gameObject.activeInHierarchy){
								if(tt.gameObject.transform.position.y > -21.5 && tt.gameObject.transform.position.y < 21.5){
									guiToolTipController.ToolTip(tt.gameObject, Localization.Get(tt.tt.ToString()));
									b[(int)tt.tt] = true;
									Debug.Log ("Tooltip showed: " + tt.tt);
								}
								else
									ToolTipsOrder.Enqueue(tt);
							}
						}
					}
				}
			}
		}
	}

	public bool IsNeedToBeToolTiped(enumToolTipsList tt){
		return !b[(int)tt];
	}

	private void newToolTip(GameObject gameObject, enumToolTipsList tt){
		if(gameObject != null && tt != null){
			ToolTipsOrder.Enqueue( new ToolTip(gameObject, tt) );
		}
	}
}
