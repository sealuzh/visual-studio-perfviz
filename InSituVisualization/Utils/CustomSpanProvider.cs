using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;

namespace InSituVisualization.Utils
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: Ioc
    internal sealed class CustomSpanProvider
    {

        public Span GetSpan(SyntaxNode memberDeclarationSyntax)
        {
            switch (memberDeclarationSyntax)
            {
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    return GetMethodSpan(methodDeclarationSyntax);
                case ConstructorDeclarationSyntax constructorDeclarationSyntax:
                    return GetConstructorSpan(constructorDeclarationSyntax);
                case PropertyDeclarationSyntax propertyDeclarationSyntax:
                    return GetPropertyGetterSpan(propertyDeclarationSyntax);
                // ReSharper disable once RedundantCaseLabel
                case InvocationExpressionSyntax invocationExpressionSyntax:
                default:
                    var lineSyntax = GetLineSyntaxNode(memberDeclarationSyntax);
                    return Span.FromBounds(lineSyntax.SpanStart, lineSyntax.FullSpan.End);
            }
        }


        private static SyntaxNode GetLineSyntaxNode(SyntaxNode syntaxNode)
        {
            var syntaxTriviaList = syntaxNode.GetTrailingTrivia();
            foreach (var trailingTrivia in syntaxTriviaList)
            {
                if (trailingTrivia.ToFullString().Contains("\n"))
                {
                    return syntaxNode;
                }
            }

            var parent = syntaxNode.Parent;
            // ReSharper disable once TailRecursiveCall
            return parent == null ? syntaxNode : GetLineSyntaxNode(parent);
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
                // finding the end of where ConcreteMethodTelemetry : Clause (TypeParameterConstraintClause)
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