using UnityEngine;
using System.Collections;

public class AutoScaleCenter : MonoBehaviour
{
	private static AutoScaleCenter mInstance = null;
	public static AutoScaleCenter Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = Object.FindObjectOfType(typeof(AutoScaleCenter)) as AutoScaleCenter;

				if (mInstance == null)
				{
                    mInstance = PerfabPool.Instance.CreateObjectWihtoutPool("Prefabs/AutoScale").GetComponent<AutoScaleCenter>();
				}
			}
			return mInstance;
		}
	}
	
    public int DefaultWidth = 640;
	public int DefaultHeight = 960;
	
    ScreenOrientation mScreenOrientation;
    int mWidth;
    int mHeight;

    bool mNeedReset = true;

    /// <summary>
    /// getter fot the screen logic height 
    /// </summary>
    public int SCREEN_LOGIC_HEIDHT { get; set; }
    public int SCREEN_LOGIC_WIDTH { get; set; }
	
	/// <summary>
    /// Let the scale center reset all UIRoots object to fix current screen
    /// </summary>
    public bool NeedReset
    {
        get { return mNeedReset; }
        set { mNeedReset = value; }
    }
	
	/// <summary>
    /// Let the scale center reset all UIRoots object to fix current screen
    /// </summary>
	public void ResetAllNow()
	{
		NeedReset = true;
	}
	
	/// <summary>
    /// The default screen ratio
    /// </summary>
	float DefaultScreenRatio
	{
		get
		{
			return (float)DefaultWidth / (float)DefaultHeight;
		}
	}

    void Awake()
    {
		if (mInstance == null)
		{
			mInstance = this;
		}
		
        DontDestroyOnLoad(gameObject);
        NeedReset = true;
		
		InvokeRepeating("TestScreenChange", 0.5f, 0.5f);
		
		if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            mInstance = this;
            SCREEN_LOGIC_HEIDHT = 960;
            SCREEN_LOGIC_WIDTH = 640;
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (NeedReset)
        {
            Reset();
            NeedReset = false;
        }
    }

    void LateUpdate()
    {
        
    }
	
	/// <summary>
    /// Test if the screen's status has changed
    /// </summary>
	void TestScreenChange()
	{
		if (mScreenOrientation != Screen.orientation
            || mWidth != Screen.width
            || mHeight != Screen.height)
        {
            NeedReset = true;
        }
	}

    /// <summary>
    /// Reset all object to current scale
    /// </summary>
    void Reset()
    {
        mWidth = Screen.width;
        mHeight = Screen.height;
        mScreenOrientation = Screen.orientation;

        // Reset all UIRoot for NGUI.
        foreach (UIRoot root in UIRoot.list)
        {
			ResetNGUIRoot(root);
        }
    }

    /// <summary>
    /// Reset all UIRoots for NGUI.
    /// </summary>
    /// <param name="_root"></param>
    void ResetNGUIRoot(UIRoot _root)
    {
		int val = DefaultHeight;
		int s_width = Screen.width;
		int s_height = Screen.height;
		
        if ((float)s_width / (float)s_height < DefaultScreenRatio)
        {
            val = DefaultWidth * s_height / s_width;
        }
        else
        {
            val = DefaultHeight;
        }
		
		_root.manualHeight = val;
        _root.maximumHeight = val;
        _root.minimumHeight = val;

        SCREEN_LOGIC_HEIDHT = val;

        // calculate the screen width in logic 
        val = DefaultWidth;

        if( (float)s_width / (float)s_height > DefaultScreenRatio )
        {
            val = DefaultHeight * s_width / s_height;
        }
        else
        {
            val = DefaultWidth;
        }

        SCREEN_LOGIC_WIDTH = val;
        
    }
}
