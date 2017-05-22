using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Seq.Apps;
using Seq.Apps.LogEvents;

namespace Seq.App.Timeout
{
    [SeqApp("Timeout",
    Description = "Writes a timeout event after a specified time period has elapsed, unless received any event resetting the timer.")]
    public class TimeoutReactor : Reactor, ISubscribeTo<LogEventData>
    {
        private Timer _timer;

        [SeqAppSetting(
            DisplayName = "Timeout (seconds)")]
        public int Timeout { get; set; }

        [SeqAppSetting(
            DisplayName = "Repeat",
            HelpText = "Whether or not the timeout should repeat if there are no events. Otherwise, it will only trigger once and wait until next event.")]
        public bool Repeat { get; set; }

        [SeqAppSetting(
            DisplayName = "Timeout Name")]
        public string Name { get; set; }

        [SeqAppSetting(
           DisplayName = "Event text",
           HelpText = "Defaults to 'Event Timeout {TimeoutName}'")]
        public string Text { get; set; } = "Event Timeout {TimeoutName}";

        [SeqAppSetting(
            DisplayName = "Event level",
            HelpText = "Level of event to generate. Defaults to 'Information'. Valid values are 'Debug', 'Error', 'Fatal', 'Information', 'Warning', 'Verbose'")]
        public Serilog.Events.LogEventLevel Level { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();

            _timer = new Timer(Timeout * 1000);
            _timer.Elapsed += (sender, args) => Log.Write(Level, Text, Name);
            _timer.AutoReset = Repeat;
            _timer.Start();
        }

        public void On(Event<LogEventData> evt)
        {
            _timer.Stop();
            _timer.Start();
        }
    }
}
