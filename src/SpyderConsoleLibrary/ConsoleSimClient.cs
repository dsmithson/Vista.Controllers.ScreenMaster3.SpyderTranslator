using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Spyder.Console.Packets;
using System.Diagnostics;

namespace Spyder.Console
{
	public delegate void RefreshPacketHandler(IPRefreshPacket pkt);

	public class ConsoleSimClient
	{
		public const int ENCODER_PORT = 20;

		public const int RX_PORT = 9991;
		public const int TX_PORT = 9990;
		public const int RX_BUFFSIZE = 1400;

		private int lastJoyX;
		private int lastJoyY;
		private int lastJoyZ;
		private int lastTBar;
		
		public bool Verbose;
		public event RefreshPacketHandler Refresh;

		private bool running = false;
        private IPAddress remoteIP;
		private Thread txThread;
		private Socket socket;

		private readonly AutoResetEvent txEvent = new AutoResetEvent(false);
		private readonly IPStatusPacket statusPacket;
		private readonly object lockObject = new object();

        public IPAddress RemoteIP
        {
            get { return remoteIP; }
        }
        
		protected void OnRefresh(IPRefreshPacket packet)
		{
            Refresh?.Invoke(packet);
        }

		public ConsoleSimClient()
		{		
			Debug.WriteLine("Constructing...");

            //Initialize status packet
            statusPacket = new IPStatusPacket
            {
                CalPromChkSumError = 0,
                HWVersion = 100,
                ModuleType = 0x300,
                PktType = (byte)IPPacketType.Status,
                SerialNum = 123,
                SWVersion = 100,
                VHWVersion = 100
            };
        }

		public void Startup(IPAddress remoteIP)
		{
			Shutdown();

			running = true;

            this.remoteIP = remoteIP;
            if (remoteIP == null)
                throw new NullReferenceException("Supplied remote IP address cannot be null");

			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Bind(new IPEndPoint(IPAddress.Any, RX_PORT));
            StartListening();

            txThread = new Thread(new ThreadStart(TxThreadWorker))
            {
                Name = "Console Status Update Thread",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            txThread.Start();

			Debug.WriteLine("Started Console Sim Client Receive Thread and Socket");
		}

		public void Shutdown()
		{
			//Set running flag to indicate to worker threads they are to terminate
			running = false;
			
			if(socket != null)
			{
				Debug.WriteLine("Shutting down client socket");
				socket.Shutdown(SocketShutdown.Both); 
				socket.Close();
				socket = null;
			}
			if(txThread != null)
			{
                txEvent.Set();
                if (!txThread.Join(10000))
                    txThread.Abort();
                
				txThread = null;
			}
		}

		public void ButtonAction(int portID, int controlID, bool pressed)
		{
			try
			{
				lock(lockObject)
				{
					int val = pressed? 1 : 0;
					statusPacket.SetUICmd(statusPacket.UICmdCount++, portID, controlID, val);
					statusPacket.Revision++;
					txEvent.Set();
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine($"{ex.GetType().Name} occurred in ButtonAction: {ex.Message}");
			}
		}
		public void RotaryAction(int rotaryIndex, int rotaryOffset)
        {
			//TODO
        }

		public void JoystickAction(int x, int y, int z)
        {
			lastJoyX = x;
			lastJoyY = y;
			lastJoyZ = z;

			PushTBarOrJoyAction();
		}


		public void TBarAction(int tbarPosition)
		{
			lastTBar = tbarPosition;

			PushTBarOrJoyAction();
		}

		private void PushTBarOrJoyAction()
        {
			try
			{
				lock (lockObject)
				{
					statusPacket.SetBarAndJoystick(lastTBar, lastJoyX, lastJoyY, lastJoyZ);
					statusPacket.Revision++;
					txEvent.Set();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"{ex.GetType().Name} occurred in ButtonAction: {ex.Message}");
			}
		}

		private void TxThreadWorker()
		{
			while(running)
			{
				try
				{
					//Do a manual send every second if nothing new has transpired
					if(statusPacket.Revision > 0)
						Thread.Sleep(20);	//High resolution update mode
					else
						txEvent.WaitOne(1000, true);

					//Validate environment before continuing
					if(!running || socket == null || statusPacket == null || remoteIP == null)
						continue;

					lock(lockObject)
						socket.SendTo(statusPacket.Data, IPStatusPacket.SIZE, SocketFlags.None, new IPEndPoint(remoteIP, TX_PORT));
				}
				catch(ThreadAbortException)
				{
					return;
				}
				catch(Exception ex)
				{
					Debug.WriteLine($"{ex.GetType().Name} occurred in txWorkerThread: {ex.Message}");
				}
			}
		}

        private void StartListening()
        {
            if (!running || socket == null)
                return;

            byte[] buffer = new byte[1400];
            EndPoint remote = new IPEndPoint(remoteIP, TX_PORT);
            socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remote, OnReceive, buffer);
        }

        private void OnReceive(IAsyncResult ar)
        {
            if (!running)
                return;

            try
            {
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                int bytes = socket.EndReceiveFrom(ar, ref remoteEP);
                if (bytes > 0)
                {
                    byte[] buffer = (byte[])ar.AsyncState;
                    ProcessRefreshPacket(buffer);
                }
            }
            catch (Exception ex)
            {
				Debug.WriteLine($"{ex.GetType().Name} processing packet receipt: {ex.Message}");
            }
            finally
            {
                if (running)
                    StartListening();
            }
        }

		/// <summary>
		/// Generates an IPRefreshPacket from a supplied byte stream and then raises the Refresh event;
		/// </summary>
		/// <param name="data"></param>
		private void ProcessRefreshPacket(byte[] data)
		{
			if(Refresh != null)
			{
				IPRefreshPacket pkt = null;
				try
				{
					pkt = new IPRefreshPacket(data);
				}
				catch(Exception ex)
				{
					Debug.WriteLine($"{ex.GetType().Name} in Refresh Packet Constructor while processing: {ex.Message}");
				}

				try
				{
					if(pkt != null)
					{
						lock(lockObject)
						{
							if(statusPacket.Revision == pkt.Revision && pkt.Revision > 0)
							{
								statusPacket.Revision = 0;
								statusPacket.UICmdCount = 0;
							}
						}
						OnRefresh(pkt);
					}
				}
				catch(Exception ex)
				{
					Debug.WriteLine($"{ex.GetType().Name} in Refresh Packet Handler: {ex.Message}");
				}
			}
		}
	}
}
