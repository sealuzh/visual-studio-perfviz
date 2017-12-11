using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private readonly IDictionary<PerformanceInfo, MethodPerformanceInfoControl> _methodPerformanceInfoControls = new Dictionary<PerformanceInfo, MethodPerformanceInfoControl>();
        private readonly IDictionary<PerformanceInfo, MethodInvocationPerformanceInfoControl> _methodInvocationPerformanceInfoControls = new Dictionary<PerformanceInfo, MethodInvocationPerformanceInfoControl>();


        public void DrawMethodDeclarationPerformanceInfo(SnapshotSpan span, PerformanceInfo performanceInfo)
        {
            var geometry = _textView.TextViewLines.GetMarkerGeometry(span);
            if (geometry == null)
            {
                return;
            }

            // TODO RR: Take care of the old PerfViz Control? dispose?
            var newControl = new MethodPerformanceInfoControl
            {
                DataContext = new MethodPerformanceInfoControlViewModel(performanceInfo)
            };
            // Align the bounds of the text geometry
            Canvas.SetLeft(newControl, geometry.Bounds.Right);
            Canvas.SetTop(newControl, geometry.Bounds.Top);

            // Drawing new Coontrol
            DrawTextRelative(span, newControl);

            // Removing old Controls
            if (_methodPerformanceInfoControls.TryGetValue(performanceInfo, out var existingControl))
            {
                // remove old existing control
                _layer.RemoveAdornment(existingControl);
                _methodPerformanceInfoControls.Remove(performanceInfo);
            }
            _methodPerformanceInfoControls[performanceInfo] = newControl;
        }

        public void DrawMethodInvocationPerformanceInfo(SnapshotSpan span, PerformanceInfo performanceInfo)
        {
            var geometry = _textView.TextViewLines.GetMarkerGeometry(span);
            if (geometry == null)
            {
                return;
            }

            // TODO RR: Take care of the old PerfViz Control? dispose?
            var newControl = new MethodInvocationPerformanceInfoControl
            {
                DataContext = new MethodInvocationPerformanceInfoControlViewModel(performanceInfo)
            };
            // Align the bounds of the text geometry
            Canvas.SetLeft(newControl, geometry.Bounds.Right);
            Canvas.SetTop(newControl, geometry.Bounds.Top);

            // Drawing new Coontrol
            DrawTextRelative(span, newControl);

            // Removing old Controls
            if (_methodInvocationPerformanceInfoControls.TryGetValue(performanceInfo, out var existingControl))
            {
                // remove old existing control
                _layer.RemoveAdornment(existingControl);
                _methodInvocationPerformanceInfoControls.Remove(performanceInfo);
            }
            _methodInvocationPerformanceInfoControls[performanceInfo] = newControl;
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
