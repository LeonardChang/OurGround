using UnityEngine;
using System.Collections;

public class Direct
{
	public int up;
	public int rightUp;
	public int right;
	public int rightDown;
	public int down;
	public int leftDown;
	public int left;
	public int leftUp;

	public Direct()
	{
		up = 1;
		rightUp = 1;
		right = 1;
		rightDown = 1;
		down = 1;
		leftDown = 1;
		left = 1;
		leftUp = 1;
	}
}

public class TileIndex
{
	public int m_x;
	public int m_y;

	public TileIndex()
	{
		m_x = 0;
		m_y = 0;
	}
	public TileIndex(int a, int b)
	{
		m_x = a;
		m_y = b;
	}
}

public class Tile
{
	public TileIndex m_index;
	public Tile m_opponentTile;
	public Direct dir;
	public Vector2 m_pos;
	public int m_width;
	public int m_height;

	public bool isPlanted;
	public bool canAward;
	public bool isRooted;
	public bool isPulling;

	public Tile()
	{
		m_index = new TileIndex();
		m_pos = Vector3.zero;
		isPlanted = false;
		isPulling = false;
		isRooted = false;
		m_opponentTile = null;
		dir = new Direct();
		m_width = 60;
		m_height = 60;
		canAward = false;
	}
}

public class Map
{
	public Tile[,] m_tiles;
	public int m_score;

	public Map(int row, int col)
	{
		m_score = 0;
		m_tiles = new Tile[row, col];
		for(int i = 0; i < row; i++)
		{
			for(int j = 0; j < col; j++)
			{
				m_tiles[i, j] = new Tile();
				m_tiles[i, j].m_index.m_x = i;
				m_tiles[i, j].m_index.m_y = j;
			}
		}
	}

	public void SetOpponentTile(Map oppMap, int row, int col)
	{
		for (int i = 0; i < row; i++)
		{
			for(int j = 0; j < col; j++)
			{
				m_tiles[i, j].m_opponentTile = oppMap.m_tiles[i, col - j - 1];
				oppMap.m_tiles[i, col - j - 1].m_opponentTile = m_tiles[i, j];
			}
		}
	}
}
