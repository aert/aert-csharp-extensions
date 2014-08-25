using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace aert_csharp_helpers.Multitouch
{
    public class TouchImageHelper : INotifyPropertyChanged
    {
        private readonly Canvas _CanvasControl;
        private readonly Image _ImageControl;

        private readonly DoubleTapHelper _DoubleTapHelper;


        public TouchImageHelper(Canvas canvasControl, Image imageControl)
        {
            _CanvasControl = canvasControl;
            _ImageControl = imageControl;

            _DoubleTapHelper = new DoubleTapHelper(_CanvasControl);

            // Manipulations
            _CanvasControl.IsManipulationEnabled = true;
            _CanvasControl.ManipulationDelta += OnManipulationDelta;
            _CanvasControl.ManipulationStarting += OnManipulationStarting;
            _CanvasControl.ManipulationInertiaStarting += OnManipulationInertiaStarting;

            // Double Click
            _CanvasControl.MouseDown += OnMouseDown;
            _CanvasControl.PreviewTouchDown += OnPreviewTouchDown;

            _CanvasControl.Loaded += OnCanvasLoaded;
            _ImageControl.RenderTransform = ImageTransform;
        }

        #region Méthodes publique

        public void ResetTransformations()
        {
            _ImageTransform = new MatrixTransform();
            _ImageControl.RenderTransform = ImageTransform;

            CenterImage();
        }

        #endregion


        #region ImageTransform - INotifyPropertyChanged

        private MatrixTransform _ImageTransform;
        public MatrixTransform ImageTransform
        {
            get { return _ImageTransform ?? (_ImageTransform = new MatrixTransform()); }
            set
            {
                if (value == _ImageTransform) return;

                _ImageTransform = value;
                RaisePropertyChanged("ImageTransform");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion


        #region Multitouch

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Matrix m = _ImageTransform.Matrix;

            // If element beyond edge, report back to WPF
            Vector pastEdgeVector;
            if (ElementPastBoundary(_ImageControl, out pastEdgeVector))
            {
                m.Translate(-1.0 * pastEdgeVector.X, -1.0 * pastEdgeVector.Y);
                _ImageTransform.Matrix = m;

                e.Complete();
                e.Handled = true;
                return;
            }

            // Find center of element and then transform to get current location of center
            FrameworkElement fe = _ImageControl;
            if (fe == null)
                return;
            Point center = new Point(fe.ActualWidth / 2, fe.ActualHeight / 2);
            center = m.Transform(center);

            // Update matrix to reflect translation and rotation
            ManipulationDelta md = e.DeltaManipulation;
            m.Translate(md.Translation.X, md.Translation.Y);
            m.ScaleAt(md.Scale.X, md.Scale.Y, center.X, center.Y);


            _ImageTransform.Matrix = m;
            RaisePropertyChanged("ImageTransform");

            e.Handled = true;
        }

        private bool ElementPastBoundary(FrameworkElement fe, out Vector pastEdgeVector)
        {
            bool pastEdge = false;

            pastEdgeVector = new Vector();

            FrameworkElement feParent = fe.Parent as FrameworkElement;
            if (feParent != null)
            {
                Rect feRect = fe.TransformToAncestor(feParent).TransformBounds(
                    new Rect(0.0, 0.0, fe.ActualWidth, fe.ActualHeight));

                if (feRect.Left > feParent.ActualWidth - 20)
                    pastEdgeVector.X = feRect.Left - (feParent.ActualWidth - 20);

                if (feRect.Right < 20)
                    pastEdgeVector.X = feRect.Right - 20;

                if (feRect.Top > feParent.ActualHeight - 20)
                    pastEdgeVector.Y = feRect.Top - (feParent.ActualHeight - 20);

                if (feRect.Bottom < 20)
                    pastEdgeVector.Y = feRect.Bottom - 20;

                if ((pastEdgeVector.X != 0) || (pastEdgeVector.Y != 0))
                    pastEdge = true;
            }

            return pastEdge;
        }

        private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            // Ask for manipulations to be reported relative to the canvas
            e.ManipulationContainer = _CanvasControl;

            // Support translation and scaling
            e.Mode = ManipulationModes.Translate | ManipulationModes.Scale;
        }

        private void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // 10 in/sec^2 deceleration
            e.TranslationBehavior.DesiredDeceleration = 10.0 * 96.0 / (1000.0 * 1000.0);

            // 960 DIPS/sec^2 deceleration
            e.ExpansionBehavior.DesiredDeceleration = 960.0 / (1000.0 * 1000.0);

            // Rotational inertia - 100 deg/sec^2 deceleration
            e.RotationBehavior.DesiredDeceleration = 100.0 / (1000.0 * 1000.0);
        }

        private void OnCanvasLoaded(object sender, RoutedEventArgs e)
        {
            CenterImage();
            _CanvasControl.ManipulationStarting += OnManipulationStarting;
            _CanvasControl.ManipulationDelta += OnManipulationDelta;

            // inertia 
            _CanvasControl.ManipulationInertiaStarting += OnManipulationInertiaStarting;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                ImageTransform = new MatrixTransform();
            }
        }

        private void OnPreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (_DoubleTapHelper.IsDoubleTap(e))
                ResetTransformations();
        }

        #endregion

        #region Méthodes privées


        private void CenterImage()
        {
            _ImageControl.Width = _CanvasControl.ActualWidth;
            _ImageControl.Height = _CanvasControl.ActualHeight;
        }

        #endregion
    }
}
