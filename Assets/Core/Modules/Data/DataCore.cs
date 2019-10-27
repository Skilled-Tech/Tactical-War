using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using System.Text;
using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class DataCore : Core.Property
    {
        public string BaseDirectory
        {
            get
            {
#if UNITY_EDITOR
                return Directory.GetCurrentDirectory();
#else
                return Application.persistentDataPath;
#endif
            }
        }

        public string AbsolutePath { get { return BaseDirectory + "/Data/"; } }

        public virtual void Save(string relativePath, byte[] data)
        {
            var directory = new DirectoryInfo(AbsolutePath + Path.GetDirectoryName(relativePath));

            if (directory.Exists == false)
                directory.Create();

            var file = new FileInfo(AbsolutePath + relativePath);

            if (file.Exists == false) file.Create().Close();

            using (var stream = file.Open(FileMode.Create))
            {
                stream.Write(data, 0, data.Length);
            }
        }
        public virtual void Save(string relativePath, string data)
        {
            Save(relativePath, Encoding.ASCII.GetBytes(data));
        }

        public virtual byte[] Load(string relativePath)
        {
            var file = new FileInfo(AbsolutePath + relativePath);

            if (file.Exists == false)
                throw new FileNotFoundException("Data File Not Found", AbsolutePath + relativePath);

            Byte[] bytes;

            using (var stream = file.OpenRead())
            {
                bytes = new byte[stream.Length];

                var read = stream.Read(bytes, 0, bytes.Length);
            }

            return bytes;
        }
        public virtual string LoadText(string relativePath)
        {
            return Encoding.ASCII.GetString(Load(relativePath));
        }

        public virtual bool Exists(string relativePath)
        {
            relativePath = "/" + relativePath;

            return File.Exists(AbsolutePath + relativePath);
        }

        public virtual void Reset()
        {
            if (Directory.Exists(AbsolutePath))
            {
                Directory.Delete(AbsolutePath, true);
            }

            Core.Quit();
        }
    }
}