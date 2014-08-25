using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace aert_csharp_helpers.Multitouch
{
    public class DoubleTapHelper
    {
        private readonly UIElement _ParentControl;

        private readonly Stopwatch _DoubleTapStopwatch = new Stopwatch();
        private Point _LastTapLocation;

        public DoubleTapHelper(UIElement parentControl)
        {
            _ParentControl = parentControl;
        }

        public bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(_ParentControl).Position;
            bool tapsAreCloseInDistance = GetDistanceBetweenPoints(currentTapPosition, _LastTapLocation) < 40;
            _LastTapLocation = currentTapPosition;

            TimeSpan elapsed = _DoubleTapStopwatch.Elapsed;
            _DoubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.7));

            return tapsAreCloseInDistance && tapsAreCloseInTime;
        }

        public static double GetDistanceBetweenPoints(Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }
    }
}
