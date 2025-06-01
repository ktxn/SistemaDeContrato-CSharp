using System.Data.SqlClient;

namespace Concilig.Entidades.Programa
{
    internal class Opcoes
    {
        static string connectionString = "Server=DESKTOP-HI5ASSH;Database=master;Trusted_Connection=True;";

        // Consulta dos arquivos importados e seus usuários
        public void ConsultarArquivosImportados()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT Id, Nome_Usuario, Nome_ArqImportado, Data_HoraImportacao, Status_Importacao FROM LogImportacao;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("===== Tabela Log =====");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Id: {reader["Id"]} | Nome_Usuario: {reader["Nome_Usuario"]} | Nome Arquivo: {reader["Nome_ArqImportado"]} | Data/Hora: {reader["Data_HoraImportacao"]} | Status: {reader["Status_Importacao"]}");
                        }

                        if (reader.NextResult())
                        {
                            Console.WriteLine("\n===== Tabela UsuariosSistema =====");
                            while (reader.Read())
                            {
                                Console.WriteLine($"Id: {reader["Id"]} | Nome Usuário: {reader["Nome_Usuario"]}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na consulta dos arquivos importados: " + ex.Message);
            }
        }

        // Consulta dos contratos importados
        public void ConsultarContratos()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT Contrato.NumeroContrato, Contrato.Produto, Contrato.Vencimento, 
                               Contrato.Valor, Cliente.Nome, Cliente.CPF 
                        FROM Contrato
                        INNER JOIN Cliente ON Contrato.ClienteId = Cliente.Id";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        Console.WriteLine("===== Contratos Importados =====");

                        while (reader.Read())
                        {
                            DateTime vencimento = reader["Vencimento"] != DBNull.Value
                                ? Convert.ToDateTime(reader["Vencimento"])
                                : DateTime.MinValue;

                            Console.WriteLine($"\nNúmero do Contrato: {reader["NumeroContrato"]}");
                            Console.WriteLine($"Produto: {reader["Produto"]}");
                            Console.WriteLine($"Vencimento: {(vencimento != DateTime.MinValue ? vencimento.ToString("dd/MM/yyyy") : "Sem Data")}");
                            Console.WriteLine($"Valor: R$ {reader["Valor"]}");
                            Console.WriteLine($"Cliente: {reader["Nome"]} | CPF: {reader["CPF"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na consulta dos contratos: " + ex.Message);
            }
        }

        // Consulta do valor total dos contratos + maior atraso de um cliente
        public void ConsultarValorTotalEMaiorAtraso()
        {
            Console.Write("Informe o CPF do cliente: ");
            string cpf = Console.ReadLine();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT Cliente.Nome, Cliente.CPF,
                               SUM(Contrato.Valor) AS ValorTotal,
                               MAX(DATEDIFF(DAY, Contrato.Vencimento, GETDATE())) AS MaiorAtraso
                        FROM Cliente
                        INNER JOIN Contrato ON Cliente.Id = Contrato.ClienteId
                        WHERE Cliente.CPF = @cpf
                        GROUP BY Cliente.Nome, Cliente.CPF";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cpf", cpf);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine($"\nCliente: {reader["Nome"]} | CPF: {reader["CPF"]}");
                                Console.WriteLine($"Valor Total dos Contratos: R$ {reader["ValorTotal"]}");
                                Console.WriteLine($"Maior Atraso em Dias: {reader["MaiorAtraso"]}");
                            }
                            else
                            {
                                Console.WriteLine("Nenhum contrato encontrado para este CPF.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na consulta: " + ex.Message);
            }
        }
    }
}
