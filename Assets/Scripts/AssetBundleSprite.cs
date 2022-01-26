using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleSprite : MonoBehaviour
{
    Sprite _sprite;
    public static AssetBundleSprite instance = null;

    public AssetBundleSprite Instance
    {
        get
        {
            if (instance != null)
                instance = this;

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        _sprite = GetComponent<Sprite>();
    }
    
    public void Set_Sprite(Sprite sprite)
    {
        _sprite = sprite;
    }
}
