using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MediaManager;
using Network.Bluetooth;
using Network;

namespace ShareTheMusic
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async private void PlayMusic(object sender, EventArgs e)
        {
            await CrossMediaManager.Current.Play("https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3");

            //await CrossMediaManager.Current.PlayFromResource("file:///android_asset/raw/thunder.mp3");
        }

        private void PauseMusic(object sender, EventArgs e)
        {
            CrossMediaManager.Current.PlayPause();
        }

        private void StopMusic(object sender, EventArgs e)
        {
            CrossMediaManager.Current.Stop();
        }

        //https://www.codeproject.com/Articles/814814/Android-Connectivity#bluetooth

        /// <summary>
        /// Simple example>
        //1. Get the clients in range.
        //2. Create a new instance of the bluetoothConnection with the factory.
        //3. Register what happens if we receive a packet of type "CalculationResponse"
        //4. Send a calculation request
        /// </summary>
        public class BluetoothExample
        {
            public async void Demo()
            {
                if (!BluetoothConnection.IsBluetoothSupported)
                {
                    Console.WriteLine("Bluetooth is not supported on this Device");
                    return;
                }

                //1. Get the clients in range.
                DeviceInfo[] devicesInRange = await ConnectionFactory.GetBluetoothDevicesAsync();
                if (devicesInRange.Length <= 0) return; //We need at least one bluetooth connection to deal with :)
                                                        //2. Create a new instance of the bluetoothConnection with the factory.
                Tuple<ConnectionResult, BluetoothConnection> bluetoothConnection = await ConnectionFactory.CreateBluetoothConnectionAsync(devicesInRange[0]);
                if (bluetoothConnection.Item1 != ConnectionResult.Connected) return; //We were not able to connect to the server.
                                                                                     //3. Register what happens if we receive a packet of type "CalculationResponse"
                bluetoothConnection.Item2.RegisterPacketHandler<CalculationResponse>((response, con) => Console.WriteLine($"Answer received {response.Result}"), this);
                //4. Send a calculation request.
                bluetoothConnection.Item2.Send(new CalculationRequest(10, 10), this);
            }
        }

        /// <summary>
        /// Simple example>
        /// 1. Start to listen on a port
        /// 2. Applying optional settings
        /// 3. Register packet listeners.
        /// 4. Handle incoming packets.
        /// </summary>
        public class SecureServerExample
        {
            private ServerConnectionContainer secureServerConnectionContainer;

            internal void Demo()
            {
                //1. Start to listen on a port
                secureServerConnectionContainer = ConnectionFactory.CreateSecureServerConnectionContainer(1234, start: false);

                //2. Apply optional settings.

                #region Optional settings

                secureServerConnectionContainer.ConnectionLost += (a, b, c) => Console.WriteLine($"{secureServerConnectionContainer.Count} {b.ToString()} Connection lost {a.IPRemoteEndPoint.Port}. Reason {c.ToString()}");
                secureServerConnectionContainer.ConnectionEstablished += connectionEstablished;
#if NET46
            secureServerConnectionContainer.AllowBluetoothConnections = true;
#endif
                secureServerConnectionContainer.AllowUDPConnections = true;
                secureServerConnectionContainer.UDPConnectionLimit = 2;

                #endregion Optional settings

                //Call start here, because we had to enable the bluetooth property at first.
                secureServerConnectionContainer.Start();

                //Don't close the application.
                Console.ReadLine();
            }

            /// <summary>
            /// We got a connection.
            /// </summary>
            /// <param name="connection">The connection we got. (TCP or UDP)</param>
            private void connectionEstablished(Connection connection, ConnectionType type)
            {
                Console.WriteLine($"{secureServerConnectionContainer.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");

                //3. Register packet listeners.
                connection.RegisterStaticPacketHandler<CalculationRequest>(calculationReceived);
                connection.RegisterStaticPacketHandler<AddStudentToDatabaseRequest>(addStudentReceived);
                connection.RegisterRawDataHandler("HelloWorld", (rawData, con) => Console.WriteLine($"RawDataPacket received. Data: {rawData.ToUTF8String()}"));
                connection.RegisterRawDataHandler("BoolValue", (rawData, con) => Console.WriteLine($"RawDataPacket received. Data: {rawData.ToBoolean()}"));
                connection.RegisterRawDataHandler("DoubleValue", (rawData, con) => Console.WriteLine($"RawDataPacket received. Data: {rawData.ToDouble()}"));
            }

            /// <summary>
            /// If the client sends us a calculation request, it will end up here.
            /// </summary>
            /// <param name="packet">The calculation packet.</param>
            /// <param name="connection">The connection who was responsible for the transmission.</param>
            private static void calculationReceived(CalculationRequest packet, Connection connection)
            {
                //4. Handle incoming packets.
                connection.Send(new CalculationResponse(packet.X + packet.Y, packet));
            }

            /// <summary>
            /// If the client sends us a add student request, it will end up here.
            /// </summary>
            /// <param name="packet">The add student request packet.</param>
            /// <param name="connection">The connection who was responsible for the transmission.</param>
            private static void addStudentReceived(AddStudentToDatabaseRequest packet, Connection connection)
            {
                //4. Handle incomming packets
                connection.Send(new AddStudentToDatabaseResponse(DatabaseResult.Success, packet));
            }
        }
    }
}
