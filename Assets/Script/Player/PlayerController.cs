using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public GameObject leftPanel;
	public GameObject rightPanel;
	public Dictionary<string, PlayerInfo> m_playDic = new Dictionary<string, PlayerInfo>();
	public Dictionary<string, Vector2> m_playRunInfo = new Dictionary<string, Vector2>();

	public List<TileIndex> usedTileList = new List<TileIndex>();
	public float ratio = 1.35f;
	public MapController mp;

	public bool startGame;

	protected List<string> playID = new List<string>();
	protected bool isPressed;

	// Use this for initialization
	void Start ()
	{
		MessageCenter.Instance.ClickButtonEvent = KickGround;
		MessageCenter.Instance.JoystickControlEvent = MovePlayer;
	}
	
	// Update is called once per frame
	void Update () {
		if (startGame) 
		{
			UpdatePos();

		}
	}

	public void CreatePlayers()
	{
		playID.Clear();
		foreach (var kp in MessageCenter.Instance.mPlayers)
		{
			playID.Add(kp.Key);
		}

		m_playDic.Clear();
		for(int i = 0; i < playID.Count; i++)
		{
			if(MessageCenter.Instance.mPlayerTeam.ContainsKey(playID[i]))
			{
				if(MessageCenter.Instance.mPlayerTeam[playID[i]] == 0)
				{
					GameObject o = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
					o.transform.parent = leftPanel.transform;
					o.transform.localScale = Vector3.one;
					SetPlayerPos(o,true);
					m_playDic.Add(playID[i], o.GetComponent<PlayerInfo>());
				}
				else if(MessageCenter.Instance.mPlayerTeam[playID[i]] == 1)
				{
					GameObject o = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
					o.transform.parent = rightPanel.transform;
					o.transform.localScale = Vector3.one;
					PlayerInfo pi = o.GetComponent<PlayerInfo>();
					pi.m_icon.spriteName = "DarkSprite1";
					SetPlayerPos(o,false);
					m_playDic.Add(playID[i], o.GetComponent<PlayerInfo>());
				}
			}
		}
		startGame = true;
	}

	public void GetMapData(Map map)
	{

	}

	public void SetPlayerPos(GameObject o, bool left)
	{
		bool canPlacePlayer = false;
		if(left)
		{
			while(!canPlacePlayer)
			{
				int row = UnityEngine.Random.Range(0, 4);
				int col = UnityEngine.Random.Range(0, 10);
				TileIndex index = new TileIndex(row, col);
				if(!usedTileList.Contains(index))
				{
					Tile tile = mp.m_MAPLeft.m_tiles[row, col];
					usedTileList.Add(index);
					canPlacePlayer = true;
					o.transform.localPosition = (Vector3)mp.m_MAPLeft.m_tiles[row,col].m_pos;
				}
			}
		}
		else
		{
			while(!canPlacePlayer)
			{
				int row = UnityEngine.Random.Range(6, 11);
				int col = UnityEngine.Random.Range(0, 10);
				TileIndex index = new TileIndex(row, col);
				if(!usedTileList.Contains(index))
				{
					Tile tile = mp.m_MAPRight.m_tiles[row, col];
					usedTileList.Add(index);
					canPlacePlayer = true;
					o.transform.localPosition = (Vector3)mp.m_MAPRight.m_tiles[row,col].m_pos;
				}
			}
		}
	}

	public bool CanMoveTo(Vector3 destPos, bool isLeft)
	{
		if(destPos.x <= -270 || destPos.x >= 270)
		{
			return false;
		}
		else if(destPos.y >= 300 || destPos.y <= -300)
		{
			return false;
		}

		TileIndex index = GetTileIndex(destPos, isLeft);
		if(index.m_x == -1)
			return false;

		if(isLeft)
		{
			int row = index.m_x;
			int col = index.m_y;

			int width = mp.m_MAPLeft.m_tiles[row,col].m_width;
			int height = mp.m_MAPLeft.m_tiles[row,col].m_height;
			float ox = mp.m_MAPLeft.m_tiles[row,col].m_pos.x;
			float oy = mp.m_MAPLeft.m_tiles[row,col].m_pos.y;

			if(destPos.y > oy)
			{
				if((row-1 >= 0) && (row-1 <= 10))
				{
					if(mp.m_MAPLeft.m_tiles[row-1, col].dir.down != 1)
						return false;
					else if(mp.m_MAPLeft.m_tiles[row-1, col].dir.down == 1)
					{
						if(destPos.x > ox)
						{
							if((col + 1 >= 1) && (col + 1 <= 9))
							{
								if(mp.m_MAPLeft.m_tiles[row, col + 1].dir.left != 1)
									return false;
								else
									return true;
							}
							else
								return false;
						}
						else if(destPos.x < ox)
						{
							if((col - 1 >= 0) && (col - 1 <= 8))
							{
								if(mp.m_MAPLeft.m_tiles[row, col - 1].dir.right != 1)
									return false;
								else
									return true;
							}
							else
								return false;
						}
					}
				}
				else
					return false;
			}
			else if(destPos.y < oy)
			{
				if((row + 1 >= 1) && (row+1 <= 10))
				{
					if(mp.m_MAPLeft.m_tiles[row + 1, col].dir.up != 1)
						return false;
					else if(mp.m_MAPLeft.m_tiles[row + 1, col].dir.up == 1)
					{
						if(destPos.x > ox)
						{
							if((col + 1 >= 1) && (col + 1 <= 9))
							{
								if(mp.m_MAPLeft.m_tiles[row, col + 1].dir.left != 1)
									return false;
								else
									return true;
							}
							else 
								return false;
						}
						else if(destPos.x < ox)
						{
							if((col - 1 >= 0) && (col - 1 <= 8))
							{
								if(mp.m_MAPLeft.m_tiles[row, col - 1].dir.right != 1)
									return false;
								else
									return true;
							}
							else
								return false;
						}
					}
				}
				else
					return false;
			}

			return false;
		}
		else
		{
			int row = index.m_x;
			int col = index.m_y;
			
			int width = mp.m_MAPRight.m_tiles[row,col].m_width;
			int height = mp.m_MAPRight.m_tiles[row,col].m_height;
			float ox = mp.m_MAPRight.m_tiles[row,col].m_pos.x;
			float oy = mp.m_MAPRight.m_tiles[row,col].m_pos.y;
			
			if(destPos.y > oy)
			{
				if((row-1 >= 0) && (row-1 <= 10))
				{
					if(mp.m_MAPRight.m_tiles[row-1, col].dir.down != 1)
						return false;
					else if(mp.m_MAPRight.m_tiles[row-1, col].dir.down == 1)
					{
						if(destPos.x > ox)
						{
							if((col + 1 >= 1) && (col + 1 <= 9))
							{
								if(mp.m_MAPRight.m_tiles[row, col + 1].dir.left != 1)
									return false;
								else
									return true;
							}
							else 
								return false;
						}
						else if(destPos.x < ox)
						{
							if((col - 1 >= 0) && (col - 1 <= 8))
							{
								if(mp.m_MAPRight.m_tiles[row, col - 1].dir.right != 1)
									return false;
								else
									return true;
							}
							else
								return false;
						}
					}
				}
				else 
					return false;
			}
			else if(destPos.y < oy)
			{
				if((row + 1 >= 1) && (row+1 <= 10))
				{
					if(mp.m_MAPRight.m_tiles[row + 1, col].dir.up != 1)
						return false;
					else if(mp.m_MAPRight.m_tiles[row + 1, col].dir.up == 1)
					{
						if(destPos.x > ox)
						{
							if((col + 1 >= 1) && (col + 1 <= 9))
							{
								if(mp.m_MAPRight.m_tiles[row, col + 1].dir.left != 1)
									return false;
								else
									return true;
							}
							else 
								return false;
						}
						else if(destPos.x < ox)
						{
							if((col - 1 >= 0) && (col - 1 <= 8))
							{
								if(mp.m_MAPRight.m_tiles[row, col - 1].dir.right != 1)
									return false;
								else
									return true;
							}
							else
								return false;
						}
					}
				}
				else
					return false;
			}

			return false;
		}
	}

