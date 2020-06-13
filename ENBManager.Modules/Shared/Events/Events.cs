using ENBManager.Infrastructure.BusinessEntities;
using Prism.Events;

namespace ENBManager.Modules.Shared.Events
{
    public class ShowSnackbarMessageEvent : PubSubEvent<string> { }
    public class ModuleActivatedEvent : PubSubEvent<GameModule> { }
    public class ScreenshotsStatusChangedModuleEvent : PubSubEvent<bool> { }
    public class ScreenshotsStatusChangedExternalEvent : PubSubEvent<bool> { }
}
