namespace RPGMDataKit.Data.Enums;

/// <summary>
/// The kinds of the files of a project that are not data: the pictures, the sounds and the movies, each kept in a folder of its own. The folders have other names in the two makers
/// (<c>Graphics/Tilesets</c> of VX Ace, <c>img/tilesets</c> of MV): <c>Storage.GetResourceFolder</c> and the methods near it tell them, so the code need not know. A kind that one maker lacks
/// is said so in the member.
/// </summary>
public enum ResourceType
{
    /// <summary>The pictures of the animations of skills and items.</summary>
    Animation,

    /// <summary>The ground of a battle background.</summary>
    Battleback1,

    /// <summary>The walls, trees and the like of a battle background.</summary>
    Battleback2,

    /// <summary>The pictures of the enemies in a battle (<c>Battlers</c> of VX Ace, <c>enemies</c> of MV).</summary>
    Battler,

    /// <summary>The sheets of the characters (the map sprites).</summary>
    Character,

    /// <summary>The faces of the actors.</summary>
    Face,

    /// <summary>The pictures far behind the map.</summary>
    Parallax,

    /// <summary>The pictures shown by the event command Show Picture.</summary>
    Picture,

    /// <summary>The sprites of the actors in the sideview battle. MV only.</summary>
    SideViewActor,

    /// <summary>The sprites of the enemies in the sideview battle. MV only.</summary>
    SideViewEnemy,

    /// <summary>The pictures of the system: the window skin, the icons, the balloons, the pictures of the vehicles.</summary>
    System,

    /// <summary>The sheets of the tilesets (A1 ... E).</summary>
    Tileset,

    /// <summary>The first picture of the title screen.</summary>
    Title1,

    /// <summary>The second picture of the title screen (the frame over the first one).</summary>
    Title2,

    /// <summary>The background music.</summary>
    Bgm,

    /// <summary>The background sound.</summary>
    Bgs,

    /// <summary>The music effects (jingles).</summary>
    Me,

    /// <summary>The sound effects.</summary>
    Se,

    /// <summary>The movies.</summary>
    Movie,
}
