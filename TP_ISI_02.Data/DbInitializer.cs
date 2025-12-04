using System.Data;
using Microsoft.Data.SqlClient;

namespace TP_ISI_02.Data
{
    public class DbInitializer
    {
        private readonly DatabaseContext _context;

        public DbInitializer(DatabaseContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                    CREATE TABLE Users (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Username NVARCHAR(100) NOT NULL UNIQUE,
                        PasswordHash NVARCHAR(255) NOT NULL,
                        Email NVARCHAR(100) NOT NULL,
                        DataCriacao DATETIME NOT NULL
                    );

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Imoveis')
                    CREATE TABLE Imoveis (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Titulo NVARCHAR(200) NOT NULL,
                        Descricao NVARCHAR(MAX) NOT NULL,
                        Preco DECIMAL(18,2) NOT NULL,
                        Localizacao NVARCHAR(200) NOT NULL,
                        DataCriacao DATETIME NOT NULL,
                        DataAtualizacao DATETIME NULL
                    );

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
                    CREATE TABLE Clientes (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Nome NVARCHAR(100) NOT NULL,
                        Email NVARCHAR(100) NOT NULL,
                        Telefone NVARCHAR(50) NOT NULL,
                        DataCriacao DATETIME NOT NULL,
                        DataAtualizacao DATETIME NULL
                    );

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Eventos')
                    CREATE TABLE Eventos (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Data DATETIME NOT NULL,
                        Descricao NVARCHAR(MAX) NOT NULL,
                        ImovelId INT NOT NULL,
                        DataCriacao DATETIME NOT NULL,
                        DataAtualizacao DATETIME NULL,
                        FOREIGN KEY (ImovelId) REFERENCES Imoveis(Id) ON DELETE CASCADE
                    );
                ";

                ((SqlCommand)command).ExecuteNonQuery();
            }
        }
    }
}
