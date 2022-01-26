using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadAssetBundlesFromMemory : MonoBehaviour
{
    void StartLoadAsset()
    {
        string path = "Assets/AssetBundle";

        StartCoroutine(LoadFromMemoryAsync(path));
    }

    IEnumerator LoadFromMemoryAsync(string path)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));

        yield return request;

        AssetBundle bundle = request.assetBundle;

    }
}
