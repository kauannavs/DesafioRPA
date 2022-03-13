using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PokeApi.Models;
using PokeApi.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;
        private PokemonService _pokemonService;

        private static readonly string[] pokemonList = new[]
        {
            "Charmander", "Squirtle", "Caterpie", "Weedle", "Pidgey", "Pidgeotto", "Rattata", "Spearow", "Fearow", "Arbok", "Pikachu", "Sandshrew"
        };

        public PokemonController(ILogger<PokemonController> logger)
        {
            _logger = logger;
            _pokemonService = new PokemonService();
        }

        /// <summary>
        /// 1. Consumir API de pokemons
        /// </summary>
        /// <returns> Informações dos pokemons pesquisados </returns>
        /// <response code="200">Retorna informações dos pokemons pesquisados</response>
        [HttpGet("BuscarInfo")]
        [Produces("application/json")]
        public List<Pokemon> Busca()
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            try
            {
                var TempoExecIni = DateTime.Now;
                _pokemonService.VerificaPasta();
                foreach (string pokemon in pokemonList)
                {
                    pokemons.Add(_pokemonService.GetPokemon(pokemon.ToLower(), false));
                    int size = pokemon.Length;
                }

                _logger.LogInformation("PokemonController.Busca() - Tempo de execução: " + (DateTime.Now - TempoExecIni));
            }
            catch (Exception ex)
            {
                _logger.LogError("PokemonController.Busca() - " + ex.Message);

            }
            return pokemons;
        }

        /// <summary>
        /// 2. Realizar consultas simulteneas
        /// </summary>
        /// <returns> Informações dos pokemons pesquisados </returns>
        /// <response code="200">Retorna informações dos pokemons pesquisados</response>
        [HttpGet("BuscarInfoOtimizado")]
        [Produces("application/json")]
        public List<Pokemon> BuscaOtimizada()
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            try
            {
                var TempoExecIni = DateTime.Now;
                _pokemonService.VerificaPasta();
                Parallel.ForEach(pokemonList, pokemon =>
                {
                    pokemons.Add(_pokemonService.GetPokemon(pokemon.ToLower(), false));
                    int size = pokemon.Length;
                });

                _logger.LogInformation("PokemonController.BuscaOtimizada() - Tempo de execução: " + (DateTime.Now - TempoExecIni));
            }
            catch (Exception ex)
            {
                _logger.LogError("PokemonController.BuscaOtimizada() - " + ex.Message);
            }

            return pokemons;
        }

        /// <summary>
        /// 3. [EXTRA] - Fazer o download da imagem do pokemon (executa busca otimizada) 
        /// </summary>
        /// <returns> Informações dos pokemons pesquisados e faz o download das imagens </returns>
        /// <response code="200">Retorna informações dos pokemons pesquisados</response>
        [HttpGet("Download")]
        [Produces("application/json")]
        public List<Pokemon> BuscaOtimizadaDownload()
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            try
            {
                var TempoExecIni = DateTime.Now;
                _pokemonService.VerificaPasta();
                Parallel.ForEach(pokemonList, pokemon =>
                {
                    pokemons.Add(_pokemonService.GetPokemon(pokemon.ToLower(), true));
                    int size = pokemon.Length;
                });

                _logger.LogInformation("PokemonController.BuscaOtimizadaDownload() - Tempo de execução: " + (DateTime.Now - TempoExecIni));
            }
            catch (Exception ex)
            {
                _logger.LogError("PokemonController.BuscaOtimizadaDownload() - " + ex.Message);
            }

            return pokemons;
        }

        /// <summary>
        /// 4. [EXTRA] - Busca Pokemons por array
        /// </summary>
        /// <remarks>
        ///  Exemplo:
        ///  ["Charmander", "Squirtle", "Caterpie", "Weedle", "Pidgey", "Pidgeotto", "Rattata", "Spearow", "Fearow", "Arbok", "Pikachu", "Sandshrew"]
        /// </remarks>
        /// <param name="pokemonsSearch">Lista de Pokemons</param>
        /// <returns> Informações dos pokemons pesquisados e faz o download das imagens </returns>
        /// <response code="200">Retorna informações dos pokemons pesquisados</response>
        [HttpPost]
        [Produces("application/json")]
        public List<Pokemon> Post(List<string> pokemonsSearch)
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            try
            {
                var TempoExecIni = DateTime.Now;
                _pokemonService.VerificaPasta();
                Parallel.ForEach(pokemonsSearch, pokemon =>
                {
                    pokemons.Add(_pokemonService.GetPokemon(pokemon.ToLower(), true));
                    int size = pokemon.Length;
                });

                _logger.LogInformation("PokemonController.Post() - Tempo de execução: " + (DateTime.Now - TempoExecIni));
            }
            catch (Exception ex)
            {
                _logger.LogError("PokemonController.Post() - " + ex.Message);
            }

            return pokemons;
        }
    }
}
