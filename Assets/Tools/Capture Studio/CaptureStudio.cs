#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
namespace CaptureStudio
{
    public static class CaptureStudio
	{
        [RequireComponent(typeof(Camera))]
        public class Behaviour : MonoBehaviour
        {
            Camera camera;

            public int Width { get { return Screen.width; } }
            public int Height { get { return Screen.height; } }

            protected virtual IEnumerator Start()
            {
                camera = GetComponent<Camera>();

                yield return Procedure();

                EditorApplication.isPlaying = false;
            }

            protected virtual IEnumerator Procedure()
            {
                yield return new WaitForEndOfFrame();
            }

            protected virtual void Capture(string fileName)
            {
                camera.Render();

                var texture = CaptureStudio.Capture();

                CaptureStudio.Save(texture, fileName);
            }
        }

        public static Texture2D Capture()
        {
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

            texture.Apply();

            return texture;
        }

        public static void Save(Texture2D texture, string fileName)
        {
            var bytes = texture.EncodeToPNG();

            var file = new FileInfo(fileName); 
            var directory = file.Directory;

            if (directory.Exists == false)
                directory.Create();

            File.WriteAllBytes(file.FullName, bytes);
        }

        public static string FormatPath(string folder, string file)
        {
            return "External/Capture Studio/" + folder + "/" + file + ".png";
        }
    }
}
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
#endif