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

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Level")]
	public class LevelCore : RegionCore.Module
	{
        public bool Unlocked
        {
            get
            {
                return Region.Progress > Index;
            }
        }

        public virtual void Unlock()
        {
            Region.Unlock(this);
        }

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField]
        protected GameScene scene;
        public GameScene Scene { get { return scene; } }

        [SerializeField]
        protected LevelBackgroundData background;
        public LevelBackgroundData Background { get { return background; } }

        public RegionCore Region { get; protected set; }
        public void Set(RegionCore region)
        {
            this.Region = region;

            this.Index = region.IndexOf(this);
        }

        public Level Instance { get { return Level.Instance; } }

        public int Index { get; protected set; }

        public virtual bool IsFirst { get { return Index == 0; } }
        public virtual bool IsLast { get { return Index >= Region.Size - 1; } }

        public virtual LevelCore Previous
        {
            get
            {
                if (IsFirst) return null;

                return Region[Index - 1];
            }
        }
        public virtual LevelCore Next
        {
            get
            {
                if (IsLast) return null;

                return Region[Index + 1];
            }
        }

        public virtual void Complete()
        {
            if (IsLast)
            {
                Region.Complete();
            }
            else
            {
                Next.Unlock();
            }
        }

        public virtual void Load()
        {
            Region.Load(this);
        }
        public virtual void Reload()
        {
            Load();
        }
    }
}