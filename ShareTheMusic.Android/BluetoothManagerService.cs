using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using Xamarin.Forms;
using ShareTheMusic.Droid;
using Java.Util;
using System.IO;
using Android.Media;

[assembly: Dependency(typeof(BluetoothManagerService))]
namespace ShareTheMusic.Droid
{
    public class BluetoothManagerService : BluetoothManager
    {
        BluetoothAdapter btAdapter = BluetoothAdapter.DefaultAdapter;
        BluetoothServerSocket mServerSocket = null;
        BluetoothSocket mSocket = null;
        BluetoothDevice mDevice = null;
        UUID uuid = UUID.FromString("30324d3a-3050-4e87-a159-f8fa6c433786");
        //List<BluetoothSocket> deviceList = new List<BluetoothSocket>();
        //List<System.IO.Stream> streamList = new List<System.IO.Stream>();

        System.IO.Stream mmInStream;
        System.IO.Stream mmOutStream;

        public string checkBluetooth(bool permition)
        {
            if (btAdapter == null)
            {
                return "NoBluetooth";
            }
            else
            {
                if (btAdapter.IsEnabled == false && permition != true)
                {
                    return "TurnOnBluetooth";
                }
                else if(permition == true)
                {
                    btAdapter.Enable();
                    return "AlreadyOn";
                }
                else
                {
                    return "AlreadyOn";
                }
            }
        }

        void RegisterBluetoothProfile()
        {
            //var a = new BluetoothProfile();
        }

        public string[] findBTdevices()
        {
            string[] deviceNames = null;
            deviceNames = (from qr in BluetoothAdapter.DefaultAdapter.BondedDevices select qr.Name).ToArray();
            return deviceNames;
        }

        public string discoverDevices()
        {
            return "not implemented yet";
        }

        public void acceptThread()
        {
            BluetoothServerSocket temp = null;

            try
            {
                temp = btAdapter.ListenUsingRfcommWithServiceRecord("ShareTheMusic", uuid);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Socket's listen() method failed", ex);
            }
            mServerSocket = temp;
        }

        public void runServerSide()
        {
            acceptThread();
            BluetoothSocket socket = null;

            System.Threading.Tasks.Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        socket = mServerSocket.Accept();
                        //deviceList.Add(socket);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Socket's accept() method failed", ex);
                    }

