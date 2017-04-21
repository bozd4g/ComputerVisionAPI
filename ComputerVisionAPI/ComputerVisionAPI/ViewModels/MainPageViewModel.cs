using ComputerVisionAPI.Annotations;
using ComputerVisionAPI.Models;
using ComputerVisionAPI.Provider;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComputerVisionAPI.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            _takeAPhotoCommand = new Command(onTakePhoto);
            _selectAPhotoCommand = new Command(onSelectPhoto);
        }

        private ComputerVisionModel.Rootobject _model =
            new ComputerVisionModel.Rootobject();

        private string
            _metaData,
            _captionText,
            _tags,
            _selectedPhoto;

        private bool _isVisible;

        private ICommand
            _takeAPhotoCommand,
            _selectAPhotoCommand;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public string MetaData
        {
            get { return _metaData; }
            set
            {
                _metaData = value;
                OnPropertyChanged(nameof(MetaData));
            }
        }

        public string CaptionText
        {
            get { return _captionText; }
            set
            {
                _captionText = value;
                OnPropertyChanged(nameof(CaptionText));
            }
        }

        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        public string SelectedPhoto
        {
            get { return _selectedPhoto; }
            set
            {
                _selectedPhoto = value;
                OnPropertyChanged(nameof(SelectedPhoto));
            }
        }

        public ICommand TakeAPhotoCommand
        {
            get { return _takeAPhotoCommand; }
            set
            {
                if (_takeAPhotoCommand == null)
                    return;
                _takeAPhotoCommand = value;
            }
        }

        public ICommand SelectAPhotoCommand
        {
            get { return _selectAPhotoCommand; }
            set
            {
                if (_selectAPhotoCommand == null)
                    return;
                _selectAPhotoCommand = value;
            }
        }

        #region Methods

        private async void onTakePhoto()
        {
            try
            {
                var media = CrossMedia.Current;
                if (!media.IsTakePhotoSupported)
                    await Application.Current.MainPage.DisplayAlert("Error", "Camera not available!", "OK");

                var img = await media.TakePhotoAsync(new StoreCameraMediaOptions());
                if (img != null)
                {
                    var manager = new ServiceManager();
                    var result = await manager.Request(img.GetStream());
                    _model = JsonConvert.DeserializeObject<ComputerVisionModel.Rootobject>(result);
                    IsVisible = true;
                    BindData();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Message: " + ex.Message, "OK");
            }
        }

        private async void onSelectPhoto()
        {
            try
            {
                var media = CrossMedia.Current;
                if (!media.IsPickPhotoSupported)
                    await Application.Current.MainPage.DisplayAlert("Error", "File directory is not found!", "OK");

                var img = await media.PickPhotoAsync();
                if (img != null)
                {
                    var manager = new ServiceManager();
                    var result = await manager.Request(img.GetStream());
                    _model = JsonConvert.DeserializeObject<ComputerVisionModel.Rootobject>(result);

                    SelectedPhoto = img.Path + " is selected.";
                    IsVisible = true;
                    BindData();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Message: " + ex.Message, "OK");
            }
        }

        private void BindData()
        {
            if (_model != null)
            {
                MetaData = "Meta\n";
                CaptionText = "Description\n";
                Tags = "Tags\n";

                MetaData = string.Format($"H: {_model.metadata.height}\n" +
                                         $"W: {_model.metadata.width}\n" +
                                         $"Format: {_model.metadata.format}");

                for (int i = 0; i < _model.description.captions.Length; i++)
                {
                    CaptionText += _model.description.captions[i].text;
                    if (i != _model.description.captions.Length)
                        CaptionText += ", ";
                }

                for (int i = 0; i < _model.description.tags.Length; i++)
                {
                    Tags += _model.description.tags[i];
                    if (i != _model.description.tags.Length)
                        Tags += ", ";
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}