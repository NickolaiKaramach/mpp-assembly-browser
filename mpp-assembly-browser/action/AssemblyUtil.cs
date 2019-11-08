using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace mpp_assembly_browser.action
{
    public static class AssemblyUtil
    {
        public static string GetTypeName(Type type)
        {
            var result = $"{type.Namespace}.{type.Name}";
            if (type.IsGenericType)
            {
                result += GetGenericArgumentsString(type.GetGenericArguments());
            }

            return result;
        }

        public static string GetMethodName(MethodBase method)
        {
            if (method.IsGenericMethod)
            {
                return method.Name + GetGenericArgumentsString(method.GetGenericArguments());
            }

            return method.Name;
        }

        public static string GetGenericArgumentsString(Type[] arguments)
        {
            var genericArgumentsString = new StringBuilder("<");
            for (int i = 0; i < arguments.Length; i++)
            {
                genericArgumentsString.Append(GetTypeName(arguments[i]));
                if (i != arguments.Length - 1)
                {
                    genericArgumentsString.Append(", ");
                }
            }

            genericArgumentsString.Append(">");

            return genericArgumentsString.ToString();
        }

        public static string CreateMethodDeclarationString(MethodInfo methodInfo)
        {
            var returnType = GetTypeName(methodInfo.ReturnType);
            var parameters = methodInfo.GetParameters();
            var declaration =
                $"{GetMethodDeclaration(methodInfo)} {returnType} {GetMethodName(methodInfo)} {GetMethodParametersString(parameters)}";

            return declaration;
        }

        public static string CreateExtensionMethodDeclarationString(MethodInfo methodInfo)
        {
            var returnType = GetTypeName(methodInfo.ReturnType);
            var parameters = new List<ParameterInfo>(methodInfo.GetParameters());
            parameters.RemoveAt(0);
            var declaration =
                $"{GetMethodDeclaration(methodInfo)} {returnType} {GetMethodName(methodInfo)} {GetMethodParametersString(parameters.ToArray())}";

            return declaration;
        }

        public static string GetMethodParametersString(ParameterInfo[] parameters)
        {
            var parametersString = new StringBuilder("(");
            for (int i = 0; i < parameters.Length; i++)
            {
                parametersString.Append(GetTypeName(parameters[i].ParameterType));
                if (i != parameters.Length - 1)
                {
                    parametersString.Append(", ");
                }
            }

            parametersString.Append(")");

            return parametersString.ToString();
        }

        public static string GetTypeDeclaration(System.Reflection.TypeInfo typeInfo)
        {
            var result = new StringBuilder();

            if (typeInfo.IsNestedPublic || typeInfo.IsPublic)
            {
                result.Append("public ");
            }
            else if (typeInfo.IsNestedPrivate)
            {
                result.Append("private ");
            }
            else if (typeInfo.IsNestedFamily)
            {
                result.Append("protected ");
            }
            else if (typeInfo.IsNestedAssembly)
            {
                result.Append("internal ");
            }
            else if (typeInfo.IsNestedFamORAssem)
            {
                result.Append("protected internal ");
            }
            else if (typeInfo.IsNestedFamANDAssem)
            {
                result.Append("private protected ");
            }
            else if (typeInfo.IsNotPublic)
            {
                result.Append("private ");
            }

            if (typeInfo.IsAbstract && typeInfo.IsSealed)
            {
                result.Append("static ");
            }
            else if (typeInfo.IsAbstract)
            {
                result.Append("abstract ");
            }
            else if (typeInfo.IsSealed)
            {
                result.Append("sealed ");
            }

            if (typeInfo.IsClass)
            {
                result.Append("class ");
            }
            else if (typeInfo.IsEnum)
            {
                result.Append("enum ");
            }
            else if (typeInfo.IsInterface)
            {
                result.Append("interface ");
            }
            else if (typeInfo.IsGenericType)
            {
                result.Append("generic ");
            }
            else if (typeInfo.IsValueType && !typeInfo.IsPrimitive)
            {
                result.Append("struct ");
            }

            result.Append($"{GetTypeName(typeInfo.AsType())} ");

            return result.ToString();
        }

        public static string GetMethodDeclaration(MethodBase methodBase)
        {
            var result = new StringBuilder();

            if (methodBase.IsAssembly)
                result.Append("internal ");
            else if (methodBase.IsFamily)
                result.Append("protected ");
            else if (methodBase.IsFamilyOrAssembly)
                result.Append("protected internal ");
            else if (methodBase.IsFamilyAndAssembly)
                result.Append("private protected ");
            else if (methodBase.IsPrivate)
                result.Append("private ");
            else if (methodBase.IsPublic)
                result.Append("public ");

            if (methodBase.IsStatic)
                result.Append("static ");
            else if (methodBase.IsAbstract)
                result.Append("abstract ");
            else if (methodBase.IsVirtual)
                result.Append("virtual ");

            return result.ToString();
        }

        public static string GetPropertyDeclaration(PropertyInfo propertyInfo)
        {
            var result = new StringBuilder(GetTypeName(propertyInfo.PropertyType));
            result.Append(" ");
            result.Append(propertyInfo.Name);

            var accessors = propertyInfo.GetAccessors(true);
            foreach (var accessor in accessors)
            {
                if (accessor.IsSpecialName)
                {
                    result.Append(" { ");
                    result.Append(accessor.Name);
                    result.Append(" } ");
                }
            }

            return result.ToString();
        }

        public static string GetEventDeclaration(EventInfo eventInfo)
        {
            var result = new StringBuilder();
            result.Append($"{GetTypeName(eventInfo.EventHandlerType)} {eventInfo.Name}");
            result.Append($" [{eventInfo.AddMethod.Name}] ");
            result.Append($" [{eventInfo.RemoveMethod.Name}] ");

            return result.ToString();
        }

        public static string GetFieldDeclaration(FieldInfo fieldInfo)
        {
            var result = new StringBuilder();
            if (fieldInfo.IsAssembly)
                result.Append("internal ");
            else if (fieldInfo.IsFamily)
                result.Append("protected ");
            else if (fieldInfo.IsFamilyOrAssembly)
                result.Append("protected internal ");
            else if (fieldInfo.IsFamilyAndAssembly)
                result.Append("private protected ");
            else if (fieldInfo.IsPrivate)
                result.Append("private ");
            else if (fieldInfo.IsPublic)
                result.Append("public ");

            if (fieldInfo.IsInitOnly)
                result.Append("readonly ");
            if (fieldInfo.IsStatic)
                result.Append("static ");

            result.Append(GetTypeName(fieldInfo.FieldType));
            result.Append(" ");
            result.Append(fieldInfo.Name);

            return result.ToString();
        }

        public static string GetConstructorDeclaration(ConstructorInfo constructorInfo)
        {
            return
                $"{GetMethodDeclaration(constructorInfo)} {GetMethodName(constructorInfo)} {GetMethodParametersString(constructorInfo.GetParameters())}";
        }

        public static Assembly GetAssembly(String pathToRead)
        {
            if (pathToRead == null)
            {
                throw new ArgumentNullException(nameof(pathToRead));
            }

            return Assembly.LoadFile(pathToRead);
        }
    }
}