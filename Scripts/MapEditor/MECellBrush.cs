using UnityEngine;
using System.Collections;

public class MECellBrush : MonoBehaviour {
	
	SpriteRenderer spriteRenderer;
	
	public BALLTYPE type;
	public bool basic;
	public int armor;
	public bool frozen;
	
	// Use this for initialization
	void Start () {
		type = BALLTYPE.LASTONE;
		GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	void Next(){
		
		if (type == BALLTYPE.LASTONE)
			type = BALLTYPE.RED;
		else
			type++;
		
		setColor ();
	}
	
	void Prev(){
		
		if (type == BALLTYPE.RED)
			type = BALLTYPE.LASTONE;
		else
			type--;
		
		setColor ();
	}
	
	public void Load(int i){
		type = (BALLTYPE)i;
		setColor ();
	}
	
	public void setColor()
	{
		if(type == BALLTYPE.BOMB)
			spriteRenderer.sprite = MapGenerator.mapGenerator.spriteBomb;
		else if(type == BALLTYPE.MOVEDOWN)
			spriteRenderer.sprite = MapGenerator.mapGenerator.spriteMoveDown;
		else
			spriteRenderer.sprite = MapGenerator.mapGenerator.spriteMain;
		
		GetComponent<Renderer>().material.color = get_color(type);
	}
	
	public Color get_color (BALLTYPE i)
	{
		switch (i) {
		case BALLTYPE.RED:
			return Color.red; 
		case BALLTYPE.GREEN:
			return Color.green;
		case BALLTYPE.BLUE:
			return Color.blue;
		case BALLTYPE.YELLOW:
			return Color.yellow;
		case BALLTYPE.CYAN:
			return Color.cyan;
		case BALLTYPE.MAGENTA:
			return Color.magenta;
		case BALLTYPE.BOMB:
			return Color.white;
		case BALLTYPE.LASTONE:
			return new Color(1, 1, 1, 0.5f);
		default:
			return Color.white;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") != 0 && GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
			float f = Input.GetAxis("Mouse ScrollWheel");
			if(f > 0)
				Next();
			else
				Prev();
		}

		if (Input.GetMouseButtonDown (0)) {
			if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
				Next();
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
				Prev();
			}
		}
	}

}
