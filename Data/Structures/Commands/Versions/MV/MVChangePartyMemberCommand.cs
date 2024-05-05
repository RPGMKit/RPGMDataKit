namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.MV
{
    public class MVChangePartyMemberCommand : ChangePartyMemberCommand
    {
        public override bool GetInitialize()
        {
            return GetParameter<bool>(2);
        }
    }
}
