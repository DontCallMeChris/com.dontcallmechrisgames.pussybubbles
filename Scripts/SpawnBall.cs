using UnityEngine;
using System.Collections;

public class SpawnBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
		IGame.Instance.newBall (transform.position, true);
		Destroy (gameObject);
	
	}
}
