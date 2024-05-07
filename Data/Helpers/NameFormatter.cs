using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Helpers;

public class NameFormatter
{
    private readonly EntityContext _context;
    private readonly bool _extendedFormat;

    public NameFormatter(EntityContext context, bool extendedFormat)
    {
        _context = context;
        _extendedFormat = extendedFormat;
    }

    private string FormatSimple(int id, string name, bool brackets = true, string literal = "", int length = 3)
    {
        if (literal != "")
            literal += ":";

        var idFormatted = _extendedFormat ? string.Format("{0:D" + length + "}", id) : id.ToString();

        if (_context.Version == StorageVersion.VXAce)
            return _extendedFormat ? $"[{idFormatted}:{literal}{name}]" : (brackets ? $"[{name}]" : name);
        else
            return _extendedFormat ? $"[{idFormatted}:{literal}{name}]" : name;
    }

    public string FormatActor(int actorId)
    {
        var actor = _context.Storage.Actors.Get().ElementAt(actorId);

        return FormatSimple(actorId, actor?.Name ?? "");
    }

    public string FormatAnimation(int animationId)
    {
        var animation = _context.Storage.Animations.Get().ElementAt(animationId);

        return FormatSimple(animationId, animation?.Name ?? "");
    }

    public string FormatArmor(int armorId)
    {
        var armor = _context.Storage.Armors.Get().ElementAt(armorId);

        return FormatSimple(armorId, armor?.Name ?? "", literal: "A");
    }

    public string FormatClass(int classId)
    {
        var @class = _context.Storage.Classes.Get().ElementAt(classId);

        return FormatSimple(classId, @class?.Name ?? "");
    }

    public string FormatCommonEvent(int commonEventId)
    {
        var commonEvent = _context.Storage.GetCommonEvent(commonEventId);

        return FormatSimple(commonEventId, commonEvent?.Name ?? "", length: 4);
    }

    public string FormatItem(int itemId)
    {
        var item = _context.Storage.Items.Get().ElementAt(itemId);

        return FormatSimple(itemId, item?.Name ?? "");
    }

    public string FormatSkill(int skillId)
    {
        var skill = _context.Storage.Skills.Get().ElementAt(skillId);

        return FormatSimple(skillId, skill?.Name ?? "");
    }

    public string FormatState(int stateId)
    {
        var state = _context.Storage.States.Get().ElementAt(stateId);

        return FormatSimple(stateId, state?.Name ?? "");
    }

    public string FormatWeapon(int weaponId)
    {
        var weapon = _context.Storage.Weapons.Get().ElementAt(weaponId);

        return FormatSimple(weaponId, weapon?.Name ?? "", literal: "W");
    }

    public string FormatSwitch(int switchId)
    {
        var sw = _context.Storage.GetSwitch(switchId);

        return FormatSimple(switchId, sw, length: 4);
    }

    public string FormatVariable(int variableId)
    {
        var v = _context.Storage.GetVariable(variableId);

        return FormatSimple(variableId, v, length: 4);
    }
}
