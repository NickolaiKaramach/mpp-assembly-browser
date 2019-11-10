namespace mpp_assembly_browser
{
    public abstract class Info
    {
        public abstract InfoType GetInfoType { get; }

        public string Name { get; set; }

        public string DeclarationName { get; set; }
    }
}