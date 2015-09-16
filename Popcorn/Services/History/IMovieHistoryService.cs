﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Popcorn.Models.Genre;
using Popcorn.Models.Movie.Full;
using Popcorn.Models.Movie.Short;

namespace Popcorn.Services.History
{
    public interface IMovieHistoryService
    {
        /// <summary>
        /// Set if movies have been seen or set as favorite
        /// </summary>
        /// <param name="movies">All movies to compute</param>
        Task SetMovieHistoryAsync(IEnumerable<MovieShort> movies);

        /// <summary>
        /// Get the favorites movies
        /// </summary>
        /// <param name="genre">The genre of the movies</param>
        /// <param name="ratingFilter">Used to filter by rating</param>
        /// <returns>Favorites movies</returns>
        Task<IEnumerable<MovieShort>> GetFavoritesMoviesAsync(MovieGenre genre, double ratingFilter);

        /// <summary>
        /// Get the seen movies
        /// </summary>
        /// <returns>Seen movies</returns>
        /// <param name="genre">The genre of the movies</param>
        /// <param name="ratingFilter">Used to filter by rating</param>
        Task<IEnumerable<MovieShort>> GetSeenMoviesAsync(MovieGenre genre, double ratingFilter);

        /// <summary>
        /// Set the movie as favorite
        /// </summary>
        /// <param name="movie">Favorite movie</param>
        Task SetFavoriteMovieAsync(MovieShort movie);

        /// <summary>
        /// Set a movie as seen
        /// </summary>
        /// <param name="movie">Seen movie</param>
        Task SetHasBeenSeenMovieAsync(MovieFull movie);
    }
}