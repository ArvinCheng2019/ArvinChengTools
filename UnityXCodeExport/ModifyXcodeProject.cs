using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode; 
using System.Collections;
using System.IO;

public class XcodeProjectMod : MonoBehaviour
{

	internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
	{
		if (Directory.Exists(dstPath))
			Directory.Delete(dstPath);
		if (File.Exists(dstPath))
			File.Delete(dstPath);

		Directory.CreateDirectory(dstPath);

		foreach (var file in Directory.GetFiles(srcPath))
			File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

		foreach (var dir in Directory.GetDirectories(srcPath))
			CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
	}

	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
		if (buildTarget == BuildTarget.iOS)
		{
			string projPath = PBXProject.GetPBXProjectPath(path);
			PBXProject proj = new PBXProject();

			proj.ReadFromString(File.ReadAllText(projPath));
			string target = proj.TargetGuidByName("Unity-iPhone");

			//proj.AddFrameworkToProject(target, "AssetsLibrary.framework", false);

			//CopyAndReplaceDirectory("Assets/Lib/mylib.framework", Path.Combine(path, "Frameworks/mylib.framework"));
			proj.AddFileToBuild(target, proj.AddFile("Frameworks/libz.dylib", "Frameworks/libz.dylib", PBXSourceTree.Source));
			proj.AddFileToBuild(target, proj.AddFile("Frameworks/CoreTelephony.framework", "Frameworks/CoreTelephony.framework", PBXSourceTree.Source));

          proj.SetBuildProperty(target, "Bundle identifier", "com.ylsm.ardinosaur");
			File.WriteAllText(projPath, proj.WriteToString());

            AddPrivate(path);

        }
	}

    static void AddPrivate(string pathToBuiltProject)
    {
        string infoPlistPath = Path.Combine(pathToBuiltProject, "./Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(infoPlistPath));

        PlistElementDict rootDict = plist.root;
        rootDict.SetString("NSPhotoLibraryUsageDescription", "是否允许此App访问你的相册，以便保存你的拍照？");
        rootDict.SetString("NSCameraUsageDescription", "是否允许此App访问你的相机，以便体验AR？");
        rootDict.SetString("NSMicrophoneUsageDescription", "是否允许此App访问你的麦克风？");
	    rootDict.SetString("NSPhotoLibraryAddUsageDescription", "是否允许此App向你的相册添加照片,以便保存你的拍照？");

        PlistElementArray pa= rootDict.CreateArray("LSApplicationQueriesSchemes");
        pa.AddString("weixin");
        pa.AddString("wechat");
        File.WriteAllText(infoPlistPath, plist.WriteToString());
    }
}