using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public GameObject leftPanel;
	public GameObject rightPanel;
	public UILabel leftScore;
	public UILabel rightScore;
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
		MessageCenter.Instance.ClickButtonEvent = GetButtonInfo;
		MessageCenter.Instance.JoystickControlEvent = MovePlayer;
	}
	
	// Update is called once per frame
	void Update () {
		if (startGame) 
		{
			UpdatePos();
			Pulling();
			CheckScore();
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
					o.GetComponent<PlayerInfo>().isLeft = true;
					m_playDic.Add(playID[i], o.GetComponent<PlayerInfo>());
                    o.GetComponent<PlayerInfo>().m_Label.text = MessageCenter.Instance.GetPlayerName(playID[i]);
				}
				else if(MessageCenter.Instance.mPlayerTeam[playID[i]] == 1)
				{
					GameObject o = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
					o.transform.parent = rightPanel.transform;
					o.transform.localScale = Vector3.one;
					PlayerInfo pi = o.GetComponent<PlayerInfo>();
					pi.m_icon.spriteName = "DarkSprite1";
					pi.isLeft = false;
					SetPlayerPos(o,false);
					m_playDic.Add(playID[i], o.GetComponent<PlayerInfo>());
                    o.GetComponent<PlayerInfo>().m_Label.text = MessageCenter.Instance.GetPlayerName(playID[i]);
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

	public Vector3 UpdatePlayerPos(Vector3 destPos, bool isLeft)
	{
		if(destPos.x <= -270)
		{
			destPos.x = -270;
		}
		
		if(destPos.x >= 270)
		{
			destPos.x = 270;
		}
		
		if(destPos.y >= 300)
		{
			destPos.y = 300;
		}
		
		if(destPos.y <= -300)
		{
			destPos.y = -300;
		}
		
		TileIndex index = GetTileIndex(destPos, isLeft);
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
					{
						destPos.y = oy;
					}
				}
			}
			else if(destPos.y < oy)
			{
				if((row + 1 >= 1) && (row+1 <= 10))
				{
					if(mp.m_MAPLeft.m_tiles[row + 1, col].dir.up != 1)
					{
						destPos.y = oy;
					}
				}
			}
			if(destPos.x > ox)
			{
				if((col + 1 >= 1) && (col + 1 <= 9))
				{
					if(mp.m_MAPLeft.m_tiles[row, col + 1].dir.left != 1)
					{
						destPos.x = ox;
					}
				}
			}
			else if(destPos.x < ox)
			{
				if((col - 1 >= 0) && (col - 1 <= 8))
				{
					if(mp.m_MAPLeft.m_tiles[row, col - 1].dir.right != 1)
					{
						destPos.x = ox;
					}
				}
			}
		}
		else
		{
			int row = index.m_x;
			int col = index.m_y;
			
			int width = mp.m_MAPRight.m_tiles[row,col].m_width;
			int height = mp.m_MAPRight.m_tiles[row,col].m_height;
			float ox = mp.m_MAPRight.m_tiles[row,col].m_pos.x;
			float oy = mp.m_MAPRight.m_tiles[row,col].m_pos.y;
			//int left = ox - width/2;
			//int right = ox + width/2;
			//int up = oy + height/2;
			//int down = oy - height/2;
			if(destPos.y > oy)
			{
				if((row-1 >= 0) && (row-1 <= 10))
				{
					if(mp.m_MAPRight.m_tiles[row-1, col].dir.down != 1)
					{
						destPos.y = oy;
					}
				}
			}
			else if(destPos.y < oy)
			{
				if((row + 1 >= 1) && (row+1 <= 10))
				{
					if(mp.m_MAPRight.m_tiles[row + 1, col].dir.up != 1)
					{
						destPos.y = oy;
					}
				}
			}
			if(destPos.x > ox)
			{
				if((col + 1 >= 1) && (col + 1 <= 9))
				{
					if(mp.m_MAPRight.m_tiles[row, col + 1].dir.left != 1)
					{
						destPos.x = ox;
					}
				}
			}
			else if(destPos.x < ox)
			{
				if((col - 1 >= 0) && (col - 1 <= 8))
				{
					if(mp.m_MAPRight.m_tiles[row, col - 1].dir.right != 1)
					{
						destPos.x = ox;
					}
				}
			}
		}
		return destPos;
	}

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

					if(pos.x >= left && pos.x <= right)
					{
						if(pos.y >= down && pos.y <= up)
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
					
					if(pos.x >= left && pos.x <= right)
					{
						if(pos.y >= down && pos.y <= up)
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

                    m_playDic[id].transform.FindChild("Icon").localRotation = q;
				}
				else
				{
					Quaternion q = new Quaternion();
					q.eulerAngles = new Vector3(0,180,0);

                    m_playDic[id].transform.FindChild("Icon").localRotation = q;
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
//					//p = UpdatePlayerPos(p,isLeft);
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
				if(!m_playDic[keyPair.Key].isKnocked && !m_playDic[keyPair.Key].isPulling)
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
						m_playDic[keyPair.Key].transform.localPosition = p;
						ChangeSprite(isLeft, keyPair.Key);
					}
					else
					{
						p = UpdatePlayerPos(p,isLeft);
						ChangeSprite(isLeft, keyPair.Key);
						m_playDic[keyPair.Key].transform.localPosition = p;
					}
				}
			}
		}
	}
	
	public float moveAnim = 0f;
	
	public void ChangeSprite(bool isLeft, string id)
	{
		moveAnim += Time.deltaTime;
		if(moveAnim > 0.5f)
		{
			moveAnim = 0;
			if(m_playDic.ContainsKey(id))
			{
				if(isLeft)
				{
					if(m_playDic[id].hasSeed)
					{
						if(m_playDic[id].m_icon.spriteName == "LightSprite1_catchSeed")
						{
							m_playDic[id].m_icon.spriteName = "LightSprite2_catchSeed";
						}
						else
						{
							m_playDic[id].m_icon.spriteName = "LightSprite1_catchSeed";
						}
					}
					else
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
				}
				else
				{
					if(m_playDic[id].hasSeed)
					{
						if(m_playDic[id].m_icon.spriteName == "DarkSprite1_catchSeed")
						{
							m_playDic[id].m_icon.spriteName = "DarkSprite2_catchSeed";
						}
						else
						{
							m_playDic[id].m_icon.spriteName = "DarkSprite1_catchSeed";
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
		}
	}

	public void GetButtonInfo(string btn, NetworkMessageInfo msg,string id)
	{
		if(m_playDic.ContainsKey(id))
		{
			if(m_playDic[id].isKnocked || m_playDic[id].isPulling) return;
		}
		else
		{
			return;
		}
		if (btn == "A")
		{
			//pull root, plant
			if(m_playDic.ContainsKey(id))
			{
				if(m_playDic[id].hasSeed)
				{
					Plant(id);
				}
				else
				{
					PullRoot(id);
				}
			}
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
					Vector3 p;
					int width;
					int height;
					if(isLeft)
					{
						p = mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.m_pos;
						width = mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_width;
						height = mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_height;
					}
					else
					{
						p = mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_opponentTile.m_pos;
						width = mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_width;
						height = mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_height;
					}

					foreach(var play in m_playDic)
					{
						float left = p.x - (width + width/2);
						float right = p.x + (width + width/2);
						float up = p.y + (height + height/2);
						float down = p.y - (height + height/2);
						Vector3 tempPos = play.Value.transform.localPosition;
						if(isLeft)
						{
							int a = index.m_x;
							int b = index.m_y;


							TileInfo tf = mp.rightTiles[mp.m_MAPLeft.m_tiles[a, b].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
							tf.knockSprite.gameObject.SetActive(true);
							tf.isKnocked = true;
							tf.cdTime = 1;

							if((a-1) >= 0)
							{
								tf = mp.rightTiles[mp.m_MAPLeft.m_tiles[a-1, b].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
								tf.knockSprite.gameObject.SetActive(true);
								tf.isKnocked = true;
								tf.cdTime = 1;
							}

							if((b-1) >= 0)
							{
								tf = mp.rightTiles[mp.m_MAPLeft.m_tiles[a, b-1].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
								tf.knockSprite.gameObject.SetActive(true);
								tf.isKnocked = true;
								tf.cdTime = 1;
							}

							if((a+1) <= 10)
							{
								tf = mp.rightTiles[mp.m_MAPLeft.m_tiles[a+1, b].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
								tf.knockSprite.gameObject.SetActive(true);
								tf.isKnocked = true;
								tf.cdTime = 1;
							}

							if((b+1) <= 9)
							{
								tf = mp.rightTiles[mp.m_MAPLeft.m_tiles[a, b+1].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
								tf.knockSprite.gameObject.SetActive(true);
								tf.isKnocked = true;
								tf.cdTime = 1;
							}

							m_playDic[id].m_icon.spriteName = "LightSprite_skill";
							m_playDic[id].canKnockOther = false;
							m_playDic[id].cdTime = 1;
						}
						else
						{
							int a = index.m_x;
							int b = index.m_y;

							TileInfo tf = mp.leftTiles[mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
							tf.knockSprite.gameObject.SetActive(true);
							tf.isKnocked = true;
							tf.cdTime = 1;

							if((a-1) >= 0)
							{
								tf = mp.leftTiles[mp.m_MAPRight.m_tiles[a-1, b].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
								tf.knockSprite.gameObject.SetActive(true);
								tf.isKnocked = true;
								tf.cdTime = 1;
							}
							
							if((b-1) >= 0)
							{
								tf = mp.leftTiles[mp.m_MAPRight.m_tiles[a, b-1].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
								tf.knockSprite.gameObject.SetActive(true);
								tf.isKnocked = true;
								tf.cdTime = 1;
							}
							
							if((a+1) <= 10)
							{
								tf = mp.leftTiles[mp.m_MAPRight.m_tiles[a+1, b].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
								tf.knockSprite.gameObject.SetActive(true);
								tf.isKnocked = true;
								tf.cdTime = 1;
							}
							
							if((b+1) <= 9)
							{
								tf = mp.leftTiles[mp.m_MAPRight.m_tiles[a, b+1].m_opponentTile.m_index].transform.GetComponent<TileInfo>();
								tf.knockSprite.gameObject.SetActive(true);
								tf.isKnocked = true;
								tf.cdTime = 1;
							}
							
							m_playDic[id].m_icon.spriteName = "LightSprite_skill";
							m_playDic[id].canKnockOther = false;
							m_playDic[id].cdTime = 1;
							m_playDic[id].m_icon.spriteName = "DarkSprite_skill";
							m_playDic[id].canKnockOther = false;
							m_playDic[id].cdTime = 1;
						}
						if(tempPos.x >= left && tempPos.x <= right && tempPos.y <= up && tempPos.y >= down)
						{
							play.Value.isKnocked = true;
							play.Value.knockTime = 0.5f;
//							if(isLeft)
//							{
//								//play.Value.m_icon.spriteName = "LightSprite_skill";
//								//play.Value.m_icon.MakePixelPerfect();
//							}
//							else
//							{
//								//play.Value.m_icon.spriteName = "DarkSprite_skill";
//								//play.Value.m_icon.MakePixelPerfect();
//							}
						}
					}
				}
			}
		}
	}
	
	public void PullRoot(string id)
	{
		bool isLeft = false;
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
				int width = mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_width;
				int height = mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_height;
				Tile tTile = new Tile();
				if(isLeft)
				{
					tTile = mp.m_MAPLeft.m_tiles[index.m_x, index.m_y];
				}
				else
				{
					tTile = mp.m_MAPRight.m_tiles[index.m_x, index.m_y];
				}
				if(tTile.isRooted)
				{
					m_playDic[id].isPulling = true;
					m_playDic[id].pullingTime = 4;
					if(isLeft)
					{
						m_playDic[id].m_icon.spriteName = "LightSprite_draw";
					}
					else
					{
						m_playDic[id].m_icon.spriteName = "DarkSprite_draw";
					}
				}
				/* foreach(var play in m_playDic)
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
				} */
			}
		}
	}
	
	public void Pulling()
	{
		foreach(var play in m_playDic)
		{
			if(play.Value.isPulling)
			{
				play.Value.pullingTime -= Time.deltaTime;
				if(play.Value.pullingTime <= 0)
				{
					play.Value.pullingTime = 0;
					play.Value.isPulling = false;
					TileIndex index = GetTileIndex(play.Value.transform.localPosition, play.Value.isLeft);
					if(play.Value.isLeft)
					{
						play.Value.m_icon.spriteName = "LightSprite1_catchSeed";
						play.Value.hasSeed = true;
						mp.leftTiles[index].GetComponent<TileInfo>().tileFlower.gameObject.SetActive(false);
						mp.rightTiles[mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.m_index].GetComponent<TileInfo>().tileFlower.gameObject.SetActive(false);
						if(mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.canAward)
						{
							//--mp.m_MAPRight.m_score;
							mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.canAward = false;
						}
						mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].isRooted = false;
						mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.isPlanted = false;
						
					}
					else
					{
						play.Value.m_icon.spriteName = "DarkSprite1_catchSeed";
						play.Value.hasSeed = true;
						mp.rightTiles[index].GetComponent<TileInfo>().tileFlower.gameObject.SetActive(false);
						mp.leftTiles[mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_opponentTile.m_index].GetComponent<TileInfo>().tileFlower.gameObject.SetActive(false);
						if(mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_opponentTile.canAward)
						{
							//--mp.m_MAPLeft.m_score;
							mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.canAward = false;
						}
						mp.m_MAPRight.m_tiles[index.m_x, index.m_y].isRooted = false;
						mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_opponentTile.isPlanted = false;
					}
				}
			}
			else
			{
				play.Value.pullingTime = 0;
			}
		}
	}

	public void Plant(string id)
	{
		bool isLeft = false;
		//kick ground
		if(m_playDic.ContainsKey(id))
		{
			if(m_playDic[id].hasSeed)
			{
				m_playDic[id].hasSeed = false;
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
					Vector3 p;
					int width;
					int height;
					if(isLeft)
					{
						if(mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].isPlanted != true &&
						   mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].isRooted != true)
						{
							mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].isPlanted = true;
							mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.isRooted = true;
							TileInfo tf = mp.leftTiles[mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_index].GetComponent<TileInfo>();
							tf.tileFlower.spriteName = "LightSeed";
							tf.tileFlower.gameObject.SetActive(true);
							tf.tileFlower.MakePixelPerfect();
							tf.hasFlower = false;
							tf.hasSeed = true;
							tf.isGrowing = true;
							tf.growTime = 10;
							tf = mp.rightTiles[mp.m_MAPLeft.m_tiles[index.m_x, index.m_y].m_opponentTile.m_index].GetComponent<TileInfo>();
							tf.tileFlower.spriteName = "LightRoot";
							tf.tileFlower.gameObject.SetActive(true);
							tf.tileFlower.MakePixelPerfect();
						}
					}
					else
					{
						if(mp.m_MAPRight.m_tiles[index.m_x, index.m_y].isPlanted != true &&
						   mp.m_MAPRight.m_tiles[index.m_x, index.m_y].isRooted != true)
						{
							mp.m_MAPRight.m_tiles[index.m_x, index.m_y].isPlanted = true;
							mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_opponentTile.isRooted  = true;

							TileInfo tf = mp.rightTiles[mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_index].GetComponent<TileInfo>();
							tf.tileFlower.spriteName = "DarkSeed";
							tf.tileFlower.MakePixelPerfect();
							tf.tileFlower.gameObject.SetActive(true);
							tf.hasFlower = false;
							tf.hasSeed = true;
							tf.isGrowing = true;
							tf.growTime = 10;
							tf = mp.leftTiles[mp.m_MAPRight.m_tiles[index.m_x, index.m_y].m_opponentTile.m_index].GetComponent<TileInfo>();
							tf.tileFlower.spriteName = "DarkRoot";
							tf.tileFlower.MakePixelPerfect();
							tf.tileFlower.gameObject.SetActive(true);
						}
					}
				}
			}
		}
	}

	public void CheckScore()
	{
		TileIndex index = new TileIndex();
		mp.m_MAPLeft.m_score = 0;
		mp.m_MAPRight.m_score = 0;
		for(int i = 0; i < mp.MAX_ROW; i++)
		{
			for(int j = 0; j < mp.MAX_COL; j++)
			{
				if(mp.leftTiles[mp.m_MAPLeft.m_tiles[i,j].m_index].GetComponent<TileInfo>().tileFlower.spriteName == "LightFlower")
				{
					mp.m_MAPLeft.m_score++;
				}
				if(mp.rightTiles[mp.m_MAPRight.m_tiles[i,j].m_index].GetComponent<TileInfo>().tileFlower.spriteName == "DarkFlower")
				{
					mp.m_MAPRight.m_score++;
				}
			}
		}
		index.m_x = -1;
		index.m_y = -1;
		leftScore.text = mp.m_MAPLeft.m_score.ToString();
		rightScore.text = mp.m_MAPRight.m_score.ToString();
//		if (mp.m_MAPLeft.m_score >= MessageCenter.Instance.mTeam1Num * 10 + MessageCenter.Instance.mTeam2Num * 3) 
//		{
//			Application.LoadLevel("LightWin");		
//		}
//		else if(mp.m_MAPRight.m_score >= MessageCenter.Instance.mTeam2Num * 10 + MessageCenter.Instance.mTeam1Num * 3)
//		{
//			Application.LoadLevel("DarkWin");
//		}
	}
}
