#if UNITY_EDITOR
using System.Collections;
using System.IO;

using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
Usage:
1. Attach this script to your chosen camera's game object.
2. Set that camera's Clear Flags field to Solid Color.
3. Use the inspector to set frameRate and framesToCapture
4. Choose your desired resolution in Unity's Game window (must be less than or equal to your screen resolution)
5. Turn on "Maximise on Play"
6. Play your scene. Screenshots will be saved to YourUnityProject/Screenshots by default.
*/

[RequireComponent(typeof(Camera))]
public class ScreenshotCapture : MonoBehaviour
{
    [SerializeField]
    new Camera camera;

    public GameObject[] subjects;

    public static int Width { get { return Screen.width; } }
    public static int Height { get { return Screen.height; } }

    Texture2D Texture;

    void Reset()
    {
        camera = GetComponent<Camera>();
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

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
        Texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);

        camera.Render();

        WriteScreenToTexture(Texture);

        SaveTexture(Texture);

        EditorApplication.isPlaying = false;
    }

    void WriteScreenToTexture(Texture2D texture)
    {
        Debug.Log(Width + " : " + Height);

        texture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);

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

#if UNITY_EDITOR
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
#endif
}
#endif