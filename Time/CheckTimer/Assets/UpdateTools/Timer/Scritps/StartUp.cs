using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUpInfo
{
    public string StartTime;
    public string EndTimes;
    public string TotalHours;
    public string CurHours;
    public int CurTimes;
    public bool IsForever;
    public string Key;
}

public class StartUp : MonoBehaviour
{
    StartUpInfo configInfo = null;

    // Use this for initialization
    void Start()
    {

        //  从配置文件读取
        configInfo = GetInfoFromJson();

        // 如果没有这个字段，表示是新开始的，或者是被修改的
        bool isHave = HasWirteInPlayerPrefs();
        if ( isHave== false)
        {
            //表示真没有，就写入
            if(CheckAllKey() ==false)
            {
                WirteUnUseKey();
                WirteEncryptData();
            }
        }

        // 如果是永久免费的，直接退出
        if(IsForever())
        {

            //转移到下一个场景
            SceneManager.LoadScene(1);
            return;
        }

        //  如果超时，就不在进入
        if (IsOutOfTime() == true)
            return;

        // 读取字段，并对比
        ReadDataFromPrefs();
    }


    StartUpInfo GetInfoFromJson()
    {
        TextAsset ta = Resources.Load("VRTools/version") as TextAsset;
        Debug.Log(ta.text);
        StartUpInfo info = JsonUtility.FromJson(ta.text, typeof(StartUpInfo)) as StartUpInfo;
        return info;
    }


    // 如果有任何一个字段，认为被修改了
    bool CheckAllKey()
    {
        return ES3.KeyExists("AppName") || ES3.KeyExists("VRVersion") || ES3.KeyExists("PowerBy");   
    }

    void  WirteUnUseKey()
    {
        ES3.Save<string>("AppName", "Unity");
        ES3.Save<string>("VRVersion", "1.0");
        ES3.Save<string>("PowerBy", "Arvin");
        ES3.Save<string>("MyApplicationName", configInfo.Key);
    }

    //第一次往数据里写的时候，总时长和当前时长是一样的
    void WirteEncryptData()
    {
        ES3.Save<string>("StartTime", configInfo.StartTime);
        ES3.Save<string>("EndTimes", configInfo.EndTimes);
        ES3.Save<string>("TotalHours", configInfo.TotalHours);
        ES3.Save<string>("CurHours", configInfo.TotalHours);
        ES3.Save<string>("CurTimes", configInfo.CurTimes.ToString());
        // 默认写入的时候都是未超时的
        ES3.Save<bool>("IsOutTime",false);

        // 判定是否是永久免费
        ES3.Save<bool>("IsForever",configInfo.IsForever);
    }
        
    void ReadDataFromPrefs()
    {
        string startTime = GetDecryptValue("StartTime", configInfo.Key);
        string endTime = GetDecryptValue("EndTimes", configInfo.Key);
        string totalhours = GetDecryptValue("TotalHours", configInfo.Key);
        string curhours = GetDecryptValue("CurHours", configInfo.Key);
        string curtimes = GetDecryptValue("CurTimes", configInfo.Key);

        // 如果不是-1，并且次数为0，就直接退出
        int usedTime = CheckTime(curtimes);
        if ( usedTime != -1 &&usedTime == 0)
        {
            Debug.LogError("used Time is error:" + usedTime);
            ES3.Save<bool>("IsOutTime", true);
            return;
        }

        DateTime tody = DateTime.Today;

        DateTime startTime_dt = Convert.ToDateTime(startTime);
        DateTime endTime_dt = Convert.ToDateTime(endTime);

        // 这俩天总共差了几个小时
        TimeSpan midTime = tody - startTime_dt;
        double houus = midTime.TotalHours;
        if (houus < 0)
        {
            Debug.LogError("houus is error:" + usedTime);
            ES3.Save<bool>("IsOutTime", true);
            return;
        }

        TimeSpan endTimeLen = endTime_dt - tody;
        double endHours = endTimeLen.TotalHours;
        if (endHours <= 0)
        {
            Debug.LogError("End houus is error:" + usedTime);
            ES3.Save<bool>("IsOutTime", true);
            return;
        }
            

        // 如果没有超时，还在有效期，去找上一次的时长,如果上一次时间的长度是10，本次时间长度是11，那么表示有问题，也不切换。
        double configCurhours = double.Parse(curhours);
        if(configCurhours < endHours)
        {
            Debug.LogError("End houus is error:" + usedTime);
            ES3.Save<bool>("IsOutTime", true);
            return;
        }

        // 如果都没问题，就去更新一下剩余时间
        ES3.Save<string>("CurHours", endHours.ToString());

        //转移到下一个场景
        SceneManager.LoadScene(1);
    }

    // 这里判定的时候，是肯定有这个值的,如果没有，就表示异常
    bool IsOutOfTime()
    {
        if(ES3.KeyExists("IsOutTime") ==false)
        {
            return false;
        }

        return ES3.Load<bool>("IsOutTime");
    }

    bool IsForever()
    {
        return ES3.Load<bool>("IsForever");
    }

    int CheckTime( string times)
    {
        int tartime = 0;
        if (int.TryParse(times, out tartime))
        {
            return tartime;
        }
        return tartime;
    }

    bool HasWirteInPlayerPrefs()
    {
        return ES3.KeyExists("MyApplicationName");
    }
    
    // 对比记录的数据（ PlayerPrefs 和 Res下的txt ） 是否正常，他们俩理论上 alllen的值是一样的
   // 对比系统时间和endtime的时间获取到时长
   // 对比当前时长和上次时长，如果上次时长大于当前时长，正确，然后更新当前时长到上次，如果小于，表示修改过。
    string GetCurTime()
    {
        return System.DateTime.Now.ToShortDateString();
    }

    string GetDecryptValue( string resName,string key)
    {
        string realValue = string.Empty;
        if(ES3.KeyExists(resName))
        {
            realValue = ES3.Load<string>(resName);
        }

        return realValue;
    }
}
