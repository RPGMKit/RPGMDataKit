using System.Text;
using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Diff;
using RPGMakerLibCoreV2.Diff.Text;
using RPGMakerLibCoreV2.Storages.Abstraction;
using Patch = RPGMakerLibCoreV2.Diff.Patch;

namespace RPGMakerLibCoreV2
{
    public static class Differ
    {
        private static DiffMatchPatchV2 _dmp = new DiffMatchPatchV2();

        public static List<Patch> CalculatePatches(Storage left, Storage right, DumpOptions options)
        {
            List<Patch> d = new();

            for (var i = 0; i < Math.Max(left.System.Get().Variables.Count, right.System.Get().Variables.Count); ++i)
            {
                var variableA = left.GetVariable(i);
                var variableB = right.GetVariable(i);

                if (string.IsNullOrWhiteSpace(variableA) && string.IsNullOrWhiteSpace(variableB))
                    continue;

                var context = new Patch.VariableData() { VariableId = i };

                var diff = CalculateTextDiff(variableA, variableB);
                if (diff.Item1 != DiffType.Equal)
                    d.Add(new()
                    {
                        Type = diff.Item1, Variable = context, Patches = diff.Item2,
                    });
            }

            for (var i = 0; i < Math.Max(left.System.Get().Switches.Count, right.System.Get().Switches.Count); ++i)
            {
                var switchA = left.GetSwitch(i);
                var switchB = right.GetSwitch(i);

                if (string.IsNullOrWhiteSpace(switchA) && string.IsNullOrWhiteSpace(switchB))
                    continue;

                var context = new Patch.SwitchData() { SwitchId = i };
                var diff = CalculateTextDiff(switchA, switchB);
                if (diff.Item1 != DiffType.Equal)
                    d.Add(new()
                    {
                        Type = diff.Item1, Switch = context, Patches = diff.Item2,
                    });
            }

            var evCount = Math.Max(left.CommonEvents.Get().Count(), right.CommonEvents.Get().Count());
            for (var i = 0; i < evCount; ++i)
            {
                var commonEventA = left.GetCommonEvent(i);
                var commonEventB = right.GetCommonEvent(i);

                if (commonEventA == null && commonEventB == null)
                    continue;

                using StringWriter bA = new();
                using StringWriter bB = new();

                commonEventA?.Dump(bA,
                    new(left, new CommonEventContext() { CommonEvent = commonEventA }, options.ExtendedFormat),
                    options);
                commonEventB?.Dump(bB,
                    new(right, new CommonEventContext() { CommonEvent = commonEventB }, options.ExtendedFormat),
                    options);

                var context = new Patch.CommonEvenData() { CommonEventId = i };
                var diff = CalculateTextDiff(bA.ToString(), bB.ToString());
                if (diff.Item1 == DiffType.Equal)
                    continue;

                d.Add(new()
                    { Type = diff.Item1, CommonEvent = context, Patches = diff.Item2 });
            }

            MapEventHelper.WalkMapEventPages(left, context =>
            {
                var otherMap = right.Maps.Get(context.MapId);
                if (otherMap == null)
                {
                    d.Add(new Patch()
                    {
                        Type = DiffType.Removed, Map = new Patch.MapData() { MapId = context.MapId },
                        // TODO: uncomment
                        // Patches = CalculateTextDiff(
                        //     m.Dump(left, new DumpOptions() { Format = DumpFormat.PrettyJson }), "").Item2
                    });
                }

                var otherEvent = right.GetMapEvent(context.MapId, context.Event.Id);
                if (otherEvent == null)
                {
                    d.Add(new Patch()
                    {
                        Type = DiffType.Removed,
                        MapEvent = new Patch.MapEventData() { MapId = context.MapId, EventId = context.Event.Id },
                        // TODO: uncomment
                        // Patches = CalculateTextDiff(
                        //     e.Dump(left, new DumpOptions() { Format = DumpFormat.PrettyJson }), "").Item2
                    });
                }

                var pageB = otherEvent?.Pages.Get(context.PageId);

                using StringWriter bA = new();
                using StringWriter bB = new();

                context.Page.Dump(bA, new(left, context, options.ExtendedFormat), options);
                pageB?.Dump(bB, new(right, new MapEventContext()
                {
                    MapId = context.MapId,
                    Map = otherMap,
                    Event = otherEvent,
                    PageId = context.PageId,
                    Page = pageB
                }, options.ExtendedFormat), options);

                var patchData = new Patch.MapEventPageData()
                    { MapId = context.MapId, EventId = context.Event.Id, PageId = context.PageId };

                var diff = CalculateTextDiff(bA.ToString(), bB.ToString());
                if (diff.Item1 == DiffType.Equal)
                    return;

                d.Add(new()
                    { Type = diff.Item1, MapEventPage = patchData, Patches = diff.Item2 });
            });

            MapEventHelper.WalkMapEventPages(right, context =>
            {
                var otherMap = left.Maps.Get(context.MapId);
                if (otherMap == null)
                {
                    d.Add(new Patch()
                    {
                        Type = DiffType.Added, Map = new Patch.MapData() { MapId = context.MapId },
                        // TODO: uncomment
                        // Patches = CalculateTextDiff(
                        //     "", m.Dump(left, new DumpOptions() { Format = DumpFormat.PrettyJson })).Item2
                    });
                }

                var otherEvent = left.GetMapEvent(context.MapId, context.Event.Id);
                if (otherEvent == null)
                {
                    d.Add(new Patch()
                    {
                        Type = DiffType.Added,
                        MapEvent = new Patch.MapEventData() { MapId = context.MapId, EventId = context.Event.Id },
                        // TODO: uncomment
                        // Patches = CalculateTextDiff(
                        //     "", e.Dump(left, new DumpOptions() { Format = DumpFormat.PrettyJson })).Item2
                    });
                }

                var pageA = otherEvent?.Pages.Get(context.PageId);
                if (pageA == null)
                {
                    using var sA = new StringWriter();

                    context.Page.Dump(sA, new(right, context, options.ExtendedFormat), options);
                    
                    var diff = CalculateTextDiff("", sA.ToString());
                    
                    if (diff.Item1 == DiffType.Equal)
                        return;

                    d.Add(new()
                    {
                        Type = diff.Item1, MapEventPage = new()
                        {
                            MapId = context.MapId,
                            EventId = context.Event.Id,
                            PageId = context.PageId
                        },
                        Patches = diff.Item2
                    });
                }
            });

            return d;
        }

