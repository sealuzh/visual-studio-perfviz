using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using VSIX_InSituVisualization.Model;
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

        public void DrawPerformanceInfo(CSharpSyntaxNode syntaxNode, SnapshotSpan span, MethodPerformanceInfo methodPerformanceInfo)
        {
            if (syntaxNode == null)
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
            UserControl control;
            switch (syntaxNode)
            {
                case MethodDeclarationSyntax _:
                    control = new MethodPerformanceInfoControl
                    {
                        DataContext = new MethodPerformanceInfoControlViewModel(methodPerformanceInfo)
                    };
                    break;
                default:
                    control = new MethodInvocationPerformanceInfoControl
                    {
                        DataContext = new MethodInvocationPerformanceInfoControlViewModel(methodPerformanceInfo)
                    };
                    break;
            }

            // Align the bounds of the text geometry
            Canvas.SetLeft(control, geometry.Bounds.Right);
            Canvas.SetTop(control, geometry.Bounds.Top);

            // Drawing new Coontrol
            _layer.RemoveAdornmentsByVisualSpan(span);
            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, control, null);
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
            _layer.RemoveAdornmentsByVisualSpan(span);
            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
        }
    }
}
