using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public GameObject leftPanel;
	public GameObject rightPanel;
	public Dictionary<string, PlayerInfo> m_playDic = new Dictionary<string, PlayerInfo>();
	public Map m_mapLeft;
	public Map m_mapRight;

	protected List<string> playID = new List<string>();

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
		
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
					SetPlayerPos(o,false);
					m_playDic.Add(playID[i], o.GetComponent<PlayerInfo>());
				}
			}
		}
	}

	public void GetMapData(Map map)
	{

	}

	public void SetPlayerPos(GameObject o, bool left)
	{
		if(left)
		{
			int row = Random.Range(6, 11);
			int col = Random.Range(0, 10);
			o.transform.localPosition = (Vector3)(MapController.Instance.m_MAPLeft.m_tiles[row,col].m_pos);
		}
		else
		{
			int row = Random.Range(0, 5);
			int col = Random.Range(0, 10);
			o.transform.localPosition = (Vector3)(MapController.Instance.m_MAPRight.m_tiles[row,col].m_pos);
		}
	}
}
