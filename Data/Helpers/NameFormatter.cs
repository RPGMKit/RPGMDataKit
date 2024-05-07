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

    private BracketType BracketsForVersion =>
        _context.Version == StorageVersion.MV ? BracketType.None : BracketType.Square;

    private FormatOptions InitOptions(FormatOptions? opt)
    {
        opt ??= new();

        opt.BracketType ??= BracketsForVersion;
        if (_extendedFormat && opt.HasExtendedFormat)
        {
            opt.SkipId = false;
            opt.SkipName = false;
            opt.Prefix = "";
            opt.Separator = ":";
        }
        else
        {
            opt.SkipId ??= true;
            opt.SkipName ??= false;
            opt.Literal = "";
        }

        opt.IdLength ??= 3;

        return opt;
    }

    private string FormatSimple(int id, string? name, FormatOptions? opt = null)
    {
        opt = InitOptions(opt);

        var doExtended = _extendedFormat && opt.HasExtendedFormat;

        if (string.IsNullOrEmpty(name))
        {
            name = "?";
            opt.SkipId = false;
            if (!doExtended)
                opt.Prefix = "#";
        }

        List<string> parts = new();

        if (!opt.SkipId!.Value)
            parts.Add(string.Format("{0:D" + opt.IdLength + "}", id));
        if (doExtended && opt.Literal != "")
            parts.Add(opt.Literal);
        if (!opt.SkipName!.Value && !string.IsNullOrEmpty(name))
            parts.Add(name);

        var str = string.Join(opt.Separator, parts);

        if (doExtended || opt.BracketType != BracketType.None)
        {
            if (opt.BracketType == BracketType.None || opt.BracketType == BracketType.Square)
                str = $"[{str}]";
            else if (opt.BracketType == BracketType.Figure)
                str = $"{{{str}}}";
        }

        if (opt.Prefix != "")
            str = opt.Prefix + str;

        return str;
    }

    public string FormatActor(int actorId)
    {
        var actor = _context.Storage.Actors.Get().Get(actorId);

        return FormatSimple(actorId, actor?.Name);
    }

    public string FormatAnimation(int animationId)
    {
        var animation = _context.Storage.Animations.Get().Get(animationId);

        return FormatSimple(animationId, animation?.Name ?? "");
    }

    public string FormatArmor(int armorId)
    {
        var armor = _context.Storage.Armors.Get().Get(armorId);

        return FormatSimple(armorId, armor?.Name, new() { Literal = "A" });
    }

    public string FormatClass(int classId)
    {
        var @class = _context.Storage.Classes.Get().Get(classId);

        return FormatSimple(classId, @class?.Name);
    }

    public string FormatCommonEvent(int commonEventId)
    {
        var commonEvent = _context.Storage.GetCommonEvent(commonEventId);

        return FormatSimple(commonEventId, commonEvent?.Name);
    }

    public string FormatEnemy(int enemyId)
    {
        var enemy = _context.Storage.Enemies.Get().Get(enemyId);

        return FormatSimple(enemyId, enemy?.Name);
    }
    
    public string FormatItem(int itemId)
    {
        var item = _context.Storage.Items.Get().Get(itemId);

        return FormatSimple(itemId, item?.Name);
    }

    public string FormatSkill(int skillId)
    {
        var skill = _context.Storage.Skills.Get().Get(skillId);

        return FormatSimple(skillId, skill?.Name);
    }

    public string FormatState(int stateId)
    {
        var state = _context.Storage.States.Get().Get(stateId);

        return FormatSimple(stateId, state?.Name);
    }

    public string FormatSwitch(int switchId, FormatOptions? options = null)
    {
        if (options == null)
            options = new();

        options.SkipId ??= _context.Version == StorageVersion.MV;
        options.IdLength ??= 4;

        var sw = _context.Storage.GetSwitch(switchId);

        return FormatSimple(switchId, sw, options);
    }

    // TODO: without bracket check
    public string FormatTroop(int troopId)
    {
        var troop = _context.Storage.Troops.Get().Get(troopId);

        return FormatSimple(troopId, troop?.Name ?? "", new() { BracketType = BracketType.None });
    }

    public string FormatTroopMember(int troopMemberId)
    {
        FormatOptions opt = new();
        opt.HasExtendedFormat = false;

        string name = "?";
        if (_context.TroopContext != null)
        {
            var member = _context.TroopContext.Troop.Members.Get(troopMemberId);
            if (member != null)
            {
                var enemy = _context.Storage.Enemies.Get().Get(member.EnemyId);
                if (enemy != null)
                    name = enemy.Name;
            }
        }

        if (_context.Version == StorageVersion.MV)
            return FormatSimple(troopMemberId, $"#{troopMemberId + 1} {name}", opt);
        else
            return FormatSimple(troopMemberId, $"{troopMemberId + 1}. {name}", opt);
    }

    public string FormatVariable(int variableId, FormatOptions? options = null)
    {
        if (options == null)
            options = new();

        options.SkipId ??= _context.Version == StorageVersion.MV;
        options.IdLength ??= 4;

        var v = _context.Storage.GetVariable(variableId);

        return FormatSimple(variableId, v, options);
    }

    public string FormatWeapon(int weaponId)
    {
        var weapon = _context.Storage.Weapons.Get().Get(weaponId);

        return FormatSimple(weaponId, weapon?.Name, new() { Literal = "W" });
    }
}
