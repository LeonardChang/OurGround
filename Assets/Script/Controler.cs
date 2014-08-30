using UnityEngine;
using System.Collections;

public class Controler : MonoBehaviour {
    public UILabel PlayerNameLabel;
    public GameObject TouchPad;

	// Use this for initialization
	void Start () {
        MessageCenter.Instance.PlayerIDRefreshEvent = RefreshPlayerNameCallback;
        UIEventListener.Get(TouchPad).onPress = OnTouchPadPress;

        RefreshPlayerName();
	}

    void OnDestroy()
    {
        MessageCenter.Instance.PlayerIDRefreshEvent = null;
    }
	
	// Update is called once per frame
	void Update () {
        if (mIsPressed)
        {
            Vector2 oldpos = mTouchPosition;
            Vector2 newpos = Input.mousePosition;

            if (Vector2.Distance(oldpos, newpos) >= 10)
            {
                Vector2 dir = newpos - oldpos;
                dir = dir.normalized;
                SendJoystick(true, dir);
            }

            mTouchPosition = newpos;
        }
	}

    void RefreshPlayerNameCallback(string _guid, int _playerID)
    {
        if (MessageCenter.Instance.IsMe(_guid))
        {
            RefreshPlayerName();
        }
    }

    void RefreshPlayerName()
    {
        PlayerNameLabel.text = MessageCenter.Instance.GetPlayerName(MessageCenter.Instance.MyGUID);
    }

    public void ClickButtonA()
    {
        MessageCenter.Instance.SendClickButton("A");
    }

    public void ClickButtonB()
    {
        MessageCenter.Instance.SendClickButton("B");
    }

    bool mIsPressed = false;
    Vector2 mTouchPosition;

    void OnTouchPadPress (GameObject _sender, bool isPressed)
    {
        //print("OnTouchPadPress");
        mIsPressed = isPressed;
        mTouchPosition = Input.mousePosition;

        if (!mIsPressed)
        {
            MessageCenter.Instance.SendJoystickControl(false, Vector2.zero);
            mLastDir = new Vector2(-999, -999);
        }
        else
        {
            MessageCenter.Instance.SendJoystickControl(true, Vector2.zero);
        }
    }

    Vector2 mLastDir = new Vector2(-999, -999);

    void SendJoystick(bool _down, Vector2 _dir)
    {
        if (_dir == mLastDir)
        {
            return;
        }

        MessageCenter.Instance.SendJoystickControl(_down, _dir);
        mLastDir = _dir;
    }
}
