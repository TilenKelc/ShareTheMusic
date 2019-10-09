using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MediaManager;

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
    }
}
