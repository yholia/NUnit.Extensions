using System.Collections.Concurrent;
using NUnit.Engine;
using NUnit.Engine.Extensibility;

namespace NUnit.Extensions.ResourceReaper;

[Extension]
public class ResourceReaper : ITestEventListener
{
    private static readonly ConcurrentStack<Action> AfterActions = new();

    static ResourceReaper()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => InvokeActions();
    }

    public static void ScheduleAction(Action action)
    {
        AfterActions.Push(action);
    }

    /// <summary>
    /// How to read report:
    /// <code>var xmlDocument = new XmlDocument();
    /// xmlDocument.LoadXml(report);
    /// var attribute = xmlDocument.FirstChild?.Attributes?["fullname"];</code>
    /// </summary>
    /// <param name="report"></param>
    public void OnTestEvent(string report)
    {
        if (report.StartsWith("<test-suite"))
        {
            InvokeActions();
        }
    }

    private static void InvokeActions()
    {
        while (!AfterActions.IsEmpty)
        {
            AfterActions.TryPop(out var action);
            action?.Invoke();
        }
    }
}
