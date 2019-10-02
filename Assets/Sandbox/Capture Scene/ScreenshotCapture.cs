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

    [SerializeField]
    protected string outPutFolder = "External/Screenshots/";
    public string OutPutFolder { get { return outPutFolder; } }

    public int Width { get { return Screen.width; } }
    public int Height { get { return Screen.height; } }

    IEnumerator Start()
    {
        yield return Procedure();
    }

    IEnumerator Procedure()
    {
        yield return new WaitForEndOfFrame();

        subjects.ForEach(x => x.SetActive(false));

        foreach (var subject in subjects)
        {
            yield return Capture(subject);
        }

        EditorUtility.RevealInFinder(outPutFolder);

        EditorApplication.isPlaying = false;
    }

    IEnumerator Capture(GameObject subject)
    {
        subject.SetActive(true);

        var texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);

        camera.Render();

        WriteScreenToTexture(texture);

        SaveTexture(subject.name, texture);

        subject.SetActive(false);

        yield break;
    }

    void WriteScreenToTexture(Texture2D texture)
    {
        texture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);

        texture.Apply();
    }

    void SaveTexture(string name, Texture2D texture)
    {
        var png = texture.EncodeToPNG();

        if (Directory.Exists(outPutFolder) == false)
            Directory.CreateDirectory(outPutFolder);

        File.WriteAllBytes(outPutFolder + name + ".png", png);
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