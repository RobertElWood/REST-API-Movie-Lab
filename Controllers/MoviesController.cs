using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_API_Lab.Models;

namespace Movie_API_Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieContext _context;

        public MoviesController(MovieContext context)
        {
            _context = context;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        //GET: Gets all movie titles within the database currently
        [HttpGet("GetTitles")]
        public async Task<ActionResult<IEnumerable<String>>> GetTitlesAlphabetical()
        {
            return GetTitles();
        }

        //GET: Gets all movie titles with a specific keyword within them
        [HttpGet("SearchByTitle/{title}")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchByTitle(string title)
        {
            return await _context.Movies.Where(m => m.Title.ToLower() == title).ToListAsync();
        }

        //GET: Gets all movie titles with a specific keyword within them
        [HttpGet("SearchByKeyword/{keyword}")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchByKeyword(string keyword)
        {
            return await _context.Movies.Where(m => m.Title.Contains(keyword) == true).ToListAsync();
        }

        //GET: Gets a single random movie from the movie list and returns it
        [HttpGet("GetRandomMovie")]
        public async Task<ActionResult<Movie>> GetRandomMovie()
        {
            return GetRandom();
        }

        //GET: Gets a single random movie from a list of movies sorted by Genre
        [HttpGet("GetRandomMovieByGenre/{genre}")]
        public async Task<ActionResult<Movie>> GetRandomMovieByGenre(string genre)
        {
            return GetRandomGenre(genre);
        }

        //GET: Gets a random selection of movies from the movie list
        [HttpGet("GetRandomMovieList/{numMovies}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetRandomMovieList(int numMovies)
        {
            return GetRandomList(numMovies);
        }

        //GET: Gets all movie genre names within the database currently
        [HttpGet("GetGenres")]
        public async Task<ActionResult<IEnumerable<String>>> GetGenresAlphabetical()
        {
            return GetGenres();
        }

        //GET: Searches all movies and only returns movie information on those of a specific genre
        //Use GetGenresAlphabetical to find genres to search for!
        [HttpGet("SearchByGenre/{genre}")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchByGenre(string genre)
        {
            return await _context.Movies.Where(m => m.Genre.Contains(genre) == true).ToListAsync();
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        //Helper function to display titles from Movies in db.
        private List<string> GetTitles()
        {
            List<string> titles = new List<string>();
            List<Movie> movies = _context.Movies.ToList();

            foreach (Movie m in movies)
            {
                titles.Add(m.Title);
            }

            titles.Sort();

            return titles;
        }

        //Helper function for getting genres from Movies in db.
        private List<string> GetGenres()
        {
            List<string> genres = new List<string>();
            List<Movie> movies = _context.Movies.ToList();

            foreach (Movie m in movies)
            {
                genres.Add(m.Genre);
            }

            genres.Sort();

            return genres;
        }

        //Helper function for grabbing ONE random movie from list
        private Movie GetRandom()
        {
            List <Movie> movies = _context.Movies.ToList();
            var random = new Random();
            int randomIndex = random.Next(movies.Count);

            Movie randomMovie = movies[randomIndex];

            return randomMovie;
        }

        //Helper function for grabbing a Random movie from one genre
        private Movie GetRandomGenre(string genre)
        {
            List<Movie> movies = _context.Movies.Where(m => m.Genre.Contains(genre) == true).ToList();

            var random = new Random();
            int randomIndex = random.Next(movies.Count);

            Movie randomMovie = movies[randomIndex];

            return randomMovie;
        }

        //Helper function for grabbing multiple random movies
        //Note that if the same movie is rolled twice, then that pass of iteration will not count.
        //This guarantees that the same movie will not turn up in the list twice.
        private List<Movie> GetRandomList(int numMovies)
        {
            List<Movie> movies = _context.Movies.ToList();

            List<Movie> randomPicks = new List<Movie>();

            for (int i = 0; i < numMovies; i++)
            {
                var random = new Random();
                var randomIndex = random.Next(movies.Count);
                Movie randomMovie = movies[randomIndex];
                
                if (randomPicks.Contains(randomMovie) == false)
                {
                    randomPicks.Add(randomMovie);
                } 
                else
                {
                    i -= 1;
                    continue;
                }
            }

            return randomPicks;
        }
    }
}
