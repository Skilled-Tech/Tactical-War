#if UNITY_EDITOR
using System.Collections;
using System.IO;

using UnityEngine;
using System;

using UnityEditor;

[RequireComponent(typeof(Camera))]
public class ScreenshotCapture : MonoBehaviour
{
    [SerializeField]
    new Camera camera;

    public GameObject[] subjects;

    public int width = 256;
    public int height = 256;

    Texture2D Texture;

    void Reset()
    {
        camera = GetComponent<Camera>();
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        Screen.SetResolution(width, height, false);

        foreach (var subject in subjects)
            subject.SetActive(false);

        foreach (var subject in subjects)
        {
            subject.SetActive(true);

            Capture();

            subject.SetActive(false);
        }
    }

    void Capture()
    {
        Texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        camera.Render();

        WriteScreenToTexture(Texture);

        SaveTexture(Texture);

        EditorApplication.isPlaying = false;
    }

    void WriteScreenToTexture(Texture2D texture)
    {
        Debug.Log(width + " : " + height);

        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        texture.Apply();
    }

    void SaveTexture(Texture2D texture)
    {
        var png = texture.EncodeToPNG();

        var directory = "Screenshots/";

        if (Directory.Exists(directory) == false)
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(directory + "Shot " + DateTime.Now.ToString("HH-mm-ss-fff") + ".png", png);
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

            if (GUILayout.Button("Capture"))
                target.Capture();
        }
    }
}
#endif