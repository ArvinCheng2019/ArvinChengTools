using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;

public class ZipTools
{

    private static void ChangeConfigZipSize()
    {
        string path = Application.streamingAssetsPath + "/AssetBundles";
        string[] fileList = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        long totalSize = 0;
        foreach (var item in fileList)
        {
            FileInfo file = new FileInfo(item);
            totalSize += file.Length;
        }
        AssetBundleConfig config= Resources.Load<AssetBundleConfig>("AssetBundleConfig");
#if UNITY_ANDROID
        config.AndroidFileSize = totalSize;
#elif UNITY_IOS
         config.IOSFileSize = totalSize;
#elif UNITY_STANDALONE_WIN
      config.WinFileSize = totalSize;
#elif UNITY_STANDALONE_OSX
   config.MacFileSize = totalSize;
#else

#endif



        UnityEngine.Debug.Log(totalSize);
    }
    [MenuItem("YLSM/CreateABZip")]
    public static void CreateAssetsZip()
    {
        ChangeConfigZipSize();
        string buildPath = Application.dataPath + "/../Tools/Python/zipasset.py";
        RunPythonMac(buildPath);
        AssetDatabase.Refresh();
    }
    private static void RunPythonMac(string args)
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = args;
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = "";
                while (result != null)
                {

                    result = reader.ReadLine();


                    UnityEngine.Debug.Log(result);
                }

            }
        }
    }
    private static void RunPythonCmd(string args)
    {
        Process pro = Process.Start(args);
        pro.WaitForExit();
    }

    }
