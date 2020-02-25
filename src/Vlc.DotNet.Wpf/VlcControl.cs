using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Vlc.DotNet.Wpf
{
    using System.Windows;
    using System.Windows.Controls;
    using Vlc.DotNet.Core.Interops.Signatures;

    /// <summary>
    /// The Wpf component that allows to display a video in a Wpf way
    /// </summary>
    public class VlcControl: UserControl, IDisposable
    {
        /// <summary>
        /// The Viewbox that contains the video image
        /// </summary>
        private Viewbox viewBox;

        /// <summary>
        /// The image that displays the video
        /// </summary>
        private readonly Image videoContent = new Image { ClipToBounds = true };

        /// <summary>
        /// The class that provides video source
        /// </summary>
        private readonly VlcVideoSourceProvider sourceProvider;

        public VlcVideoSourceProvider SourceProvider => sourceProvider;

        /// <summary>
        /// Defines if <see cref="VlcVideoSourceProvider.VideoSource"/> pixel format is <see cref="PixelFormats.Bgr32"/> or <see cref="PixelFormats.Bgra32"/>
        /// </summary>
        public bool IsAlphaChannelEnabled
        {
            get
            {
                return sourceProvider.IsAlphaChannelEnabled;
            }

            set
            {
                sourceProvider.IsAlphaChannelEnabled = value;
            }
        }
        
        /// <summary>
        /// Gets or set the current time of media
        /// </summary>
        public long Time
        {
            get { return (long)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }


        /// <summary>
        /// Gets or sets the volume of media
        /// </summary>
        public int Volume
        {
            get { return (int)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty as the backing store for VolumeProperty.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register(nameof(Volume), typeof(int), typeof(VlcControl), new PropertyMetadata(0));


        /// <summary>
        /// The DependencyProperty as the backing store for TimeProperty.
        /// </summary>
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(nameof(Time), typeof(long), typeof(VlcControl), new PropertyMetadata((long)0));


        /// <summary>
        /// Gets the state of <see cref="MediaPlayer"/>, id it is not created yet <see cref="MediaStates.NothingSpecial"/> is returned
        /// </summary>
        public MediaStates State
        {
            get { return (MediaStates)GetValue(StateProperty); }
        }

        /// <summary>
        /// The DependencyProperty as the backing store for the readonly property StatePropertyKey
        /// </summary>
        internal static readonly DependencyPropertyKey StatePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(State), typeof(MediaStates), typeof(VlcControl), new PropertyMetadata(MediaStates.NothingSpecial));

        /// <summary>
        /// The DependencyProperty used to allow access to StateProperty readonly property
        /// </summary>
        public static readonly DependencyProperty StateProperty = StatePropertyKey.DependencyProperty;


        /// <summary>
        /// The constructor
        /// </summary>
        public VlcControl()
        {
            sourceProvider = new VlcVideoSourceProvider(this.Dispatcher);
            this.viewBox = new Viewbox
            {
                Child = this.videoContent,
                Stretch = Stretch.Uniform
            };

            this.Content = this.viewBox;
            this.Background = Brushes.Black;

            // Binds the VideoSource to the Image.Source property
            this.videoContent.SetBinding(Image.SourceProperty, new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = sourceProvider });

            //Binds the Time to VlcControl.TimeProperty
            this.SetBinding(TimeProperty, new Binding(nameof(VlcVideoSourceProvider.Time)) { Source = sourceProvider,Mode=BindingMode.TwoWay,UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged });
            this.SetBinding(VolumeProperty, new Binding(nameof(VlcVideoSourceProvider.Volume)) { Source = sourceProvider, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            sourceProvider.StateChanged += SourceProvider_StateChanged;
        }

        private void SourceProvider_StateChanged(object sender, VlcMediaPlayerStateChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(()=>
            {
                SetValue(StatePropertyKey, e.State);
            }));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            sourceProvider.StateChanged -= SourceProvider_StateChanged;
            sourceProvider.Dispose();
        }
    }
}