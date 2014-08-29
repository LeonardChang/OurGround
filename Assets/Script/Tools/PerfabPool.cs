using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 预设对象池
/// </summary>
public class PerfabPool : MonoBehaviour
{
    private static PerfabPool mInstance = null;
    public static PerfabPool Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = Object.FindObjectOfType(typeof(PerfabPool)) as PerfabPool;

                if (mInstance == null)
                {
                    Object prefab = ResMgr.Instance.Load<Object>("Prefabs/PerfabPool");
                    GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

                    mInstance = obj.GetComponent<PerfabPool>();
                }
            }
            return mInstance;
        }
    }

    private class PoolObject
    {
        private bool mActive = false;
        private GameObject mObj = null;

        public bool Active
        {
            get { return mActive; }
            set { mActive = value; }
        }

        public GameObject Obj
        {
            get { return mObj; }
            set { mObj = value; }
        }
    }

    private class PoolSetting
    {
        private string mPerfabName = "";
        private Object mPerfab = null;
        private int mPoolSize = 30;

        public string PerfabName
        {
            get { return mPerfabName; }
            set { mPerfabName = value; }
        }

        public Object Perfab
        {
            get { return mPerfab; }
            set { mPerfab = value; }
        }

        public int PoolSize
        {
            get { return mPoolSize; }
            set { mPoolSize = value; }
        }
    }

    private Dictionary<string, List<PoolObject>> mPools = new Dictionary<string, List<PoolObject>>();
    private Dictionary<string, PoolSetting> mPoolSettings = new Dictionary<string, PoolSetting>();

    Dictionary<int, Transform> mLayerNodes = new Dictionary<int, Transform>();

    public int PoolDefaultSize = 30;

    class ObjectLink
    {
        public string PoolName;
        public GameObject Object;
        public float Delay;

        public ObjectLink(GameObject _obj, string _pool, float _delay)
        {
            PoolName = _pool;
            Object = _obj;
            Delay = _delay;
        }
    }

    List<ObjectLink> mDelayDeleteList = new List<ObjectLink>();

    void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            mInstance = this;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mDelayDeleteList.Count; i++ )
        {
            ObjectLink obj = mDelayDeleteList[i];
            obj.Delay -= Time.deltaTime;

            if (obj.Delay <= 0)
            {
                DestoryObject(obj.Object, obj.PoolName);
                mDelayDeleteList.RemoveAt(i);
                i -= 1;
            }
        }
    }

    void LateUpdate()
    {
        //foreach (GameObject obj in mNeedUpdateWidgetList)
        //{
        //    //print("out --> " + obj.name);
            
        //    obj.layer = obj.transform.parent.gameObject.layer;

        //    obj.SetActive(false);
        //    obj.SetActive(true);
        //}

        //mNeedUpdateWidgetList.Clear();
    }

    /// <summary>
    /// 从某个池创建一个perfab
    /// </summary>
    /// <param name="_perfab"></param>
    /// <param name="_poolName"></param>
    /// <returns></returns>
    public GameObject CreateObject(string _perfab, string _poolName)
    {
        if (!mPools.ContainsKey(_poolName))
        {
            mPools[_poolName] = new List<PoolObject>();

            PoolSetting setting = new PoolSetting();
            setting.PoolSize = PoolDefaultSize;
            setting.PerfabName = _perfab;
            mPoolSettings[_poolName] = setting;
        }

        return GetFreeObject(_poolName);
    }

    /// <summary>
    /// 创建一个不在池中的perfab
    /// </summary>
    /// <param name="_perfab"></param>
    /// <returns></returns>
    public GameObject CreateObjectWihtoutPool(string _perfab)
    {
        //print(_perfab.Replace("\\", "\\\\"));
        Object perfab = ResMgr.Instance.Load<Object>(_perfab);
        GameObject newObj = Instantiate(perfab, Vector3.zero, Quaternion.identity) as GameObject;

        return newObj;
    }

    /// <summary>
    /// 从某个池销毁一个perfab
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="_poolName"></param>
    public void DestoryObject(GameObject _object, string _poolName, float _time = 0)
    {
        if (_time == 0)
        {
            if (mPools.ContainsKey(_poolName) && !SetFreeObject(_object, _poolName))
            {
                Destroy(_object);
            }
        }
        else
        {
            mDelayDeleteList.Add(new ObjectLink(_object, _poolName, _time));
        }
    }

    /// <summary>
    /// 清空所有的池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var list in mPools.Values)
        {
            foreach (var po in list)
            {
                Destroy(po.Obj);
            }
            list.Clear();
        }
        mPools.Clear();
    }

    private GameObject GetFreeObject(string _poolName)
    {
        foreach (PoolObject poolObject in mPools[_poolName])
        {
            if (!poolObject.Active)
            {
                poolObject.Obj.SetActive(true);
                poolObject.Obj.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
                poolObject.Active = true;

                return poolObject.Obj;
            }
        }

        if (mPoolSettings[_poolName].Perfab == null)
        {
            mPoolSettings[_poolName].Perfab = ResMgr.Instance.Load<Object>(mPoolSettings[_poolName].PerfabName);
        }
        GameObject newObj = Instantiate(mPoolSettings[_poolName].Perfab, Vector3.zero, Quaternion.identity) as GameObject;

        if (mPools[_poolName].Count < mPoolSettings[_poolName].PoolSize)
        {
            PoolObject po = new PoolObject();
            po.Active = true;
            po.Obj = newObj;

            mPools[_poolName].Add(po);
        }
       
        return newObj;
    }

    private bool SetFreeObject(GameObject _object, string _poolName)
    {
        foreach (PoolObject obj in mPools[_poolName])
        {
            if (obj.Active && _object.Equals(obj.Obj))
            {
                int layer = _object.layer;
                if (!mLayerNodes.ContainsKey(layer))
                {
                    GameObject node = new GameObject();
                    node.transform.parent = transform;
                    node.layer = layer;
                    node.name = "_" + LayerMask.LayerToName(layer) + "_node";
                    UIPanel panel = node.AddComponent<UIPanel>();
                    panel.enabled = false;

                    mLayerNodes[layer] = node.transform;
                }

                _object.transform.parent = mLayerNodes[layer];
                _object.SetActive(false);
                obj.Active = false;

                return true;
            }
        }

        return false;
    }
}