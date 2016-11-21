#define notDEBUGSAVE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class ChapterData {
	//VERSION
	public int version;
	
	//CHAPTER
	public int ID;
	public GAMETYPE GameType;
	public string Name;
	public string Description;
	
	//ICON
	public string IconName;
	
	//ICONTEXTURE
	public byte[] IconTexture;
	
	//LEVEL
	public List<LevelData> LevelList;
	private LevelData currentLevelData;


	
	public int[] IDsNeedToBeFinishedForUnlock;
	
	public ChapterData(){
		Debug.Log ("ChapterData:" + this.ToString());
		
		//		DATABLOCK d = DATABLOCK.NONE;
		//		while (true) {
		//			Debug.Log("DATA:"+d);
		//			if(d == DATABLOCK.END)
		//				break;
		//			d++;
		//		}
		
		
		version = 4;
		ID = 0;
		GameType = GAMETYPE.NOSELECTED;
		Name = "Chapter Name";
		Description = "Chapter Description";
		
		IconName = "";
		IconTexture = null;
		
		LevelList = new List<LevelData>();
		currentLevelData = null;
		
		//SaveDataInFile ();
		//LoadDataFromFile ();
	}

	private void Clear(){
		version = 4;
		ID = 0;
		GameType = GAMETYPE.NOSELECTED;
		Name = "Chapter Name";
		Description = "Chapter Description";
		IconName = "";
		IconTexture = null;
		LevelList.Clear ();
		currentLevelData = null;
	}
	
	public void SaveDataInFile(string filename = "lvl.bytes"){
		BinaryWriter _BinaryWriter = new BinaryWriter(File.OpenWrite(filename));
		
		_BinaryWriter.Write ("BubbleRockMap");	//title
		
		//VERSION
		_BinaryWriter.Write ((byte)	DATABLOCK.VERSION);
		_BinaryWriter.Write ((int)	4);
		
		_BinaryWriter.Write ((byte)	DATABLOCK.CHAPTER);
		_BinaryWriter.Write ((int)	ID);
		_BinaryWriter.Write ((byte)	GameType);
		_BinaryWriter.Write (		Name);
		_BinaryWriter.Write (		Description);
		
		
		
		
		foreach (LevelData levelData in LevelList) {
			_BinaryWriter.Write ((byte)	DATABLOCK.LEVEL);

			if(levelData.BallsColorLine != null){
				if(levelData.BallsColorLine.Count > 0){
					_BinaryWriter.Write ((byte)	DATABLOCK.BALLCOLORLINE);
					foreach(BALLTYPE bt in levelData.BallsColorLine){
						if(bt != BALLTYPE.LASTONE)
							_BinaryWriter.Write ((byte)	bt);
						else
							break;
					}
					_BinaryWriter.Write ((byte)	BALLTYPE.LASTONE);
				}
			}
			
			foreach (LevelData.CellBall cell in levelData.CellBallList) {
				_BinaryWriter.Write ((byte)	DATABLOCK.CELL_BALL);
				_BinaryWriter.Write ((float)cell.position.x);
				_BinaryWriter.Write ((float)cell.position.y);
				_BinaryWriter.Write ((byte) cell.type);
				_BinaryWriter.Write ((bool) cell.basic);
				_BinaryWriter.Write ((bool) cell.frozen);
				_BinaryWriter.Write ((int) cell.armor);
				
				/* *
				 * x
				 * y
				 * type
				 * basic
				 * frozen
				 * armor
				 * 
				 * */
				
			}
			
			_BinaryWriter.Write ((byte)	DATABLOCK.LEVELEND);
		}
		
		_BinaryWriter.Write ((byte)DATABLOCK.END);
		
		_BinaryWriter.Flush ();
		_BinaryWriter.Close ();
		
		
		#if DEBUGSAVE
		Debug.Log("_BinaryWriter: DONE");
		#endif
		
	}
	
	public void LoadDataFromFile(string filename = "lvl.bytes"){
		Clear ();
		BinaryReader _BinaryReader = new BinaryReader (File.OpenRead(filename));
		LoadData (_BinaryReader);
	}

	public void LoadDataFromMemory(Stream s){
		Clear ();
		BinaryReader _BinaryReader = new BinaryReader (s);
		LoadData (_BinaryReader);
	}

	private void LoadData(BinaryReader _BinaryReader){
		string title = _BinaryReader.ReadString ();
		if (title != "BubbleRockMap") {
			Debug.Log ("Bad title " + title);
			return;
		}
		
		while (ReadBlock(_BinaryReader))
			continue;
		_BinaryReader.Close ();
	}
	
	private bool ReadBlock(BinaryReader _BinaryReader){
		DATABLOCK block = (DATABLOCK) _BinaryReader.ReadByte ();
		switch (block) {
			
		case DATABLOCK.VERSION:
			#if DEBUGSAVE
			Debug.Log("ReadBlock: VERSION");
			#endif
			version = _BinaryReader.ReadInt32();
			#if DEBUGSAVE
			Debug.Log ("Version " + version);
			#endif
			if(version < 4){
				Debug.LogError ("OLDVERSION");
				return false;
			}
			return true;
			
		case DATABLOCK.CHAPTER:
			#if DEBUGSAVE
			Debug.Log("ReadBlock: CHAPTER");
			#endif
			
			ID = _BinaryReader.ReadInt32();
			GameType = (GAMETYPE) _BinaryReader.ReadByte();
			Name = _BinaryReader.ReadString();
			Description = _BinaryReader.ReadString();
			
			return true;
			
		case DATABLOCK.LEVEL:
			#if DEBUGSAVE
			Debug.Log("ReadBlock: LEVEL");
			#endif
			currentLevelData = new LevelData();
			LevelList.Add(currentLevelData);
			while (ReadBlock(_BinaryReader))
				continue;
			
			return true;
			
		case DATABLOCK.CELL_BALL:
			#if DEBUGSAVE
			Debug.Log("ReadBlock: CELL_BALL");
			#endif
			if(currentLevelData == null){
				Debug.LogError("ReadBlock: CELL_BALL NO LEVEL");
				return false;
			}
			
			LevelData.CellBall ball = new LevelData.CellBall();
			
			ball.position.Set(
				_BinaryReader.ReadSingle(),
				_BinaryReader.ReadSingle(),
				0.0f);
			
			ball.type = (BALLTYPE)_BinaryReader.ReadByte();
			ball.basic = _BinaryReader.ReadBoolean();
			ball.frozen = _BinaryReader.ReadBoolean();
			ball.armor = _BinaryReader.ReadInt32();
			
			currentLevelData.CellBallList.Add(ball);
			
			return true;

		case DATABLOCK.BALLCOLORLINE:
			#if DEBUGSAVE
			Debug.Log("ReadBlock: BALLCOLORLINE");
			#endif

			BALLTYPE bt = (BALLTYPE)_BinaryReader.ReadByte();
			while(bt != BALLTYPE.LASTONE){
				currentLevelData.BallsColorLine.Add(bt);
				bt = (BALLTYPE)_BinaryReader.ReadByte();
			}
			return true;
			
		case DATABLOCK.LEVELEND:
			#if DEBUGSAVE
			Debug.Log("ReadBlock: LEVELEND");
			#endif
			return false;
			
		case DATABLOCK.END:
			#if DEBUGSAVE
			Debug.Log("ReadBlock: END");
			#endif
			return false;
			
		default:
			#if DEBUGSAVE
			Debug.LogError("ReadBlock: NO DATA");
			#endif
			return false;
		}
	}
}