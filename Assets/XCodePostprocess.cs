using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode.New.Extensions;

#if UNITY_IOS || UNITY_EDITOR
using UnityEditor.iOS.Xcode.New;
#endif

namespace BuildBuddy
{
    public static class XcodePostProcessBuild
    {

        [PostProcessBuild]
        public static void ChangeXcodePlist(BuildTarget target, string pathToProject)
        {
            if (target == BuildTarget.iOS)
            {
#if UNITY_IOS || UNITY_EDITOR

                //edit plist
                string pListPath = pathToProject + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(pListPath));
                var root = plist.root;

                var capsArr = root.CreateArray("UIBackgroundModes");
                capsArr.AddString("remote-notification");

                capsArr = root.CreateArray("UIRequiredDeviceCapabilities");
                capsArr.AddString("armv7");
                capsArr.AddString("gamekit");

                plist.WriteToFile(pListPath);

#endif
            }
        }

        [PostProcessBuild]
        public static void ChangeXcodeProject(BuildTarget target, string pathToProject)
        {
            if (target == BuildTarget.iOS)
            {
#if UNITY_IOS || UNITY_EDITOR
                //open project file
                string pathToProjectFull = pathToProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

                PBXProject pbxProject = new PBXProject();
                pbxProject.ReadFromFile(pathToProjectFull);

                string targetGuid = pbxProject.TargetGuidByName("Unity-iPhone");
                pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
                pbxProject.SetTeamId(targetGuid, PlayerSettings.iOS.appleDeveloperTeamID);

                //doing in build buddy
                //-weak_framework CoreMotion - weak - lSystem - weak_framework GameKit - weak_framework MediaPlayer - weak_framework MobileCoreServices
                pbxProject.AddFrameworkToProject(targetGuid, "CoreMotion.framework", true);
                pbxProject.AddFrameworkToProject(targetGuid, "GameKit.framework", true);
                pbxProject.AddFrameworkToProject(targetGuid, "MediaPlayer.framework", true);
                pbxProject.AddFrameworkToProject(targetGuid, "MobileCoreServices.framework", true);
                pbxProject.AddFrameworkToProject(targetGuid, "UserNotifications.framework", true);
                pbxProject.AddFrameworkToProject(targetGuid, "MessageUI.framework", true);

                // Add stickers extension target
                /*string stickersInfoPlistPath = Application.dataPath + "/../build/extern_resources/ios/Stickers/Info.plist";

                // Copy assets
				string pathToStickersXcassets = Application.dataPath + "/../build/extern_resources/ios/Stickers";
				try 
				{
					if(Directory.Exists(pathToProject + "/Stickers"))
						Directory.Delete(pathToProject + "/Stickers", true);
					FileUtil.CopyFileOrDirectory(pathToStickersXcassets, pathToProject + "/Stickers");

					string stickersTargetGuid = PBXProjectExtensions.AddAppExtensionStickers(pbxProject, targetGuid, "Stickers", PlayerSettings.bundleIdentifier + ".Stickers", "Stickers/Info.plist");

					pbxProject.SetTeamId(stickersTargetGuid, PlayerSettings.iOS.appleDeveloperTeamID);

					pbxProject.AddFileToBuild(stickersTargetGuid, pbxProject.AddFolderReference(pathToProject + "/Stickers/Stickers.xcassets", "Stickers/Stickers.xcassets"));
					pbxProject.AddFileToBuild(stickersTargetGuid, pbxProject.AddFolderReference(pathToProject + "/Stickers/Info.plist", "Stickers/Info.plist"));
				}
				catch (System.Exception ex)
				{
					var res = ex.Message;
					utl.LogError(res);
				}*/
#if MASTER
				// Copy the entitlement file to the xcode project
				var entitlementPath = Application.dataPath + "/../build/" + PlayerSettings.productName + ".entitlement";
				var entitlementFileName = Path.GetFileName(entitlementPath);
				var unityTarget = PBXProject.GetUnityTargetName();
				var relativeDestination = unityTarget + "/" + entitlementFileName;
				var destinationPath = pathToProject + "/" + relativeDestination;

				FileUtil.CopyFileOrDirectory(entitlementPath, destinationPath);
				// Add the pbx configs to include the entitlements files on the project
				pbxProject.AddFile(relativeDestination, entitlementFileName);
				pbxProject.AddBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", relativeDestination);

				pbxProject.AddBuildProperty(targetGuid, "SystemCapabilities", "{com.apple.Push = {enabled = 1;};}");
				pbxProject.AddBuildProperty(targetGuid, "SystemCapabilities", "{com.apple.BackgroundModes = {enabled = 1;};}");
				pbxProject.AddBuildProperty(targetGuid, "SystemCapabilities", "{com.apple.GameCenter = {enabled = 1;};}");
				pbxProject.AddBuildProperty(targetGuid, "SystemCapabilities", "{com.apple.InAppPurchase = {enabled = 1;};}");
				pbxProject.AddBuildProperty(targetGuid, "SystemCapabilities", "{com.apple.iCloud = {enabled = 1;};}");
				pbxProject.AddBuildProperty(targetGuid, "SystemCapabilities", "{com.apple.GameControllers.appletvos = {enabled = 1;};}");
#endif

                pbxProject.WriteToFile(pathToProjectFull);
#endif
            }
        }

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToProject)
        {
            if (target == BuildTarget.iOS)
            {
#if UNITY_IOS
				//XcodeProject pr = new XcodeProject(pathToProject);
				//pr.EditProject(); //cause errors

#endif
            }
        }

    }


}