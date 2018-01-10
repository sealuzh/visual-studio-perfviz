using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using VSIX_InSituVisualization.ViewModels;
using VSIX_InSituVisualization.Views;

namespace VSIX_InSituVisualization
{
    public class MethodAdornmentLayer
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

        private readonly IDictionary<MethodDeclarationSyntax, MethodPerformanceInfoControl> _methodPerformanceInfoControls = new Dictionary<MethodDeclarationSyntax, MethodPerformanceInfoControl>();
        private readonly IDictionary<InvocationExpressionSyntax, MethodInvocationPerformanceInfoControl> _methodInvocationPerformanceInfoControls = new Dictionary<InvocationExpressionSyntax, MethodInvocationPerformanceInfoControl>();

        public void DrawMethodDeclarationPerformanceInfo(MethodDeclarationSyntax methodDeclarationSyntax, SnapshotSpan span, MethodPerformanceInfo methodPerformanceInfo)
        {
            if (methodDeclarationSyntax == null)
            {
                return;
            }
            if (methodPerformanceInfo == null)
            {
                return;
            }
            if (span == default(SnapshotSpan))
            {
                return;
            }
            var geometry = _textView.TextViewLines.GetMarkerGeometry(span);
            if (geometry == null)
            {
                return;
            }

            // TODO RR: Take care of the old PerfViz Control? dispose?
            var newControl = new MethodPerformanceInfoControl
            {
                DataContext = new MethodPerformanceInfoControlViewModel(methodPerformanceInfo)
            };
            // Align the bounds of the text geometry
            Canvas.SetLeft(newControl, geometry.Bounds.Right);
            Canvas.SetTop(newControl, geometry.Bounds.Top);

            // Drawing new Coontrol
            DrawTextRelative(span, newControl);

            // Removing old Controls
            if (_methodPerformanceInfoControls.TryGetValue(methodDeclarationSyntax, out var existingControl))
            {
                // remove old existing control
                _layer.RemoveAdornment(existingControl);
                _methodPerformanceInfoControls.Remove(methodDeclarationSyntax);
            }
            _methodPerformanceInfoControls[methodDeclarationSyntax] = newControl;
        }

        public void DrawMethodInvocationPerformanceInfo(InvocationExpressionSyntax invocationExpressionSyntax, SnapshotSpan span, MethodPerformanceInfo methodPerformanceInfo)
        {
            if (invocationExpressionSyntax == null)
            {
                return;
            }
            if (methodPerformanceInfo == null)
            {
                return;
            }
            if (span == default(SnapshotSpan))
            {
                return;
            }

            var geometry = _textView.TextViewLines.GetMarkerGeometry(span);
            if (geometry == null)
            {
                return;
            }

            // TODO RR: Take care of the old PerfViz Control? dispose?
            var newControl = new MethodInvocationPerformanceInfoControl
            {
                DataContext = new MethodInvocationPerformanceInfoControlViewModel(methodPerformanceInfo)
            };
            // Align the bounds of the text geometry
            Canvas.SetLeft(newControl, geometry.Bounds.Right);
            Canvas.SetTop(newControl, geometry.Bounds.Top);

            // Drawing new Coontrol
            DrawTextRelative(span, newControl);


            // Removing old Controls
            if (_methodInvocationPerformanceInfoControls.TryGetValue(invocationExpressionSyntax, out var existingControl))
            {
                // remove old existing control
                _layer.RemoveAdornment(existingControl);
                _methodInvocationPerformanceInfoControls.Remove(invocationExpressionSyntax);
            }
            _methodInvocationPerformanceInfoControls[invocationExpressionSyntax] = newControl;
        }

        public void DrawRedSpan(SnapshotSpan span)
        {
            var geometry = _textView.TextViewLines.GetMarkerGeometry(span);
            if (geometry == null)
            {
                return;
            }

            // Create the pen and brush to color the box behind the a's
            var brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            brush.Freeze();

            var penBrush = new SolidColorBrush(Colors.Red);
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

            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
        }


        public void DrawTextRelative(SnapshotSpan span, UIElement control)
        {
            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, control, null);
        }
    }
}
