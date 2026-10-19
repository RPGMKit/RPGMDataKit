using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The pictures, sounds and movies of a project: the folder of each kind (<see cref="ResourceType"/>), the path of a resource, and the file that exists. The folders are the ones of the stock
/// projects of both editors (<c>Graphics/Tilesets</c>, <c>img/tilesets</c>, <c>Audio/BGM</c>, <c>audio/bgm</c>...).
/// </summary>
public sealed class ResourceTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public ResourceTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private Storage Open(Version v) => v == Version.VXAce ? new VXAceStorage(_folder, "Data") : new MVStorage(_folder);

    private static string Combine(string root, string path) => Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar));

    public static IEnumerable<object[]> Folders() =>
    [
        [Version.VXAce, ResourceType.Animation, "Graphics/Animations"],
        [Version.VXAce, ResourceType.Battleback1, "Graphics/Battlebacks1"],
        [Version.VXAce, ResourceType.Battleback2, "Graphics/Battlebacks2"],
        [Version.VXAce, ResourceType.Battler, "Graphics/Battlers"],
        [Version.VXAce, ResourceType.Character, "Graphics/Characters"],
        [Version.VXAce, ResourceType.Face, "Graphics/Faces"],
        [Version.VXAce, ResourceType.Parallax, "Graphics/Parallaxes"],
        [Version.VXAce, ResourceType.Picture, "Graphics/Pictures"],
        [Version.VXAce, ResourceType.System, "Graphics/System"],
        [Version.VXAce, ResourceType.Tileset, "Graphics/Tilesets"],
        [Version.VXAce, ResourceType.Title1, "Graphics/Titles1"],
        [Version.VXAce, ResourceType.Title2, "Graphics/Titles2"],
        [Version.VXAce, ResourceType.Bgm, "Audio/BGM"],
        [Version.VXAce, ResourceType.Bgs, "Audio/BGS"],
        [Version.VXAce, ResourceType.Me, "Audio/ME"],
        [Version.VXAce, ResourceType.Se, "Audio/SE"],
        [Version.VXAce, ResourceType.Movie, "Movies"],
        [Version.Mv, ResourceType.Animation, "img/animations"],
        [Version.Mv, ResourceType.Battleback1, "img/battlebacks1"],
        [Version.Mv, ResourceType.Battleback2, "img/battlebacks2"],
        [Version.Mv, ResourceType.Battler, "img/enemies"],
        [Version.Mv, ResourceType.Character, "img/characters"],
        [Version.Mv, ResourceType.Face, "img/faces"],
        [Version.Mv, ResourceType.Parallax, "img/parallaxes"],
        [Version.Mv, ResourceType.Picture, "img/pictures"],
        [Version.Mv, ResourceType.SideViewActor, "img/sv_actors"],
        [Version.Mv, ResourceType.SideViewEnemy, "img/sv_enemies"],
        [Version.Mv, ResourceType.System, "img/system"],
        [Version.Mv, ResourceType.Tileset, "img/tilesets"],
        [Version.Mv, ResourceType.Title1, "img/titles1"],
        [Version.Mv, ResourceType.Title2, "img/titles2"],
        [Version.Mv, ResourceType.Bgm, "audio/bgm"],
        [Version.Mv, ResourceType.Bgs, "audio/bgs"],
        [Version.Mv, ResourceType.Me, "audio/me"],
        [Version.Mv, ResourceType.Se, "audio/se"],
        [Version.Mv, ResourceType.Movie, "movies"],
    ];

    [Theory]
    [MemberData(nameof(Folders))]
    public void TheFolderOfAKindIsTheOneTheMakerNamesUnderTheProject(Version v, ResourceType type, string expected)
    {
        var storage = Open(v);

        Assert.True(storage.HasResourceType(type));
        Assert.Equal(Combine(_folder, expected), storage.GetResourceFolder(type));
    }

    [Theory]
    [InlineData(ResourceType.SideViewActor)]
    [InlineData(ResourceType.SideViewEnemy)]
    public void TheSpritesOfTheSideviewBattleAreOfMvOnly(ResourceType type)
    {
        var storage = Open(Version.VXAce);

        Assert.False(storage.HasResourceType(type));
        Assert.Throws<DoesNotExistInVersionException>(() => storage.GetResourceFolder(type));
        Assert.Throws<DoesNotExistInVersionException>(() => storage.GetResourcePath(type, "Actor1"));
        Assert.Throws<DoesNotExistInVersionException>(() => storage.FindResourceFile(type, "Actor1"));
        Assert.True(Open(Version.Mv).HasResourceType(type));
    }

    [Fact]
    public void EveryKindIsKnownToAtLeastOneMakerAndAllButTwoToBoth()
    {
        var vx = Open(Version.VXAce);
        var mv = Open(Version.Mv);
        var kinds = Enum.GetValues<ResourceType>();

        Assert.All(kinds, type => Assert.True(mv.HasResourceType(type), $"{type} is not in MV"));
        Assert.Equal(2, kinds.Count(type => !vx.HasResourceType(type)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ThePathIsTheFolderTheNameAndTheExtension(Version v)
    {
        var storage = Open(v);
        var folder = storage.GetResourceFolder(ResourceType.Tileset);

        Assert.Equal(Path.Combine(folder, "Outside_A2"), storage.GetResourcePath(ResourceType.Tileset, "Outside_A2"));
        Assert.Equal(Path.Combine(folder, "Outside_A2.png"), storage.GetResourcePath(ResourceType.Tileset, "Outside_A2", ".png"));
        Assert.Equal(Path.Combine(folder, "Monsters", "Slime.png"), storage.GetResourcePath(ResourceType.Tileset, "Monsters/Slime", ".png"));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANameThatIsEmptyOrLeadsOutOfTheFolderIsRefused(Version v)
    {
        var storage = Open(v);

        Assert.Throws<ArgumentException>(() => storage.GetResourcePath(ResourceType.Picture, ""));
        Assert.Throws<ArgumentException>(() => storage.GetResourcePath(ResourceType.Picture, "  "));
        Assert.Throws<ArgumentException>(() => storage.GetResourcePath(ResourceType.Picture, "../data/Actors"));
        Assert.Throws<ArgumentException>(() => storage.GetResourcePath(ResourceType.Picture, "a/../../b"));
        Assert.Throws<ArgumentException>(() => storage.GetResourcePath(ResourceType.Picture, @"..\x"));
        Assert.Throws<ArgumentException>(() => storage.GetResourcePath(ResourceType.Picture, Path.GetFullPath("/etc/passwd")));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheFileOfAResourceIsFoundWhateverTheCaseOfItsName(Version v)
    {
        var storage = Open(v);
        var folder = storage.GetResourceFolder(ResourceType.Tileset);
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "OUTSIDE_a2.png"), "");

        var found = storage.FindResourceFile(ResourceType.Tileset, "Outside_A2");

        Assert.Equal(Path.Combine(folder, "OUTSIDE_a2.png"), found);
        Assert.Null(storage.FindResourceFile(ResourceType.Tileset, "Inside_A2"));
        Assert.Null(storage.FindResourceFile(ResourceType.Picture, "Outside_A2"));   // no folder at all
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AFileInAFolderOfTheNameIsFound(Version v)
    {
        var storage = Open(v);
        var folder = Path.Combine(storage.GetResourceFolder(ResourceType.Battler), "Monsters");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "slime.png"), "");

        Assert.Equal(Path.Combine(folder, "slime.png"), storage.FindResourceFile(ResourceType.Battler, "Monsters/Slime"));
    }

    [Fact]
    public void TheExtensionsAreThoseOfTheMaker()
    {
        var vx = Open(Version.VXAce);
        var mv = Open(Version.Mv);

        Assert.Equal([".png", ".jpg", ".jpeg", ".bmp"], vx.GetResourceExtensions(ResourceType.Face));
        Assert.Equal([".png"], mv.GetResourceExtensions(ResourceType.Face));
        Assert.Equal([".ogg", ".wav", ".mp3", ".wma", ".mid", ".midi"], vx.GetResourceExtensions(ResourceType.Bgm));
        Assert.Equal([".ogg", ".m4a"], mv.GetResourceExtensions(ResourceType.Se));
        Assert.Equal([".ogv"], vx.GetResourceExtensions(ResourceType.Movie));
        Assert.Equal([".webm", ".mp4"], mv.GetResourceExtensions(ResourceType.Movie));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheFolderOfOneKindCanBeReplacedAndTheOthersStay(Version v)
    {
        var storage = Open(v);
        var tilesets = storage.GetResourceFolder(ResourceType.Tileset);
        var faces = storage.GetResourceFolder(ResourceType.Face);

        storage.SetResourceFolder(ResourceType.Tileset, "img/tilesets_game");

        var game = Combine(_folder, "img/tilesets_game");
        Assert.Equal(game, storage.GetResourceFolder(ResourceType.Tileset));
        Assert.Equal(Path.Combine(game, "Outside_A2.png"), storage.GetResourcePath(ResourceType.Tileset, "Outside_A2", ".png"));
        Assert.Equal(faces, storage.GetResourceFolder(ResourceType.Face));

        Directory.CreateDirectory(game);
        File.WriteAllText(Path.Combine(game, "outside_a2.PNG"), "");
        Assert.Equal(Path.Combine(game, "outside_a2.PNG"), storage.FindResourceFile(ResourceType.Tileset, "Outside_A2"));

        storage.ResetResourceFolder(ResourceType.Tileset);
        Assert.Equal(tilesets, storage.GetResourceFolder(ResourceType.Tileset));
        Assert.Null(storage.FindResourceFile(ResourceType.Tileset, "Outside_A2"));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABackslashAndAWholePathAreAcceptedAsTheFolder(Version v)
    {
        var storage = Open(v);
        var elsewhere = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Elsewhere." + Guid.NewGuid().ToString("N"));

        storage.SetResourceFolder(ResourceType.Picture, @"extra\pictures");
        Assert.Equal(Combine(_folder, "extra/pictures"), storage.GetResourceFolder(ResourceType.Picture));

        storage.SetResourceFolder(ResourceType.Picture, elsewhere);
        Assert.Equal(elsewhere, storage.GetResourceFolder(ResourceType.Picture));
        Assert.Equal(Path.Combine(elsewhere, "Hero.png"), storage.GetResourcePath(ResourceType.Picture, "Hero", ".png"));
    }

    [Fact]
    public void ABuiltInKindTheMakerLacksCanBeRegistered()
    {
        var storage = Open(Version.VXAce);
        Assert.False(storage.HasResourceType(ResourceType.SideViewActor));
        Assert.Throws<DoesNotExistInVersionException>(() => storage.SetResourceFolder(ResourceType.SideViewActor, "Graphics/SV Actors"));

        storage.RegisterResource(ResourceType.SideViewActor, "Graphics/SV Actors");

        Assert.True(storage.HasResourceType(ResourceType.SideViewActor));
        Assert.True(storage.HasResource("sideviewactor"));
        Assert.Equal(Combine(_folder, "Graphics/SV Actors"), storage.GetResourceFolder(ResourceType.SideViewActor));
        Assert.Equal([".png", ".jpg", ".jpeg", ".bmp"], storage.GetResourceExtensions(ResourceType.SideViewActor));

        storage.UnregisterResource("SideViewActor");
        Assert.False(storage.HasResourceType(ResourceType.SideViewActor));
    }

    [Fact]
    public void EveryKindHasANameAndTheNamesAreDifferentIgnoringTheCase()
    {
        var kinds = Enum.GetValues<ResourceType>();

        Assert.Equal(kinds.Length, ResourceTypes.Names.Count);
        Assert.All(kinds, type => Assert.True(ResourceTypes.Names.ContainsKey(type), $"{type} has no name"));
        Assert.Equal(kinds.Length, ResourceTypes.Names.Values.Select(n => n.ToLowerInvariant()).Distinct().Count());
        Assert.Equal("Picture", ResourceType.Picture.NameOf());
        Assert.Equal("SideViewActor", ResourceType.SideViewActor.NameOf());
    }

    [Fact]
    public void ANameIsParsedToItsKindWhateverTheCase()
    {
        Assert.True(ResourceTypes.TryParse("pICTURE", out var type));
        Assert.Equal(ResourceType.Picture, type);
        Assert.True(ResourceTypes.TryParse(" bgm ", out type));
        Assert.Equal(ResourceType.Bgm, type);
        Assert.False(ResourceTypes.TryParse("Portrait", out _));
        Assert.False(ResourceTypes.TryParse(null, out _));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AKindIsReachedByNameInAnyCaseAsByTheEnum(Version v)
    {
        var storage = Open(v);

        Assert.True(storage.HasResource("tileset"));
        Assert.True(storage.HasResource("TILESET"));
        Assert.False(storage.HasResource("Portrait"));
        Assert.False(storage.HasResource(null!));
        Assert.Equal(storage.GetResourceFolder(ResourceType.Tileset), storage.GetResourceFolder("tILEset"));
        Assert.Equal(storage.GetResourcePath(ResourceType.Tileset, "A", ".png"), storage.GetResourcePath("TILESET", "A", ".png"));
        Assert.Equal(storage.GetResourceExtensions(ResourceType.Tileset), storage.GetResourceExtensions("tileset"));

        var folder = storage.GetResourceFolder(ResourceType.Tileset);
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "Outside_A2.png"), "");
        Assert.Equal(Path.Combine(folder, "Outside_A2.png"), storage.FindResourceFile("tileset", "outside_a2"));

        storage.SetResourceFolder("tileset", "other/place");
        Assert.Equal(Combine(_folder, "other/place"), storage.GetResourceFolder(ResourceType.Tileset));
        storage.ResetResourceFolder("TILESET");
        Assert.Equal(folder, storage.GetResourceFolder(ResourceType.Tileset));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheNamesOfTheProjectAreTheKindsOfTheMakerInOrder(Version v)
    {
        var storage = Open(v);
        var expected = Enum.GetValues<ResourceType>().Where(storage.HasResourceType).Select(t => t.NameOf());

        Assert.Equal(expected, storage.ResourceNames);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AKindOfYourOwnIsRegisteredAndWorksLikeTheOthers(Version v)
    {
        var storage = Open(v);

        storage.RegisterResource("Portrait", @"img\portraits", ["png", ".webp"]);

        Assert.True(storage.HasResource("PORTRAIT"));
        Assert.Equal("Portrait", storage.ResourceNames.Last());
        Assert.Equal(Combine(_folder, "img/portraits"), storage.GetResourceFolder("portrait"));
        Assert.Equal([".png", ".webp"], storage.GetResourceExtensions("Portrait"));
        Assert.Equal(Path.Combine(_folder, "img", "portraits", "Hero.png"), storage.GetResourcePath("portrait", "Hero", ".png"));

        var folder = storage.GetResourceFolder("Portrait");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "HERO.WEBP"), "");
        Assert.Equal(Path.Combine(folder, "HERO.WEBP"), storage.FindResourceFile("Portrait", "hero"));
        Assert.Null(storage.FindResourceFile("Portrait", "villain"));

        storage.SetResourceFolder("Portrait", "elsewhere");
        Assert.Equal(Combine(_folder, "elsewhere"), storage.GetResourceFolder("Portrait"));
        storage.ResetResourceFolder("Portrait");
        Assert.Equal(folder, storage.GetResourceFolder("Portrait"));

        storage.UnregisterResource("PORTRAIT");
        Assert.False(storage.HasResource("Portrait"));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AKindOfYourOwnWithoutExtensionsReadsThePicturesOfTheMaker(Version v)
    {
        var storage = Open(v);

        storage.RegisterResource("Portrait", "img/portraits");

        Assert.Equal(storage.GetResourceExtensions(ResourceType.Picture), storage.GetResourceExtensions("Portrait"));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ARegisteredKindIsOfThatStorageOnly(Version v)
    {
        var storage = Open(v);
        storage.RegisterResource("Portrait", "img/portraits");

        Assert.False(Open(v).HasResource("Portrait"));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABadRegistrationIsRefused(Version v)
    {
        var storage = Open(v);

        Assert.Throws<ArgumentException>(() => storage.RegisterResource("", "x"));
        Assert.Throws<ArgumentException>(() => storage.RegisterResource("  ", "x"));
        Assert.Throws<ArgumentException>(() => storage.RegisterResource("a/b", "x"));
        Assert.Throws<ArgumentException>(() => storage.RegisterResource(@"a\b", "x"));
        Assert.Throws<ArgumentException>(() => storage.RegisterResource("Portrait", ""));
        Assert.Throws<ArgumentException>(() => storage.RegisterResource("picture", "x"));
        Assert.Throws<ArgumentException>(() => storage.RegisterResource(ResourceType.Picture, "x"));

        storage.RegisterResource("Portrait", "x");
        Assert.Throws<ArgumentException>(() => storage.RegisterResource("PORTRAIT", "y"));
        Assert.False(storage.HasResource(""));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AKindOfTheMakerCannotBeUnregistered(Version v)
    {
        var storage = Open(v);

        Assert.Throws<ArgumentException>(() => storage.UnregisterResource("Picture"));
        Assert.True(storage.HasResourceType(ResourceType.Picture));
        Assert.Throws<ArgumentException>(() => storage.UnregisterResource("Portrait"));
    }

    [Fact]
    public void ANameThatIsNotAKindIsRefusedAndOneOfAMissingBuiltInKindIsAVersionError()
    {
        var vx = Open(Version.VXAce);

        var unknown = Assert.Throws<ArgumentException>(() => vx.GetResourceFolder("Portrait"));
        Assert.Contains("Picture", unknown.Message);
        Assert.Throws<ArgumentException>(() => vx.GetResourcePath("Portrait", "Hero"));
        Assert.Throws<ArgumentException>(() => vx.GetResourceExtensions("Portrait"));
        Assert.Throws<ArgumentException>(() => vx.FindResourceFile("Portrait", "Hero"));
        Assert.Throws<ArgumentException>(() => vx.SetResourceFolder("Portrait", "x"));
        Assert.Throws<ArgumentException>(() => vx.ResetResourceFolder("Portrait"));
        Assert.Throws<ArgumentException>(() => vx.GetResourceFolder(null!));

        Assert.Throws<DoesNotExistInVersionException>(() => vx.GetResourceFolder("sideviewactor"));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AnEmptyFolderIsRefusedAndTheFolderFollowsTheProjectWhenItMoves(Version v)
    {
        var storage = Open(v);

        Assert.Throws<ArgumentException>(() => storage.SetResourceFolder(ResourceType.Face, ""));
        Assert.Throws<ArgumentException>(() => storage.SetResourceFolder(ResourceType.Face, "  "));

        storage.SetResourceFolder(ResourceType.Face, "pics/faces");
        var other = Path.Combine(_folder, "other");
        storage.ChangeProjectPath(other, v == Version.VXAce ? "Data" : "data");

        Assert.Equal(Combine(other, "pics/faces"), storage.GetResourceFolder(ResourceType.Face));
    }

    [Fact]
    public void ThePreferredExtensionWinsAndMvDoesNotLookForAJpg()
    {
        var vx = Open(Version.VXAce);
        var folder = vx.GetResourceFolder(ResourceType.Face);
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "Actor1.jpg"), "");
        File.WriteAllText(Path.Combine(folder, "Actor1.png"), "");
        Assert.Equal(Path.Combine(folder, "Actor1.png"), vx.FindResourceFile(ResourceType.Face, "Actor1"));

        var mv = Open(Version.Mv);
        var mvFolder = mv.GetResourceFolder(ResourceType.Face);
        Directory.CreateDirectory(mvFolder);
        File.WriteAllText(Path.Combine(mvFolder, "Actor2.jpg"), "");
        Assert.Null(mv.FindResourceFile(ResourceType.Face, "Actor2"));
    }
}
