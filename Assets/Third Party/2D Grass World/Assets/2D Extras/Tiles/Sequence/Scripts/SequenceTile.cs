using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityEngine.Tilemaps
{
	[Serializable]
	public class SequenceTile : Tile
	{
		[SerializeField]
		public Sprite[] m_Sprites;
        private int currentSequence = 0;

		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			if ((m_Sprites != null) && (m_Sprites.Length > 0))
			{
                if (currentSequence >= m_Sprites.Length)
                    currentSequence = 0;

				tileData.sprite = m_Sprites[currentSequence];
                currentSequence++;
			}
		}

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Sequence Tile")]
		public static void CreateSequenceTile()
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Sequence Tile", "New Sequence Tile", "asset", "Save Sequence Tile", "Assets");

			if (path == "")
				return;

			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SequenceTile>(), path);
		}
#endif
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(SequenceTile))]
	public class SequenceTileEditor : Editor
	{
		private SequenceTile tile { get { return (target as SequenceTile); } }

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			int count = EditorGUILayout.DelayedIntField("Number of Sprites", tile.m_Sprites != null ? tile.m_Sprites.Length : 0);
			if (count < 0)
				count = 0;
			if (tile.m_Sprites == null || tile.m_Sprites.Length != count)
			{
				Array.Resize<Sprite>(ref tile.m_Sprites, count);
			}

			if (count == 0)
				return;

			EditorGUILayout.LabelField("Place sequence sprites.");
			EditorGUILayout.Space();

			for (int i = 0; i < count; i++)
			{
				tile.m_Sprites[i] = (Sprite) EditorGUILayout.ObjectField("Sprite " + (i+1), tile.m_Sprites[i], typeof(Sprite), false, null);
			}		
			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(tile);
		}
	}
#endif
}
