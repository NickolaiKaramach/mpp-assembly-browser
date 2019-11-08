namespace mpp_assembly_browser
{
    public class MemberInfo : Info
    {
        public MemberInfo() : base()
        {
        }
        
        public override InfoType GetInfoType => InfoType.Member;
    }
}