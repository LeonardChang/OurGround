using UnityEngine;
using System.Collections.Generic;

public class ResMgr
{
    private static ResMgr mInstance = null;

    private static object mSyncRoot = new System.Object();

    public static ResMgr Instance
    {
        get
        {
            if (mInstance == null)
            {
                lock (mSyncRoot)
                {
                    if (mInstance == null)
                    {
                        mInstance = new ResMgr();
                    }
                }
            }
            return mInstance;
        }
    }
    
    Dictionary<string, Object> mCacheList = new Dictionary<string, Object>();

    /// <summary>
    /// 载入一个资源
    /// 资源必须是UnityEngine.Object或其子类型
    /// </summary>
    /// <param name="_path">资源路径</param>
    /// <returns></returns>
    public T Load<T>(string _path) where T : Object
    {
        if (mCacheList.ContainsKey(_path))
        {
            return mCacheList[_path] as T;
        }

        T obj = Resources.Load(_path, typeof(T)) as T;
        if (!TryStoreIntoCache(_path, obj))
        {
            DebugUtils.Warning("[ResMgr] Don't cache resource: " + _path, DebugUtils.WarningLevel.Low);
        }

        return obj;
    }

    bool TryStoreIntoCache(string _path, Object _object)
    {
        if (_object == null || !QualitySetting.HighMemorySize)
        {
            return false;
        }

        mCacheList[_path] = _object;
        return true;
    }

    /// <summary>
    /// 内存清理
    /// </summary>
    public void ClearCache()
    {
        mCacheList.Clear();
    }
}
