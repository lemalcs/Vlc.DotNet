using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlc.DotNet.Core.Interops.Signatures;

namespace Vlc.DotNet.Wpf
{
    /// <summary>
    /// Holds state of media file according to <see cref="MediaStates"/>.
    /// </summary>
    public class VlcMediaPlayerStateChangedEventArgs:EventArgs
    {
        /// <summary>
        /// Indicates the current state of media file
        /// </summary>
        public MediaStates State { get; private set; }

        public VlcMediaPlayerStateChangedEventArgs(MediaStates state)
        {
            State = state;
        }
    }
}
