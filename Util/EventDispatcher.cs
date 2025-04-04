using System.Diagnostics;
using System.Reflection;
using TextGameLib.Interfaces;

namespace TextGameLib.Util;

/// <summary>
/// 事件管理类
/// </summary>
public static class EventDispatcher
{
    private static readonly Dictionary<int, List<Delegate>> m_EventTable = new();

    /// <summary>
    /// 添加无参数事件
    /// </summary>
    /// <param name="eventType">事件的ID</param>
    /// <param name="callBack">事件</param>
    public static void RegisterEvent(int eventType, Action callBack)
    {
        if (OnListenerAdding(eventType, callBack))
        {
            if (!m_EventTable[eventType].Contains(callBack))
                m_EventTable[eventType].Add(callBack);
        }
    }

    /// <summary>
    /// 添加单参数事件
    /// </summary>
    public static void RegisterEvent<T>(int eventType, Action<T> callBack)
    {
        if (OnListenerAdding(eventType, callBack))
        {
            if (!m_EventTable[eventType].Contains(callBack))
                m_EventTable[eventType].Add(callBack);
        }
    }

    /// <summary>
    /// 添加双参数事件
    /// </summary>
    public static void RegisterEvent<T1, T2>(int eventType, Action<T1, T2> callBack)
    {
        if (OnListenerAdding(eventType, callBack))
        {
            if (!m_EventTable[eventType].Contains(callBack))
                m_EventTable[eventType].Add(callBack);
        }
    }

    /// <summary>
    /// 添加三参数事件
    /// </summa
    public static void RegisterEvent<T1, T2, T3>(int eventType, Action<T1, T2, T3> callBack)
    {
        if (OnListenerAdding(eventType, callBack))
        {
            if (!m_EventTable[eventType].Contains(callBack))
                m_EventTable[eventType].Add(callBack);
        }
    }

    /// <summary>
    /// 移除无参数事件
    /// </summary>
    public static void RemoveEvent(int eventType, Action callBack)
    {
        m_EventTable[eventType].Remove(callBack);

        // 事件的挂载频繁，不做移除处理
        //OnListenerRemoved(eventType);
    }

    /// <summary>
    /// 移除单参数事件
    /// </summary>
    public static void RemoveEvent<T>(int eventType, Action<T> callBack)
    {
        m_EventTable[eventType].Remove(callBack);
    }

    /// <summary>
    /// 移除双参数事件
    /// </summary>
    public static void RemoveEvent<T1, T2>(int eventType, Action<T1, T2> callBack)
    {
        m_EventTable[eventType].Remove(callBack);
    }

    /// <summary>
    /// 移除三参数事件
    /// </summary>
    public static void RemoveEvent<T1, T2, T3>(int eventType, Action<T1, T2, T3> callBack)
    {
        m_EventTable[eventType].Remove(callBack);
    }

    /// <summary>
    /// 对某个事件进行完全移除
    /// </summary>
    /// <param name="eventType"></param>
    public static void RemoveAll(int eventType)
    {
        if (m_EventTable.ContainsKey(eventType))
            m_EventTable[eventType] = null;
    }

    /// <summary>
    /// 调用无参数事件，倒序比较安全
    /// </summary>
    public static void FireEvent(int eventType)
    {
        if (m_EventTable.TryGetValue(eventType, out var list))
        {
            var cache = list.ToArray(); // TODO 优化，List._version不给访问
            for (int i = cache.Length - 1; i >= 0; i--)
            {
                if (cache[i] is Action action)
                {
                    action();
                }
            }
        }
    }

    /// <summary>
    /// 调用单参数事件
    /// </summary>
    public static void FireEvent<T>(int eventType, T args)
    {
        if (m_EventTable.TryGetValue(eventType, out var list))
        {
            var cache = list.ToArray();
            for (int i = cache.Length - 1; i >= 0; i--)
            {
                if (cache[i] is Action<T> actionT)
                {
                    actionT(args);
                }
            }
        }
    }

    /// <summary>
    /// 调用双参数事件
    /// </summary>
    public static void FireEvent<T1, T2>(int eventType, T1 arg1, T2 arg2)
    {
        if (m_EventTable.TryGetValue(eventType, out var list))
        {
            var cache = list.ToArray();
            for (int i = cache.Length - 1; i >= 0; i--)
            {
                if (cache[i] is Action<T1, T2> actionTT)
                {
                    actionTT(arg1, arg2);
                }
            }
        }
    }

    /// <summary>
    /// 调用三参数事件
    /// </summary>
    public static void FireEvent<T1, T2, T3>(int eventType, T1 arg1, T2 arg2, T3 arg3)
    {
        if (m_EventTable.TryGetValue(eventType, out var list))
        {
            var cache = list.ToArray();
            for (int i = cache.Length - 1; i >= 0; i--)
            {
                if (cache[i] is Action<T1, T2, T3> actionTTT)
                {
                    actionTTT(arg1, arg2, arg3);
                }
            }
        }
    }

    /// <summary>
    /// 尝试添加事件监听的时候调用，防止出现异常，当然，只有编辑器时期会执行
    /// </summary>
    /// <returns>是否满足添加事件监听的条件</returns>
    private static bool OnListenerAdding(int eventType, Delegate handler)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, []);
        }
        List<Delegate> list = m_EventTable[eventType];
        if (list == null)
            return false;
        return true;
    }
}