//	public void UpdatePlayerPos(out Vector3 destPos, bool isLeft)
//	{
//		if(destPos.x <= -270)
//		{
//			destPos.x = -270;
//		}
//		
//		if(destPos.x >= 270)
//		{
//			destPos.x = 270;
//		}
//		
//		if(destPos.y >= 300)
//		{
//			destPos.y = 300;
//		}
//		
//		if(destPos.y <= -300)
//		{
//			destPos.y = -300;
//		}
//		
//		TileIndex index = GetTileIndex(destPos, isLeft);
//		if(isLeft)
//		{
//			int row = index.m_x;
//			int col = index.m_y;
//			
//			int width = mp.m_MAPLeft.m_tiles[row,col].m_width;
//			int height = mp.m_MAPLeft.m_tiles[row,col].m_height;
//			float ox = mp.m_MAPLeft.m_tiles[row,col].m_pos.x;
//			float oy = mp.m_MAPLeft.m_tiles[row,col].m_pos.y;
//			
//			if(destPos.y > oy)
//			{
//				if((row-1 >= 0) && (row-1 <= 10))
//				{
//					if(mp.m_MAPLeft.m_tiles[row-1, col].dir.down != 1)
//					{
//						if(destPos.x > ox)
//						{
//							if((col + 1 >= 1) && (col + 1 <= 9))
//							{
//								if(mp.m_MAPLeft.m_tiles[row, col + 1].dir.left == 1)
//								{
//									destPos.x
//								}
//							}
//						}
//						else if(destPos.x < ox)
//						{
//							if((col - 1 >= 0) && (col - 1 <= 8))
//							{
//								if(mp.m_MAPLeft.m_tiles[row, col - 1].dir.right != 1)
//									return false;
//								else
//									return true;
//							}
//						}
//					}
//				}
//			}
//			else if(destPos.y < oy)
//			{
//				if((row + 1 >= 1) && (row+1 <= 10))
//				{
//					if(mp.m_MAPLeft.m_tiles[row + 1, col].dir.up != 1)
//						return false;
//					else if(mp.m_MAPLeft.m_tiles[row + 1, col].dir.up == 1)
//					{
//						if(destPos.x > ox)
//						{
//							if((col + 1 >= 1) && (col + 1 <= 9))
//							{
//								if(mp.m_MAPLeft.m_tiles[row, col + 1].dir.left != 1)
//									return false;
//								else
//									return true;
//							}
//						}
//						else if(destPos.x < ox)
//						{
//							if((col - 1 >= 0) && (col - 1 <= 8))
//							{
//								if(mp.m_MAPLeft.m_tiles[row, col - 1].dir.right != 1)
//									return false;
//								else
//									return true;
//							}
//						}
//					}
//				}
//			}
//		}
//		else
//		{
//			int row = index.m_x;
//			int col = index.m_y;
//			
//			int width = mp.m_MAPRight.m_tiles[row,col].m_width;
//			int height = mp.m_MAPRight.m_tiles[row,col].m_height;
//			float ox = mp.m_MAPRight.m_tiles[row,col].m_pos.x;
//			float oy = mp.m_MAPRight.m_tiles[row,col].m_pos.y;
//			//int left = ox - width/2;
//			//int right = ox + width/2;
//			//int up = oy + height/2;
//			//int down = oy - height/2;
//			
//			if(destPos.y > oy)
//			{
//				if((row-1 >= 0) && (row-1 <= 10))
//				{
//					if(mp.m_MAPRight.m_tiles[row-1, col].dir.down != 1)
//						return false;
//					else if(mp.m_MAPRight.m_tiles[row-1, col].dir.down == 1)
//					{
//						if(destPos.x > ox)
//						{
//							if((col + 1 >= 1) && (col + 1 <= 9))
//							{
//								if(mp.m_MAPRight.m_tiles[row, col + 1].dir.left != 1)
//									return false;
//								else
//									return true;
//							}
//						}
//						else if(destPos.x < ox)
//						{
//							if((col - 1 >= 0) && (col - 1 <= 8))
//							{
//								if(mp.m_MAPRight.m_tiles[row, col - 1].dir.right != 1)
//									return false;
//								else
//									return true;
//							}
//						}
//					}
//				}
//			}
//			else if(destPos.y < oy)
//			{
//				if((row + 1 >= 1) && (row+1 <= 10))
//				{
//					if(mp.m_MAPRight.m_tiles[row + 1, col].dir.up != 1)
//						return false;
//					else if(mp.m_MAPRight.m_tiles[row + 1, col].dir.up == 1)
//					{
//						if(destPos.x > ox)
//						{
//							if((col + 1 >= 1) && (col + 1 <= 9))
//							{
//								if(mp.m_MAPRight.m_tiles[row, col + 1].dir.left != 1)
//									return false;
//								else
//									return true;
//							}
//						}
//						else if(destPos.x < ox)
//						{
//							if((col - 1 >= 0) && (col - 1 <= 8))
//							{
//								if(mp.m_MAPRight.m_tiles[row, col - 1].dir.right != 1)
//									return false;
//								else
//									return true;
//							}
//						}
//					}
//				}
//			}
//		}
//	}

	public TileIndex GetTileIndex(Vector3 pos, bool isLeft)
	{
		TileIndex index = new TileIndex();

		for(int i = 0; i < mp.MAX_ROW; i++)
		{
			for(int j = 0; j < mp.MAX_COL; j++)
			{
				if(isLeft)
				{
					int width = mp.m_MAPLeft.m_tiles[i,j].m_width;
					int height = mp.m_MAPLeft.m_tiles[i,j].m_height;
					float ox = mp.m_MAPLeft.m_tiles[i,j].m_pos.x;
					float oy = mp.m_MAPLeft.m_tiles[i,j].m_pos.y;
					float left = ox - width/2;
					float right = ox + width/2;
					float up = oy + height/2;
					float down = oy - height/2;

					if(pos.x > left && pos.x < right)
					{
						if(pos.y > down && pos.y < up)
						{
							return mp.m_MAPLeft.m_tiles[i,j].m_index;
						}
					}
				}
				else
				{
					int width = mp.m_MAPRight.m_tiles[i,j].m_width;
					int height = mp.m_MAPRight.m_tiles[i,j].m_height;
					float ox = mp.m_MAPRight.m_tiles[i,j].m_pos.x;
					float oy = mp.m_MAPRight.m_tiles[i,j].m_pos.y;
					float left = ox - width/2;
					float right = ox + width/2;
					float up = oy + height/2;
					float down = oy - height/2;
					
					if(pos.x > left && pos.x < right)
					{
						if(pos.y > down && pos.y < up)
						{
							return mp.m_MAPRight.m_tiles[i,j].m_index;
						}
					}
				}
			}
		}
		index.m_x = -1;
		index.m_y = -1;
		return index;
	}

	public void MovePlayer(bool press, Vector2 pos, string id)
	{
		if(press)
		{
			isPressed = true;

			if(m_playRunInfo.ContainsKey(id))
			{
				m_playRunInfo[id] = pos * ratio;
			}
			else
			{
				m_playRunInfo.Add(id, pos * ratio);
			}

			if(m_playDic.ContainsKey(id))
			{	
				if(pos.x > 0)
				{
					Quaternion q = new Quaternion();
					q.eulerAngles = new Vector3(0,0,0);
					
					m_playDic[id].transform.localRotation = q;
				}
				else
				{
					Quaternion q = new Quaternion();
					q.eulerAngles = new Vector3(0,180,0);
					
					m_playDic[id].transform.localRotation = q;
				}
			}

//			bool isLeft = false;
//			if(m_playDic.ContainsKey(id))
//			{
//				Vector3 p = m_playDic[id].transform.localPosition;
//				p += (Vector3)pos;
//				
//				if(pos.x > 0)
//				{
//					Quaternion q = new Quaternion();
//					q.eulerAngles = new Vector3(0,0,0);
//					
//					m_playDic[id].transform.localRotation = q;
//				}
//				else
//				{
//					Quaternion q = new Quaternion();
//					q.eulerAngles = new Vector3(0,180,0);
//					
//					m_playDic[id].transform.localRotation = q;
//				}
//				
//				if(MessageCenter.Instance.mPlayerTeam.ContainsKey(id))
//				{
//					if(MessageCenter.Instance.mPlayerTeam[id] == 0)
//					{
//						isLeft = true;
//					}
//					else
//					{
//						isLeft = false;
//					}
//				}
//
//				if(CanMoveTo(p,isLeft))
//				{
//					m_playDic[id].transform.localPosition = p;
//				}
//				else
//				{
//					//UpdatePlayerPos(p,isLeft);
//				}
//				//m_playDic[id].transform.localPosition += pos;
//			}
		}
		else
		{
			isPressed = false;
			if(m_playRunInfo.ContainsKey(id))
			{
				m_playRunInfo.Remove(id);
			}
		}
	}

	public void UpdatePos()
	{
		bool isLeft = false;
		foreach (var keyPair in m_playRunInfo)
		{
			if(m_playDic.ContainsKey(keyPair.Key))
			{
				Vector3 p = m_playDic[keyPair.Key].transform.localPosition;
				p += (Vector3)keyPair.Value * Time.deltaTime * 60;
				if(MessageCenter.Instance.mPlayerTeam.ContainsKey(keyPair.Key))
				{
					if(MessageCenter.Instance.mPlayerTeam[keyPair.Key] == 0)
					{
						isLeft = true;
					}
					else
					{
						isLeft = false;
					}
				}
				
				if(CanMoveTo(p,isLeft))
				{
					ChangeSprite(isLeft, keyPair.Key);
					m_playDic[keyPair.Key].transform.localPosition = p;
				}
				else
				{
					//UpdatePlayerPos(p,isLeft);
				}
			}
		}
	}

	public void ChangeSprite(bool isLeft, string id)
	{
		if(m_playDic.ContainsKey(id))
		{
			if(isLeft)
			{
				if(m_playDic[id].m_icon.spriteName == "LightSprite1")
				{
					m_playDic[id].m_icon.spriteName = "LightSprite2";
				}
				else
				{
					m_playDic[id].m_icon.spriteName = "LightSprite1";
				}
			}
			else
			{
				if(m_playDic[id].m_icon.spriteName == "DarkSprite1")
				{
					m_playDic[id].m_icon.spriteName = "DarkSprite2";
				}
				else
				{
					m_playDic[id].m_icon.spriteName = "DarkSprite1";
				}
			}
		}
	}

	public void KickGround(string btn, NetworkMessageInfo msg,string id)
	{
		if (btn == "A")
		{
			//pull root, plant
		}
		else if(btn == "B")
		{
			bool isLeft = false;
			//kick ground
			if(m_playDic.ContainsKey(id))
			{
				Vector3 pos = m_playDic[id].transform.localPosition;
				if(MessageCenter.Instance.mPlayerTeam.ContainsKey(id))
				{
					if(MessageCenter.Instance.mPlayerTeam[id] == 0)
					{
						isLeft = true;
					}
					else
					{
						isLeft = false;
					}
				}
				TileIndex index = new TileIndex();
				index = GetTileIndex(pos, isLeft);
				if(index.m_x != -1)
				{
					Vector3 p = mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.m_pos;
					int width = mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_width;
					int height = mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_height;
					foreach(var play in m_playDic)
					{
						float left = p.x - width/2;
						float right = p.x + width/2;
						float up = p.y + height/2;
						float down = p.y - height/2;
						Vector3 pos = play.Value.transform.localPosition;
						if(pos.x >= left && pos.x <= right && pos.y <= up && pos.y >= down)
						{
							play.Value.isKnocked = true;
							if(isLeft)
							{
								play.Value.m_icon.spriteName = "DarkSprite_skill";
							}
							else
							{
								play.Value.m_icon.spriteName = "LightSprite_skill";
							}
						}
					}
				}
			}
		}
	}

	public bool ContainsPos(Vector3 pos)
	{

	}
}