        public static Tuple<DiffType, List<Diff.Text.Internal.Patch>> CalculateTextDiff(string left, string right)
        {
            DiffType diffType;
            var patches = _dmp.patch_make_by_line(left ?? "", right ?? "");
            if (patches.Count == 0)
                diffType = DiffType.Equal;
            else if (string.IsNullOrEmpty(left))
                diffType = DiffType.Added;
            else if (string.IsNullOrEmpty(right))
                diffType = DiffType.Removed;
            else
                diffType = DiffType.Modified;

            return new(diffType, patches);
        }

        private static List<Diff.Text.Internal.Patch> CalculatePatches(string left, string right)
        {
            return _dmp.patch_make_by_line(left, right);
        }

        private static string CalculatePatch(string left, string right)
        {
            return _dmp.patch_toText(CalculatePatches(left, right));
        }

        public static void DumpPatches(string filePath, List<Patch> patches)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(patches, Formatting.Indented));
        }

        public static void DumpPatchesAsText(string directoryName, List<Patch> patches, Storage old = null, Storage @new = null)
        {
            List<string> mapContent = new();
            List<string> switchContent = new();
            List<string> variableContent = new();

            foreach (var p in patches)
            {
                if (p.Map != null)
                {
                    if (p.Type == DiffType.Added)
                        mapContent.Add($"New map #{p.Map.MapId} ('{@new.GetMapInfo(p.Map.MapId).Name}')");
                    else if (p.Type == DiffType.Removed)
                        mapContent.Add($"Removed map #{p.Map.MapId} ('{old.GetMapInfo(p.Map.MapId).Name}')");
                }
                
                if (p.MapEvent != null)
                {
                    if (p.Type == DiffType.Added)
                        mapContent.Add($"New map event #{p.MapEvent.EventId} '{@new.GetMapEvent(p.MapEvent.MapId, p.MapEvent.EventId).Name}' in map #{p.MapEvent.MapId} ('{@new.GetMapInfo(p.MapEvent.MapId).Name}')");
                    else if (p.Type == DiffType.Removed)
                        mapContent.Add($"Removed map #{p.MapEvent.MapId} ('{old.GetMapInfo(p.MapEvent.MapId).Name}')");
                }

                if (p.Switch != null)
                {
                    if (p.Type == DiffType.Added)
                        switchContent.Add($"New #{p.Switch.SwitchId} '{@new.GetSwitch(p.Switch.SwitchId)}'");
                    else if (p.Type == DiffType.Removed)
                        switchContent.Add($"Deleted #{p.Switch.SwitchId} '{old.GetSwitch(p.Switch.SwitchId)}'");
                    else if (p.Type == DiffType.Modified)
                        switchContent.Add($"Changed #{p.Switch.SwitchId} '{old.GetSwitch(p.Switch.SwitchId)}' => '{@new.GetSwitch(p.Switch.SwitchId)}'");
                }

                if (p.Variable != null)
                {
                    if (p.Type == DiffType.Added)
                        variableContent.Add($"New #{p.Variable.VariableId} '{@new.GetVariable(p.Variable.VariableId)}'");
                    else if (p.Type == DiffType.Removed)
                        variableContent.Add($"Deleted #{p.Variable.VariableId} '{old.GetVariable(p.Variable.VariableId)}'");
                    else if (p.Type == DiffType.Modified)
                        variableContent.Add($"Changed #{p.Variable.VariableId} '{old.GetVariable(p.Variable.VariableId)}' => '{@new.GetVariable(p.Variable.VariableId)}'");
                }

                if (p.CommonEvent != null || p.MapEventPage != null)
                {
                    var fileName = p.CommonEvent != null
                        ? $"CommonEvent_{p.CommonEvent.CommonEventId:D3}.patch"
                        : $"Map_{p.MapEventPage.MapId:D3}_Event_{p.MapEventPage.EventId:D3}_Page_{p.MapEventPage.PageId}.patch";

                    var patchText = string.Concat(p.Patches.Select(_ => _.ToString() + "\r\n"));
                    File.WriteAllText(Path.Combine(directoryName, fileName), patchText);
                }
            }

            if (mapContent.Count > 0)
                File.WriteAllLines(Path.Combine(directoryName, "Maps.patch"), switchContent);
            if (switchContent.Count > 0)
                File.WriteAllLines(Path.Combine(directoryName, "Switches.patch"), switchContent);
            if (variableContent.Count > 0)
                File.WriteAllLines(Path.Combine(directoryName, "Variables.patch"), variableContent);
        }
        
        public static List<Patch> LoadPatches(string filePath)
        {
            return JsonConvert.DeserializeObject<List<Patch>>(File.ReadAllText(filePath));
        }

        public enum PatchStatus
        {
            Full,
            Partial,
            Error
        }

        public class PatchResult
        {
            public PatchStatus Status;
            public string Text;
        }

        public static PatchResult Patch(string text, List<Diff.Text.Internal.Patch> patches, bool reverse = false)
        {
            if (reverse)
                patches = _dmp.ReversePatches(patches);
            
            var ret = new PatchResult();
            if (patches.Count == 0)
            {
                ret.Text = text;
                ret.Status = PatchStatus.Full;
                return ret;
            }

            var result = _dmp.patch_apply(patches, text);
            ret.Text = result[0] as string;
            var map = result[1] as bool[];

            var countOk = map.Count(_ => _);

            if (countOk == 0)
                ret.Status = PatchStatus.Partial;
            else if (countOk != map.Length)
            {
                ret.Status = PatchStatus.Partial;
            }
            else
                ret.Status = PatchStatus.Full;

            return ret;
        }
    }
}
