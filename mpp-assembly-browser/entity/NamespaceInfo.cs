namespace mpp_assembly_browser
{
    public class NamespaceInfo : ContainerInfo
    {
        public NamespaceInfo() : base()
        {
        }

        public override InfoType GetInfoType => InfoType.Namespace;
    }
}