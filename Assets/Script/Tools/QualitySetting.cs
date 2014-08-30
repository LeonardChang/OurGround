using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 画质设置
/// </summary>
public class QualitySetting 
{
    static bool IsLowDevice(string _deviceModel)
    {
#if UNITY_ANDROID
//		string[] devices = DataDictionary.Instance.GetLowAndroidDevice();
//        if (devices == null || devices.Length == 0)
//        {
//            //SystemUI.Instance.ShowDebugString("can not get devices list");
//            return false;
//        }
//
//        List<string> low = new List<string>(devices);
//        //SystemUI.Instance.ShowDebugString("devices list count = " + low.Count.ToString());
//        foreach (var name in low)
//        {
//            if (string.IsNullOrEmpty(name))
//            {
//                continue;
//            }
//
//            if (_deviceModel.StartsWith(name))
//            {
//                return true;
//            }
//        }
#elif UNITY_IPHONE
        if (iPhone.generation == iPhoneGeneration.iPhone
            || iPhone.generation == iPhoneGeneration.iPhone3G
            || iPhone.generation == iPhoneGeneration.iPhone3GS
            || iPhone.generation == iPhoneGeneration.iPhone4
            || iPhone.generation == iPhoneGeneration.iPhone4S
            || iPhone.generation == iPhoneGeneration.iPad1Gen
            || iPhone.generation == iPhoneGeneration.iPodTouch1Gen
            || iPhone.generation == iPhoneGeneration.iPodTouch2Gen
            || iPhone.generation == iPhoneGeneration.iPodTouch3Gen
            || iPhone.generation == iPhoneGeneration.iPodTouch4Gen)
        {
            return true;
        }
#endif

        return false;
    }

    public static bool HighQuality
    {
        get
        {
#if UNITY_ANDROID || UNITY_IPHONE
            // Fix bug in SGX543, SGX544 & OpenGL ES 3.0
            if (SystemInfo.graphicsDeviceName.IndexOf("SGX 543") != -1
                || SystemInfo.graphicsDeviceName.IndexOf("SGX 544") != -1
                || SystemInfo.graphicsDeviceName.IndexOf("SGX543") != -1
                || SystemInfo.graphicsDeviceName.IndexOf("SGX544") != -1
                || SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL ES 3")
                || IsLowDevice(SystemInfo.deviceModel))
            {
                return false;
            }
#endif


            if (!PlayerPrefs.HasKey("GraphicsQualitySetting"))
            {
#if UNITY_ANDROID
                int value = SystemInfo.graphicsMemorySize >= 170 ? 1 : 0;

#elif UNITY_IPHONE
                int value = 1;
#else
                int value = SystemInfo.graphicsMemorySize >= 170 ? 1 : 0;
#endif

                if (!SystemInfo.supportsImageEffects)
                {
                    value = 0;
                }

                PlayerPrefs.SetInt("GraphicsQualitySetting001", value);
                PlayerPrefs.Save();
            }

            return PlayerPrefs.GetInt("GraphicsQualitySetting001") == 1;
        }

        set
        {
            PlayerPrefs.SetInt("GraphicsQualitySetting001", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool AllowShadow
    {
        get
        {
#if UNITY_ANDROID || UNITY_IPHONE
            if (IsLowDevice(SystemInfo.deviceModel))
            {
                return false;
            }
#elif UNITY_EDITOR
            return true;
#endif

#if UNITY_ANDROID
            return SystemInfo.graphicsMemorySize >= 170;
#elif UNITY_IPHONE
            return true;
#else
            return true;
#endif
        }
    }

    public static bool HighMemorySize
    {
        get
        {
#if UNITY_IPHONE
            if (iPhone.generation == iPhoneGeneration.iPhone
                || iPhone.generation == iPhoneGeneration.iPhone3G
                || iPhone.generation == iPhoneGeneration.iPhone3GS
                || iPhone.generation == iPhoneGeneration.iPhone4
                || iPhone.generation == iPhoneGeneration.iPhone4S
                || iPhone.generation == iPhoneGeneration.iPad1Gen
                || iPhone.generation == iPhoneGeneration.iPodTouch1Gen
                || iPhone.generation == iPhoneGeneration.iPodTouch2Gen
                || iPhone.generation == iPhoneGeneration.iPodTouch3Gen
                || iPhone.generation == iPhoneGeneration.iPodTouch4Gen)
            {
                return false;
            }
            else
            {
                return SystemInfo.systemMemorySize >= 1000;
            }
#else
            return SystemInfo.systemMemorySize >= 1000;
#endif
        }
    }
}
