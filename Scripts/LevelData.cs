
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public enum BALLTYPE : byte
{
	RED = 0,
	GREEN,
	BLUE,
	YELLOW,
	CYAN,
	MAGENTA,
	LASTCOLOR,
	BOMB,
	MOVEDOWN,
	LASTONE
};

public enum DATABLOCK : byte
{
	NONE = 0,
	VERSION,	//int, version of file
	CHAPTER,	//id, gmtype, name, description
	ICON,		//string name
	ICONTEXTURE, //byte[]
	LEVEL = 0x10,	//start level block
	BALL,
	OBJECT,
	JOINT,
	CELL_BALL,
	BALLCOLORLINE,	//list from colors, yolo, for puzzle game :)
	LEVELEND = 0x20,	//just block end
	END = 0xFF	
	//********
	//TODO:Level data blocks
	//********
};

//	Version 1
[Serializable]
public class LevelData {

	[Serializable]
	public class CellBall {
		public Vector3 position;
		public BALLTYPE type;
		public bool basic;
		public bool frozen;
		public int armor;

		public CellBall(){
//			position = Vector3.zero;
//			type = BALLTYPE.LASTONE;
//			basic = false;
//			frozen = false;
//			armor = 0;
		}

		public CellBall(BALLTYPE type, bool basic, int armor){
			this.position = Vector3.zero;
			this.type = type;
			this.basic = basic;
			this.armor = armor;
			this.frozen = false;
		}
	}

	[Serializable]
	public class MeshPrim {
		public Vector3 position;
		public Vector3[] vertices;

		public MeshPrim(){
			this.position = Vector3.zero;
		}
		
		public MeshPrim(Vector3 position, Vector3[] vertices){
			this.position = position;
			this.vertices = vertices;
		}

		public void Update(Vector3 position, Vector3[] vertices){
			this.position = position;
			this.vertices = vertices;
		}
	}

	public List<CellBall> CellBallList;
	public List<MeshPrim> MeshPrimList;
	public List<BALLTYPE> BallsColorLine;

	//TODO: Objects in LevelData


	public LevelData(){
		CellBallList = new List<CellBall> ();
		MeshPrimList = new List<MeshPrim> ();
		BallsColorLine = new List<BALLTYPE> ();
	}

	public bool isEmpty(){
		foreach (CellBall cell in CellBallList)
			if (cell.type != BALLTYPE.LASTONE)
				return false;
		return true;
	}

}
