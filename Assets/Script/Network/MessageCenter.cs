using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageCenter : MonoBehaviour
{
    public System.Action<string, int> PlayerIDRefreshEvent = null;

    public System.Action<string, NetworkMessageInfo> ClickButtonEvent = null;
    public System.Action<bool, Vector2> JoystickControlEvent = null;

    static MessageCenter mInstance = null;
    NetworkView mView;

    Dictionary<string, NetworkPlayer> mPlayers = new Dictionary<string, NetworkPlayer>();

    List<int> mIDs = new List<int>();
    Dictionary<string, int> mPlayerID = new Dictionary<string, int>();

    public static MessageCenter Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject obj = new GameObject("_MessageCenter");
                mInstance = obj.AddComponent<MessageCenter>();
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
            mPlayerID[player.guid] = playerID;

            networkView.RPC("RefreshPlayerID", RPCMode.Others, player.guid, playerID);
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
        print("RefreshPlayerID " + _guid + " " + _playerID);
        mPlayerID[_guid] = _playerID;
        if (PlayerIDRefreshEvent != null)
        {
            PlayerIDRefreshEvent(_guid, _playerID);
        }
    }

    [RPC]
    void ClickButton(string _btn, NetworkMessageInfo _info)
    {
        print(_info.sender.guid + " ClickButton " + _btn);
        if (ClickButtonEvent != null)
        {
            ClickButtonEvent(_btn, _info);
        }
    }

    [RPC]
    void JoystickControl(bool _down, float _x, float _y, NetworkMessageInfo _info)
    {
        Vector2 dir = new Vector2(_x, _y);
        print(_info.sender.guid + " Joystick " + _down.ToString() + " " + dir.ToString());
        if (JoystickControlEvent != null)
        {
            JoystickControlEvent(_down, dir);
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
        //print("[SendJoysticlControl] " + _down.ToString() + " " + _dir.ToString());
        networkView.RPC("JoystickControl", RPCMode.Server, _down, _dir.x, _dir.y);
    }

    public void SendClickButton(string _btn)
    {
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
