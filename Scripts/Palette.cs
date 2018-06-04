using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class Palette : ScriptableObject
{
    [System.Serializable]
    public struct PaletteItem
    {
        public Color albedo;
        [Range(0, 1)]
        public float smoothness;
        [Range(0, 1)]
        public float metallic;
        [Range(0, 1)]
        public float noiseScale;
        [Range(0, 1)]
        public float emission;
    }
    public PaletteItem[] items;

    [SerializeField] Texture2D texture, properties;


#if UNITY_EDITOR
    void OnValidate()
    {
        if (items == null) return;
        if (texture == null)
        {
            texture = new Texture2D(256, 1, TextureFormat.RGBA32, false, true);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            AssetDatabase.AddObjectToAsset(texture, this);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(texture);
        }
        if (properties == null)
        {
            properties = new Texture2D(256, 1, TextureFormat.RGBA32, false, true);
            properties.filterMode = FilterMode.Point;
            properties.wrapMode = TextureWrapMode.Clamp;
            AssetDatabase.AddObjectToAsset(properties, this);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(properties);
        }
        texture.name = this.name + " Colors";
        properties.name = this.name + " Properties";
        var pixels = texture.GetPixels();
        var propertyPixels = properties.GetPixels();
        for (var x = 0; x < 256; x++)
        {
            var pi = items[x % items.Length];
            pixels[x] = pi.albedo;
            propertyPixels[x] = new Color(pi.smoothness, pi.metallic, pi.noiseScale, pi.emission);
        }
        properties.SetPixels(propertyPixels);
        properties.Apply();
        texture.SetPixels(pixels);
        texture.Apply();
    }
#endif

}
