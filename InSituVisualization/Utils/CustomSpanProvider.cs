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
                case ForEachStatementSyntax forEachStatementSyntax:
                    return GetLoopIdentifierSpan(forEachStatementSyntax);
                case ForStatementSyntax forStatementSyntax:
                    return GetLoopIdentifierSpan(forStatementSyntax);
                case DoStatementSyntax doStatementSyntax:
                    return GetLoopIdentifierSpan(doStatementSyntax);
                case WhileStatementSyntax whileStatementSyntax:
                    return GetLoopIdentifierSpan(whileStatementSyntax);
                // ReSharper disable once RedundantCaseLabel
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    return GetInvocationExpressionSpan(invocationExpressionSyntax);
                default:
                    var lineSyntax = GetLineSyntaxNode(memberDeclarationSyntax);
                    return lineSyntax.Span.ToSpan();
            }
        }

        private static Span GetInvocationExpressionSpan(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var spanStart = invocationExpressionSyntax.SpanStart;
            var spanEnd = invocationExpressionSyntax.ArgumentList.Span.End;
            return Span.FromBounds(spanStart, spanEnd);
        }

        private static Span GetLoopIdentifierSpan(SyntaxNode syntaxNode)
        {
            foreach (var token in syntaxNode.DescendantNodesAndTokens(descend => true))
            {
                if (token.GetTrailingTrivia().Any(trivia => trivia.RawKind == (int)SyntaxKind.EndOfLineTrivia))
                {
                    return Span.FromBounds(syntaxNode.SpanStart, token.Span.End);
                }
            }
            return Span.FromBounds(syntaxNode.SpanStart, syntaxNode.Span.End);
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
            var spanEnd = constructorDeclarationSyntax.ParameterList.Span.End;
            if (constructorDeclarationSyntax.Initializer != null)
            {
                spanEnd = constructorDeclarationSyntax.Initializer.Span.End;
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
            var spanEnd = methodDeclarationSyntax.ParameterList.Span.End;
            if (methodDeclarationSyntax.ConstraintClauses.Any())
            {
                // finding the end of where T : Clause (TypeParameterConstraintClause)
                spanEnd = methodDeclarationSyntax.ConstraintClauses.Span.End;
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
                return Span.FromBounds(propertyDeclarationSyntax.Identifier.SpanStart, propertyDeclarationSyntax.SemicolonToken.Span.End);
            }
            var getAccessorDeclarationSyntax = propertyDeclarationSyntax.AccessorList.Accessors.FirstOrDefault(ac => ac.Kind() == SyntaxKind.GetAccessorDeclaration);
            if (getAccessorDeclarationSyntax == null)
            {
                return default(Span);
            }
            if (getAccessorDeclarationSyntax.SemicolonToken == default(SyntaxToken))
            {
                return Span.FromBounds(getAccessorDeclarationSyntax.SpanStart, getAccessorDeclarationSyntax.Span.End);
            }
            return default(Span);
        }
    }
}