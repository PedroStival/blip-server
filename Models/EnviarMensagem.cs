using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class EnviarMensagem : ComunicacaoModel
    {
        public EnviarMensagem(string acao, string mensagem, string usuarioMencionado = null)
        {
            Acao = acao;
            Mensagem = mensagem;
            UsuarioMencionado = usuarioMencionado;
        }
        public string Mensagem { get; set; }
        public string UsuarioMencionado { get; set; }
    }
}
