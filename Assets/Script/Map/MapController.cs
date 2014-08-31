using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapController : MonoBehaviour {

	public int MAX_ROW;
	public int MAX_COL;
	public GameObject m_objLeft;
	public GameObject m_objRight;
	public UISprite m_bgLeft;
	public UISprite m_bgRight;

	public TextAsset mapA;
	public TextAsset mapB;

	public PlayerController pc;

	public Map m_MAPLeft;
	public Map m_MAPRight;

	protected float m_ScreenWidth;
	protected float m_ScreenHeight;

	public Dictionary<TileIndex,GameObject> leftTiles = new Dictionary<TileIndex,GameObject>();
	public Dictionary<TileIndex,GameObject> rightTiles = new Dictionary<TileIndex,GameObject>();

	static MapController mInstance = null;

	// Use this for initialization
	void Start () 
	{
		m_ScreenWidth = AutoScaleCenter.Instance.SCREEN_LOGIC_WIDTH;
		m_ScreenHeight = AutoScaleCenter.Instance.SCREEN_LOGIC_HEIDHT;
		InitTile();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static MapController Instance
	{
		get
		{
			if (mInstance == null)
			{
				GameObject obj = GameObject.FindObjectOfType(typeof(MapController)) as GameObject;
				if(obj != null)
					mInstance = obj.GetComponent<MapController>();
			}
			return mInstance;
		}
	}

	public void InitTile()
	{
		//string path1 = "Resources/Data/SceneA.txt";
		//string path2 = "Resources/Data/SceneB.txt";

		m_MAPLeft = new Map(MAX_ROW, MAX_COL);
		m_MAPLeft.m_score = MessageCenter.Instance.mTeam1Num * 10;
		m_MAPRight = new Map(MAX_ROW, MAX_COL);
		m_MAPRight.m_score = MessageCenter.Instance.mTeam2Num * 10;

		GetMapData(mapA, m_MAPLeft);
		GetMapData(mapB, m_MAPRight);

		m_MAPLeft.SetOpponentTile(m_MAPRight, MAX_ROW, MAX_COL);
		InitFlower();
		CreateTiles();
		pc.SendMessage("CreatePlayers");
	}

	public void CreateTiles()
	{
		for(int i = 0; i < MAX_ROW; i++) 
		{
			for(int j = 0; j < MAX_COL; j++)
			{
				GameObject o = GameObject.Instantiate(Resources.Load("Prefabs/Tile")) as GameObject;
				o.transform.parent = m_objLeft.transform;
				o.transform.localScale = Vector3.one;
				o.transform.localPosition = (Vector3)m_MAPLeft.m_tiles[i,j].m_pos;
				o.name = "Tile(" + i.ToString() + ", " + j.ToString() + ")";

				TileInfo tf = o.GetComponent<TileInfo>();
				tf.tileBG.spriteName = "LightGround";
				tf.isLeft = true;
				if(m_MAPLeft.m_tiles[i,j].dir.up == 0)
				{
					tf.tileUpBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileUpBlock.gameObject.SetActive(false);
				}

				if(m_MAPLeft.m_tiles[i,j].dir.left == 0)
				{
					tf.tileLeftBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileLeftBlock.gameObject.SetActive(false);
				}

				if(m_MAPLeft.m_tiles[i,j].dir.down == 0)
				{
					tf.tileDownBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileDownBlock.gameObject.SetActive(false);
				}

				if(m_MAPLeft.m_tiles[i,j].dir.right == 0 )
				{
					tf.tileRightBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileRightBlock.gameObject.SetActive(false);
				}
				
				if(m_MAPLeft.m_tiles[i,j].isPlanted)
				{
					//Debug.Log("Flower");
					tf.tileFlower.gameObject.SetActive(true);
					tf.tileFlower.spriteName = "LightFlower";
					tf.tileFlower.MakePixelPerfect();
				}
				else if(m_MAPLeft.m_tiles[i,j].isRooted)
				{
					//Debug.Log("Root");
					tf.tileFlower.gameObject.SetActive(true);
					tf.tileFlower.spriteName = "DarkRoot";
					tf.tileFlower.MakePixelPerfect();
				}
				else
				{
					tf.tileFlower.gameObject.SetActive(false);
				}

				leftTiles.Add(m_MAPLeft.m_tiles[i,j].m_index,o);

				GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Tile")) as GameObject;
				obj.transform.parent = m_objRight.transform;
				obj.transform.localScale = Vector3.one;
				obj.name = "Tile(" + i.ToString() + ", " + j.ToString() + ")";
				obj.transform.localPosition = (Vector3)m_MAPLeft.m_tiles[i,j].m_pos;

				tf = obj.GetComponent<TileInfo>();
				tf.tileBG.spriteName = "DarkGround";
				tf.isLeft = false;
				if(m_MAPRight.m_tiles[i,j].dir.up == 0)
				{
					tf.tileUpBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileUpBlock.gameObject.SetActive(false);
				}
				
				if(m_MAPRight.m_tiles[i,j].dir.left == 0)
				{
					tf.tileLeftBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileLeftBlock.gameObject.SetActive(false);
				}
				
				if(m_MAPRight.m_tiles[i,j].dir.down == 0)
				{
					tf.tileDownBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileDownBlock.gameObject.SetActive(false);
				}
				
				if(m_MAPRight.m_tiles[i,j].dir.right == 0 )
				{
					tf.tileRightBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileRightBlock.gameObject.SetActive(false);
				}
				if(m_MAPRight.m_tiles[i,j].isPlanted)
				{
					tf.tileFlower.gameObject.SetActive(true);
					tf.tileFlower.spriteName = "DarkFlower";
					tf.tileFlower.MakePixelPerfect();
				}
				else if(m_MAPRight.m_tiles[i,j].isRooted)
				{
					tf.tileFlower.gameObject.SetActive(true);
					tf.tileFlower.spriteName = "LightRoot";
					tf.tileFlower.MakePixelPerfect();
				}
				else
				{
					tf.tileFlower.gameObject.SetActive(false);
				}
				rightTiles.Add(m_MAPRight.m_tiles[i,j].m_index,obj);
			}
		}
	}

	public void GetMapData(TextAsset txtRes, Map map)
	{
		//try 
		{
            int[,] result = new int[3 * 11, 3 * 10];

            string[] lines = txtRes.text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string[] strs = lines[i].Split(',');

                for (int j = 0; j < strs.Length; j++)
                {
                    result[i, j] = int.Parse(strs[j]);
                }
            }

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    map.m_tiles[i, j].dir.up = result[i * 3 + 0, j * 3 + 1];
                    map.m_tiles[i, j].dir.down = result[i * 3 + 2, j * 3 + 1];
                    map.m_tiles[i, j].dir.left = result[i * 3 + 1, j * 3 + 0];
                    map.m_tiles[i, j].dir.right = result[i * 3 + 1, j * 3 + 2];

                    map.m_tiles[i, j].dir.leftUp = result[i * 3 + 0, j * 3 + 0];
                    map.m_tiles[i, j].dir.leftDown = result[i * 3 + 0, j * 3 + 2];
                    map.m_tiles[i, j].dir.rightUp = result[i * 3 + 2, j * 3 + 0];
                    map.m_tiles[i, j].dir.rightDown = result[i * 3 + 2, j * 3 + 2];
                }
            }

            //// Create an instance of StreamReader to read from a file.
            //// The using statement also closes the StreamReader.
            //string[] line = txtRes.text.Split('\n');
            //int index = -1;
            //// Read and display lines from the file until the end of 
            //// the file is reached.
            //for (int n = 0; n < line.Length; n++)
            //{
            //    index += 1; // line number
            //    string[] result = line[n].Split(',');
            //    for (int i = 0; i < result.Length; i++)
            //    {
            //        if (index == 0 || index % 3 == 0)
            //        {
            //            if (i % 3 == 1)
            //            {
            //                map.m_tiles[index / 3, i].dir.up = System.Int32.Parse(result[i]);
            //            }
            //        }
            //        else if (index % 3 == 1)
            //        {
            //            if (i == 0 || i % 3 == 0)
            //            {
            //                map.m_tiles[index / 3, i].dir.left = System.Int32.Parse(result[i]);
            //            }
            //            else if (i % 3 == 2)
            //            {
            //                map.m_tiles[index / 3, i].dir.right = System.Int32.Parse(result[i]);
            //            }
            //        }
            //        else if (index % 3 == 2)
            //        {
            //            if (i % 3 == 1)
            //            {
            //                map.m_tiles[index / 3, i].dir.down = System.Int32.Parse(result[i]);
            //            }
            //        }
            //    }
            //    Debug.Log(line);
            //}
		}
        //catch(Exception e)
        //{
        //    // Let the user know what went wrong.
        //    Debug.Log("The file could not be read:");
        //    Debug.Log(e.Message);
        //}

		SetMapPos(map);
		
	}

	public void SetMapPos(Map map)
	{
		for (int i = 0; i < MAX_ROW; i++)
		{
			for(int j = 0; j < MAX_COL; j++)
			{
				int x = 0;
				int y = 0;

				x = -270 + (j * 60);
				y = 300 - (i * 60);

				map.m_tiles[i,j].m_pos = new Vector3(x,y);
			}
		}
	}
	
	void InitFlower()
	{
		int tFlowersLeft = MessageCenter.Instance.mTeam1Num * 10;
		if(tFlowersLeft == 0) tFlowersLeft = 10;
		while(tFlowersLeft > 0)
		{
			int tRow = UnityEngine.Random.Range(0, 5);
			int tColumn = UnityEngine.Random.Range(0, 10);
			Tile tTile = m_MAPLeft.m_tiles[tRow, tColumn];
			if(!tTile.isPlanted
			&& !tTile.isRooted)
			{
				--tFlowersLeft;
				tTile.isPlanted = true;
				tTile.m_opponentTile.isRooted = true;
				tTile.canAward = true;
			}
		}
		tFlowersLeft = MessageCenter.Instance.mTeam2Num * 10;
		if(tFlowersLeft == 0) tFlowersLeft = 10;
		while(tFlowersLeft > 0)
		{
			int tRow = UnityEngine.Random.Range(6, 11);
			int tColumn = UnityEngine.Random.Range(0, 10);
			Tile tTile = m_MAPRight.m_tiles[tRow, tColumn];
			if(!tTile.isPlanted
			&& !tTile.isRooted)
			{
				--tFlowersLeft;
				tTile.isPlanted = true;
				tTile.m_opponentTile.isRooted = true;
				tTile.canAward = true;
			}
		}
	}
}
