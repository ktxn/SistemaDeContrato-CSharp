using System.Data.SqlClient;
using System.Threading;

namespace Concilig.Entidades.CodFonte
{
    public class Iniciar
    {
        // String de conexão com o SQL Server
        static string connectionString = "Server=DESKTOP-HI5ASSH;Database=master;Trusted_Connection=True;";

        // Propriedades
        public string nivelPermissao { get; set; }
        public static string login { get; set; }
        private string senha { get; set; }
        // Método principal para Login
        public void Login()
        {
            Console.Clear();
            Console.WriteLine("===== LOGIN =====\n");

            Console.Write("Digite seu login: ");
            login = Console.ReadLine();

            Console.Write("Digite sua senha: ");
            senha = Console.ReadLine();

            nivelPermissao = ObterNivelPermissao(login, senha);

            if (nivelPermissao == null)
            {
                Console.WriteLine("\nLogin ou senha incorretos.");
                return;
            }

            Console.WriteLine($"\nLogin bem-sucedido como {login}.");
            Thread.Sleep(2000);

            // Chama a interface correspondente
            Program interfaces = new Program();

            if (string.IsNullOrEmpty(nivelPermissao))
            {
                Console.WriteLine("Falha no login. Encerrando.");
                return;
            }

            // Aqui você pode decidir a interface dependendo do usuário
            if (nivelPermissao == "ADM")
                interfaces.InterfaceAdm(nivelPermissao);
            else
                interfaces.InterfaceUsuario();

        }

        // Verifica no banco se login e senha existem e retorna o nível de permissão
        private string ObterNivelPermissao(string login, string senha)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT Nivel_Permissao 
                        FROM UsuariosSistema 
                        WHERE Nome_Usuario = @login AND Senha = @senha";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@senha", senha);

                        var resultado = command.ExecuteScalar();

                        if (resultado != null)
                        {
                            return resultado.ToString();
                        }
                        else
                        {
                            return null; // Login inválido
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nOcorreu um erro na conexão: " + ex.Message);
                return null;
            }
        }
    }
}
