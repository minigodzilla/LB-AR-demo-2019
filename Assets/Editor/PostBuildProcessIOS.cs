#if UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class PostBuildProcessIOS
{
    private static readonly string nonEncryption = "ITSAppUsesNonExemptEncryption";
    private static readonly string container = "com.apple.developer.icloud-container-identifiers";
    private static readonly string keyValue = "com.apple.developer.ubiquity-kvstore-identifier";
    private static readonly string teamPrefix = "$(TeamIdentifierPrefix)$(CFBundleIdentifier)";
    private static readonly string remote = "remote-notification";
    private static readonly string UIBG = "UIBackgroundModes";
    private static readonly string bundleLocaliztions = "CFBundleLocalizations";

    private static readonly string[] SUPPORTED_LANGUAGES = { "en" };

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string path)
    {
        if(buildTarget == BuildTarget.iOS)
        {
            string plistPath = path + "/Info.plist";

            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            Debug.Log("Automation >> in progress ...");

            rootDict.SetBoolean(nonEncryption, false);
            rootDict.CreateArray(container);
            rootDict.SetString(keyValue , teamPrefix);

            var arr = rootDict.CreateArray(UIBG);
            arr.AddString(remote);

            var localizations = rootDict.CreateArray(bundleLocaliztions);

            foreach (var lang in SUPPORTED_LANGUAGES)
            {
                localizations.AddString(lang);
            }

            File.WriteAllText(plistPath, plist.WriteToString());

            Debug.Log("Automation >> Completed!");
        }
    }
}
#endif