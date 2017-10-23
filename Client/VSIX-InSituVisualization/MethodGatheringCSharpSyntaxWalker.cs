using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{

    internal class MethodGatheringCSharpSyntaxWalker : CSharpSyntaxWalker
    {
        /// <summary>
        /// The Walkers are originally set up to only visit the nodes. This is done in order to perform better.
        /// Therefore we need to tell the walker to also visit the Nodes.
        /// </summary>
        public MethodGatheringCSharpSyntaxWalker() : base(SyntaxWalkerDepth.Token)
        {
        }

        public IList<MethodDeclarationSyntax> Methods { get; } = new List<MethodDeclarationSyntax>();

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Methods.Add(node);

            // Basemethods need to be called - since one wants to get deeper down into the tree
            base.VisitMethodDeclaration(node);
        }
    }
}
