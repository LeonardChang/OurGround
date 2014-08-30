using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class Server : MonoBehaviour
{
    public static string CONNECT_SUCCESS = "CONNECT_SUCCESS";
    public static string ERROR_ALREADY_ONLINE = "The player is already online";

    public int MaxConnection = 32;
    public int ListenPort = 25000;
    public string Password = "111111";

    System.Action<string> mFinishEvent = null;

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

    public void CreateServer(System.Action<string> _callback)
    {
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

        StartCoroutine("StartCreateServer");
    }

    IEnumerator StartCreateServer()
    {
        Network.incomingPassword = Password;
        bool useNat = false; // !Network.HavePublicAddress();
        NetworkConnectionError error = Network.InitializeServer(MaxConnection, ListenPort, useNat);

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

    public string IPAddress
    {
        get
        {
            string hostName = Dns.GetHostName();
            IPHostEntry entry = Dns.GetHostEntry(hostName);
            IPAddress ip = new IPAddress(entry.AddressList[0].Address);
            return ip.ToString();
        }
    }

    public int Port
    {
        get
        { 
            return ListenPort;
        }
    }

    public void Disconnect()
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            Network.Disconnect();
        }
    }

    public static Server Create()
    {
        GameObject obj = new GameObject("_Server");
        DontDestroyOnLoad(obj);
        return obj.AddComponent<Server>();
    }
}
