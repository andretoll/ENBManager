using ENBManager.Core.BusinessEntities.Base;
using Prism.Events;

namespace ENBManager.Core.Events
{
    public class GameSelectedEvent : PubSubEvent<GameBase> { }
}
