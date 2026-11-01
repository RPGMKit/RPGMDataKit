using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IEventPage : IPage
{
    public IEventPageCondition Condition { get; }

    public IPageGraphic Graphic { get; }

    public int MoveType { get; set; }

    public int MoveSpeed { get; set; }

    public int MoveFrequency { get; set; }

    public IMoveRoute MoveRoute { get; }

    public bool WalkAnime { get; set; }

    public bool StepAnime { get; set; }

    public bool DirectionFix { get; set; }

    public bool Through { get; set; }

    public EventPriority PriorityType { get; set; }

    public EventTriggerType Trigger { get; set; }
}
