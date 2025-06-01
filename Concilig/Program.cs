using Concilig.Entidades.CodFonte;
using Concilig.Entidades.Programa;

class Program
{
    public string nivelPermissao;
    private bool continuar = true;
    private Opcoes opcoes = new Opcoes();
    private DirCarga arquivo = new DirCarga();
    

    static void Main(string[] args)
    {
        Program programa = new Program();

        programa.Executar();
    }

    void Executar()
    {
        Iniciar login = new Iniciar();
        login.Login();
    }

    // Interface para administradores
    public void InterfaceAdm(string nomeUsuario)
    {
        // Inicializa DirCarga após obter o nome do usuário
        arquivo = new DirCarga();

        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("===== SISTEMA DE IMPORTAÇÃO DE CARGAS =====\n");
            Console.WriteLine($"Usuário: {nomeUsuario}\n");
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Importar Arquivo");
            Console.WriteLine("2 - Consultar os arquivos importados e seus usuários");
            Console.WriteLine("3 - Consultar os contratos importados");
            Console.WriteLine("4 - Consultar o valor total dos contratos e maior atraso do cliente");
            Console.WriteLine("0 - Sair");

            Console.Write("\nOpção: ");
            string opcao = Console.ReadLine();

            Console.WriteLine();

            switch (opcao)
            {
                case "1":
                    arquivo.LocArquivo(nomeUsuario);
                    break;
                case "2":
                    opcoes.ConsultarArquivosImportados();
                    break;
                case "3":
                    opcoes.ConsultarContratos();
                    break;
                case "4":
                    opcoes.ConsultarValorTotalEMaiorAtraso();
                    break;
                case "0":
                    continuar = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            if (continuar)
            {
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

        Console.WriteLine("\nEncerrando o sistema...");
    }

    // Interface para usuários comuns
    public void InterfaceUsuario()
    {
        int opcao = -1;

        do
        {
            Console.Clear();
            Console.WriteLine("=== INTERFACE USUÁRIO ===");
            Console.WriteLine("1 - Consultar Contratos");
            Console.WriteLine("0 - Sair");
            Console.Write("Selecione uma opção: ");

            string input = Console.ReadLine();

            if (!int.TryParse(input, out opcao))
            {
                Console.WriteLine("\nOpção inválida. Pressione ENTER para continuar...");
                Console.ReadLine();
                continue;
            }

            switch (opcao)
            {
                case 1:
                    Console.WriteLine("\n>>> Consultar Contratos selecionado.");
                    opcoes.ConsultarValorTotalEMaiorAtraso();
                    Console.WriteLine("\nPressione ENTER para voltar ao menu.");
                    Console.ReadLine();
                    break;

                case 0:
                    Console.WriteLine("\nSaindo da interface de usuário...");
                    break;

                default:
                    Console.WriteLine("\nOpção inválida. Pressione ENTER para continuar...");
                    Console.ReadLine();
                    break;
            }

        } while (opcao != 0);
    }
}
