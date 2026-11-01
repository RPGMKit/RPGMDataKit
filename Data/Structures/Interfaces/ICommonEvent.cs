using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Interfaces;

public interface ICommonEvent : IHasId, ICommandContainer
{

    public string Name { get; set; }
    
    public CommonEventTriggerType Trigger { get; set; }
    
    public int SwitchId { get; set; }
}