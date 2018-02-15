using System;

namespace ProbeInjector
{
    internal static class DocumentationCommentIdDeriver
    {
        public static string GetDocumentationCommentId(string methodFullName)
        {
            if (string.IsNullOrWhiteSpace(methodFullName))
            {
                return methodFullName;
            }
            var indexOfSeparator = methodFullName.IndexOf(" ", StringComparison.Ordinal);
            var methodFullNameWithoutReturnValue = methodFullName.Substring(indexOfSeparator + 1);
            if (methodFullNameWithoutReturnValue.EndsWith("()"))
            {
                methodFullNameWithoutReturnValue =
                    methodFullNameWithoutReturnValue.Substring(0, methodFullNameWithoutReturnValue.Length - 2);
            }
            return $"M:{methodFullNameWithoutReturnValue.Replace("::", ".")}";
        }
    }
}
