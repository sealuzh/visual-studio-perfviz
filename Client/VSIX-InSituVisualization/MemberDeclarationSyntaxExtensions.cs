using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{
    internal static class MemberDeclarationSyntaxExtensions
    {
        public static SyntaxToken GetMemberIdentifier(this MemberDeclarationSyntax memberDeclarationSyntax)
        {
            switch (memberDeclarationSyntax)
            {
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    return methodDeclarationSyntax.Identifier;
                case ConstructorDeclarationSyntax constructorDeclarationSyntax:
                    return constructorDeclarationSyntax.Identifier;
                case PropertyDeclarationSyntax propertyDeclarationSyntax:
                    return propertyDeclarationSyntax.Identifier;
                default:
                    Debug.WriteLine($"MemberDeclarationSyntax not implemented: {memberDeclarationSyntax.GetType().Name}");
                    return default(SyntaxToken);
            }
        }
    }
}
