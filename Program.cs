using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Text.Json;
using System.Linq;

namespace ConsoleApp3
{
    public class Program
    {

        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> _connections = new Dictionary<int, TcpClient>();
        static readonly Dictionary<int, string> _usuarios = new Dictionary<int, string>();

        static void Main(string[] args)
        {
            IniciarServidor();
        }

        private static void IniciarServidor()
        {
            int count = 1;

            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5000);
            ServerSocket.Start();

            while (true)
            {
                var client = ServerSocket.AcceptTcpClient();
                lock (_lock) _connections.Add(count, client);
                Console.WriteLine("Usuário conectado");

                Thread t = new Thread(DistribuirMensagens);
                t.Start(count);
                count++;
            }
        }

        private static void DistribuirMensagens(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = _connections[id];

            while (true)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int byte_count = stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, byte_count);
                EscutarMensagens(data ,id);
                Console.WriteLine(data);
            }

            lock (_lock) _connections.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private static void EscutarMensagens(string data, int id)
        {
            var comunicacao = JsonSerializer.Deserialize<ComunicacaoModel>(data);

            switch (comunicacao.Acao)
            {
                case "registrar-usuario":
                    var registrarUsuario = JsonSerializer.Deserialize<RegistrarUsuarioModel>(data);
                    RegistrarUsuario(id, registrarUsuario);
                    break;
                case "mensagem":
                    var mensagem = JsonSerializer.Deserialize<EnviarMensagem>(data);
                    var usuario = _usuarios.FirstOrDefault(x => x.Key == id);
                    if (mensagem.UsuarioMencionado == null)
                    {
                        EnviarMensagem($"{usuario.Value} enviou: {mensagem.Mensagem}");
                    } else
                    {
                        EnviarMensagem($"{usuario.Value} para {mensagem.UsuarioMencionado} enviou : {mensagem.Mensagem}");
                    }
                    
                    break;
                default:
                    break;
            }

        }

        private static void RegistrarUsuario(int id, RegistrarUsuarioModel model)
        {
            if (!_usuarios.ContainsKey(id))
            {
                _usuarios.Add(id, model.Nome);
                AtualizarContatos();
                EnviarMensagem($"{model.Nome} entrou no chat.");
            }
        }

        private static void EnviarMensagem(string msg)
        {
            var listaContatos = new EnviarMensagem("mensagem", msg);
            var json = JsonSerializer.Serialize(listaContatos);

            byte[] buffer = Encoding.ASCII.GetBytes(json);

            lock (_lock)
            {
                foreach (TcpClient c in _connections.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private static void AtualizarContatos()
        {
            var usuarios = _usuarios.Select(x => new ContatoModel(x.Key, x.Value));

            var listaContatos = new ListaContatoModel("atualizar-contatos", usuarios.ToList());
            var json = JsonSerializer.Serialize(listaContatos);

            byte[] buffer = Encoding.ASCII.GetBytes(json);

            lock (_lock)
            {
                foreach (TcpClient c in _connections.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

    }
}
