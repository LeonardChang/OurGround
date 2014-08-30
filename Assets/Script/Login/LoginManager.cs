using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginManager : MonoBehaviour {
	
	QRProxy mQRProxy = new QRProxy();
	public UITexture mQRCamera;
	public UITexture mQRCode;
	void OnEnable()
	{
		MessageCenter.Instance.ClientConnectServerEvent += OnClientConnectServer;
	}
	
	/* void OnDisable()
	{
		MessageCenter.Instance.ClientConnectServerEvent -= OnClientConnectServer;
	}
	 */
	void Start () {
		ShowScene(0);
		
        MessageCenter mc = MessageCenter.Instance;
	}
	
	float time = 0;
	
	void Update () {
		time += Time.deltaTime;
		if(time >= 0.5f)
		{
			if(cameraOpened)
			{
				ScanQRCode();
			}
			time = 0;
			UpdatePlayerSpirte();
		}
	}

	// --------ChooseScene--------
	
	public GameObject chooseScene;
	public GameObject clientScene;
	public GameObject serverScene;
	
	public UILabel serverAddress;

    public GameObject BGMNode;
	
    Server mServer;
    Client mClient;

    public void ClickServer()
    {
        mServer = Server.Create();
        mServer.CreateServer(ConnectServerFinishCallback);
		ShowScene(1);
		RefreshMenu();
    }

    void ConnectServerFinishCallback(string _result)
    {
        if (_result == Server.CONNECT_SUCCESS)
        {
            serverAddress.text = "IP:" + mServer.IPAddress + " Port:" + mServer.Port.ToString();
			mQRCode.mainTexture = mQRProxy.GetQRCode(mServer.IPAddress.ToString() + ":" + mServer.Port.ToString());
        }
    }

    public void ClickClient()
    {
		Debug.Log("a");
		ShowScene(2);
		#if !UNITY_STANDALONE
		OpenCamera();
		#endif
        Destroy(BGMNode);
    }

	void ShowScene(int _mode)
	{
		switch(_mode)
		{
		case 0:
			chooseScene.SetActive(true);
			serverScene.SetActive(false);
			clientScene.SetActive(false);
			break;
		case 1:
			chooseScene.SetActive(false);
			serverScene.SetActive(true);
			clientScene.SetActive(false);
			break;
		case 2:
			chooseScene.SetActive(false);
			serverScene.SetActive(false);
			clientScene.SetActive(true);
			break;
		}
	}
	
	// --------Server Scene--------
	
	public UILabel targetText1;
	public UILabel targetText2;
	public List<UISprite> team1 = new List<UISprite>();
	public List<UILabel> team1Text = new List<UILabel>();
	public List<UISprite> team2 = new List<UISprite>();
	public List<UILabel> team2Text = new List<UILabel>();
	
	public int playersNum1 = 0;
	public int playersNum2 = 0;
	
	void UpdatePlayerSpirte()
	{
		int i = 0;
		for(i = 0; i < 4;++i)
		{
			if(team1[i].spriteName == "LightSprite1")
			{	
				team1[i].spriteName = "LightSprite2";
			}
			else
			{
				team1[i].spriteName = "LightSprite1";
			}
			if(team2[i].spriteName == "DarkSprite1")
			{	
				team2[i].spriteName = "DarkSprite2";
			}
			else
			{
				team2[i].spriteName = "DarkSprite1";
			}
		}
		
	}
	
	void RefreshMenu()
	{
		targetText1.text = "Target: " + (playersNum1 * 10 + playersNum2 * 3).ToString();
		targetText2.text = "Target: " + (playersNum2 * 10 + playersNum1 * 3).ToString();
	}
	
	void OnClientConnectServer(int _playerID, NetworkPlayer _player)
	{
		if(playersNum1 == playersNum2)
		{
			if(Random.Range(0,1) < 0.5)
			{
				SetTeam(_playerID, 0, _player);
			}
			else
			{
				SetTeam(_playerID, 1, _player);
			}
		}
		else if(playersNum1 < playersNum2)
		{
			SetTeam(_playerID, 0, _player);
		}
		else
		{
			SetTeam(_playerID, 1, _player);
		}
		RefreshMenu();
	}
	
	void SetTeam(int _playerID, int _team, NetworkPlayer _player)
	{
		if(_team == 0)
		{
			++playersNum1;
			MessageCenter.Instance.SetPlayerTeam(_player, _team);
			team1[playersNum1 - 1].gameObject.SetActive(true);
			team1Text[playersNum1 - 1].text = "Player " + _playerID.ToString();
		}
		else
		{
			++playersNum2;
			team2[playersNum2 - 1].gameObject.SetActive(true);
			team2Text[playersNum2 - 1].text = "Player " + _playerID.ToString();
		}
	}
	
	public void ServerClickGameStart()
	{
        if (MessageCenter.Instance.mPlayerTeam.Keys.Count < 2)
        {
            return;
        }
		MessageCenter.Instance.SetTeamNum(playersNum1, playersNum2);
		Application.LoadLevel("MainScene");
	}
	
	
	// --------Client Scene--------
	
	public UIInput ipInputer;
	public UIInput portInputer;
	bool cameraOpened;
	void OpenCamera()
	{
		cameraOpened = true;
		mQRProxy.OpenCamera(mQRCamera);
		mQRCamera.transform.localRotation = Quaternion.AngleAxis(mQRProxy.CAMERA_ANGLE, Vector3.back);
	}
	
	void ScanQRCode()
	{
		if(cameraOpened)
		{
			string tIPAddress = mQRProxy.ScanQRCode();
			if(tIPAddress != null)
			{
				string[] texts = tIPAddress.Split(':');
				ipInputer.value = texts[0];
				portInputer.value = texts[1];
				ClientClickConnect();
			}
		}
	}
	
	public void ClientClickConnect()
	{
		string ip = ipInputer.value;
		string port = portInputer.value;

        if (ip.Split('.').Length != 4)
        {
            return;
        }

        int portID = 0;
        if (!int.TryParse(port, out portID))
        {
            return;
        }

		cameraOpened = false;
#if !UNITY_STANDALONE
		mQRProxy.CloseCamera();
#endif
        mClient = Client.Create();
        mClient.ConnectToServer(ip, portID, ConnectClientFinishCallback);
	}
	
	void ConnectClientFinishCallback(string _result)
    {
        if (_result == Client.CONNECT_SUCCESS)
        {
            Application.LoadLevel("Controller");
        }
    }
}
