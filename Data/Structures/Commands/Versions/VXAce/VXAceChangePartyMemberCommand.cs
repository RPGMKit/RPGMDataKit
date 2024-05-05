namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.VXAce
{
    public class VXAceChangePartyMemberCommand : ChangePartyMemberCommand
    {
        public override bool GetInitialize()
        {
            return GetParameter<int>(2) > 0;
        }
    }
}
