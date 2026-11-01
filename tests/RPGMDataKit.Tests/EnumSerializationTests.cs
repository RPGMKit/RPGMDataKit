using System.Text;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>An enum property of an entity is written as the number the maker's file has (the same data as when the property was an int), and read from it.</summary>
public sealed class EnumSerializationTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheTriggerAndThePriorityOfAPageAreWrittenAsNumbersAndReadBack(Version v)
    {
        var mapEvent = v.New<IEvent>();
        var page = mapEvent.Pages.First()!;
        page.Trigger = EventTriggerType.Autorun;
        page.PriorityType = EventPriority.AboveCharacters;
        var path = Path.Combine(Path.GetTempPath(), "enum-" + Guid.NewGuid().ToString("N"));

        try
        {
            v.Save(mapEvent, path);
            var bytes = File.ReadAllBytes(path);
            var text = Encoding.Latin1.GetString(bytes);

            if (v.Name == "MV")
            {
                Assert.Contains("\"trigger\":3", text);
                Assert.Contains("\"priorityType\":2", text);
            }
            else
            {
                // Ruby Marshal: the symbol, then a Fixnum (the letter i, and the number plus 5 for a small one: 3 is 8, 2 is 7)
                Assert.Contains("@trigger" + "i\u0008", text);
                Assert.Contains("@priority_type" + "i\u0007", text);
            }

            var loaded = (IEvent)v.Load(typeof(IEvent), path);
            var loadedPage = loaded.Pages.First()!;
            Assert.Equal(EventTriggerType.Autorun, loadedPage.Trigger);
            Assert.Equal(EventPriority.AboveCharacters, loadedPage.PriorityType);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheTriggerOfACommonEventIsWrittenAsANumberAndReadBack(Version v)
    {
        var commonEvent = v.New<ICommonEvent>();
        commonEvent.Trigger = CommonEventTriggerType.Parallel;
        var path = Path.Combine(Path.GetTempPath(), "enum-" + Guid.NewGuid().ToString("N"));

        try
        {
            v.Save(commonEvent, path);
            var text = Encoding.Latin1.GetString(File.ReadAllBytes(path));

            Assert.Contains(v.Name == "MV" ? "\"trigger\":2" : "@trigger" + "i\u0007", text);
            Assert.Equal(CommonEventTriggerType.Parallel, ((ICommonEvent)v.Load(typeof(ICommonEvent), path)).Trigger);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
