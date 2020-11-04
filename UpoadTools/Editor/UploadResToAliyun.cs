using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Aliyun.OSS;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using SimpleJson;

public class UploadResToAliyun : EditorWindow {

    [MenuItem("YLSM/UploadResToAliyun")]
    static void Open()
    {
        AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/reslist";
        DllPath= Application.streamingAssetsPath + "/AssetBundles/dlllist";
        ABJsonPath= Application.streamingAssetsPath + "/assetslist.json";
        DllJsonPath = Application.streamingAssetsPath + "/dlllist.json";
        //////////////////////////////////////////////////////////////////

        UploadResToAliyun window = GetWindow<UploadResToAliyun>(true);
     //   UploadResToAliyun window = GetWindowWithRect(typeof(UploadResToAliyun), new Rect(0, 0, 580, 200)) as UploadResToAliyun;
        window.Show();
    }
    enum Platform
    {
        IOS,Android,PC
    }
    Platform platform = Platform.IOS;
    enum ResType
    {
        AssetBundle,Dll
    }
    ResType resType= ResType.AssetBundle;

    string AccessKeyId = "LTAI30SMSZmK7Aw1";
    string AccessKeySecret = "JPizSPQpRhclJEDN7TPo7lKNYY5w2r";
    string Endpoint = "oss-cn-beijing.aliyuncs.com";
    string bucketName = "hotfixarkit";
    string UploadPath;
    string info;
    string UploadJsonPath;
    static string AssetBundlePath;
    static string DllPath;
    static string ABJsonPath;
    static string DllJsonPath;
    void TextField(string name,ref string value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name + ":");
        value = EditorGUILayout.TextField(value);
        EditorGUILayout.EndHorizontal();
    }
    private void OnGUI()
    {      

        EditorGUILayout.BeginVertical();
    
        TextField("AccessKeyId", ref AccessKeyId);
        TextField("AccessKeySecret", ref AccessKeySecret);
        TextField("Endpoint", ref Endpoint);
        TextField("bucketName", ref bucketName);

        TextField("AssetBundlePath", ref AssetBundlePath);
        TextField("DllPath", ref DllPath);
        TextField("ABJsonPath", ref ABJsonPath);
        TextField("DllJsonPath", ref DllJsonPath);


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请选择平台:");
        platform=(Platform) EditorGUILayout.EnumPopup(platform);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请选择上传资源类型:");
        resType = (ResType)EditorGUILayout.EnumPopup(resType);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("确认上传"))
        {
            UpLoad();

        }
        EditorGUILayout.LabelField(info);
        EditorGUILayout.EndVertical();

    }
    int needUploadCount=0;
    int uploadedCount=0;

    void UpLoad()
    {
        OssClient client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
        switch (platform)
        {
            case Platform.Android:
                UploadPath = "ylsm/android/";
                configPath = andriodJsonUrl;
                break;
            case Platform.IOS:
                UploadPath = "ylsm/ios/";
                configPath = iosResJsonUrl;
                break;
            case Platform.PC:
                UploadPath = "ylsm/pc/";
                configPath = pcResJsonUrl;
                break;
        }
        string[] fileNames;
        switch (resType)
        {
            case ResType.AssetBundle:
               // configPath = Config.AssetslistConfigFileServerPath;
              //  configFileName = Config.AssetsConfigName;
                SaveAssetsConfig();
                //下载服务器config并和本地config比较
                try
                {
                    if (DownLoadConfig())
                    {
                        localcfgDict = ReadConfig(Application.streamingAssetsPath + "/assetslist.json");
                        servercfgDict = ReadConfig(Application.temporaryCachePath + "/tmp.json");
                        CompareFile();
                    }
                }
                catch (Exception)
                {
                    localcfgDict = ReadConfig(Application.streamingAssetsPath + "/assetslist.json");
                    foreach (var item in localcfgDict.Keys)
                    {
                        needUpLoadList.Add(item);
                    }
                
                }
       
                UnityEngine.Debug.Log(needUpLoadList.Count);

                fileNames = Directory.GetFiles(AssetBundlePath);

                string copyToFloder = Application.streamingAssetsPath + "/temp";
                if (!Directory.Exists(copyToFloder))
                {
                    Directory.CreateDirectory(copyToFloder);
                }
                else
                {
                    DelectDir(copyToFloder);
                }

                foreach (var item in needUpLoadList)
                {
                    File.Copy(Application.streamingAssetsPath + "/AssetBundles/reslist/" + item, copyToFloder+"/"+item);
                }



                UploadJsonPath = UploadPath+"json/assetslist.json";
                UploadPath += "reslist/";      
              Process.Start(Application.dataPath+"/../Tools/Upload.exe", UploadPath + " " + copyToFloder + " "+ABJsonPath+" "+ UploadJsonPath);
                break;
            case ResType.Dll:
               // configPath = Config.DllListConfigFileServerPath;
              //  configFileName = Config.DLLConfigName;
                SaveDllConfig();
                fileNames = Directory.GetFiles(DllPath);
                UploadJsonPath = UploadPath+"json/dlllist.json";
                UploadPath += "dlllist/";
                Process.Start(Application.dataPath + "/../Tools/Upload.exe", UploadPath + " " + DllPath + " " + DllJsonPath + " " + UploadJsonPath);
                break;
            default:
                fileNames = null;
                break;
        }

    }

    public static void DelectDir(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    #region 生成dllconfig和resconfig两个json

    private static void SaveDllConfig()
    {
        Dictionary<string, string> fileList = GetFileList("AssetBundles/dlllist");
        StreamWriter write = new StreamWriter(Application.streamingAssetsPath + "/dlllist.json");
        write.WriteLine("{");
        foreach (var item in fileList)
        {
            write.WriteLine("\"" + item.Key + "\":\"" + item.Value + "\",");
        }
        write.WriteLine("}");
        write.Close();
        AssetDatabase.Refresh();

    }
    private static Dictionary<string, string> GetFileList(string DirectoryName)
    {
        Dictionary<string, string> dirct = new Dictionary<string, string>();
        string[] fileList = Directory.GetFiles(Application.streamingAssetsPath + "/" + DirectoryName);
        foreach (var item in fileList)
        {
            if (!item.EndsWith(".meta"))
                dirct.Add(Path.GetFileName(item), AppUtlis.GetMD5(item));
        }
        return dirct;
    }

    public static void SaveAssetsConfig()
    {

        Dictionary<string, string> mfileMD5list = new Dictionary<string, string>();
        string jsonFile = Application.dataPath + "/StreamingAssets/assetslist.json";
        string tarDir = Application.dataPath + "/StreamingAssets/AssetBundles/reslist";
        if (File.Exists(jsonFile))
        {
            File.Delete(jsonFile);
        }
        DirectoryInfo dir = new DirectoryInfo(tarDir);
        FileInfo[] inf = dir.GetFiles();
        foreach (FileInfo finf in inf)
        {
            if (finf.Name.EndsWith("meta"))
                continue;

            string md5 = AppUtlis.GetMD5(tarDir + "/" + finf.Name);
            mfileMD5list.Add(finf.Name, md5);
        }

        string jsonvalue = SimpleJson.SimpleJson.SerializeObject(mfileMD5list);


        StreamWriter write = new StreamWriter(jsonFile);
        write.WriteLine("{");
        foreach (var item in mfileMD5list)
        {
            write.WriteLine("\"" + item.Key + "\":\"" + item.Value + "\",");
        }
        write.WriteLine("}");
        write.Close();
        AssetDatabase.Refresh();

    }
    
    #endregion


    //配置文件名 dlllist.json / assetslist.json
    private static string configFileName;

    private static  string localFilePath;

    private static string tempPath;
    //资源表服务器地址
    private string configPath;

    string pcResJsonUrl = "http://hotfixarkit.oss-cn-beijing.aliyuncs.com/ylsm/pc/json/assetslist.json";
    string iosResJsonUrl = "http://hotfixarkit.oss-cn-beijing.aliyuncs.com/ylsm/ios/json/assetslist.json";
    string andriodJsonUrl= "http://hotfixarkit.oss-cn-beijing.aliyuncs.com/ylsm/andriod/json/assetslist.json";

    private bool DownLoadConfig()
    {
        Stream oStream = null;
        string tempConfig = Application.temporaryCachePath+"/tmp.json";
        if (File.Exists(tempConfig))
        {
            File.Delete(tempConfig);
        }
        try
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(configPath);
            req.Timeout = 3000;
            oStream = req.GetResponse().GetResponseStream();
            using (StreamReader respStreamReader = new StreamReader(oStream, Encoding.UTF8))
            {
                string line = string.Empty;
                while ((line = respStreamReader.ReadLine()) != null)
                {
                    UTF8Encoding utf8 = new UTF8Encoding(false);
                    using (StreamWriter sw = new StreamWriter(tempConfig, true, utf8))
                    {
                        sw.WriteLine(line);
                    }

                }
            }
            return true;
        }
        catch (Exception)
        {

         throw;
        }

    
    }

    private  List<string> needUpLoadList = new List<string>(10);
    private static Dictionary<string, string> localcfgDict = new Dictionary<string, string>(50);
    private static Dictionary<string, string> servercfgDict = new Dictionary<string, string>(50);

    private bool CompareFile()
    {
        needUpLoadList.Clear();
        foreach (var item in localcfgDict.Keys)
        //  foreach (var item in servercfgDict.Keys)
        {
            if (!servercfgDict.ContainsKey(item))
            {
                needUpLoadList.Add(item);
            }
            else
            {
                if (localcfgDict[item] != servercfgDict[item])
                {
                    needUpLoadList.Add(item);
                }
            }
        }

        return needUpLoadList.Count > 0;
    }

    private static Dictionary<string, string> ReadConfig(string path)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>(50);
        StreamReader reader = new StreamReader(path);
        string config = reader.ReadToEnd();
        JsonObject jsroot = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(config);
        foreach (var item in jsroot)
        {
            dict.Add(item.Key.ToString(), item.Value.ToString());
        }
        return dict;
    }
}
