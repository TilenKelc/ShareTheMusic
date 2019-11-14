using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        Stream mmInStream;
        Stream mmOutStream;

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

            while (true)
            {
                try
                {
                    socket = mServerSocket.Accept();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Socket's accept() method failed", ex);
                }

                if (socket != null)
                {
                    manageConnectedSocket(socket);
                    mServerSocket.Close();
                    break;
                }
            }
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
            manageConnectedSocket(mSocket);
        }

        public void cancel()
        {
            try
            {
                mSocket.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Could not close the connect socket", ex);
            }
        }

        public void manageConnectedSocket(BluetoothSocket socket)
        {
            mSocket = socket;
            Stream tmpIn = null;
            Stream tmpOut = null;

            try
            {
                tmpIn = socket.InputStream;
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occurred when creating input stream", ex);
            }
            try
            {
                tmpOut = socket.OutputStream;
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occurred when creating output stream", ex);
            }

            mmInStream = tmpIn;
            mmOutStream = tmpOut;
        }

        public byte[] read()
        {
            try
            {
                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;

                do
                {
                    numberOfBytesRead = mmInStream.Read(myReadBuffer, 0, myReadBuffer.Length);

                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                }
                while (mmInStream.IsDataAvailable());

                System.Diagnostics.Debug.WriteLine(myCompleteMessage, "Reading");
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Input stream was disconnected", ex);
            }

            return null;
        }

        public void write(byte[] bytes)
        {
            try
            {
                mmOutStream.Write(bytes);
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occurred when sending data", ex);
            }
        }
    }
}