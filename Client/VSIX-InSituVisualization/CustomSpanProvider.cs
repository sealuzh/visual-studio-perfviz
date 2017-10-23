using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;

namespace VSIX_InSituVisualization
{
    internal sealed class CustomSpanProvider
    {

        public Span GetSpan(MemberDeclarationSyntax memberDeclarationSyntax)
        {
            switch (memberDeclarationSyntax)
            {
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    return GetMethodSpan(methodDeclarationSyntax);
                case ConstructorDeclarationSyntax constructorDeclarationSyntax:
                    return GetConstructorSpan(constructorDeclarationSyntax);
                case PropertyDeclarationSyntax propertyDeclarationSyntax:
                    return GetPropertyGetterSpan(propertyDeclarationSyntax);
                default:
                    Debug.WriteLine($"MemberDeclarationSyntax not implemented: {memberDeclarationSyntax.GetType().Name}");
                    return default(Span);
            }
        }

        private static Span GetConstructorSpan(ConstructorDeclarationSyntax constructorDeclarationSyntax)
        {
            var spanStart = constructorDeclarationSyntax.Identifier.SpanStart;
            var spanEnd = constructorDeclarationSyntax.ParameterList.FullSpan.End;
            if (constructorDeclarationSyntax.Initializer != null)
            {
                spanEnd = constructorDeclarationSyntax.Initializer.FullSpan.End;
            }
            return Span.FromBounds(spanStart, spanEnd);
        }

        private static Span GetMethodSpan(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.SemicolonToken != default(SyntaxToken))
            {
                return default(Span);
            }
            var spanStart = methodDeclarationSyntax.Identifier.SpanStart;
            var spanEnd = methodDeclarationSyntax.ParameterList.FullSpan.End;
            if (methodDeclarationSyntax.ConstraintClauses.Any())
            {
                // finding the end of where T : Clause (TypeParameterConstraintClause)
                spanEnd = methodDeclarationSyntax.ConstraintClauses.FullSpan.End;
            }
            return Span.FromBounds(spanStart, spanEnd);
        }

        /// <summary>
        /// Only returning time for Getter
        /// </summary>
        /// <param name="propertyDeclarationSyntax"></param>
        /// <returns></returns>
        private static Span GetPropertyGetterSpan(PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            if (propertyDeclarationSyntax.ExpressionBody != null && propertyDeclarationSyntax.SemicolonToken != default(SyntaxToken))
            {
                // inline getter
                return Span.FromBounds(propertyDeclarationSyntax.Identifier.SpanStart, propertyDeclarationSyntax.SemicolonToken.FullSpan.End);
            }
            var getAccessorDeclarationSyntax = propertyDeclarationSyntax.AccessorList.Accessors.FirstOrDefault(ac => ac.Kind() == SyntaxKind.GetAccessorDeclaration);
            if (getAccessorDeclarationSyntax == null)
            {
                return default(Span);
            }
            if (getAccessorDeclarationSyntax.SemicolonToken == default(SyntaxToken))
            {
                return Span.FromBounds(getAccessorDeclarationSyntax.SpanStart, getAccessorDeclarationSyntax.FullSpan.End);
            }
            return default(Span);
        }
    }
}