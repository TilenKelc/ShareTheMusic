using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

using System.IO;
using MP3Sharp;

namespace ShareTheMusic
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        List<string> pathList = new List<string>();
        bool bluetoothConnection = false;

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
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
                if (bluetoothConnection)
                {
                    SendData();
                    Thread.Sleep(800);
                }

                bool temp = DependencyService.Get<MediaManager>().PlayPause(pathList[ShowSongFile.SelectedIndex]);

                //Stop.IsVisible = true;
                if (temp == true)
                {
                    PlayPause.Source = "pause.png";
                }
                else
                {
                    PlayPause.Source = "play.png";
                }
            }
            else
            {
                await DisplayAlert("WARNING", "No song selected", "OK");
            }
            
        }

        private void StopMusic(object sender, EventArgs e)
        {
            //Stop.IsVisible = false;
            //PlayPause.Text = "Play";
            //DependencyService.Get<MediaManager>().Stop();   
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
                    DependencyService.Get<BluetoothManager>().Read();

                    bluetoothConnection = true;
                }
                else if(!string.IsNullOrEmpty(deviceText) && deviceText == "Host")
                {
                    DependencyService.Get<BluetoothManager>().runServerSide();
                    bluetoothConnection = true;
                }
            }
        }

        public void SendData()
        {   
            System.Threading.Tasks.Task.Run(() =>
            {
                MP3Stream stream = new MP3Stream(pathList[ShowSongFile.SelectedIndex]);
                byte[] buffer = new byte[1000];
                int bytesReturned = 1;

                while (bytesReturned > 0)
                {
                    bytesReturned = stream.Read(buffer, 0, buffer.Length);
                    DependencyService.Get<BluetoothManager>().Write(buffer);
                }
                stream.Close();

            }).ConfigureAwait(false);
        }

        private void NextOne(object sender, EventArgs e)
        {
            string song;

            if((ShowSongFile.SelectedIndex+1) >= pathList.Count)
            {
                song = pathList[0];
            }
            else
            {
                song = pathList[ShowSongFile.SelectedIndex+1];
            }

            DependencyService.Get<MediaManager>().Stop();
            bool temp = DependencyService.Get<MediaManager>().PlayPause(song);

            if (temp == true)
            {
                PlayPause.Source = "pause.png";
            }
            else
            {
                PlayPause.Source = "play.png";
            }
        }

        private void PreviousOne(object sender, EventArgs e)
        {
            string song;

            if ((ShowSongFile.SelectedIndex - 1) <= pathList.Count)
            {
                song = pathList[0];
            }
            else
            {
                song = pathList[ShowSongFile.SelectedIndex - 1];
            }

            DependencyService.Get<MediaManager>().Stop();
            bool temp = DependencyService.Get<MediaManager>().PlayPause(song);

            if (temp == true)
            {
                PlayPause.Source = "pause.png";
            }
            else
            {
                PlayPause.Source = "play.png";
            }
        }
    }
}
