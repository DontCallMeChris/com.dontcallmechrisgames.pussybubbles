using UnityEngine;
using System.Collections;

public class MECell : MonoBehaviour
{

	private SpriteRenderer spriteRenderer;
	private SpriteRenderer childSR;

	public LevelData.CellBall pCellBall;


	// Use this for initialization
	void Start ()
	{
		MapGenerator.mapGenerator.MECellList.Add (this);
		name = "MECell " + MapGenerator.mapGenerator.MECellList.Count;
		transform.parent = MapGenerator.mapGenerator.gameObject.transform;
		if(pCellBall == null)
			gameObject.SetActive (false);
	}

	public void Clear(){
		ClearChild ();
		pCellBall = null;
		gameObject.SetActive (false);
	}

	public void ConnectToCell (LevelData.CellBall cell)
	{
		pCellBall = cell;
		transform.position = pCellBall.position;
		setColor ();
		gameObject.SetActive (true);
	}

	private SpriteRenderer GetSprite ()
	{
		if (spriteRenderer == null) {
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}

		return spriteRenderer;
	}

	public void Snap(){
		transform.position = Snap2_64f (transform.position);
		transform.position = Snap2_217f (transform.position);
	}

	private Vector2 Snap2_64f(Vector2 input){
		//Every 1.28f
		// or Every 0.64f?

		Vector2 Vresult = new Vector2(
			Mathf.Round(input.x / 0.64f) * 0.64f,
			Mathf.Round(input.y / 0.64f) * 0.64f
		);
		return Vresult;
	}

	private Vector2 Snap2_217f(Vector2 input){
		//Every 2.217f
		// or Every 0.64f?
		
		Vector2 Vresult = new Vector2(
			Mathf.Round(input.x / 0.64f) * 0.64f,
			Mathf.Round((input.y + 19.2f) / 2.217f) * 2.217f - 19.2f
			);
		return Vresult;
	}

	void Next ()
	{
		if (pCellBall.type == BALLTYPE.LASTONE)
			pCellBall.type = BALLTYPE.RED;
		else
			pCellBall.type++;

		setColor ();
	}

	void Prev ()
	{
		if (pCellBall.type == BALLTYPE.RED)
			pCellBall.type = BALLTYPE.LASTONE;
		else
			pCellBall.type--;
        
		setColor ();
	}

	public void Load (int i)
	{
		pCellBall.type = (BALLTYPE)i;
		setColor ();
	}

	private void AddChild ()
	{
		GameObject child = new GameObject ();
		child.transform.parent = this.transform;
		child.transform.localPosition = Vector3.zero;
		child.name = "Child" + child.transform.position.ToString ();
		childSR = child.AddComponent<SpriteRenderer> ();
	}

	public SpriteRenderer GetChildrenSprite ()
	{
		if (childSR == null)
			AddChild ();
		childSR.enabled = true;
		return childSR;
	}

	public void ClearChild ()
	{
		if (childSR != null)
			childSR.enabled = false;
	}

	public void setColor ()
	{

		ClearChild ();
		if (pCellBall.armor > 0)
			GetChildrenSprite ().sprite = MapGenerator.mapGenerator.spriteArmor;

		if (pCellBall.type == BALLTYPE.BOMB)
			GetSprite().sprite = MapGenerator.mapGenerator.spriteBomb;
		else if (pCellBall.type == BALLTYPE.MOVEDOWN)
			GetSprite().sprite = MapGenerator.mapGenerator.spriteMoveDown;
		else if (pCellBall.basic == true)
			GetSprite().sprite = MapGenerator.mapGenerator.spriteBasic;
		else
			GetSprite().sprite = MapGenerator.mapGenerator.spriteMain;

		//renderer.material.color = get_color (pCellBall.type);
		GetSprite().color = get_color (pCellBall.type);
	}

	public Color get_color (BALLTYPE i)
	{
		switch (i) {
		case BALLTYPE.RED:
			return new Color(0.9f, 0.1f, 0.1f, 1.0f);
		case BALLTYPE.GREEN:
			return new Color(0.1f, 0.9f, 0.1f, 1.0f);
		case BALLTYPE.BLUE:
			return new Color(0.1f, 0.1f, 0.9f, 1.0f);
		case BALLTYPE.YELLOW:
			return Color.yellow;
		case BALLTYPE.CYAN:
			return Color.cyan;
		case BALLTYPE.MAGENTA:
			return Color.magenta;
		case BALLTYPE.BOMB:
			return Color.white;
		case BALLTYPE.MOVEDOWN:
			return Color.white;
		case BALLTYPE.LASTONE:
			return new Color(1, 1, 1, 0.5f);
		default:
			return Color.white;
		}
	}

	public void ToBrush ()
	{
		pCellBall.type = MapGenerator.mapGenerator.BrushMECell.GetComponent<MECellBrush> ().type;
		pCellBall.basic = MapGenerator.mapGenerator.BrushMECell.GetComponent<MECellBrush> ().basic;
		pCellBall.armor = MapGenerator.mapGenerator.BrushMECell.GetComponent<MECellBrush> ().armor;
		pCellBall.frozen = MapGenerator.mapGenerator.BrushMECell.GetComponent<MECellBrush> ().frozen;
		setColor ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetAxis("Mouse ScrollWheel") != 0 && GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
			float f = Input.GetAxis("Mouse ScrollWheel");
			if(f > 0)
				Next();
			else
				Prev();
		}

		if (Input.GetKey (KeyCode.LeftShift)) {
			if (Input.GetMouseButtonDown (0)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					Next ();
				}
			}
			if (Input.GetMouseButtonDown (1)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					Prev ();
				}
			}
		}
		else if(Input.GetKey (KeyCode.LeftControl)){
			if (Input.GetMouseButtonDown (1)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					Destroy(gameObject);
				}
			}
		} else {
			if (Input.GetMouseButtonDown (0)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					ToBrush ();
				}
			}

			if (Input.GetMouseButtonDown (1)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					MapGenerator.mapGenerator.SelectedMECell = gameObject;
				}
			}

			if (Input.GetMouseButtonDown (2)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					Snap();
				}
			}
		}

		pCellBall.position = transform.position;
	}


	void OnDestroy(){
		MapGenerator.mapGenerator.MECellList.Remove (this);
		MapGenerator.mapGenerator.mCurrentLevelData.CellBallList.Remove (pCellBall);
		Debug.Log("destroy");
	}

}
