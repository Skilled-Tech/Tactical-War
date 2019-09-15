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

using PlayFab;
using PlayFab.ClientModels;

using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class PlayFabCatalogCore : PlayFabCore.Module
    {
        public const string Version = "Default";

        public List<CatalogItem> Items { get; protected set; }

        public virtual int Size { get { return Items.Count; } }

        public CatalogItem this[int index] { get { return Items[index]; } }

        public virtual CatalogItem Find(string itemID)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].ItemId == itemID)
                    return Items[i];

            return null;
        }

        #region Request
        public virtual void Request()
        {
            if (IsLoggedIn)
            {
                var request = new GetCatalogItemsRequest
                {
                    CatalogVersion = Version
                };

                PlayFabClientAPI.GetCatalogItems(request, ResultCallback, ErrorCallback);
            }
            else
            {
                LoadResult();
            }
        }

        public const string FileName = "Catalog.json";
        protected virtual void LoadResult()
        {
            var filePath = FormatFilePath(FileName);

            if (Core.Data.Exists(filePath))
            {
                var json = Core.Data.LoadText(filePath);

                var request = JsonConvert.DeserializeObject<GetCatalogItemsResult>(json);

                ResultCallback(request);
            }
            else
            {
                var error = new PlayFabError()
                {
                    ErrorMessage = "No Local Data Found"
                };

                ErrorCallback(error);
            }
        }
        protected virtual void SaveResult(GetCatalogItemsResult request)
        {
            var json = JsonConvert.SerializeObject(request, Formatting.Indented);

            Core.Data.Save(FormatFilePath(FileName), json);
        }

        public delegate void ResultDelegate(PlayFabCatalogCore catalog);
        public event ResultDelegate OnRetrieved;
        void ResultCallback(GetCatalogItemsResult result)
        {
            if (IsLoggedIn)
            {
                SaveResult(result);
            }

            Items = result.Catalog;

            if (OnRetrieved != null) OnRetrieved(this);

            Respond(null);
        }

        public delegate void ErrorDelegate(PlayFabError error);
        public event ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(error);
        }

        public delegate void ResponseCallback(PlayFabCatalogCore catalog, PlayFabError error);
        public event ResponseCallback OnResponse;
        void Respond(PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
        #endregion
    }
}