using UnityEngine;
using System.Collections;

public class MeshPrimitive{
	
	public GameObject gameObject;
	MeshFilter meshFilter;
	MeshRenderer meshRenderer;
	PolygonCollider2D collider;
	Mesh mesh;
	
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uv;

	LevelData.MeshPrim meshPrim;
	
	public MeshPrimitive(){

		vertices = new Vector3[]{
			new Vector3(0, 0, 0),
			new Vector3(0, 15, 0),
			new Vector3(15, 0, 0),
			
		};



		int l = ((vertices.Length - 2) * 3);
		triangles = new int[l];

		for (int i = 0, j = 0; i < l; i+=3, j++) {
			triangles[i] = 0;
			triangles[i+1] = j+1;
			triangles[i+2] = j+2;
		}


		uv = new Vector2[]{
			vertices[0],
			vertices[1],
			vertices[2]
		};
		
		gameObject = new GameObject ("MeshPrimitive");
		meshFilter = gameObject.AddComponent<MeshFilter> ();
		
		mesh = meshFilter.mesh;
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		meshRenderer.material = MapGenerator.mapGenerator.MeshMaterial;
		
		collider = gameObject.AddComponent<PolygonCollider2D> ();
		collider.pathCount = vertices.Length;
		collider.points = uv;

		meshPrim = new LevelData.MeshPrim (Vector3.zero, vertices);
		MapGenerator.mapGenerator.mCurrentLevelData.MeshPrimList.Add (meshPrim);
	}

	public void SetVerts(Vector3[] v3){
		vertices = v3;
		
		int l = ((vertices.Length - 2) * 3);
		triangles = new int[l];
		
		for (int i = 0, j = 0; i < l; i+=3, j++) {
			triangles[i] = 0;
			triangles[i+1] = j+1;
			triangles[i+2] = j+2;
		}
		
		
		uv = new Vector2[v3.Length];

		for (int i = 0; i < uv.Length; i++) {
			uv[i] = new Vector2(vertices[i].x, vertices[i].y);
		}
		

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		collider.pathCount = vertices.Length;
		collider.points = uv;
		meshPrim.Update(Vector3.zero, vertices);
	}
}
