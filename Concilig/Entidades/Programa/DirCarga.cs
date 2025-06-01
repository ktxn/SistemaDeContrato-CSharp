using Concilig.Entidades.SQL_Server;
using CsvHelper;
using CsvHelper.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace Concilig.Entidades.CodFonte
{
    internal class DirCarga : Iniciar
    {
        private string nomeUsuario { get; set; }
        private string caminho { get; set; }
        private string nomeArquivo { get; set; }
        private string diretorio { get; set; } = @"C:\Users\ktn\Desktop\Projeto\Repositorio";
        private int qtdRegistros { get; set; }
        private string status { get; set; }

        private readonly string connectionString = "Server=DESKTOP-HI5ASSH;Database=master;Trusted_Connection=True;";

        // Localiza e carrega o arquivo CSV
        public void LocArquivo(string nomeUsuario)
        {
            Console.Write("Informe a nomenclatura e extensão da carga a ser importada (Ex: cargas.csv): ");
            nomeArquivo = Console.ReadLine();
            caminho = Path.Combine(diretorio, nomeArquivo);

            Console.WriteLine($"\nBuscando arquivo em: {caminho}");

            if (!File.Exists(caminho))
            {
                Console.WriteLine("Arquivo não encontrado.");
                return;
            }
            
            Console.WriteLine("Arquivo encontrado com sucesso!");

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                status = "Concluido";

                string insertLog = @"
                INSERT INTO LogImportacao (Nome_Usuario, Nome_ArqImportado, Data_HoraImportacao, Status_Importacao)
                VALUES (@NomeUsuario, @NomeArquivo, GETDATE(), @Status)";

                using var cmdLog = new SqlCommand(insertLog, connection);
                cmdLog.Parameters.Add("@NomeUsuario", System.Data.SqlDbType.NVarChar, 100).Value = nomeUsuario ?? (object)DBNull.Value;
                cmdLog.Parameters.Add("@NomeArquivo", System.Data.SqlDbType.NVarChar, 100).Value = nomeArquivo ?? (object)DBNull.Value;
                cmdLog.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, 50).Value = status ?? (object)DBNull.Value;

                Console.WriteLine($"Inserindo log: Usuário= {nomeUsuario}, Arquivo= {nomeArquivo}, Status= {status}");

                int linhasAfetadas = cmdLog.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                    Console.WriteLine("\nCarga importada e log registrada com sucesso!");
                else
                    Console.WriteLine("\nFalha ao registrar log de importação.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao registrar log: " + ex.Message);
                Console.WriteLine();
            }

            Carregar();
        }

        // Carrega e importa os dados do CSV
        private void Carregar()
        {
            qtdRegistros = 0;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
            };

            using var reader = new StreamReader(caminho);
            using var csv = new CsvReader(reader, config);

            var registros = csv.GetRecords<ContratoCSV>().ToList();

            foreach (var item in registros)
            {
                InserirOuAtualizarNoBanco(item);
                qtdRegistros += 1;
            }

            AddCarga(nomeArquivo, qtdRegistros);

            Console.WriteLine();
            Console.WriteLine($"Importação do arquivo {nomeArquivo} concluída!");
            Console.WriteLine($"Quantidade de registros: {qtdRegistros}");
        }

        // Insere ou atualiza os dados do contrato
        private void InserirOuAtualizarNoBanco(ContratoCSV contratoCsv)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // Verifica se o cliente existe
            var comandoCliente = new SqlCommand(
                "SELECT Id FROM Cliente WHERE CPF = @cpf", connection);
            comandoCliente.Parameters.AddWithValue("@cpf", contratoCsv.CPF);

            var clienteId = comandoCliente.ExecuteScalar();

            if (clienteId == null)
            {
                var cmdInsertCliente = new SqlCommand(
                    "INSERT INTO Cliente (Nome, CPF) OUTPUT INSERTED.Id VALUES (@nome, @cpf)", connection);
                cmdInsertCliente.Parameters.AddWithValue("@nome", contratoCsv.Nome);
                cmdInsertCliente.Parameters.AddWithValue("@cpf", contratoCsv.CPF);

                clienteId = (int)cmdInsertCliente.ExecuteScalar();
            }

            // Verifica se o contrato existe
            var comandoContrato = new SqlCommand(
                "SELECT Id FROM Contrato WHERE NumeroContrato = @numero", connection);
            comandoContrato.Parameters.AddWithValue("@numero", contratoCsv.Contrato);

            var contratoExiste = comandoContrato.ExecuteScalar();

            if (contratoExiste != null)
            {
                // UPDATE
                var cmdUpdateContrato = new SqlCommand(
                    @"UPDATE Contrato 
                      SET Produto = @produto, 
                          Vencimento = @vencimento, 
                          Valor = @valor, 
                          ClienteId = @clienteId 
                      WHERE NumeroContrato = @numero", connection);

                cmdUpdateContrato.Parameters.AddWithValue("@produto", contratoCsv.Produto);
                cmdUpdateContrato.Parameters.AddWithValue("@vencimento", DateTime.ParseExact(contratoCsv.Vencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                cmdUpdateContrato.Parameters.AddWithValue("@valor", decimal.Parse(contratoCsv.Valor.Replace(".", "").Replace(",", "."), CultureInfo.InvariantCulture));
                cmdUpdateContrato.Parameters.AddWithValue("@clienteId", (int)clienteId);
                cmdUpdateContrato.Parameters.AddWithValue("@numero", contratoCsv.Contrato);

                cmdUpdateContrato.ExecuteNonQuery();

                Console.WriteLine($"Contrato {contratoCsv.Contrato} atualizado com sucesso.");
            }
            else
            {
                // INSERT
                var cmdInsertContrato = new SqlCommand(
                    @"INSERT INTO Contrato (NumeroContrato, Produto, Vencimento, Valor, ClienteId)
                      VALUES (@numero, @produto, @vencimento, @valor, @clienteId)", connection);

                cmdInsertContrato.Parameters.AddWithValue("@numero", contratoCsv.Contrato);
                cmdInsertContrato.Parameters.AddWithValue("@produto", contratoCsv.Produto);
                cmdInsertContrato.Parameters.AddWithValue("@vencimento", DateTime.ParseExact(contratoCsv.Vencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                cmdInsertContrato.Parameters.AddWithValue("@valor", decimal.Parse(contratoCsv.Valor.Replace(".", "").Replace(",", "."), CultureInfo.InvariantCulture));
                cmdInsertContrato.Parameters.AddWithValue("@clienteId", (int)clienteId);

                cmdInsertContrato.ExecuteNonQuery();

                Console.WriteLine($"Contrato {contratoCsv.Contrato} inserido com sucesso.");
            }
        }

        // Adiciona a carga no log do banco
        private void AddCarga(string nomeArquivo, int qtdRegistros)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var cmdInsertCargas = new SqlCommand(
                @"INSERT INTO Cargas (Nome_ArqImportacao, Qtd_registros) 
                  OUTPUT INSERTED.Id 
                  VALUES (@nomeArquivo, @qtdRegistros)", connection);

            cmdInsertCargas.Parameters.AddWithValue("@nomeArquivo", nomeArquivo);
            cmdInsertCargas.Parameters.AddWithValue("@qtdRegistros", qtdRegistros);

            var idGerado = (int)cmdInsertCargas.ExecuteScalar();

            Console.WriteLine($"Registro inserido na tabela Cargas com ID: {idGerado}");
        }
    }
}
