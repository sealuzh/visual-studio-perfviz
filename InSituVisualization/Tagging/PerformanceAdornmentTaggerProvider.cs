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
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace InSituVisualization.Tagging
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [ContentType("projection")]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class PerformanceAdornmentTaggerProvider : IViewTaggerProvider
    {
#pragma warning disable 649 // "field never assigned to" -- field is set by MEF.
        [Import]
        internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService;
#pragma warning restore 649

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null) throw new ArgumentNullException(nameof(textView));

            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            if (buffer != textView.TextBuffer)
            {
                return null;
            }

            var tagAggregator = BufferTagAggregatorFactoryService.CreateTagAggregator<PerformanceTag>(textView.TextBuffer);
            return textView.Properties.GetOrCreateSingletonProperty(() => new PerformanceAdornmentTagger((IWpfTextView)textView, tagAggregator)) as ITagger<T>;
        }
    }
}