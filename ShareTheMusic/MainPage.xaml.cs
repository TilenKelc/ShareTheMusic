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
            /*
            string temp = "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of de Finibus Bonorum et Malorum (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, Lorem ipsum dolor sit amet.., comes from a line in section 1.10.32." +
                "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of de Finibus Bonorum et Malorum (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, Lorem ipsum dolor sit amet.., comes from a line in section 1.10.32." +
                "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of de Finibus Bonorum et Malorum (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, Lorem ipsum dolor sit amet.., comes from a line in section 1.10.32." +
                "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of de Finibus Bonorum et Malorum (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, Lorem ipsum dolor sit amet.., comes from a line in section 1.10.32." +
                "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of de Finibus Bonorum et Malorum (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, Lorem ipsum dolor sit amet.., comes from a line in section 1.10.32." +
                "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of de Finibus Bonorum et Malorum (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, Lorem ipsum dolor sit amet.., comes from a line in section 1.10.32." +
                "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of de Finibus Bonorum et Malorum (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, Lorem ipsum dolor sit amet.., comes from a line in section 1.10.32.";
            byte[] send = Encoding.ASCII.GetBytes(temp);
            DependencyService.Get<BluetoothManager>().Write(send);*/

            
            byteArrayFile = System.IO.File.ReadAllBytes(pathList[ShowSongFile.SelectedIndex]);
            DependencyService.Get<BluetoothManager>().Write(byteArrayFile);

        }

        private void CheckButton(object sender, EventArgs e)
        {
            DependencyService.Get<BluetoothManager>().Read();
        }

        private void Convert(object sender, EventArgs e)
        {
            
            //DependencyService.Get<MediaManager>().PlayFromByte(byteArrayFile);
        }
    }
}
