using System.Diagnostics;
using System.Reflection;
using TextGameLib.Util;

namespace TextGameLib.Core;

/// <summary>
/// 主要的游戏基类
/// </summary>
public abstract class MainBehavior
{
    protected event MessageSend? SendMessageEvent;

    public delegate void MessageSend(string msg, long gID);

    /// <summary>
    /// 输入玩家的操作字符串，返回对应的输出
    /// </summary>
    public abstract string InputAndOutput(List<string> input, long userID, long groupID);

    /// <summary>
    /// 返回操作教程等内容
    /// </summary>
    public abstract string GetHelpContent(int page = 0);

    /// <summary>
    /// 关闭游戏后清理资源与内存等，需要手动清空一些数据
    /// </summary>
    public abstract string Close();

    public void SubscribeMessageSender(MessageSend sender)
    {
        SendMessageEvent += sender;
    }

    public void UnsubscribeMessageSender(MessageSend sender)
    {
        SendMessageEvent -= sender;
    }

    public abstract void SetRootPath(string path);

    internal void SendMessage(string msg, long gID)
    {
        SendMessageEvent?.Invoke(msg, gID);
    }

    #region DEBUG

    [Conditional("DEBUG")]
    public static void ValidateInheritance()
    {
        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .Where(t => typeof(MainBehavior).IsAssignableFrom(t) && t != typeof(MainBehavior));

        if (allTypes.Count() > 1)
            throw new InvalidOperationException("只应该存在一个 MainBehavior 的继承类");

        var attri = allTypes.First().GetCustomAttribute<MainBehaviorAttribute>() ?? throw new InvalidOperationException("MainBehavior 的继承类必须有 MainBehaviorAttribute 特性");
    }

    #endregion DEBUG
}