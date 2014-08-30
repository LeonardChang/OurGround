﻿using UnityEngine;
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

	public Map m_MAPLeft;
	public Map m_MAPRight;

	protected float m_ScreenWidth;
	protected float m_ScreenHeight;

	public List<GameObject> leftTiles = new List<GameObject>();
	public List<GameObject> rightTiles = new List<GameObject>();

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

	public void InitTile()
	{
		//string path1 = "Resources/Data/SceneA.txt";
		//string path2 = "Resources/Data/SceneB.txt";
		m_MAPLeft = new Map(MAX_ROW, MAX_COL);
		m_MAPRight = new Map(MAX_ROW, MAX_COL);
		GetMapData(mapA, m_MAPLeft);
		GetMapData(mapB, m_MAPRight);

		m_MAPLeft.SetOpponentTile(m_MAPRight, MAX_ROW, MAX_COL);

		CreateTiles();
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

				TileInfo tf = o.GetComponent<TileInfo>();
				tf.tileBG.spriteName = "LightGround";
				if(m_MAPLeft.m_tiles[i,j].dir.up == 1)
				{
					tf.tileUpBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileUpBlock.gameObject.SetActive(false);
				}

				if(m_MAPLeft.m_tiles[i,j].dir.left == 1)
				{
					tf.tileLeftBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileLeftBlock.gameObject.SetActive(false);
				}

				if(m_MAPLeft.m_tiles[i,j].dir.down == 1)
				{
					tf.tileDownBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileDownBlock.gameObject.SetActive(false);
				}

				if(m_MAPLeft.m_tiles[i,j].dir.right == 1 )
				{
					tf.tileRightBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileRightBlock.gameObject.SetActive(false);
				}

				leftTiles.Add(o);

				GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Tile")) as GameObject;
				obj.transform.parent = m_objRight.transform;
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = (Vector3)m_MAPLeft.m_tiles[i,j].m_pos;

				tf = obj.GetComponent<TileInfo>();
				tf.tileBG.spriteName = "DarkGround";

				if(m_MAPRight.m_tiles[i,j].dir.up == 1)
				{
					tf.tileUpBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileUpBlock.gameObject.SetActive(false);
				}
				
				if(m_MAPRight.m_tiles[i,j].dir.left == 1)
				{
					tf.tileLeftBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileLeftBlock.gameObject.SetActive(false);
				}
				
				if(m_MAPRight.m_tiles[i,j].dir.down == 1)
				{
					tf.tileDownBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileDownBlock.gameObject.SetActive(false);
				}
				
				if(m_MAPRight.m_tiles[i,j].dir.right == 1 )
				{
					tf.tileRightBlock.gameObject.SetActive(true);
				}
				else
				{
					tf.tileRightBlock.gameObject.SetActive(false);
				}
				rightTiles.Add(o);
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

				x = -270 + (i * 60);
				y = 300 - (j * 60);

				map.m_tiles[i,j].m_pos = new Vector3(x,y);
			}
		}
	}
}
