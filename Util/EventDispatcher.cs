using TextGameLib.Interfaces;

namespace TextGameLib.Util;

/// <summary>
/// 局部事件分发器
/// </summary>
public class EventDispatcher : IMemory
{
    private readonly Dictionary<int, List<EventRegInfo>> _allEventListenerDic;

    public EventDispatcher()
    {
        _allEventListenerDic = [];
    }

    public void AddEventListener(int eventId, Action eventCallback)
    {
        AddEventListenerImp(eventId, eventCallback);
    }

    public void AddEventListener<T>(int eventId, Action<T> eventCallback)
    {
        AddEventListenerImp(eventId, eventCallback);
    }

    public void AddEventListener<T1, T2>(int eventId, Action<T1, T2> eventCallback)
    {
        AddEventListenerImp(eventId, eventCallback);
    }

    public void AddEventListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> eventCallback)
    {
        AddEventListenerImp(eventId, eventCallback);
    }

    /// <summary>
    /// 移除所有事件监听。
    /// </summary>
    public void DestroyAllEventListener()
    {
        foreach (var kv in _allEventListenerDic)
        {
            foreach (var eventRegInfo in kv.Value)
            {
                EventRegInfo.Release(eventRegInfo);
            }
            kv.Value.Clear();
        }
        _allEventListenerDic.Clear();
    }

    /// <summary>
    /// 删除监听
    /// </summary>
    public void RemoveEventListener(int eventID, Delegate listener)
    {
        if (_allEventListenerDic.TryGetValue(eventID, out var listenerList))
        {
            foreach (var node in listenerList)
            {
                if (node.Callback == listener)
                {
                    node.IsDeleted = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 增加事件监听具体实现。
    /// </summary>
    private void AddEventListenerImp(int eventId, Delegate listener)
    {
        if (!_allEventListenerDic.TryGetValue(eventId, out var listenerList))
        {
            listenerList = [];
            _allEventListenerDic.Add(eventId, listenerList);
        }

        var existNode = listenerList.Find(node => node.Callback == listener);
        if (existNode != null)
        {
            existNode.IsDeleted = false;
            return;
        }

        listenerList.Add(EventRegInfo.Alloc(listener));
    }

    public void Clear()
    {
        DestroyAllEventListener();
    }

    public class EventRegInfo : IMemory
    {
        /// <summary>
        /// 事件回调
        /// </summary>
        public Delegate? Callback;

        /// <summary>
        /// 事件是否删除
        /// </summary>
        public bool IsDeleted;

        public EventRegInfo(Delegate callback)
        {
            Callback = callback;
            IsDeleted = false;
        }

        public EventRegInfo()
        { }

        public void Clear()
        {
            IsDeleted = false;
        }

        public static EventRegInfo Alloc(Delegate callback)
        {
            EventRegInfo ret = MemoryPool.Acquire<EventRegInfo>();
            ret.Callback = callback;
            ret.IsDeleted = false;
            return ret;
        }

        public static void Release(EventRegInfo eventRegInfo)
        {
            MemoryPool.Release(eventRegInfo);
        }
    }
}