using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

using System.Threading.Tasks;
using System.IO;

namespace ShareTheMusic
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        List<string> pathList = new List<string>();

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
            if(ShowSongFile.SelectedItem != null)
            {

                bool temp = DependencyService.Get<MediaManager>().PlayPause(pathList[ShowSongFile.SelectedIndex]);
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
                string deviceText = await DisplayActionSheet("Select device", "Cancel", null, deviceNames);
                
                if (!string.IsNullOrEmpty(deviceText) && deviceText != "Cancel")
                {
                    DependencyService.Get<BluetoothManager>().runClientSide(deviceText);
                }
                else
                {
                    DependencyService.Get<BluetoothManager>().runServerSide();
                }
                Send.IsVisible = true;
            }
        }

        private void SendData(object sender, EventArgs e)
        {
            string test = "Testing";
            byte[] temp = Encoding.ASCII.GetBytes(test);
            DependencyService.Get<BluetoothManager>().write(temp);
        }

        private void CheckButton(object sender, EventArgs e)
        {
            if (DependencyService.Get<BluetoothManager>().read() != null)
            {
                byte[] temp = DependencyService.Get<BluetoothManager>().read();
                string result = Encoding.UTF8.GetString(temp);
                CheckTxt.Text = result;
            }
        }

        private void Convert(object sender, EventArgs e)
        {
            byte[] temp = DependencyService.Get<MediaManager>().Convert(pathList[ShowSongFile.SelectedIndex]);
        }
    }
}
