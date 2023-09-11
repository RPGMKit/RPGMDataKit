using DiffMatchPatch;

namespace RPGMakerLibCoreV2.Diff.Text
{
    public class TextDiff
    {
        public DiffType DiffType { get; set; }
        public List<Patch> Patches { get; set; }
    }
}