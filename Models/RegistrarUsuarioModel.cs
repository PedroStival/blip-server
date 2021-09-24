using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class RegistrarUsuarioModel : ComunicacaoModel
    {
        public RegistrarUsuarioModel(string acao, string nome)
        {
            Nome = nome;
            Acao = acao;
        }
        public string Nome { get; set; }
    }
}
