using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PauseChamp
{
    public partial class MainForm : Form
    {
#if DEBUG
        private readonly TimeSpan _pauseLength = TimeSpan.FromSeconds(5);
#else
        private readonly TimeSpan _pauseLength = TimeSpan.FromMinutes(5);
#endif

        private readonly Timer _updateTimer = new Timer
        {
            Interval = 1000,
        };

        private readonly Timer _reshowTimer = new Timer
        {
#if DEBUG
            Interval = (int)TimeSpan.FromSeconds(12).TotalMilliseconds,
#else
            Interval = (int) TimeSpan.FromHours(2).TotalMilliseconds,
#endif
            Enabled = true,
        };

        private DateTime _startTime;

        public MainForm()
        {
            InitializeComponent();
            InitializeBounds();
            InitializeTimers();
        }

        protected override void OnLoad(EventArgs e)
        {
#if DEBUG
            Start();
#else
            End();
#endif

            base.OnLoad(e);
        }

        void Start()
        {
            _startTime = DateTime.Now.AddSeconds(1);
            _updateTimer.Start();
            UpdateLabel();
            ShowInTaskbar = true;
            Opacity = 1;
            Show();
        }

        void End()
        {
            _updateTimer.Stop();
            Hide();
            Opacity = 0;
            ShowInTaskbar = false;
        }

        private void InitializeBounds()
        {
            var rect = Screen.PrimaryScreen.Bounds;
            var margin = 128;

            rect.X += margin;
            rect.Y += margin;
            rect.Width -= 2 * margin;
            rect.Height -= 2 * margin;

            Bounds = rect;
        }

        private void InitializeTimers()
        {
            _updateTimer.Tick += (s, e) =>
            {
                if (DateTime.Now - this._startTime - TimeSpan.FromSeconds(1) > this._pauseLength)
                    End();

                UpdateLabel();
            };

            _reshowTimer.Tick += (s, e) =>
            {
                Start();
            };
        }

        private void UpdateLabel()
        {
            if (DateTime.Now - this._startTime > this._pauseLength)
                statusLabel.Text = @"Done";
            else
                statusLabel.Text = (_pauseLength - (DateTime.Now - _startTime)).ToString(@"hh\:mm\:ss");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            base.OnClosing(e);
        }
    }
}
