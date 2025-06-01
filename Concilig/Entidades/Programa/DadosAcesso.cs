using System.Data.SqlClient;

namespace Concilig.Entidades.Programa
{
    public class DadosAcesso
    {
        public string UsuarioLogado { get; private set; }
        public string Permissao { get; private set; }
        private string connectionString = "Server=DESKTOP-HI5ASSH;Database=master;Trusted_Connection=True;";

        public bool Login()
        {
            Console.Write("Login: ");
            string login = Console.ReadLine();

            Console.Write("Senha: ");
            string senha = Console.ReadLine();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Nivel_Permissao FROM UsuariosSistema WHERE Nome_Usuario = @login AND Senha = @senha";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@senha", senha);

                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            UsuarioLogado = login;
                            Permissao = result.ToString();
                            Console.WriteLine($"\nLogin realizado com sucesso! Permissão: {Permissao}\n");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Login ou senha incorretos.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na conexão: " + ex.Message);
                return false;
            }
        }

        public bool EhAdministrador()
        {
            return Permissao == "ADM";
        }

        public bool EhVisualizador()
        {
            return Permissao == "Visualizador";
        }
    }
}