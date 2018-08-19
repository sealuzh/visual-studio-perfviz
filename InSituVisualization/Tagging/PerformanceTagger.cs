//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using DryIoc;
using InSituVisualization.Predictions;
using InSituVisualization.TelemetryMapper;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace InSituVisualization.Tagging
{
    /// <summary>
    /// Determines which spans of text get a PerformanceTag
    /// </summary>
    /// <remarks>
    /// This is a data-only component. The tagging system is a good fit for presenting data-about-text.
    /// The <see cref="PerformanceAdornmentTagger"/> takes color tags produced by this tagger and creates corresponding UI for this data.
    /// </remarks>
    internal class PerformanceTagger : ITagger<PerformanceTag>
    {
        private readonly ITextBuffer _buffer;
        private readonly ITelemetryDataMapper _telemetryDataMapper;
        // TODO RR: Remove
        private SyntaxTree _originalTree;

        public PerformanceTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            buffer.Changed += (sender, args) => HandleBufferChanged(args);
            _telemetryDataMapper = IocHelper.Container.Resolve<ITelemetryDataMapper>();
        }

        #region ITagger implementation

        public IEnumerable<ITagSpan<PerformanceTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return GetTagsInternal().Result;
        }

        private async Task<IEnumerable<ITagSpan<PerformanceTag>>> GetTagsInternal()
        {
            var document = _buffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document == null)
            {
                return new List<ITagSpan<PerformanceTag>>();
            }
            var syntaxTree = await GetValidSyntaxTreeAsync(document).ConfigureAwait(false);
            if (syntaxTree == null)
            {
                return new List<ITagSpan<PerformanceTag>>();
            }

            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
            // TODO RR:
            var telemetryDataMapper = IocHelper.Container.Resolve<ITelemetryDataMapper>();
            var predictionEngine = IocHelper.Container.Resolve<IPredictionEngine>();
            var performanceSyntaxWalker = new AsyncSyntaxWalker(
                predictionEngine,
                document,
                semanticModel,
                telemetryDataMapper);
            //await performanceSyntaxWalker.VisitAsync(syntaxTree, _originalTree).ConfigureAwait(false);

            var methods = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();
            var list = new List<ITagSpan<PerformanceTag>>();
            foreach (var methodDeclarationSyntax in methods)
            {
                var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
                if (methodSymbol == null)
                {
                    continue;
                }

                var performanceInfo = _telemetryDataMapper.GetMethodPerformanceInfoAsync(methodSymbol).Result;
                SnapshotSpan span = methodDeclarationSyntax.GetIdentifierSnapshotSpan(_buffer.CurrentSnapshot);
                list.Add(new TagSpan<PerformanceTag>(span, new MethodPerformanceTag(performanceInfo)));
            }
            return list;
        }

        private async Task<SyntaxTree> GetValidSyntaxTreeAsync(Document document)
        {
            if (document == null)
            {
                return null;
            }
            var syntaxTree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
            if (syntaxTree.GetDiagnostics().Any())
            {
                // there are errors in the code -> do not perform operations
                return null;
            }
            if (_originalTree == null)
            {
                _originalTree = syntaxTree;
            }
            return syntaxTree;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        #endregion

        /// <summary>
        /// Handle buffer changes. The default implementation expands changes to full lines and sends out
        /// a <see cref="TagsChanged"/> event for these lines.
        /// </summary>
        /// <param name="args">The buffer change arguments.</param>
        protected virtual void HandleBufferChanged(TextContentChangedEventArgs args)
        {
            if (args.Changes.Count == 0)
                return;

            var temp = TagsChanged;
            if (temp == null)
                return;

            // Combine all changes into a single span so that
            // the ITagger<>.TagsChanged event can be raised just once for a compound edit
            // with many parts.

            var snapshot = args.After;
            var start = args.Changes[0].NewPosition;
            var end = args.Changes[args.Changes.Count - 1].NewEnd;

            var totalAffectedSpan = new SnapshotSpan(snapshot.GetLineFromPosition(start).Start, snapshot.GetLineFromPosition(end).End);

            temp(this, new SnapshotSpanEventArgs(totalAffectedSpan));
        }
    }
}
