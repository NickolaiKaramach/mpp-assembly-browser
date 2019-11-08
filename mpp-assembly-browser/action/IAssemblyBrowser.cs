namespace mpp_assembly_browser
{
    public interface IAssemblyBrowser
    {
        ContainerInfo[] GetNamespaces(string pathToRead);
    }
}