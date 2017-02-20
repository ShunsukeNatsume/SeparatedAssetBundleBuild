using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UTJ
{
    /// <summary>
    /// AssetBundleManifest
    /// </summary>
	public class AssetBundleBuildsManifest : ScriptableObject, ISerializationCallbackReceiver
    {
		private List<string> allAssetBundles = new List<string>();
        private Dictionary<string, string[]> allDependencies = new Dictionary<string, string[]>();
        private Dictionary<string, Hash128> allHash = new Dictionary<string, Hash128>();

		//==For Serialize==
		[System.Serializable]
		public class Serialize{
			public string baseAssetName;
			public string hash;
			public string[] dependencies;
			public Serialize(string assetName, string[] dependencies, Hash128 hash128){
				this.baseAssetName = assetName;
				this.dependencies = dependencies;
				this.hash = hash128.ToString();
			}
		}
		[SerializeField] private Serialize[] serializes = {};
		//=================

		private void Awake(){
			for(int i = 0; i < serializes.Length; i++){
				allAssetBundles.Add(serializes[i].baseAssetName);
				allDependencies.Add(serializes[i].baseAssetName,serializes[i].dependencies);
				allHash.Add(serializes[i].baseAssetName, Hash128.Parse(serializes[i].hash));
			}
			this.serializes = null;
		}

        public string[] GetAllAssetBundles()
        {			
            return allAssetBundles.ToArray();
        }
        public string[] GetAllDependencies(string name)
        {
            string[] val = null;
            allDependencies.TryGetValue(name, out val);
            return val;
        }
        public Hash128 GetAssetBundleHash(string name)
        {			
            Hash128 hash = new Hash128();
            allHash.TryGetValue(name, out hash);
            return hash;
        }

        public void AddUnityManifest(UnityEngine.AssetBundleManifest manifest)
        {
            if (manifest == null) { return; }
            string[] inOriginList = manifest.GetAllAssetBundles();
            foreach (var origin in inOriginList)
            {
                allAssetBundles.Add(origin);
                allDependencies.Add(origin, manifest.GetAllDependencies(origin));
                allHash.Add(origin, manifest.GetAssetBundleHash(origin));
            }
        }

		public AssetBundleBuildsManifest Copy()
		{
			var copy = ScriptableObject.CreateInstance<AssetBundleBuildsManifest>();
			copy.allAssetBundles = this.allAssetBundles;
			copy.allDependencies = this.allDependencies;
			copy.allHash = this.allHash;
			return copy;
		}

		#region ISerializationCallbackReceiver implementation
		public void OnBeforeSerialize ()
		{
			List<Serialize> list = new List<Serialize>();
			for(int i = 0; i < allAssetBundles.Count; i++){
				string name = allAssetBundles[i];
				list.Add ( new Serialize( name, this.GetAllDependencies(name), GetAssetBundleHash(name)) );
			}
			this.serializes = list.ToArray();
		}
		public void OnAfterDeserialize ()
		{
		}
		#endregion

    }
}