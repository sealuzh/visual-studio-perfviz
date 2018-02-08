using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InSituVisualization.Model;
using InSituVisualization.Utils;
using InSituVisualization.ViewModels;
using InSituVisualization.Views;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace InSituVisualization
{
    internal class MethodAdornmentLayer
    {
        /// <summary>
        /// Text textView where the adornment is created.
        /// </summary>
        private readonly IWpfTextView _textView;

        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer _layer;

        public MethodAdornmentLayer(IWpfTextView textView)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _layer = textView.GetAdornmentLayer("MemberPerformanceAdorner");
        }

        public void DrawMethodPerformanceInfo(MethodDeclarationSyntax node, MethodPerformanceInfo methodPerformanceInfo)
        {
            if (methodPerformanceInfo == null)
            {
                return;
            }

            // TODO RR: Take care of the old PerfViz Control? dispose?
            var control = new MethodPerformanceInfoControl
            {
                DataContext = new MethodPerformanceInfoControlViewModel(methodPerformanceInfo)
            };
            DrawControl(node, control);
        }

        public void DrawMethodInvocationPerformanceInfo(InvocationExpressionSyntax node, MethodPerformanceInfo methodPerformanceInfo)
        {
            if (methodPerformanceInfo == null)
            {
                return;
            }

            // TODO RR: Take care of the old PerfViz Control? dispose?
            var control = new MethodInvocationPerformanceInfoControl
            {
                DataContext = new MethodInvocationPerformanceInfoControlViewModel(methodPerformanceInfo)
            };
            DrawControl(node, control);
        }

        public void DrawLoopPerformanceInfo(SyntaxNode node, LoopPerformanceInfo loopPerformanceInfo)
        {
            var control = new LoopPerformanceInfoControl
            {
                DataContext = new LoopPerformanceInfoControlViewModel(loopPerformanceInfo)
            };
            DrawControl(node, control);
        }

        private void DrawControl(SyntaxNode node, UIElement control)
        {
            var span = node.GetIdentifierSnapshotSpan(_textView);
            if (span == default(SnapshotSpan))
            {
                return;
            }

            var geometry = _textView.TextViewLines.GetMarkerGeometry(span);
            if (geometry == null)
            {
                return;
            }

            // Align the bounds of the text geometry
            Canvas.SetLeft(control, geometry.Bounds.Right);
            Canvas.SetTop(control, geometry.Bounds.Top);

            // Drawing new Coontrol
            _layer.RemoveAdornmentsByVisualSpan(span);
            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, control, null);
        }

        public void DrawSpan(SyntaxNode syntaxNode, Color color)
        {
            DrawSpan(syntaxNode.GetSnapshotSpan(_textView), color);
        }

        public void DrawSpan(SnapshotSpan span, Color color)
        {
            var geometry = _textView.TextViewLines.GetMarkerGeometry(span);
            if (geometry == null)
            {
                return;
            }

            // Create the pen and brush to color the box behind the a's
            var brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            brush.Freeze();

            var penBrush = new SolidColorBrush(color);
            penBrush.Freeze();
            var pen = new Pen(penBrush, 0.5);
            pen.Freeze();

            var drawing = new GeometryDrawing(brush, pen, geometry);
            drawing.Freeze();

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();

            var image = new Image
            {
                Source = drawingImage,
            };

            // Align the image with the top of the bounds of the text geometry
            Canvas.SetLeft(image, geometry.Bounds.Left);
            Canvas.SetTop(image, geometry.Bounds.Top);
            _layer.RemoveAdornmentsByVisualSpan(span);
            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
        }
    }
}
