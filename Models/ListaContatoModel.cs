using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class ListaContatoModel : ComunicacaoModel {
        public ListaContatoModel(string acao, List<ContatoModel> contatos)
        {
            Acao = acao;
            Contatos = contatos;
        }
        public List<ContatoModel> Contatos { get; set; }
    }
    public class ContatoModel
    {
        public ContatoModel(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}