                    if (socket != null)
                    {
                        //manageConnectedSocket();
                        manageConnectedSocket(socket);
                        break;
                    }
                }

            }).ConfigureAwait(false);
        }

        public void ConnectThread(BluetoothDevice device)
        {
            BluetoothSocket temp = null;
            mDevice = device;

            try
            {
                temp = device.CreateRfcommSocketToServiceRecord(uuid);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Socket's create() method failed", ex);
            }
            mSocket = temp;
        }

        public void runClientSide(string deviceName)
        {
            BluetoothDevice tmp = (from qr in BluetoothAdapter.DefaultAdapter.BondedDevices where qr.Name == deviceName select qr).FirstOrDefault();
            ConnectThread(tmp);

            try
            {
                mSocket.Connect();
                //deviceList.Add(mSocket);
            }
            catch (Exception ex)
            {
                try
                {
                    mSocket.Close();
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Could not close the client socket", ex);
                }
            }
            //manageConnectedSocket();
            manageConnectedSocket(mSocket);
        }

        public void Cancel()
        {
            try
            {
                mSocket.Close();
                btAdapter.Disable();
                //streamList.Clear();
                //deviceList.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Could not close the connect socket", ex);
            }
        }

        public void manageConnectedSocket(BluetoothSocket socket)
        {
            
            mSocket = socket;

            System.IO.Stream tmpIn = null;
            System.IO.Stream tmpOut = null;

            try
            {
                tmpIn = socket.InputStream;
            }
            catch (System.IO.IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occurred when creating input stream", ex);
            }

            try
            {
                tmpOut = socket.OutputStream;
            }
            catch (System.IO.IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occurred when creating output stream", ex);
            }

            mmInStream = tmpIn;
            mmOutStream = tmpOut;
            
            /*
            foreach(BluetoothSocket socketTemp in deviceList)
            {
                mSocket = socketTemp;

                System.IO.Stream tmpIn = null;
                System.IO.Stream tmpOut = null;

                try
                {
                    tmpIn = socketTemp.InputStream;
                }
                catch (System.IO.IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occurred when creating input stream", ex);
                }

                try
                {
                    tmpOut = socketTemp.OutputStream;
                }
                catch (System.IO.IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occurred when creating output stream", ex);
                }

                mmInStream = tmpIn;
                mmOutStream = tmpOut;
                streamList.Add(mmOutStream);
            }*/
        }

        [Obsolete]
        public void Read()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                AudioTrack _output;

                int buffsize = AudioTrack.GetMinBufferSize(44100, ChannelOut.Stereo, Android.Media.Encoding.Pcm16bit);
                //_output = new AudioTrack(Android.Media.Stream.Music, 44100, ChannelOut.Stereo, Android.Media.Encoding.Pcm16bit,
                    //buffsize, AudioTrackMode.Stream);
                var AABuilder = new AudioAttributes.Builder();

                AABuilder.SetContentType(AudioContentType.Music);
                AABuilder.SetUsage(AudioUsageKind.Media);
                
                var AfBuilder = new AudioFormat.Builder();
                AfBuilder.SetSampleRate(44100);
                AfBuilder.SetEncoding(Android.Media.Encoding.Pcm16bit);
                AfBuilder.SetChannelMask(ChannelOut.Stereo);


                _output = new AudioTrack(AABuilder.Build(), AfBuilder.Build(), buffsize, AudioTrackMode.Stream, AudioManager.AudioSessionIdGenerate);
                _output.Play();

                byte[] myReadBuffer = new byte[1000];
                int count = 4;
                System.Threading.Tasks.Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            
                            int a=mmInStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                            if (a > 0)
                            {
                                if (count == 0)
                                    _output.Write(myReadBuffer, 0, myReadBuffer.Length);
                                else
                                    count--;
                            }
                            else
                            {
                                var t = a;
                            }
                        }
                        catch (System.IO.IOException ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Input stream was disconnected", ex);
                        }
                    }
                }).ContinueWith((t) =>
                {
                    var a = t.Exception;
                });
            }).ContinueWith((t) =>
            {
                
                var a = t.Exception;
            });
        }

        //public void Write(byte[] bytes)
        //{
        //    /*
        //    foreach(System.IO.Stream temp in streamList)
        //    {
        //        try
        //        {
        //            temp.Write(bytes);
        //        }
        //        catch (System.IO.IOException ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine("Error occurred when sending data", ex);
        //        }
        //    }
        //    *
        //    */

        //    try
        //    {
        //        mmOutStream.Write(bytes);
        //    }
        //    catch (System.IO.IOException ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Error occurred when sending data", ex);
        //    }

        //}

        public void Write(System.IO.Stream stream)
        {
            /*
            foreach(System.IO.Stream temp in streamList)
            {
                try
                {
                    temp.Write(bytes);
                }
                catch (System.IO.IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error occurred when sending data", ex);
                }
            }
            *
            */
            AudioTrack _output;

            int buffsize = AudioTrack.GetMinBufferSize(44100, ChannelOut.Stereo, Android.Media.Encoding.Pcm16bit);
            //_output = new AudioTrack(Android.Media.Stream.Music, 44100, ChannelOut.Stereo, Android.Media.Encoding.Pcm16bit,
            //buffsize, AudioTrackMode.Stream);
            var AABuilder = new AudioAttributes.Builder();

            AABuilder.SetContentType(AudioContentType.Music);
            AABuilder.SetUsage(AudioUsageKind.Media);

            var AfBuilder = new AudioFormat.Builder();
            AfBuilder.SetSampleRate(44100);
            AfBuilder.SetEncoding(Android.Media.Encoding.Pcm16bit);
            AfBuilder.SetChannelMask(ChannelOut.Stereo);


            _output = new AudioTrack(AABuilder.Build(), AfBuilder.Build(), buffsize, AudioTrackMode.Stream, AudioManager.AudioSessionIdGenerate);
            _output.Play();
            try
            {
                byte[] buffer = new byte[1000];
                int bytesReturned = 1;

                while (bytesReturned > 0)
                {
                    bytesReturned = stream.Read(buffer, 0, buffer.Length);
                    mmOutStream.Write(buffer);
                    _output.Write(buffer, 0, buffer.Length);
                    //DependencyService.Get<BluetoothManager>().Write(buffer);
                }
                stream.Close();
                
            }
            catch (System.IO.IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occurred when sending data", ex);
            }

        }
    }
}