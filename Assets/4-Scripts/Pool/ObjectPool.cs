namespace Game.CustomPool
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	public class ObjectPool<T> where T : UnityEngine.Object
	{
		private List<ObjectPoolContainer<T>> list;
		
		private Dictionary<T, ObjectPoolContainer<T>> lookup;
		
		private Func<T> factoryFunc;
		
		private int lastIndex = 0;

		public ObjectPool(Func<T> factoryFunc, int initialSize)
		{
			this.factoryFunc = factoryFunc;

			list = new List<ObjectPoolContainer<T>>(initialSize);
			lookup = new Dictionary<T, ObjectPoolContainer<T>>(initialSize);

			Warm(initialSize);
		}

		private void Warm(int capacity)
		{
			for (int i = 0; i < capacity; i++)
			{
				CreateContainer();
			}
		}

		private ObjectPoolContainer<T> CreateContainer()
		{
			var container = new ObjectPoolContainer<T>();
			
			container.Item = factoryFunc();
			
			list.Add(container);
			
			return container;
		}

		public T RepairLastItem()
		{
			var container = lookup.Last().Value;
			container.Item = factoryFunc();
			
			if (container.Item is GameObject go)
				Debug.LogException(new Exception($"$Пул объект:{go.name} был восстановлен")); 
			else 
				Debug.LogException(new Exception($"$Пул объект:??? был восстановлен"));

			return container.Item;
		}

		public T GetItem()
		{
			ObjectPoolContainer<T> container = null;
			for (int i = 0; i < list.Count; i++)
			{
				lastIndex++;
				if (lastIndex > list.Count - 1) lastIndex = 0;
				
				if (list[lastIndex].Used)
				{
					continue;
				}
				else
				{
					container = list[lastIndex];
					break;
				}
			}

			if (container == null)
			{
				container = CreateContainer();
			}

			container.Consume();
			
			lookup.Add(container.Item, container);
			
			return container.Item;
		}

		public void ReleaseItem(object item)
		{
			ReleaseItem((T) item);
		}

		public void ReleaseItem(T item)
		{
			if (lookup.ContainsKey(item))
			{
				var container = lookup[item];
				
				container.Release();
				
				lookup.Remove(item);
			}
			else
			{
				Debug.LogWarning("This object pool does not contain the item provided: " + item);
			}
		}

		public void Clear()
		{
			foreach (var kv in lookup)
			{
				if(kv.Value.Item != null) UnityEngine.Object.Destroy(kv.Value.Item);
			}

			foreach (var poolContainer in list)
			{
				if (poolContainer.Item != null) UnityEngine.Object.Destroy(poolContainer.Item);
			}
		}

		public int Count => list.Count;
		
		public int CountUsedItems => lookup.Count;
	}
}
