using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace aert_csharp_helpers.Multitouch
{
    class SwipeHelper
    {
        private readonly UIElement _ParentControl;
        private TouchPoint _TouchStart;
        private bool _AlreadySwiped;
        private DispatcherTimer _Timer;

        public SwipeHelper(UIElement parentControl)
        {
            _ParentControl = parentControl;

            _ParentControl.PreviewTouchDown += OnTouchDown;
            _ParentControl.PreviewTouchMove += OnTouchMove;

            _Timer = new DispatcherTimer();
        }

        #region ParentControl Events

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            _TouchStart = e.GetTouchPoint(_ParentControl);
        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {
            if (!_AlreadySwiped)
            {

                var touch = e.GetTouchPoint(_ParentControl);

                //right now a swipe is 200 pixels 

                //Swipe Left
                if (_TouchStart != null && touch.Position.X > (_TouchStart.Position.X + 200))
                {
                    if (OnSwipeLeft != null)
                        OnSwipeLeft(sender, e);
                    _AlreadySwiped = true;
                    StartTimer();
                }

                //Swipe Right
                if (_TouchStart != null && touch.Position.X < (_TouchStart.Position.X - 200))
                {
                    if (OnSwipeRight != null)
                        OnSwipeRight(sender, e);
                    _AlreadySwiped = true;
                    StartTimer();
                }
            }
            //e.Handled = true;
        }

        #endregion

        #region Méthodes privées


        private void StopTimer()
        {
            if (_Timer != null)
            {
                _Timer.Stop();
                _Timer = null;
            }
        }

        private void StartTimer()
        {
            StopTimer();
            _Timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(800) };
            _Timer.Tick += TimerOnTick;
            _Timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            _AlreadySwiped = false;
            StopTimer();
        }

        #endregion

        #region Events

        public event EventHandler OnSwipeLeft;
        public event EventHandler OnSwipeRight;

        #endregion
    }
}
