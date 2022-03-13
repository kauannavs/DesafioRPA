using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PokeApi.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace PokeApi.Services
{
    public class PokemonService
    {
        #region Atributos
        static string NameArquivo = "PokemonInfo_.txt";
        static string PastaArmazenamento = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Armazenamento");
        static readonly string CaminhoArquivo = Path.Combine(PastaArmazenamento, NameArquivo);
        private const string baseUrl = "https://pokeapi.co/api/v2/pokemon/";
        #endregion

        public Pokemon GetPokemon(string pokemonName, bool download)
        {
            try
            {
                var client = new RestClient(baseUrl);
                var request = new RestRequest(pokemonName, Method.Get);
                var response = client.ExecuteAsync(request).Result;

                if (!response.IsSuccessful) 
                    throw new Exception("Response: "+ response.Content);

                Pokemon pokemon = new Pokemon()
                {
                    Name = this.GetName(response.Content),
                    Peso = GetPeso(response.Content),
                    Altura = GetAltura(response.Content),
                    Imagem = GetImagem(response.Content, download),
                    Habilidades = GetHabilidades(response.Content),
                    Tipos = GetTipos(response.Content)
                };

                Save(pokemon);
                return pokemon;
            }
            catch (Exception ex) 
            {
                throw new Exception("PokemonService.GetPokemon() -" + ex.Message);
            }
            
        }

        public string GetName(string response)
        {
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(response);
                JArray jsonForms = JsonConvert.DeserializeObject<JArray>(Convert.ToString(json["forms"]));
                JObject forms = JsonConvert.DeserializeObject<JObject>(Convert.ToString(jsonForms.First));
                string name = Convert.ToString(forms.First).Replace("\"name\":", "").Replace("\"", "").Trim();

                return name;
            }
            catch (Exception ex)
            {
                throw new Exception("PokemonService.GetName() -" + ex.Message);
            }
        }

        public string GetPeso(string response)
        {
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(response);
                string weight = Convert.ToString(json["weight"]);
            
                return weight;
            }
            catch (Exception ex)
            {
                throw new Exception("PokemonService.GetPeso() -" + ex.Message);
            }
        }

        public string GetAltura(string response)
        {
            try
            { 
                JObject json = JsonConvert.DeserializeObject<JObject>(response);
                string height = Convert.ToString(json["height"]);
                return height;
            }
            catch (Exception ex)
            {
                throw new Exception("PokemonService.GetAltura() -" + ex.Message);
            }
        }

        public string GetImagem(string response, bool download)
        {
            try 
            { 
                JObject json = JsonConvert.DeserializeObject<JObject>(response);
                JObject jsonSprites = JsonConvert.DeserializeObject<JObject>(Convert.ToString(json["sprites"]));
                string front_default = "";
                foreach (var sprite in jsonSprites) 
                {
                    if (Convert.ToString(sprite.Key).Contains("front_default")) 
                    {
                        front_default = Convert.ToString(sprite.Value);
                        break;
                    }
                }

                if (download) DownloadImage(front_default);
                
                return front_default;
            }
            catch (Exception ex)
            {
                throw new Exception("PokemonService.GetImagem() -" + ex.Message);
            }
        }

        public void DownloadImage(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    throw new Exception("Pokemon não possui imagem para download");

                var client = new RestClient(url);
                var stream = client.DownloadStreamAsync(new RestRequest("", Method.Get)).ContinueWith((image) =>
                {
                    Stream img = image.Result;
                    var info = url.Split("/");
                    var nameFile = info[info.Length - 1];
                    var fileName = PastaArmazenamento + "\\" + nameFile;

                    if (File.Exists(fileName)) File.Delete(fileName);
                    using (var arquivo = File.Create(fileName))
                    {
                        img.CopyTo(arquivo);
                    }
                });

            }
            catch (Exception ex)
            {
                throw new Exception("PokemonService.DownloadImage() -" + ex.Message);
            }
        }

        public List<string> GetHabilidades(string response)
        {
            try
            { 
                JObject json = JsonConvert.DeserializeObject<JObject>(response);
                JArray jsonAbilites = JsonConvert.DeserializeObject<JArray>(Convert.ToString(json["abilities"]));
                List<string> habilidades = new List<string>();
                foreach (var jsonAbility in jsonAbilites)
                {
                    JObject ability = JsonConvert.DeserializeObject<JObject>(Convert.ToString(jsonAbility.First).Replace("\"ability\":", ""));
                    var abilityName = Convert.ToString(ability.First).Replace("\"name\":", "").Replace("\"", "").Trim();
                    habilidades.Add(abilityName);
                }

                return habilidades;
            }
            catch (Exception ex)
            {
                throw new Exception("PokemonService.GetHabilidades() -" + ex.Message);
            }
        }

        public List<string> GetTipos(string response)
        {
            try 
            { 
                JObject json = JsonConvert.DeserializeObject<JObject>(response);
                JArray jsonTypes = JsonConvert.DeserializeObject<JArray>(Convert.ToString(json["types"]));
                List<string> tipos = new List<string>();
                foreach (var jsonType in jsonTypes)
                {
                    JObject type = JsonConvert.DeserializeObject<JObject>(Convert.ToString(jsonType.Last).Replace("\"type\":", ""));
                    var abilityName = Convert.ToString(type.First).Replace("\"name\":", "").Replace("\"", "").Trim();
                    tipos.Add(abilityName);
                }

                return tipos;
            }
            catch (Exception ex)
            {
                throw new Exception("PokemonService.GetTipos() -" + ex.Message);
            }
        }

        public void VerificaPasta() 
        {
            if (!Directory.Exists(PastaArmazenamento)) Directory.CreateDirectory(PastaArmazenamento);
            if (File.Exists(CaminhoArquivo)) File.Delete(CaminhoArquivo);
            
            string pokemonInfo = "";
            
            #region Construindo primeira linha do arquivo (linha de titulos)
            var obj = new Pokemon();
            foreach (var propriedade in obj.GetType().GetProperties())
            {
                pokemonInfo += (string.IsNullOrEmpty(pokemonInfo) ? "" : ";") + propriedade.Name;
            }
            #endregion
            
            File.AppendAllText(CaminhoArquivo, pokemonInfo+"\n");
        }

        public void Save(Pokemon pokemon) 
        {
            try
            { 
                string propriedades = "";
                //Busca todas as propriedades do objeto pokemon
                foreach (var propriedade in pokemon.GetType().GetProperties())
                {
                    string propriedadeValue = "";
                    #region trata valor da propriedade
                    if (propriedade.GetValue(pokemon).ToString().Contains("List"))
                    {
                        System.Collections.IList lista = (System.Collections.IList)propriedade.GetValue(pokemon);
                        foreach (var item in lista)
                        {
                            propriedadeValue += (string.IsNullOrEmpty(propriedadeValue) ? "" : ",") + item;
                        }
                    }
                    else
                        propriedadeValue = propriedade.GetValue(pokemon).ToString();
                    #endregion

                    propriedades += (string.IsNullOrEmpty(propriedades) ? "" : ";") +
                    propriedadeValue.Replace(";", "-");
                }

                lock (this) {
                    using (var write = File.AppendText(CaminhoArquivo))
                    {
                        write.WriteLine(propriedades);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PokemonService.Save() -" + ex.Message);
            }
        }

    }
}
