using UnityEngine;
using System.Collections;

public enum GroundType
{
	NONE,
	GROUND,
	UNDERGROUND,
	BOLCK,
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
}

public class Tile
{
	public TileIndex m_index;
	public Tile m_opponentTile;
	public Vector2 m_pos;
	public int m_width;
	public int m_height;

	public bool isPlanted;
	public bool isPulling;
	public bool isUnderGround;

	public GroundType type; 

	public Tile()
	{
		m_pos = Vector3.zero;
		isPlanted = false;
		isPulling = false;
		isUnderGround = false;
		type = GroundType.NONE;
		m_opponentTile = null;
	}
}

public class Map
{
	public Tile[,] m_tiles;

	public Map(int row, int col)
	{
		m_tiles = new Tile[row, col];
	}

	public void SetOpponentTile(Map oppMap, int row, int col)
	{
		for (int i = 0; i < row; i++)
		{
			for(int j = 0; j < col; j++)
			{
				m_tiles[i, j].m_opponentTile = oppMap.m_tiles[i, col - j];
				oppMap.m_tiles[i, col - j].m_opponentTile = m_tiles[i, j];
			}
		}
	}
}
