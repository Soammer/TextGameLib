namespace TextGameLib.Util;

[AttributeUsage(AttributeTargets.Class)]
public class MainBehaviorAttribute(string name, string version) : Attribute
{
    public string Name { get; internal set; } = name;

    public string Version { get; internal set; } = version;
}