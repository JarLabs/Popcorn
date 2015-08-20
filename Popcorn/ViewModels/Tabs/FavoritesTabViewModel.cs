﻿using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Comparers;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.ViewModels.Main;

namespace Popcorn.ViewModels.Tabs
{
    public class FavoritesTabViewModel : TabsViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the FavoritesTabViewModel class.
        /// </summary>
        public FavoritesTabViewModel()
        {
            RegisterMessages();
            RegisterCommands();
            TabName = LocalizationProviderHelper.GetLocalizedValue<string>("FavoritesTitleTab");
        }

        #endregion

        #region Methods

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<ChangeLanguageMessage>(
                this,
                language => { TabName = LocalizationProviderHelper.GetLocalizedValue<string>("FavoritesTitleTab"); });

            Messenger.Default.Register<ChangeFavoriteMovieMessage>(
                this,
                async message =>
                {
                    await LoadMoviesAsync();
                });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            ReloadMovies = new RelayCommand(async () =>
            {
                var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                mainViewModel.IsConnectionInError = false;
                await LoadMoviesAsync();
            });
        }

        #endregion

        #region Method -> LoadMoviesAsync

        /// <summary>
        /// Load favorites movies
        /// </summary>
        public override async Task LoadMoviesAsync()
        {
            IsLoadingMovies = true;
            var favoritesMovies = await MovieHistoryService.GetFavoritesMoviesAsync();
            var movies = favoritesMovies.ToList();
            var moviesToAdd = movies.Except(Movies, new MovieShortComparer()).ToList();
            var moviesToRemove = Movies.Except(movies, new MovieShortComparer()).ToList();
            foreach (var movie in moviesToAdd)
            {
                Movies.Add(movie);
            }

            foreach (var movie in moviesToRemove)
            {
                Movies.Remove(movie);
            }

            IsLoadingMovies = false;
            IsMovieFound = Movies.Any();
            CurrentNumberOfMovies = Movies.Count();
            MaxNumberOfMovies = movies.Count();
            await MovieService.DownloadCoverImageAsync(moviesToAdd);
        }

        #endregion

        #endregion
    }
}