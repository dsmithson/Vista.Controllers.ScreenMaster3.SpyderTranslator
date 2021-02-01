using System;
using System.ComponentModel;
using System.Net;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using Spyder.Console.Controls.Bindable;
using System.Runtime.CompilerServices;
using Spyder.Console.Modules;
using Spyder.Console.Packets;
using Knightware.Diagnostics;

namespace Spyder.Console
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ConsoleSimClient client;

        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        public Dispatcher Dispatcher
        {
            get { return dispatcher; }
        }

        private ObservableCollection<BindableSegment> segments;
        public ObservableCollection<BindableSegment> Segments
        {
            get { return segments; }
            set
            {
                if (segments != value)
                {
                    segments = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindowViewModel() : this(new ConsoleSimClient())
        {
        }

        public MainWindowViewModel(ConsoleSimClient client)
        {
            //Create default segment objects for our bindable list
            segments = new ObservableCollection<BindableSegment>();
            int portID = 0;
            for (int i = 0; i < 16; i++)
            {
                this.segments.Add(new BindableSegment() { PortID = portID++, SegmentType = SegmentType.Segment2x8 });
            }
            for (int i = 0; i < 4; i++)
            {
                this.segments.Add(new BindableSegment() { PortID = portID++, SegmentType = SegmentType.Segment4x4 });
            }
            this.segments.Add(new BindableSegment() { PortID = portID++, SegmentType = SegmentType.Encoder });


            //Initialize console simulator client
            this.client = client;
            client.Refresh += client_Refresh;
        }

        public void Shutdown()
        {
            client.Refresh -= new RefreshPacketHandler(client_Refresh);
            //client.Shutdown();
        }

        public void ButtonAction(int port, int buttonIndex, bool pressed)
        {
            client.ButtonAction(port, buttonIndex, pressed);
        }

        private void client_Refresh(IPRefreshPacket pkt)
        {
            var updatedSegmentData = pkt.GetPortRefresh();

            dispatcher.BeginInvoke((ThreadStart)(() =>
                {
                    foreach (int port in updatedSegmentData.Keys)
                    {
                        var segment = (Segment)updatedSegmentData[port];

                        //Need to force the segment type to update on the source module; it only returns 2x8 typed segments?
                        segment.Type = this.segments[port].SegmentType;
                        this.segments[port].CopyFrom(segment, LcdDisplayMode.Segment);
                    }
                }));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
