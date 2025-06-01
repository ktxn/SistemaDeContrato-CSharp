🖥️1. Descrição do Projeto
O projeto desenvolvido tem como objetivo criar um sistema de importação de arquivos CSV para um banco de dados SQL Server. O sistema possui funcionalidades tanto para administradores quanto para usuários, oferecendo operações como:

  .Importação de arquivos CSV contendo dados de contratos.

  .Consulta dos arquivos importados por usuário.

  .Consulta dos contratos importados.

  .Cálculo do valor total dos contratos e do maior atraso de pagamento.

  .Login com controle de acesso (Admin e Usuário comum).

🚀 2. Como Executar o Código:
✅ Pré-requisitos

  ✔️ .NET instalado na sua máquina (versão recomendada: .NET 6 ou superior).

  ✔️ SQL Server instalado e em execução (pode ser local).

  ✔️ Banco de dados com as tabelas já criadas.

  ✔️ Uma pasta no diretório local contendo os arquivos CSV que você deseja importar.

⚙️ 2.1. Configuração do Banco de Dados:

- O código está atualmente configurado com a string de conexão: (Server=DESKTOP-HI5ASSH;Database=master;Trusted_Connection=True;)
- Se o seu servidor for diferente, você deve alterar esta string dentro da classe Iniciar.cs
- Se desejar, pode criar um banco próprio.

📂 2.2. Diretório dos Arquivos CSV:

- O sistema procura os arquivos CSV em um diretório fixo, que provavelmente está definido na classe DirCarga.cs.
  .Verifique este trecho na classe: (private string diretorio { get; set; } = @"C:\Users\ktn\Desktop\Projeto\Repositorio";)
  ✔️ Sugestão: Crie essa pasta no seu computador e insira os arquivos CSV corretamente formatados.

🏃 2.3. Executando o Projeto:

  .Abra o terminal (CMD, PowerShell ou Terminal do Visual Studio).
  .Navegue até a pasta do projeto.
  .Execute
  .O sistema abrirá um console solicitando login.
  .A partir do login, você acessará a interface administrativa ou de usuário comum, de acordo com as permissões cadastradas no banco.

💡 3. Sugestões de Melhorias:

🖥️ 3.1. Interface Gráfica ou Web
  - Desenvolver uma interface web usando ASP.NET Core + Razor Pages ou React + API em .NET, proporcionando maior facilidade de uso.
  - Alternativamente, criar uma interface desktop com Windows Forms ou WPF.

🗄️ 3.2. Melhorias no Banco de Dados
  - Tornar a string de conexão dinâmica, utilizando arquivos de configuração (appsettings.json).
  - Criar um banco de dados dedicado ao sistema, com tabelas específicas para usuários, contratos e logs (Igual às utilizadas no desenvolvimento do projeto).
  - Implementar procedures e views para otimizar consultas.

📦 3.3. Organização do Projeto
  - Separar o projeto em camadas:
  - Apresentação: Console ou interface Web.
  - Negócio: Regras e lógicas.
  - Dados: Acesso ao banco.
  - Entidades: Modelos de dados.
  - Implementar princípios de POO, como SOLID.

🔒 3.4. Segurança
  - Criptografar senhas no banco (nunca armazenar texto puro).
  - Adicionar controle de tentativas de login e bloqueio após múltiplas falhas.

☁️ 3.5. Escalabilidade
  - Migrar o banco para um serviço em nuvem (Azure SQL, AWS RDS, etc.).
  - Transformar o projeto em uma API REST para integrar com outros sistemas ou aplicativos móveis.

🔧 3.6. Funcionalidades Futuras
  - Relatórios em PDF ou Excel gerados automaticamente.
  - Envio de notificações por e-mail após importações.
  - Dashboard com estatísticas dos contratos.

✍️ 4. Conclusão
"Este projeto oferece uma base sólida para sistemas de processamento de dados via arquivos CSV integrados a um banco SQL Server. Com as melhorias listadas, ele pode se transformar em uma aplicação profissional, robusta e escalável."
