#define LEVELVERSION4

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapGenerator : MonoBehaviour {




	public GameObject MECell;
	public GameObject BrushMECell;
	public List<MECell> MECellList;
	public LevelData mCurrentLevelData;
	public ChapterData mChapterData;

	public List<MeshObject> MeshObjectList;


	public float gridX;
	public float gridY;
	public float spacing;

	public Sprite spriteBomb;
	public Sprite spriteMoveDown;
	public Sprite spriteBasic;
	public Sprite spriteMain;
	public Sprite spriteArmor;

	public GameObject SelectedMECell;

	private List<Vector3> verts;


	int curentLvl;
	public UILabel curentLvlUI;

	public static MapGenerator mapGenerator;
	public Material MeshMaterial;



	void Awake(){
		if(mapGenerator != null)
			Debug.LogError("LEFT COPY OF MAP GENERATOR!!!!");
		mapGenerator = this;

		mChapterData = new ChapterData ();
		if(mChapterData.LevelList.Count == 0)
			mChapterData.LevelList.Add(new LevelData());
		curentLvl = 0;
		mCurrentLevelData = mChapterData.LevelList [curentLvl];

		MECellList = new List<MECell> ();
		MeshObjectList = new List<MeshObject> ();
		verts = new List<Vector3> ();

		SelectedMECell = null;


	}
    
	// Use this for initialization
	void Start () {

		curentLvlUI.text = ((1 + curentLvl).ToString () + " / " + (mChapterData.LevelList.Count).ToString());

		ReloadLevel ();

//		new MeshObject ();
	}
	
	// Update is called once per frame
	void Update () {


		if (SelectedMECell != null) {
			Vector2 v = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			SelectedMECell.transform.position = v;

			if (Input.GetMouseButtonUp (1)) {
				MapGenerator.mapGenerator.SelectedMECell = null;
			}

		}

		if (Input.GetKey (KeyCode.LeftShift)) {
			if (Input.GetMouseButtonDown (0)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					
					Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					verts.Add(pos);
									
				}
			}
			if (Input.GetMouseButtonDown (1)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					
					GameObject go = new GameObject();
					MeshObject mo = go.AddComponent<MeshObject>();
					mo.SetVerts(verts);
					verts = new List<Vector3>();
                }
            }
        }


		if (Input.GetKey (KeyCode.LeftControl)) {
			if (Input.GetMouseButtonDown (0)) {
				if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {

				Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				GameObject go = Instantiate (MECell, pos, Quaternion.identity) as GameObject;
				LevelData.CellBall ball = new LevelData.CellBall ();

				ball.position = go.transform.position;

				go.GetComponent<MECell>().ConnectToCell(ball);
				go.GetComponent<MECell>().ToBrush();

				mCurrentLevelData.CellBallList.Add (ball);

				}
			}
		}
	}

	void ReloadLevel(){
		foreach (MECell _Cell in MECellList) {
			_Cell.Clear();
		}
		foreach (LevelData.CellBall _CellBall in mCurrentLevelData.CellBallList) {
			GetFreeCell().ConnectToCell(_CellBall);
		}

		foreach (LevelData.MeshPrim _MeshPrim in mCurrentLevelData.MeshPrimList) {
//			GameObject go = new GameObject();
//			MeshObject mo = go.AddComponent<MeshObject>();
//			go.transform.position = _MeshPrim.position;
//			mo.SetVerts(_MeshPrim.vertices);
			//TODO: запили! это не работает
        }

		if(mCurrentLevelData.BallsColorLine.Count == 0)
			mCurrentLevelData.BallsColorLine.Add(BALLTYPE.LASTONE);
		if(BubbleColorLine.BCLlist != null)
			BubbleColorLine.BCLlist[0].Reset();
	}

	MECell GetFreeCell(){
		foreach (MECell mecell in MECellList) {
			if (!mecell.gameObject.activeSelf) {
				return mecell;
			}
		}
	
		GameObject go = Instantiate (MECell, Vector3.zero, Quaternion.identity) as GameObject;
		return go.GetComponent<MECell>();
	}
    
	#if LEVELVERSION4
	public void SaveMap(){

		mChapterData.SaveDataInFile ();
	}
	#endif

	public void LoadMap(){
		mChapterData.LoadDataFromFile ();
		ReloadLevel ();
    }


	public void NextLvl(){
		if (curentLvl + 1 == mChapterData.LevelList.Count)
			mChapterData.LevelList.Add (new LevelData ());

		if (curentLvl < mChapterData.LevelList.Count) {
			curentLvl++;
			curentLvlUI.text = ((1 + curentLvl).ToString () + " / " + (mChapterData.LevelList.Count).ToString());
			mCurrentLevelData = mChapterData.LevelList[curentLvl];
			ReloadLevel();
		}
	}

	public void PrevLvl(){
		if (curentLvl > 0) {

			if(mCurrentLevelData.isEmpty())
				mChapterData.LevelList.RemoveAt(curentLvl);

			curentLvl--;
			curentLvlUI.text = ((1 + curentLvl).ToString () + " / " + (mChapterData.LevelList.Count).ToString());
			mCurrentLevelData = mChapterData.LevelList[curentLvl];
			ReloadLevel();
		}
	}
}
