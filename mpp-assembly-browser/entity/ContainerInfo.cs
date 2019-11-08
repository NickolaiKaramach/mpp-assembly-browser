using System.Collections.Generic;

namespace mpp_assembly_browser
{
    public abstract class ContainerInfo : Info
    {
        public ContainerInfo()
        {
            Infos = new List<Info>();
        }
        public List<Info> Infos { get;}
        internal void AddInfo(Info info)
        {
            Infos.Add(info);
        }
    }
}