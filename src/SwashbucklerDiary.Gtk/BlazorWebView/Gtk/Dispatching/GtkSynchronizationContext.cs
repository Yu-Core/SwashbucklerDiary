namespace Microsoft.Maui.Controls.Compatibility.Platform.GTK
{
    public class GtkSynchronizationContext : SynchronizationContext
    {
        private readonly int mainThreadID = Thread.CurrentThread.ManagedThreadId;

        public override void Post(SendOrPostCallback d, object? state)
        {
            Invoke((s, e) =>
            {
                d(state);
            });
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadID)
            {
                d(state);
            }
            else
            {
                var evt = new ManualResetEvent(false);
                Exception? exception = null;
                Invoke((s, e) =>
                {
                    try
                    {
                        d(state);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    finally
                    {
                        evt.Set();
                    }
                });

                evt.WaitOne();

                if (exception != null)
                    throw exception;
            }
        }

        static void Invoke(EventHandler d)
        {
            InvokeCB @object = new InvokeCB(d);
            GLib.Functions.TimeoutAdd(0, 0u, @object.Invoke);
        }

        internal class InvokeCB
        {
            private EventHandler d;

            private object sender;

            private EventArgs args;

            internal InvokeCB(EventHandler d)
            {
                this.d = d;
                args = EventArgs.Empty;
                sender = this;
            }

            internal InvokeCB(EventHandler d, object sender, EventArgs args)
            {
                this.d = d;
                this.args = args;
                this.sender = sender;
            }

            internal bool Invoke()
            {
                d(sender, args);
                return false;
            }
        }
    }
}
