#if UNITY_EDITOR
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;

using UnityEngine;
using System;

using UnityEditor;

[RequireComponent(typeof(Camera))]
public class ScreenshotCapture : MonoBehaviour
{
    Camera _camera;
    public new Camera camera
    {
        get
        {
            if(_camera == null)
                _camera = GetComponent<Camera>();

            return _camera;
        }
    }

    public List<GameObject> subjects;

    public int Width { get { return Screen.width; } }
    public int Height { get { return Screen.height; } }

    IEnumerator Start()
    {
        yield return Procedure();
    }

    IEnumerator Procedure()
    {
        yield return new WaitForEndOfFrame();

        foreach (var subject in subjects)
        {
            yield return Capture(subject);
        }

        EditorApplication.isPlaying = false;
    }

    IEnumerator Capture(GameObject prefab)
    {
        var instance = Instantiate(prefab);

        var texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);

        camera.Render();

        WriteScreenToTexture(texture);

        var path = GetFilePath(prefab);

        SaveTexture(path, texture);

        Destroy(instance);

        yield break;
    }

    string GetFilePath(GameObject prefab)
    {
        var result = AssetDatabase.GetAssetPath(prefab);

        result = Path.ChangeExtension(result, "png");

        return result;
    }

    void WriteScreenToTexture(Texture2D texture)
    {
        texture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);

        texture.Apply();
    }

    void SaveTexture(string path, Texture2D texture)
    {
        var bytes = texture.EncodeToPNG();

        var file = new FileInfo(path);
        var directory = file.Directory;

        if (directory.Exists == false)
            directory.Create();

        File.WriteAllBytes(file.FullName, bytes);
    }

    [CustomEditor(typeof(ScreenshotCapture))]
    public class Inspector : Editor
    {
        new ScreenshotCapture target;

        void OnEnable()
        {
            this.target = base.target as ScreenshotCapture;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif