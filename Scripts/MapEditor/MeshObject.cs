using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshObject : MonoBehaviour {

	public MeshPrimitive meshPrimitive;
	List<GameObject> verts;

	// Use this for initialization
	void Start () {

		MapGenerator.mapGenerator.MeshObjectList.Add (this);
		name = "MeshObject " + MapGenerator.mapGenerator.MeshObjectList.Count;
		transform.parent = MapGenerator.mapGenerator.gameObject.transform;


	}

	void AddVerts(Vector3 v3){
		GameObject vert = new GameObject ("Vert");
		vert.transform.parent = gameObject.transform;
		vert.transform.position = v3;
		vert.AddComponent<Verts> ();
		verts.Add (vert);

	}

	public void SetVerts(List<Vector3> v3){
		verts = new List<GameObject> ();

		foreach (Vector3 v in v3) {
			AddVerts(v);
		}

		meshPrimitive = new MeshPrimitive ();
		meshPrimitive.gameObject.transform.parent = gameObject.transform;

		meshPrimitive.SetVerts (v3.ToArray());
	}

	public void SetVerts(Vector3[] v3){
		verts = new List<GameObject> ();
		
		foreach (Vector3 v in v3) {
			AddVerts(v);
		}
		
		meshPrimitive = new MeshPrimitive ();
		meshPrimitive.gameObject.transform.parent = gameObject.transform;
        
        meshPrimitive.SetVerts (v3);
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey (KeyCode.LeftControl)){
			if (Input.GetMouseButtonDown (1)) {
				if (meshPrimitive.gameObject.GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					Destroy(gameObject);
                }
            }
        }
//		if (Input.GetMouseButtonDown (1)) {
//			if (meshPrimitive.gameObject.collider2D.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
//				MapGenerator.mapGenerator.SelectedMECell = gameObject;
//			}
//		}
//					
//		if (MapGenerator.mapGenerator.SelectedMECell == gameObject) {
//            gameObject.GetComponentInParent<MeshObject> ().UpdateVerts ();
//        }
	}

	public void UpdateVerts () {
		List<Vector3> v3 = new List<Vector3> ();
		foreach (GameObject v in verts) {
			v3.Add(v.transform.position);
		}
		meshPrimitive.SetVerts (v3.ToArray());
    }

	void OnDestroy(){
		MapGenerator.mapGenerator.MeshObjectList.Remove (this);
		//MapGenerator.mapGenerator.mCurrentLevelData.CellBallList.Remove ();
		Debug.Log("destroy MESH");
    }
}
