using UnityEngine;
using System.Collections;

public class Verts : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.AddComponent<CircleCollider2D> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (1)) {
			if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
				MapGenerator.mapGenerator.SelectedMECell = gameObject;
			}
		}
		
		if (Input.GetMouseButtonDown (2)) {
			if (GetComponent<Collider2D>().OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
				Snap();
				gameObject.GetComponentInParent<MeshObject>().UpdateVerts();
			}
		}

		if (MapGenerator.mapGenerator.SelectedMECell == gameObject) {
			gameObject.GetComponentInParent<MeshObject> ().UpdateVerts ();
		}
	}

	private void Snap(){
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
}
