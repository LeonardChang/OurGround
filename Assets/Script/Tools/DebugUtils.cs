using System;

/// <summary>
/// 该类所有方法仅在Editor中有效
/// </summary>
public class DebugUtils
{
    public enum WarningLevel
    {
        VeryLow = 0,
        Low = 1,
        Normal = 2,
        High = 3,
        VeryHigh = 4,
    }

    // 当前警告级别
    static WarningLevel mWarningLevel = WarningLevel.Normal;

    /// <summary>
    /// 断言
    /// 抛出Exception
    /// </summary>
    /// <param name="_condition"></param>
    /// <param name="_info">信息</param>
    /// <returns>_condition</returns>
    public static bool Assert(bool _condition, string _info = null)
    {
#if UNITY_EDITOR
        if (_condition == false)
        {
            if (string.IsNullOrEmpty(_info))
            {
                throw new Exception();
            }
            else
            {
                throw new Exception(_info);
            }
        }
#else
        if (_condition == false && !string.IsNullOrEmpty(_info))
        {
            throw new Exception(_info);
        }
#endif

        return _condition;
    }

    /// <summary>
    /// 打印
    /// </summary>
    /// <param name="_log">信息</param>
    /// <returns>打印是否成功</returns>
    public static bool Print(string _log)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(_log);
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// 错误
    /// </summary>
    /// <param name="_log">信息</param>
    /// <returns>打印是否成功</returns>
    public static bool Error(string _log)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogError(_log);
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// 警告
    /// </summary>
    /// <param name="_log">信息</param>
    /// <param name="_level">警告级别</param>
    /// <returns>打印是否成功</returns>
    public static bool Warning(string _log, WarningLevel _level = WarningLevel.Normal)
    {
#if UNITY_EDITOR
        if ((int)_level >= (int)mWarningLevel)
        {
            UnityEngine.Debug.LogWarning(_log);
        }
        return true;
#else
        return false;
#endif
    }
}