using UnityEngine;
using System.Collections;

public class GUIToolTipController : MonoBehaviour {

	Vector3 Position2;
	GameObject pivotObject;
	Vector3 offset;
	int pivotpoint; 


	public GameObject _toolTip;
	public UISprite _sprite;
	public UILabel _label;


//	string textlabel
//	{ 
//		set {
//			textlabel = value;
//			_label.text = textlabel;
//		}
//		get {
//			return textlabel;
//		}
//	}

	Vector3 World2GuiCoords(Vector3 v3){

		return v3 + offset;

	}

	Vector3 convert2222(Vector3 v3){
		// 720 vs 22.22603 -> 21.5x13.45 x2
		// 13.45*2 = 27
		// 43 - 720
		// 22.22 = 360

		return new Vector3(v3.x * 16.2f, v3.y * 16.2f);
	}

	public void ToolTip(GameObject go, string s){
		pivotObject = go;
		_label.text = s;
		this.gameObject.SetActive(true);
	}

	public void HideToolTip(){
		_sprite.GetComponent<TweenColor>().ResetToBeginning();
		this.gameObject.SetActive(false);
		this.pivotObject = null;
	}

	void OnEnable(){
		_sprite.GetComponent<TweenColor>().PlayForward();

		if(pivotObject != null){
			Position2 = convert2222(pivotObject.transform.position);
		}
		_toolTip.transform.localPosition = Vector3.Lerp(_toolTip.transform.localPosition, World2GuiCoords (Position2), 1f);
	}

	// Use this for initialization
	void Start () {


	}

	void setpivot(int pp){
		switch(pp){
		case 0:
			_sprite.flip = UISprite.Flip.Nothing;
			offset.Set(64f, -64f, 0f);
			break;
		case 1:
			offset.Set(-64f, -64f, 0f);
			_sprite.flip = UISprite.Flip.Horizontally;
			break;
		case 2:
			offset.Set(64f, 64f, 0f);
			_sprite.flip = UISprite.Flip.Vertically;
			break;
		case 3:
			offset.Set(-64f, 64f, 0f);
			_sprite.flip = UISprite.Flip.Both;
			break;
		default:
			goto case 0;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(pivotObject != null){
			Position2 = convert2222(pivotObject.transform.position);
		}
		_toolTip.transform.localPosition = Vector3.Lerp(_toolTip.transform.localPosition, World2GuiCoords (Position2), 1f);

		if(Position2.x > 71.5)
			if(Position2.y < -232)
				setpivot(3);
			else
				setpivot(1);
		else
			if(Position2.y < -232)
				setpivot(2);
			else
				setpivot(0);

	}

	void OnLevelWasLoaded(int level) {
		
		HideToolTip();	
	}

}
