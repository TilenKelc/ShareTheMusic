using System;
using System.Collections.Generic;
using System.Text;

namespace ShareTheMusic
{
    public interface MediaManager
    {
        bool PlayPause(string url);

        void Stop();

        //void PlayFromByte(byte[] byteArrayFile);

        IList<string> GetFileLocation();
    }
}
