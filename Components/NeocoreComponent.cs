using Microsoft.AspNetCore.Components;

namespace Neocore.Components;

public abstract class NeocoreComponent : ComponentBase
{
    [Inject]
    protected IHttpContextAccessor? HttpContextAccessor { get; set; }

    protected bool IsPrerendering => 
        HttpContextAccessor?.HttpContext?.Response.HasStarted == false;

    protected bool EnableLifecycleLogging { get; set; } = false;

    protected override void OnInitialized() 
    { 
        if (EnableLifecycleLogging)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            LogLifecycle("OnInitialized");
            Console.ResetColor();
        }

        base.OnInitialized();
    }

    protected override void OnParametersSet() 
    { 
        if (EnableLifecycleLogging)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            LogLifecycle("OnParametersSet");
            Console.ResetColor();
        }

        base.OnParametersSet();
    }

    protected override void OnAfterRender(bool firstRender) 
    { 
        if (EnableLifecycleLogging)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            LogLifecycle($"OnAfterRender (FirstRender: {firstRender})");
            Console.ResetColor();
        }

        base.OnAfterRender(firstRender);
    }

    private void LogLifecycle(string methodName)
    {
        Console.WriteLine($"{GetType().Name} - {methodName}");
    }
}
