using UnityEngine;
using System.Collections;

public class TestNetwork : MonoBehaviour {
    public UIInput Inputer;
    public UILabel Label;

	// Use this for initialization
	void Start () {
        MessageCenter mc = MessageCenter.Instance;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    Server mServer;
    Client mClient;

    public void ClickServer()
    {
        mServer = Server.Create();
        mServer.CreateServer(ConnectServerFinishCallback);
    }

    void ConnectServerFinishCallback(string _result)
    {
        if (_result == Server.CONNECT_SUCCESS)
        {
            Label.text = mServer.IPAddress + ":" + mServer.Port.ToString();
        }
    }

    public void ClickClient()
    {
        string text = Inputer.value;
        string[] texts = text.Split(':');

        mClient = Client.Create();
        mClient.ConnectToServer(texts[0], int.Parse(texts[1]), ConnectClientFinishCallback);
    }

    void ConnectClientFinishCallback(string _result)
    {
        if (_result == Client.CONNECT_SUCCESS)
        {
            Label.text = "OK";
        }
    }
}
