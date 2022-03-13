using System.Collections.Generic;

namespace PokeApi.Models
{
    public class Pokemon
    {
        public string Name { get; set; }
        public List<string> Tipos { get; set; }
        public List<string> Habilidades { get; set; }
        public string Peso { get; set; }
        public string Altura { get; set; }
        public string Imagem { get; set; }
    }
}
