using Util;

namespace Game.CustomPool
{
	
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class PoolManager : Singleton<PoolManager>
	{
		private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;
		
		private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup;

		private GameObject rootTransform;

		private bool dirty = false;
		
		public Transform root => rootTransform.transform;

		private void OnEnable()
		{
			prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
			instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
			rootTransform = new GameObject();
			
			UnityEngine.Object.DontDestroyOnLoad(rootTransform);
			
#if UNITY_EDITOR
			rootTransform.name = "PoolRoot";
#endif
		}


		public void warmPool(GameObject prefab, int size)
		{
			if (prefabLookup.ContainsKey(prefab))
			{
				throw new Exception("Pool for prefab " + prefab.name + " has already been created");
			}

			var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab); }, size);
			prefabLookup[prefab] = pool;

			dirty = true;
		}

		public GameObject SpawnObject(GameObject prefab)
		{
			return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
		}

		public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if (!prefabLookup.ContainsKey(prefab))
			{
				WarmPool(prefab, 1);
			}

			var pool = prefabLookup[prefab];

			var clone = pool.GetItem();
			
			if (clone == null)
			{
				UnityEngine.Debug.LogWarning($"из пула был уничтожен объект, созданный от {prefab.gameObject.name}!!!");
				clone = pool.RepairLastItem();
			}

			clone.transform.SetPositionAndRotation(position, rotation);
			
			clone.SetActive(true);
			
			if (parent != null) clone.transform.SetParent(parent);

			instanceLookup.Add(clone, pool);
			
			dirty = true;
			
			return clone;
		}

		/// <summary>
		/// Деактивировать объект не возвращая в пул. Для префабов на основе уже созданных объектов
		/// </summary>
		/// <param name="clone"></param>
		public void DeactiveObject(GameObject clone)
		{
			clone.SetActive(false);
			clone.transform.SetParent(root);
			clone.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		}

		public bool ReleaseObject(GameObject clone)
		{
			clone.SetActive(false);
			
			clone.transform.SetParent(root);
			
			clone.transform.position = Vector3.zero;
			clone.transform.rotation = Quaternion.identity;
			clone.transform.localScale = Vector3.one;
			
			if (instanceLookup.ContainsKey(clone))
			{
				
				instanceLookup[clone].ReleaseItem(clone);
				
				instanceLookup.Remove(clone);
				
				dirty = true;
				
				return true;
			}
			else
			{
				Debug.LogWarning("No pool contains the object: " + clone.name);
				return false;
			}
		}


		private GameObject InstantiatePrefab(GameObject prefab)
		{
			var go = UnityEngine.Object.Instantiate(prefab) as GameObject;
			if (root != null) go.transform.parent = root;
			return go;
		}

		public void PrintStatus()
		{
			foreach (KeyValuePair<GameObject, ObjectPool<GameObject>> keyVal in prefabLookup)
			{
				Debug.Log(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name,
					keyVal.Value.CountUsedItems, keyVal.Value.Count));
			}
		}
		
		public void ClearAllObjectsFromPool()
		{
			foreach (var kv in instanceLookup)
			{
				instanceLookup[kv.Key].Clear();
				
				UnityEngine.Object.Destroy(kv.Key);
			}
			
			instanceLookup.Clear();
			
			foreach (var kv in prefabLookup)
			{
				prefabLookup[kv.Key].Clear();
			}
			
			prefabLookup.Clear();
		}


		public int GetCountSize(GameObject prefab)
		{
			if (prefabLookup.ContainsKey(prefab))
			{
				return 0;
			}

			var pool = prefabLookup[prefab];

			return pool.Count;
		}

		public int GetAllCountSize()
		{
			if (prefabLookup == null)
			{
				return 0;
			}
			else
			{
				int allCount = 0;
				var keys = prefabLookup.Keys.ToList();
				for (int i = 0; i < keys.Count; i++)
				{
					allCount += prefabLookup[keys[i]].Count;
				}

				return allCount;
			}
		}
		
		public void WarmPool(GameObject prefab, int size)
		{
			warmPool(prefab, size);
		}

	}
}


