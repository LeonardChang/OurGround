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

        if (Input.GetKeyUp(KeyCode.Z))
        {
            ClickButtonA();
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            ClickButtonB();
        }

        bool isAnyPress = false;
        if (mPressUp || mPressDown || mPressLeft || mPressRight)
        {
            isAnyPress = true;
        }

        if (!mPressUp && Input.GetKeyDown(KeyCode.UpArrow))
        {
            mPressUp = true;
        }
        if (mPressUp && Input.GetKeyUp(KeyCode.UpArrow))
        {
            mPressUp = false;
        }

        if (!mPressDown && Input.GetKeyDown(KeyCode.DownArrow))
        {
            mPressDown = true;
        }
        if (mPressDown && Input.GetKeyUp(KeyCode.DownArrow))
        {
            mPressDown = false;
        }

        if (!mPressLeft && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            mPressLeft = true;
        }
        if (mPressLeft && Input.GetKeyUp(KeyCode.LeftArrow))
        {
            mPressLeft = false;
        }

        if (!mPressRight && Input.GetKeyDown(KeyCode.RightArrow))
        {
            mPressRight = true;
        }
        if (mPressRight && Input.GetKeyUp(KeyCode.RightArrow))
        {
            mPressRight = false;
        }

        if (mPressUp && mPressLeft)
        {
            SendJoystick(false, (new Vector2(-1, 1)).normalized);
        }
        else if (mPressUp && mPressRight)
        {
            SendJoystick(false, (new Vector2(1, 1)).normalized);
        }
        else if (mPressUp)
        {
            SendJoystick(false, (new Vector2(0, 1)).normalized);
        }
        else if (mPressDown && mPressLeft)
        {
            SendJoystick(false, (new Vector2(-1, -1)).normalized);
        }
        else if (mPressDown && mPressRight)
        {
            SendJoystick(false, (new Vector2(1, -1)).normalized);
        }
        else if (mPressDown)
        {
            SendJoystick(false, (new Vector2(0, -1)).normalized);
        }
        else if (mPressLeft)
        {
            SendJoystick(false, (new Vector2(-1, 0)).normalized);
        }
        else if (mPressRight)
        {
            SendJoystick(false, (new Vector2(1, 0)).normalized);
        }
        else if (isAnyPress)
        {
            MessageCenter.Instance.SendJoystickControl(false, Vector2.zero);
        }
	}

    bool mPressUp = false;
    bool mPressDown = false;
    bool mPressLeft = false;
    bool mPressRight = false;

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
        //Handheld.Vibrate();
        MessageCenter.Instance.SendClickButton("A");
    }

    public void ClickButtonB()
    {
        //Handheld.Vibrate();
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
            //Handheld.Vibrate();
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
