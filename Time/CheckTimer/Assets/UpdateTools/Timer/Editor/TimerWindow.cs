using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class TimerWindow : EditorWindow {

	[MenuItem("ArvinTool/程序验证")]
	static void OpenAddWindow()
	{
		TimerWindow myWindow = (TimerWindow)EditorWindow.GetWindow(typeof(TimerWindow), false, "程序授权", true);//创建窗口
		myWindow.Show();//展示
	}

    string myAppName = "12345678901234567890123456789100";
    bool groupEnabled;
    bool IsToday = true;
    bool IsAlways = false;
    int day =1,runTimes =-1;
    string todayTime;

    private void OnGUI()
	{
        GUILayout.Label("程序可使用时间设置工具", EditorStyles.boldLabel);
        myAppName = EditorGUILayout.TextField("程序密钥：", myAppName);
        IsToday = EditorGUILayout.Toggle("是否写入记录文件：", IsToday);
        GUILayout.Label("程序开始日期为： " + DateTime.Today.ToLongDateString(), EditorStyles.boldLabel);
        day = EditorGUILayout.IntField("程序可运行天数：", day);


        GUILayout.Space(10);
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        IsAlways = EditorGUILayout.Toggle("程序是否永久有效：", IsAlways);
        GUILayout.Space(10);
        runTimes = EditorGUILayout.IntField("程序可使用次数：", runTimes);
        GUILayout.Label("(注 ：-1次为永久有效)", EditorStyles.label);
        GUILayout.Space(10);
        EditorGUILayout.EndToggleGroup();

        if(GUILayout.Button("执行"))
        {
            if(myAppName.Length !=32)
            {
                EditorUtility.DisplayDialog("秘钥长度有问题","程序密钥长度不是32","OK");
                return;
            }

            RunIt();
        }
    }

    void RunIt()
    {
        DateTime startTime = DateTime.Today;
        DateTime targetDate = startTime.AddDays(day);

        TimeSpan midTime = targetDate - startTime;
        string times = string.Format("时间间隔为{0}天零{1}小时,结束日期为：{2}", midTime.Days, midTime.Hours,targetDate.ToLongDateString());
        Debug.Log("总共可运行：" + midTime.TotalHours + "  小时，  " + times);

        StartUpInfo startInfo = new StartUpInfo();
        startInfo.StartTime = startTime.ToLongDateString();
        startInfo.EndTimes = targetDate.ToLongDateString(); ;
        startInfo.TotalHours = midTime.TotalHours.ToString();
        startInfo.CurHours = string.Empty;
        startInfo.CurTimes = runTimes;
        startInfo.IsForever = IsAlways;

        startInfo.Key = myAppName;

        string toJson = JsonUtility.ToJson(startInfo);
        string fileNamePath = Application.dataPath + "/Resources/VRTools/";
        
        if(Directory.Exists(fileNamePath) ==false)
        {
            Directory.CreateDirectory(fileNamePath);
        }
        string fuPath = fileNamePath + "version.json";
        using (FileStream fs = new FileStream(fuPath, FileMode.Create, FileAccess.Write))
        {
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(936));
            sw.Write(toJson);
            sw.Close();
        }
    }
}
