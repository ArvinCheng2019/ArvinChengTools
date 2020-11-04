#!/usr/bin/env python
#coding:utf-8
 
import zipfile
import glob
import os

def ZipFile(path,zipname):
	allfiles=[]
	for root, dirs, files in os.walk(path):
		for filename in files:
			#print(filename)
			name, suf = os.path.splitext(filename)
			if(suf!=".meta"):
				allfiles.append(os.path.join(root, filename))
	f = zipfile.ZipFile(zipname, 'w', zipfile.ZIP_DEFLATED)
	startindex=path.rindex('/');
	print (startindex)
	for file in allfiles:
		print(file)
		f.write(file,file[startindex:])
	f.close()
	print('zip success!!!')
def getFileSize(filePath,size=0):
	for	root,dirs,files in os.walk(filePath):
		for f in files:
			size+=os.path.getsize(os.path.join(root,f))
	return size		
curpath=os.getcwd()
#path="E:\UnityGitWorkSpace\ARHomeV2\Assets\StreamingAssets\AssetBundles"
#zipname="E:\UnityGitWorkSpace\ARHomeV2\Assets\StreamingAssets\AssetBundles.zip"
path=curpath+"/Assets/StreamingAssets/AssetBundles"
zipname=curpath+"/Assets/StreamingAssets/AssetBundles.zip"
print(path)
ZipFile(path,zipname)
size=getFileSize(path);
print("zipsize:{0}".format(size));
#files=glob.glob(path)
#result = input("Please any key to continue:")


	
	