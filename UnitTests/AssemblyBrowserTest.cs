using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpp_assembly_browser;
using mpp_assembly_browser.action;
using TestAssembly;

namespace UnitTests
{
    [TestClass]
    public class AssemblyBrowserTest
    {
        private readonly IAssemblyBrowser _assemblyBrowser = new AssemblyBrowser();

        //To automatically add test assembly to runtime
        private C _myClass = new C();
        private static readonly Assembly TestAssembly = Assembly.Load("TestAssembly");
        private readonly string _testLocation = TestAssembly.Location;

        [TestMethod]
        public void NamespaceTest()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var namespaces = _assemblyBrowser.GetNamespaces(_testLocation);
            var _namespace = namespaces[0];
            var currentNamespace = TestAssembly.GetTypes()[1].Namespace;
            Assert.AreEqual(_namespace.DeclarationName, currentNamespace);
        }

        [TestMethod]
        public void TypesTest()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var namespaces = _assemblyBrowser.GetNamespaces(_testLocation);
            var _namespace = namespaces[0];
            var types = _namespace.Infos;

            Assert.AreEqual(TestAssembly.GetTypes().Length, types.Count);
        }

        [TestMethod]
        public void TypesNameTest()
        {
            var flag = false;
            var namespaces = _assemblyBrowser.GetNamespaces(_testLocation);
            foreach (var _namespace in namespaces)
            {
                if (_namespace.DeclarationName == "TestAssembly" ||
                    _namespace.DeclarationName == "TestAssembly.Another")
                {
                    flag = true;
                }
            }

            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void ExtensionMethodName()
        {
            var namespaces = _assemblyBrowser.GetNamespaces(_testLocation);
            var types = namespaces[0].Infos;
            foreach (var type in types)
            {
                if (type.Name == "MyClass")
                {
                    bool flag = false;
                    ;
                    foreach (var member in ((ContainerInfo) type).Infos)
                    {
                        if (member.Name == "ExtMethod")
                        {
                            flag = true;
                        }
                    }

                    Assert.IsTrue(flag);
                }
            }
        }

        [TestMethod]
        public void TypeNamesTest()
        {
            var namespaces = _assemblyBrowser.GetNamespaces(_testLocation);
            var types = namespaces[0].Infos;
            foreach (var type in types)
            {
                if (type.Name != "A" && type.Name != "B" && type.Name != "C")
                {
                    Assert.Fail($"Error in type name {type.Name}");
                }
            }
        }
    }
}