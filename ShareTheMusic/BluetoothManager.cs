using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareTheMusic
{
    public interface BluetoothManager
    {
        string checkBluetooth(bool permition);

        string[] findBTdevices();

        void runServerSide();

        void runClientSide(string deviceName);

        void Cancel();

        void Read();

        //void Write(byte[] bytes);
        void Write(Stream stream);
    }
}
