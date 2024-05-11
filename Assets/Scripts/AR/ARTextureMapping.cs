using System;
using UnityEngine;

[Serializable]
public class TextureMapping
{
    public Texture2D baseMap;
    public Texture2D normalMap;

    public Color baseColor = Color.white;

    public string baseValue = "_BaseMap";
    public string normalValue = "_BumpMap";
    public string colorValue = "_BaseColor";
}
