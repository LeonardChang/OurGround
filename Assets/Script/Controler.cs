using UnityEngine;
using System.Collections;

public class Controler : MonoBehaviour {
    public UILabel PlayerNameLabel;
    public GameObject TouchPad;

    public GameObject Quan;
    public GameObject Cross;
    public Camera MainCamera;

    public LineRenderer Line;

    Vector3 mVec = new Vector3(0, 0, -0.1f);

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
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
            Vector2 newpos = Input.GetTouch(mTouchID).position;
#else
            Vector2 newpos = Input.mousePosition;
#endif


            Vector3 pos = GetCurrentTouchPosition(newpos);
            Quan.transform.localPosition = pos;
            Line.SetPosition(1, GetCurrentTouchPosition(newpos, Space.World) + mVec);

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
            Line.gameObject.SetActive(false);
        }
        else
        {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
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

            mStartPosition = Input.GetTouch(mTouchID).position;
#else
            mStartPosition = Input.mousePosition;
#endif

            MessageCenter.Instance.SendJoystickControl(true, Vector2.zero);

            Quan.SetActive(true);
            Cross.SetActive(true);
            Line.gameObject.SetActive(true);

            Vector3 pos = GetCurrentTouchPosition(mStartPosition);

            Quan.transform.localPosition = pos;
            Cross.transform.localPosition = pos;

            Line.SetPosition(0, GetCurrentTouchPosition(mStartPosition, Space.World) + mVec);
            Line.SetPosition(1, GetCurrentTouchPosition(mStartPosition, Space.World) + mVec);
        }
    }

    Vector3 GetCurrentTouchPosition(Vector3 _mouse, Space _space = Space.Self)
    {
        Ray ray = MainCamera.ScreenPointToRay(_mouse);
        RaycastHit hit = new RaycastHit();
        if (TouchPad.collider.Raycast(ray, out hit, float.MaxValue))
        {
            if (_space == Space.Self)
            {
                return TouchPad.transform.InverseTransformPoint(hit.point);
            }
            else
            {
                return hit.point;
            }
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
