using System.Linq;
using UnityEngine;
using UnityEditor;

public static class SaveDataUtilityFunctions
{
    [MenuItem("Assets/Decrypt Save Data", true)]
    private static bool IsDecryptValid()
    {
        return 0 != Selection.objects.Count(o => AssetDatabase.GetAssetPath(o).EndsWith(".sav"));
    }

    [MenuItem("Assets/Decrypt Save Data")]
    private static void Decrypt()
    {
        var paths = Selection.objects
            .Where(o => AssetDatabase.GetAssetPath(o).EndsWith(".sav"))
            .Select(AssetDatabase.GetAssetPath)
            .Select(System.IO.Path.GetFullPath);

        foreach (var path in paths)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            string json = Cipher.Decrypt(bytes);

            string newPath = System.IO.Path.ChangeExtension(path, ".txt");
            System.IO.File.WriteAllText(newPath, json);
        }

        AssetDatabase.Refresh();
    }
}