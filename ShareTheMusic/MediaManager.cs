using System;
using System.Collections.Generic;
using System.Text;

namespace ShareTheMusic
{
    public interface MediaManager
    {
        bool PlayPause(string url);

        void Stop();

        IList<string> GetFileLocation();
    }
}
