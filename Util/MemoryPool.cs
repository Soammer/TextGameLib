using System.Diagnostics;
using TextGameLib.Interfaces;

namespace TextGameLib.Util;

public static class MemoryPool
{
    private static readonly Dictionary<Type, MemoryCollection> _memoryCollections = [];
    private static bool _enableStrictCheck = false;

    /// <summary>
    /// 获取或设置是否开启强制检查。
    /// </summary>
    public static bool EnableStrictCheck
    {
        get => _enableStrictCheck;
        set => _enableStrictCheck = value;
    }

    public static int Count => _memoryCollections.Count;

    #region 公开方法

    /// <summary>
    /// 清除所有内存池。
    /// </summary>
    public static void ClearAll()
    {
        lock (_memoryCollections)
        {
            foreach (var memoryCollection in _memoryCollections)
                memoryCollection.Value.RemoveAll();

            _memoryCollections.Clear();
        }
    }

    /// <summary>
    /// 从内存池获取内存对象。
    /// </summary>
    public static T Acquire<T>() where T : class, IMemory, new()
    {
        return GetMemoryCollection(typeof(T)).Acquire<T>();
    }

    /// <summary>
    /// 从内存池获取内存对象。
    /// </summary>
    public static IMemory Acquire(Type memoryType)
    {
        InternalCheckMemoryType(memoryType);
        return GetMemoryCollection(memoryType).Acquire();
    }

    /// <summary>
    /// 将内存对象归还内存池。
    /// </summary>
    public static void Release(IMemory memory)
    {
        if (memory == null)
            throw new Exception("Memory 对象为空");

        Type memoryType = memory.GetType();
        InternalCheckMemoryType(memoryType);
        GetMemoryCollection(memoryType).Release(memory);
    }

    public static void Add<T>(int count) where T : class, IMemory, new()
    {
        GetMemoryCollection(typeof(T)).Add<T>(count);
    }

    public static void Add(Type memoryType, int count)
    {
        InternalCheckMemoryType(memoryType);
        GetMemoryCollection(memoryType).Add(count);
    }

    public static void Remove<T>(int count) where T : class, IMemory
    {
        GetMemoryCollection(typeof(T)).Remove(count);
    }

    public static void Remove(Type memoryType, int count)
    {
        InternalCheckMemoryType(memoryType);
        GetMemoryCollection(memoryType).Remove(count);
    }

    public static void RemoveAll<T>() where T : class, IMemory
    {
        GetMemoryCollection(typeof(T)).RemoveAll();
    }

    public static void RemoveAll(Type memoryType)
    {
        InternalCheckMemoryType(memoryType);
        GetMemoryCollection(memoryType).RemoveAll();
    }

    public static void RemoveMemoryCollections<T>() where T : class, IMemory
    {
        RemoveMemoryCollections(typeof(T));
    }

    public static void RemoveMemoryCollections(Type memoryType)
    {
        InternalCheckMemoryType(memoryType);
        lock (_memoryCollections)
        {
            if (_memoryCollections.TryGetValue(memoryType, out var memoryCollection))
            {
                memoryCollection.RemoveAll();
                _memoryCollections.Remove(memoryType);
            }
        }
    }

    #endregion 公开方法

    #region 私有方法

    /// <summary>
    /// 检查类型
    /// </summary>
    private static void InternalCheckMemoryType(Type memoryType)
    {
        if (!_enableStrictCheck)
            return;

        Debug.Assert(memoryType != null && memoryType.IsClass, "Type is not Class or invalid");
        Debug.Assert(!memoryType.IsAbstract, "Memory type is an abstract class type.");
    }

    private static MemoryCollection GetMemoryCollection(Type memoryType)
    {
        MemoryCollection? memoryCollection;
        lock (_memoryCollections)
        {
            if (!_memoryCollections.TryGetValue(memoryType, out memoryCollection))
            {
                memoryCollection = new MemoryCollection(memoryType);
                _memoryCollections.Add(memoryType, memoryCollection);
            }
        }

        return memoryCollection;
    }

    #endregion 私有方法

    /// <summary>
    /// 内存池收集器。
    /// </summary>
    internal sealed class MemoryCollection(Type memoryType)
    {
        private readonly Queue<IMemory> _memories = new();
        private readonly Type _memoryType = memoryType;


        /// <summary>
        /// 请求一块内存对象
        /// </summary>
        public T Acquire<T>() where T : IMemory, new()
        {
            if (typeof(T) != _memoryType)
                throw new InvalidOperationException("请求的内存类型与收集器类型不匹配");

            lock (_memories)
            {
                if (_memories.Count > 0)
                    return (T)_memories.Dequeue();
            }

            return new T();
        }

        public IMemory Acquire()
        {
            lock (_memories)
            {
                if (_memories.Count > 0)
                    return _memories.Dequeue();
            }

            return (Activator.CreateInstance(_memoryType) as IMemory)!;
        }

        /// <summary>
        /// 释放（回收）一块内存对象
        /// </summary>
        /// <param name="memory"></param>
        public void Release(IMemory memory)
        {
            memory.Clear();
            lock (_memories)
            {
                if (_enableStrictCheck && _memories.Contains(memory))
                    throw new InvalidOperationException("内存对象已经存在于内存池中");

                _memories.Enqueue(memory);
            }
        }

        /// <summary>
        /// 增加一块新的对象
        /// </summary>
        public void Add<T>(int count) where T : class, IMemory, new()
        {
            if (typeof(T) != _memoryType)
                throw new InvalidOperationException("请求的内存类型与收集器类型不匹配");

            lock (_memories)
            {
                while (count-- > 0)
                    _memories.Enqueue(new T());
            }
        }

        public void Add(int count)
        {
            lock (_memories)
            {
                while (count-- > 0)
                    _memories.Enqueue((IMemory)Activator.CreateInstance(_memoryType)!);
            }
        }

        /// <summary>
        /// 移除内存对象
        /// </summary>
        public void Remove(int count)
        {
            lock (_memories)
            {
                if (count > _memories.Count)
                    count = _memories.Count;

                while (count-- > 0)
                    _memories.Dequeue();
            }
        }

        public void RemoveAll()
        {
            lock (_memories)
            {
                _memories.Clear();
            }
        }
    }
}

