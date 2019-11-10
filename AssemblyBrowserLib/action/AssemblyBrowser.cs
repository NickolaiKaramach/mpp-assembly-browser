using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace mpp_assembly_browser.action
{
    public class AssemblyBrowser : IAssemblyBrowser
    {
        private readonly List<MethodInfo> _extensionMethods = new List<MethodInfo>();

        public ContainerInfo[] GetNamespaces(string pathToRead)
        {
            var assembly = AssemblyUtil.GetAssembly(pathToRead);
            var types = assembly.GetTypes();

            var namespaces = new Dictionary<string, ContainerInfo>();
            var result = ParseTypes(types, namespaces);

            AddExtensionMethodsToResult(result);

            return result;
        }

        private ContainerInfo[] ParseTypes(Type[] types, Dictionary<string, ContainerInfo> namespaces)
        {
            foreach (var type in types)
            {
                ParseTypeAndAddToNamespace(namespaces, type);
            }

            return namespaces.Values.ToArray();
        }

        private void ParseTypeAndAddToNamespace(Dictionary<string, ContainerInfo> namespaces, Type type)
        {
            var typeNamespace = type.Namespace;
            if (typeNamespace == null)
            {
                return;
            }

            if (!namespaces.TryGetValue(typeNamespace, out var namespaceInfo))
            {
                namespaceInfo = new NamespaceInfo {DeclarationName = typeNamespace};
                namespaces.Add(typeNamespace, namespaceInfo);
            }

            var typeInfo = GetTypeInfo(type);
            namespaceInfo?.AddInfo(typeInfo);
        }

        private TypeInfo GetTypeInfo(Type type)
        {
            var typeInfo = new TypeInfo
            {
                DeclarationName = AssemblyUtil.GetTypeDeclaration(type.GetTypeInfo()),
                Name = type.Name
            };

            var members = type.GetMembers(BindingFlags.NonPublic
                                          | BindingFlags.Instance
                                          | BindingFlags.Public
                                          | BindingFlags.Static);
            ParseMembers(members, typeInfo);

            return typeInfo;
        }

        private void ParseMembers(System.Reflection.MemberInfo[] members, TypeInfo typeInfo)
        {
            foreach (var member in members)
            {
                var memberInfo = new MemberInfo();
                switch (member.MemberType)
                {
                    case MemberTypes.Method:
                        var method = (MethodInfo) member;
                        if (method.IsDefined(typeof(ExtensionAttribute), false))
                        {
                            _extensionMethods.Add(method);
                        }

                        memberInfo.DeclarationName = AssemblyUtil.CreateMethodDeclarationString(method);
                        break;

                    case MemberTypes.Property:
                        memberInfo.DeclarationName = AssemblyUtil.GetPropertyDeclaration((PropertyInfo) member);
                        break;

                    case MemberTypes.Field:
                        memberInfo.DeclarationName = AssemblyUtil.GetFieldDeclaration(((FieldInfo) member));
                        break;

                    case MemberTypes.Event:
                        memberInfo.DeclarationName = AssemblyUtil.GetEventDeclaration((EventInfo) member);
                        break;

                    case MemberTypes.Constructor:
                        memberInfo.DeclarationName = AssemblyUtil.GetConstructorDeclaration((ConstructorInfo) member);
                        break;

                    default:
                        memberInfo.DeclarationName =
                            AssemblyUtil.GetTypeDeclaration((System.Reflection.TypeInfo) member);
                        break;
                }

                if (memberInfo.DeclarationName != null)
                {
                    memberInfo.Name = member.Name;
                    typeInfo.AddInfo(memberInfo);
                }
            }
        }

        private void AddExtensionMethodsToResult(ContainerInfo[] containers)
        {
            foreach (var method in _extensionMethods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length < 0)
                {
                    continue;
                }

                var param = parameters[0];
                var extendedType = param.ParameterType;
                
                foreach (var container in containers)
                {
                    if (container.DeclarationName != extendedType.Namespace)
                    {
                        continue;
                    }

                    var types = container.Infos;
                    foreach (var type in types)
                    {
                        if (type.DeclarationName == AssemblyUtil.GetTypeDeclaration(extendedType.GetTypeInfo()))
                        {
                            ((TypeInfo) type).AddInfo(
                                new MemberInfo
                                {
                                    DeclarationName = "ext. method: " +
                                                      AssemblyUtil.CreateExtensionMethodDeclarationString(method),
                                    Name = method.Name
                                });
                        }
                    }
                }
            }
        }
    }
}