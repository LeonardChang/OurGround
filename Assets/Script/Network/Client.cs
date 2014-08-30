using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public static string CONNECT_SUCCESS = "CONNECT_SUCCESS";
    public static string ERROR_ALREADY_ONLINE = "The player is already online";

    System.Action<string> mFinishEvent = null;

    public string Password = "111111";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    string mIp;
    int mPort;
    string mPassword;

    public void ConnectToServer(string _ip, int _port, System.Action<string> _callback)
    {
        mIp = _ip;
        mPort = _port;
        mPassword = Password;
        mFinishEvent = _callback;

        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            Debug.LogError(ERROR_ALREADY_ONLINE);

            if (mFinishEvent != null)
            {
                mFinishEvent(ERROR_ALREADY_ONLINE);
            }

            return;
        }

        StartCoroutine("StartConnect");
    }

    IEnumerator StartConnect()
    {
        NetworkConnectionError error = Network.Connect(mIp, mPort, mPassword);

        if (error != NetworkConnectionError.NoError)
        {
            Debug.LogError(error.ToString());

            if (mFinishEvent != null)
            {
                mFinishEvent(error.ToString());
            }
        }
        else
        {
            while (Network.peerType == NetworkPeerType.Connecting)
            {
                yield return null;
            }

            if (mFinishEvent != null)
            {
                mFinishEvent(CONNECT_SUCCESS);
            }
        }

        mFinishEvent = null;
    }

    public void Disconnect()
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            Network.Disconnect();
        }
    }

    public static Client Create()
    {
        GameObject obj = new GameObject("_Client");
        DontDestroyOnLoad(obj);
        return obj.AddComponent<Client>();
    }
}
