using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubbleColorLine : MonoBehaviour
{

	public BALLTYPE type;
	public int n;
	public static List<BubbleColorLine> BCLlist;
	public static List<BALLTYPE> BallsColorLine;
	SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start ()
	{

		if (BCLlist == null) {
			BCLlist = new List<BubbleColorLine> ();
		}
		if (BallsColorLine == null) {
			BallsColorLine = MapGenerator.mapGenerator.mCurrentLevelData.BallsColorLine;
		}

		n = BCLlist.Count;
		name = "BallsLine+" + n;
		BCLlist.Add (this);

		this.transform.localPosition = new Vector3 (13.45f, (14.5f - n * 2.22f), -4f);
		spriteRenderer = GetComponent<SpriteRenderer> ();
		type = BALLTYPE.LASTONE;
		setColor ();

		if (BallsColorLine.Count == n + 1)
			ResetAll ();
	}

	public void Reset ()
	{
		BallsColorLine = MapGenerator.mapGenerator.mCurrentLevelData.BallsColorLine;
		int enough = BallsColorLine.Count - BCLlist.Count;
		if (enough > 0) {
			for (int i = 0; i < enough; i++) {
				Instantiate (this);
			}
		} else {
			ResetAll ();
		}
	}

	private void ResetAll ()
	{

		foreach (BubbleColorLine BCL in BCLlist) {
			if (BCL.n < BallsColorLine.Count) {
				BCL.type = BallsColorLine [BCL.n];
				BCL.gameObject.SetActive (true);
				BCL.setColor ();
			} else {
				BCL.gameObject.SetActive (false);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Input.GetKey (KeyCode.LeftControl)) {
			if (Input.GetMouseButtonDown (1)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					if (BCLlist.Count > 1) {
						gameObject.SetActive(false);
						BallsColorLine.RemoveAt(n);
					}
				}	
			}
		} else {
			if (Input.GetMouseButtonDown (0)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					Click (true);
				}
			}
			if (Input.GetMouseButtonDown (1)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					Click (false);
				}
			}
		}
	}

	void Click (bool increase)
	{
		if (increase)
			Next ();
		else
			Prev ();

		BallsColorLine[n] = type;

		if (BallsColorLine.Count == n + 1){
			BallsColorLine.Add(BALLTYPE.LASTONE);
			Reset();
		}


	}
	
	void Next ()
	{
		if (type == BALLTYPE.LASTONE)
			type = BALLTYPE.LASTCOLOR;
		
		if (type == BALLTYPE.LASTCOLOR)
			type = BALLTYPE.RED;
		else
			type++;
		
		setColor ();
	}
	
	void Prev ()
	{
		if (type == BALLTYPE.LASTONE)
			type = BALLTYPE.LASTCOLOR;

		if (type == BALLTYPE.RED)
			type = BALLTYPE.LASTCOLOR;
		else
			type--;
		
		setColor ();
	}

	public void setColor ()
	{
		if (type == BALLTYPE.BOMB)
			spriteRenderer.sprite = MapGenerator.mapGenerator.spriteBomb;
		else if (type == BALLTYPE.MOVEDOWN)
			spriteRenderer.sprite = MapGenerator.mapGenerator.spriteMoveDown;
		else
			spriteRenderer.sprite = MapGenerator.mapGenerator.spriteMain;
		
		spriteRenderer.color = get_color (type);

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
			return new Color (1, 1, 1, 0.5f);
		default:
			return Color.white;
		}
	}
}
