using DiffMatchPatch;

namespace RPGMakerLibCoreV2.Diff.Text
{
    public class DiffMatchPatchV2 : diff_match_patch
    {
        private List<DiffMatchPatch.Diff> diff_lineMode(string text1, string text2)
        {
            var a = diff_linesToChars(text1, text2);
            text1 = (string)a[0];
            text2 = (string)a[1];
            var linearray = a[2] as List<string>;
            var diffs = diff_main(text1, text2, false);
            diff_charsToLines(diffs, linearray);
            return diffs;
        }

        public List<DiffMatchPatch.Patch> patch_make_by_line(string text1, string text2)
        {
            // Check for null inputs not needed since null can't be passed in C#.
            // No diffs provided, compute our own.
            List<DiffMatchPatch.Diff> diffs = diff_lineMode(text1, text2);
            if (diffs.Count > 2)
            {
                diff_cleanupSemantic(diffs);
                diff_cleanupEfficiency(diffs);
            }
            
            return patch_make(text1, diffs);
        }

        public List<DiffMatchPatch.Patch> ReversePatches(List<DiffMatchPatch.Patch> patches)
        {
            List<DiffMatchPatch.Patch> result = new();
            foreach (var p in patches)
            {
                var newP = new DiffMatchPatch.Patch
                {
                    length1 = p.length2,
                    length2 = p.length1,
                    start1 = p.start2,
                    start2 = p.start1
                };

                foreach (var d in p.diffs)
                {
                    var newOp = d.operation switch
                    {
                        Operation.DELETE => Operation.INSERT,
                        Operation.INSERT => Operation.DELETE,
                        _ => Operation.EQUAL
                    };

                    newP.diffs.Add(new(newOp, d.text));
                }
            }

            return result;
        }
    }
}
