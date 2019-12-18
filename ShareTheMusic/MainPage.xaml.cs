using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

using System.IO;

namespace ShareTheMusic
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        List<string> pathList = new List<string>();
        bool bluetoothConnection = false;
        bool musicPlayFromByte = false;

        public MainPage()
        {
            InitializeComponent();
            LoadSongFilePaths();
        }

        public void LoadSongFilePaths()
        {
            pathList = (List<string>)DependencyService.Get<MediaManager>().GetFileLocation();
            List<string> nameList = new List<string>();

            foreach (var path in pathList)
            {
                nameList.Add(Path.GetFileName(path));

            }
            ShowSongFile.ItemsSource = nameList;
        }

        async private void PlayMusic(object sender, EventArgs e)
        {
            if (ShowSongFile.SelectedItem != null)
            {
                bool temp = DependencyService.Get<MediaManager>().PlayPause(pathList[ShowSongFile.SelectedIndex]);
                
                if (bluetoothConnection == false)
                {
                    byte[] byteArrayFile = System.IO.File.ReadAllBytes(pathList[ShowSongFile.SelectedIndex]);
                    DependencyService.Get<BluetoothManager>().Write(byteArrayFile);
                }

                Stop.IsVisible = true;
                if (temp == true)
                {
                    PlayPause.Text = "Pause";
                }
                else
                {
                    PlayPause.Text = "Play";
                }
            }
            else
            {
                await DisplayAlert("WARNING", "No song selected", "OK");
            }
     
        }

        private void StopMusic(object sender, EventArgs e)
        {
            Stop.IsVisible = false;
            PlayPause.Text = "Play";
            DependencyService.Get<MediaManager>().Stop();   
        }

        private async void Connect(object sender, EventArgs e)
        {
            string checkBluetooth = DependencyService.Get<BluetoothManager>().checkBluetooth(false);
            if(checkBluetooth == "NoBluetooth")
            {
                await DisplayAlert("WARNING", "This device does not support Bluetooth", "OK");
            }
            else if (checkBluetooth == "TurnOnBluetooth")
            {
                bool result = await DisplayAlert("Enable Bluetooth", "Enable Bluetooth?", "OK", "Cancel");
                if (result)
                {
                    checkBluetooth = DependencyService.Get<BluetoothManager>().checkBluetooth(true);
                    Thread.Sleep(1000);
                }
            }
            if (checkBluetooth == "AlreadyOn")
            {
                string[] deviceNames = DependencyService.Get<BluetoothManager>().findBTdevices();
                string deviceText = await DisplayActionSheet("Select device", "Cancel", "Host", deviceNames);
                
                if (!string.IsNullOrEmpty(deviceText) && deviceText != "Host" && deviceText != "Cancel")
                {
                    DependencyService.Get<BluetoothManager>().runClientSide(deviceText);
                    //DependencyService.Get<BluetoothManager>().Read();
                    musicPlayFromByte = true;
                    bluetoothConnection = true;
                }
                else if(!string.IsNullOrEmpty(deviceText) && deviceText == "Host")
                {
                    DependencyService.Get<BluetoothManager>().runServerSide();
                    bluetoothConnection = true;
                }
                Send.IsVisible = true;
                CheckTxt.IsVisible = true;
            }
        }
        byte[] byteArrayFile = null;
        private void SendData(object sender, EventArgs e)
        {       
            byteArrayFile = System.IO.File.ReadAllBytes(pathList[ShowSongFile.SelectedIndex]);
            DependencyService.Get<BluetoothManager>().Write(byteArrayFile);
        }

        private void CheckButton(object sender, EventArgs e)
        {
            DependencyService.Get<BluetoothManager>().Read();
        }
    }
}
