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

namespace Game
{
    [CreateAssetMenu(menuName = PlayFabCore.MenuPath + "Catalog")]
    public class PlayFabCatalog : PlayFabCore.CatalogsCore.Module
	{
        public RequestHandler GetRequest;
        public class RequestHandler : PlayFabRequestHandler<GetCatalogItemsRequest, GetCatalogItemsResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetCatalogItems;

            public virtual void Send(string version)
            {
                var request = CreateRequest();

                request.CatalogVersion = version;

                Send(request);
            }
        }

        public virtual string Version { get { return name; } }

        public List<CatalogItem> Items { get { return GetRequest.Latest.Catalog; } }

        public virtual bool Valid { get { return Items != null; } }

        public int Size { get { return Items.Count; } }

        public CatalogItem this[int index] { get { return Items[index]; } }

        public override void Configure()
        {
            base.Configure();

            GetRequest = new RequestHandler();
            GetRequest.OnResponse += ResponseCallback;
            GetRequest.OnResult += RetrieveCallback;
        }

        public virtual void Request()
        {
            GetRequest.Send(Version);
        }

        public event PlayFabRequestsUtility.ResponseDelegate<PlayFabCatalog> OnResponse;
        void ResponseCallback(GetCatalogItemsResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }

        public event PlayFabRequestsUtility.ResaultDelegate<PlayFabCatalog> OnRetrieved;
        void RetrieveCallback(GetCatalogItemsResult result)
        {
            if (OnRetrieved != null) OnRetrieved(this);
        }
    }
}