using UnityEngine;
using System.Collections;

public class Controler : MonoBehaviour {
    public UILabel PlayerNameLabel;
    public GameObject TouchPad;

    public GameObject Quan;
    public GameObject Cross;
    public Camera MainCamera;

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
            Vector2 newpos = Input.GetTouch(mTouchID).position;

            Quan.transform.localPosition = GetCurrentTouchPosition(newpos);

            Vector2 dir = newpos - mStartPosition;
            dir = dir.normalized;
            SendJoystick(true, dir);
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
            SendJoystick(true, (new Vector2(-1, 1)).normalized);
        }
        else if (mPressUp && mPressRight)
        {
            SendJoystick(true, (new Vector2(1, 1)).normalized);
        }
        else if (mPressUp)
        {
            SendJoystick(true, (new Vector2(0, 1)).normalized);
        }
        else if (mPressDown && mPressLeft)
        {
            SendJoystick(true, (new Vector2(-1, -1)).normalized);
        }
        else if (mPressDown && mPressRight)
        {
            SendJoystick(true, (new Vector2(1, -1)).normalized);
        }
        else if (mPressDown)
        {
            SendJoystick(true, (new Vector2(0, -1)).normalized);
        }
        else if (mPressLeft)
        {
            SendJoystick(true, (new Vector2(-1, 0)).normalized);
        }
        else if (mPressRight)
        {
            SendJoystick(true, (new Vector2(1, 0)).normalized);
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
    Vector2 mStartPosition;

    int mTouchID;

    void OnTouchPadPress (GameObject _sender, bool isPressed)
    {
        //print("OnTouchPadPress");
        mIsPressed = isPressed;

        if (!mIsPressed)
        {
            MessageCenter.Instance.SendJoystickControl(false, Vector2.zero);
            mLastDir = new Vector2(-999, -999);

            Quan.SetActive(false);
            Cross.SetActive(false);
        }
        else
        {
            RaycastHit hit = new RaycastHit();
            foreach (var touch in Input.touches)
            {
                Ray ray = MainCamera.ScreenPointToRay(touch.position);
                if (TouchPad.collider.Raycast(ray, out hit, float.MaxValue))
                {
                    mTouchID = touch.fingerId;
                    break;
                }
            }

            //Handheld.Vibrate();
            mStartPosition = Input.GetTouch(mTouchID).position;
            MessageCenter.Instance.SendJoystickControl(true, Vector2.zero);

            Quan.SetActive(true);
            Cross.SetActive(true);

            Quan.transform.localPosition = GetCurrentTouchPosition(mStartPosition);
            Cross.transform.localPosition = GetCurrentTouchPosition(mStartPosition);
        }
    }

    Vector3 GetCurrentTouchPosition(Vector3 _mouse)
    {
        Ray ray = MainCamera.ScreenPointToRay(_mouse);
        RaycastHit hit = new RaycastHit();
        if (TouchPad.collider.Raycast(ray, out hit, float.MaxValue))
        {
            return TouchPad.transform.InverseTransformPoint(hit.point);
        }

        return Vector3.zero;
    }

    Vector2 mLastDir = new Vector2(-999, -999);

    void SendJoystick(bool _down, Vector2 _dir)
    {
        if (_dir == mLastDir)
        {
            return;
        }

        MessageCenter.Instance.SendJoystickControl(_down, _dir * 2f);
        mLastDir = _dir;
    }
}
