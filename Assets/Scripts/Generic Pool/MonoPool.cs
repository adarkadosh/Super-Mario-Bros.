using System.Collections.Generic;
using UnityEngine;


public class MonoPool<T> : MonoSingleton<MonoPool<T>>where T : MonoBehaviour, IPoolable
{
    [SerializeField] private T prefab;
    [SerializeField] private int initialSize;
    [SerializeField] private Transform parent;

    protected Stack<T> Available;
    private int _activeCount;

    private void Awake()
    {
        Available = new Stack<T>();
        AddItemsToPool();
    }

    private void AddItemsToPool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var obj = Instantiate(prefab, parent, true);
            obj.gameObject.SetActive(false);
            Available.Push(obj);
        }
    }

    public T Get()
    {
        if (Available.Count == 0)
            AddItemsToPool();
        var obj = Available.Pop();
        obj.gameObject.SetActive(true);
        obj.Reset();
        _activeCount++;
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        Available.Push(obj);
        _activeCount--;
    }

    public int CountActive => _activeCount;
    
}