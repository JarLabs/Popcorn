﻿using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Popcorn.Models.Localization;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;

namespace Popcorn.ViewModels.Settings
{
    /// <summary>
    /// Application's settings
    /// </summary>
    public sealed class SettingsViewModel : ViewModelBase
    {
        #region Logger

        /// <summary>
        /// Logger of the class
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        #region Property -> DownloadLimit

        private int _downloadLimit;

        /// <summary>
        /// Download limit
        /// </summary>
        public int DownloadLimit
        {
            get { return _downloadLimit; }
            set { Set(() => DownloadLimit, ref _downloadLimit, value); }
        }

        #endregion

        #region Property -> UploadLimit

        private int _uploadLimit;

        /// <summary>
        /// Upload limit
        /// </summary>
        public int UploadLimit
        {
            get { return _uploadLimit; }
            set { Set(() => UploadLimit, ref _uploadLimit, value); }
        }

        #endregion

        #region Property -> Language

        private Language _language;

        /// <summary>
        /// Language
        /// </summary>
        public Language Language
        {
            get { return _language; }
            private set { Set(() => Language, ref _language, value); }
        }

        #endregion

        #endregion

        #region Commands

        #region Command -> InitializeAsyncCommand

        /// <summary>
        /// Command used to initialize asynchronously properties
        /// </summary>
        public RelayCommand InitializeAsyncCommand { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SettingsViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            Logger.Debug("Initializing a new instance of SettingsViewModel");

            RegisterCommands();
        }

        #endregion

        #region Methods

        #region Method -> InitializeAsync

        /// <summary>
        /// Load asynchronously the languages of the application for the current instance
        /// </summary>
        /// <returns>Instance of SettingsViewModel</returns>
        private async Task InitializeAsync()
        {
            Language = await Language.CreateAsync();
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            InitializeAsyncCommand = new RelayCommand(async () => await InitializeAsync());
        }

        #endregion

        #endregion
    }
}