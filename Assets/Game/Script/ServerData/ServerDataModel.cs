using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServerData<T>
{
	T GetKey();
}

public interface IServerDataClear
{
	//bool IsClear(SceneType type);

	void Clear();
}

// FIXME IComparableでobjectの比較を行っていて重い処理なので調整
public class ServerDataModel <T, T2> : IServerDataClear where T : IServerData<T2> where T2 : System.IComparable
{
	// とりあえずListで Masterデータはarrayでも良さそう
	protected List<T> list = null;

	protected int count = 0;
	public int Count { get { return count; }}

	//リソースを削除するシーン
	//protected SceneType clearSceneType = SceneType.None;

	public T this[int index]
	{
		get { return list[index]; }
	}

	public bool IsClear()
	{
		return true;
	}

	/// <summary>
	/// Clear dictionary.
	/// </summary>
	public void Clear()
	{
		count = 0;
		if (list != null)
		{
			list.Clear();
			OnClear();
		}
	}

	protected virtual void OnClear(){}

	/// <summary>
	/// InsertOrUpdate items.
	/// </summary>
	public void Save(T[] items)
	{
		if (items == null)
		{
			Debug.Log("items is null");
			return;
		}
		int count = items.Length;
		// 0だった場合は確保しておく
		if (this.count <= 0 || list == null)
		{
			list = new List<T>(count);
		}
		for (int i = 0; i < count; ++i)
		{
			Save(items[i]);
		}
	}
		
	/// <summary>
	/// InsertOrUpdate
	/// </summary>
	public void Save(T item)
	{
		if (item == null)
		{
			Debug.Log("item is null");
			return;
		}
		if (Count <= 0 || list == null)
		{
			list = new List<T>(1);
		}

		T2 key = item.GetKey();
		for (int i = 0; i < Count; i++)
		{
			if (key.CompareTo(list[i].GetKey()) == 0)
			{
				OnSave(item);
				list[i] = item;
				return;
			}
		}
		list.Add(item);
		// 内部の数増やす
		count++;
		OnSave(item);
	}

	protected virtual void OnSave(T item){}
		
	/// <summary>
	/// []で取得
	/// </summary>
	/// <returns>The all.</returns>
	public T[] GetAll()
	{
        if (count <= 0)
        {
            list = new List<T>(1);
        }
        return list.ToArray();
	}

	public List<T> GetAllByList()
	{
		if (count <= 0)
		{
			list = new List<T>(1);
		}
		return list;
	}

	public IEnumerable<T> List()
	{
		return list;
	}

	/// <summary>
	/// Keyで取得
	/// </summary>
	public T Get(T2 key)
	{
		for (int i = 0; i < count; i++)
		{
			if (key.CompareTo(list[i].GetKey()) == 0)
			{
				return list[i];
			}
		}

		return default;
	}

	public T GetByIndex(int index)
	{
		if (index >= count)
		{
			return default;
		}
		return list[index];
	}

	public bool Removes(T2[] keys)
	{
		int count = keys.Length;
		for (int i = 0; i < count; ++i)
		{
			if (!Remove(keys[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool Remove(T2 key)
	{
		for (int i = 0; i < count; i++)
		{
			if (key.CompareTo(list[i].GetKey()) == 0)
			{
				OnRemove(list[i]);
				list.Remove(list[i]);
				count--;
				return true;
			}
		}
		return false;
	}

	protected virtual void OnRemove(T item){}

	/// <summary>
	/// Containses the key.
	/// </summary>
	public bool ContainsKey(T2 key)
	{
		for (int i = 0; i < count; i++)
		{
			if (key.CompareTo(list[i].GetKey()) == 0)
			{
				return true;
			}
		}
		return false;
	}
}