using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageCenter : MonoBehaviour
{
    public System.Action<string, int> PlayerIDRefreshEvent = null;

    public System.Action<string, NetworkMessageInfo, string> ClickButtonEvent = null;
    public System.Action<bool, Vector2, string> JoystickControlEvent = null;

	public System.Action<int, NetworkPlayer> ClientConnectServerEvent = null;

    static MessageCenter mInstance = null;
    NetworkView mView;

    public Dictionary<string, NetworkPlayer> mPlayers = new Dictionary<string, NetworkPlayer>();

    List<int> mIDs = new List<int>();
    public Dictionary<string, int> mPlayerID = new Dictionary<string, int>();
	public Dictionary<string, int> mPlayerTeam = new Dictionary<string, int>();
	
	public int mTeam1Num = 0;
	public int mTeam2Num = 0;
	
    public static MessageCenter Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = Object.FindObjectOfType(typeof(MessageCenter)) as MessageCenter;

                if (mInstance == null)
                {
                    GameObject obj = new GameObject("_MessageCenter");
                    mInstance = obj.AddComponent<MessageCenter>();
                }
            }
            return mInstance;
        }
    }

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
            mView = gameObject.AddComponent<NetworkView>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy() 
    { 
        if (mInstance == this) mInstance = null;
    }

    void OnEnable() 
    { 
        if (mInstance == null) mInstance = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Network.peerType == NetworkPeerType.Client && !IsInvoking("SendHeartBeat"))
        {
            InvokeRepeating("SendHeartBeat", 0, 2);
        }
	}
	
	public void SetPlayerTeam(NetworkPlayer _player, int _team)
	{
		if (!mPlayerTeam.ContainsKey(_player.guid))
        {
            mPlayerTeam[_player.guid] = _team;
        }
	}
	
    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log(player.ipAddress + " connected");
        if (!mPlayers.ContainsKey(player.guid))
        {
            mPlayers[player.guid] = player;
        }
        if (Network.isServer)
        {
            int playerID = NewTempID;
            mIDs.Add(playerID);
            mPlayerID[player.guid] = playerID;
            networkView.RPC("RefreshPlayerID", RPCMode.All, player.guid, playerID);
			if(ClientConnectServerEvent != null)
			{
				ClientConnectServerEvent(mPlayerID[player.guid], player);
			}
        }
    }
	
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log(player.ipAddress + " disconnected");
        if (mPlayers.ContainsKey(player.guid))
        {
            mPlayers.Remove(player.guid);
        }

        if (Network.isServer && mPlayerID.ContainsKey(player.guid))
        {
            mIDs.Remove(mPlayerID[player.guid]);
        }

        if (mPlayerID.ContainsKey(player.guid))
        {
            mPlayerID.Remove(player.guid);
        }
    }
	
	public void SetTeamNum(int _Team1Num, int _Team2Num)
	{
		mTeam1Num = _Team1Num;
		mTeam2Num = _Team2Num;
	}

    int NewTempID
    {
        get
        {
            for (int i = 1; i < 999; i++)
            {
                if (!mIDs.Contains(i))
                {
                    return i;
                }
            }

            return 0;
        }
    }

    #region PRC

    [RPC]
    void RefreshPlayerID(string _guid, int _playerID, NetworkMessageInfo info)
    {
        //print("RefreshPlayerID " + _guid + " " + _playerID);
        mPlayerID[_guid] = _playerID;
        if (PlayerIDRefreshEvent != null)
        {
            PlayerIDRefreshEvent(_guid, _playerID);
        }
    }

    [RPC]
    void ClickButton(string _btn, NetworkMessageInfo _info)
    {
        //print(_info.sender.guid + " ClickButton " + _btn);
        if (ClickButtonEvent != null)
        {
            ClickButtonEvent(_btn, _info, _info.sender.guid);
        }
    }

    [RPC]
    void JoystickControl(bool _down, float _x, float _y, NetworkMessageInfo _info)
    {
        Vector2 dir = new Vector2(_x, _y);
        //print(_info.sender.guid + " Joystick " + _down.ToString() + " " + dir.ToString());
        if (JoystickControlEvent != null)
        {
            JoystickControlEvent(_down, dir, _info.sender.guid);
        }
    }

    [RPC]
    void HeartBeat(NetworkMessageInfo _info)
    {
        //print(GetPlayerName(_info.sender.guid) + "heart beat once.");
    }

    #endregion

    public string GetPlayerName(string _guid)
    {
        return mPlayerID.ContainsKey(_guid) ? (mPlayerID[_guid].ToString() + "P") : _guid;
    }

    public bool IsMe(string _guid)
    {
        if (!mPlayers.ContainsKey(_guid))
        {
            return false;
        }
        return mPlayers[_guid] == Network.player;
    }

    public string MyGUID
    {
        get
        {
            return Network.player.guid;
        }
    }

    #region message

    public void SendJoystickControl(bool _down, Vector2 _dir)
    {
        if (!Network.isClient)
        {
            return;
        }
        //print("[SendJoysticlControl] " + _down.ToString() + " " + _dir.ToString());
        networkView.RPC("JoystickControl", RPCMode.Server, _down, _dir.x, _dir.y);
    }

    public void SendClickButton(string _btn)
    {
        if (!Network.isClient)
        {
            return;
        }
        //print("[SendClickButton] " + _btn);
        networkView.RPC("ClickButton", RPCMode.Server, _btn);
    }

    void SendHeartBeat()
    {
        if (Network.isClient)
        {
            //print("SendHeartBeat");
            networkView.RPC("HeartBeat", RPCMode.Server);
        }
    }

    #endregion
}
