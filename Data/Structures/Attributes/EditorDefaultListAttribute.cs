namespace RPGMDataKit.Data.Structures.Attributes;

/// <summary>
/// A list of texts that the editor gives to a parameter of a new command (see <see cref="EditorDefaultBaseAttribute"/>), such as the choices Yes and No
/// of Show Choices. It is the same in all makers; the property is a <c>List&lt;string&gt;</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class EditorDefaultListAttribute(params string[] items) : EditorDefaultBaseAttribute([])
{
    public IReadOnlyList<string> Items { get; } = items;

    public override object? CreateValue(Storages.TypeMap map, Type propertyType) => Items.ToList();
}
