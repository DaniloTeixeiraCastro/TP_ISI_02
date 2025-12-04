using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Data.Repositories
{
    public class ImovelRepository : IImovelRepository
    {
        private readonly DatabaseContext _context;

        public ImovelRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Imovel>> GetAllAsync()
        {
            var imoveis = new List<Imovel>();
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Titulo, Descricao, Preco, Localizacao, DataCriacao, DataAtualizacao FROM Imoveis";

                using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        imoveis.Add(MapReaderToImovel(reader));
                    }
                }
            }
            return imoveis;
        }

        public async Task<Imovel> GetByIdAsync(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Titulo, Descricao, Preco, Localizacao, DataCriacao, DataAtualizacao FROM Imoveis WHERE Id = @Id";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToImovel(reader);
                    }
                }
            }
            return null;
        }

        public async Task<Imovel> AddAsync(Imovel imovel)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Imoveis (Titulo, Descricao, Preco, Localizacao, DataCriacao) 
                    OUTPUT INSERTED.Id
                    VALUES (@Titulo, @Descricao, @Preco, @Localizacao, @DataCriacao)";

                ((SqlCommand)command).Parameters.AddWithValue("@Titulo", imovel.Titulo);
                ((SqlCommand)command).Parameters.AddWithValue("@Descricao", imovel.Descricao);
                ((SqlCommand)command).Parameters.AddWithValue("@Preco", imovel.Preco);
                ((SqlCommand)command).Parameters.AddWithValue("@Localizacao", imovel.Localizacao);
                ((SqlCommand)command).Parameters.AddWithValue("@DataCriacao", imovel.DataCriacao);

                var newId = (int)await ((SqlCommand)command).ExecuteScalarAsync();
                imovel.Id = newId;
                return imovel;
            }
        }

        public async Task<bool> UpdateAsync(Imovel imovel)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Imoveis 
                    SET Titulo = @Titulo, 
                        Descricao = @Descricao, 
                        Preco = @Preco, 
                        Localizacao = @Localizacao, 
                        DataAtualizacao = @DataAtualizacao
                    WHERE Id = @Id";

                ((SqlCommand)command).Parameters.AddWithValue("@Id", imovel.Id);
                ((SqlCommand)command).Parameters.AddWithValue("@Titulo", imovel.Titulo);
                ((SqlCommand)command).Parameters.AddWithValue("@Descricao", imovel.Descricao);
                ((SqlCommand)command).Parameters.AddWithValue("@Preco", imovel.Preco);
                ((SqlCommand)command).Parameters.AddWithValue("@Localizacao", imovel.Localizacao);
                ((SqlCommand)command).Parameters.AddWithValue("@DataAtualizacao", DateTime.UtcNow);

                var rowsAffected = await ((SqlCommand)command).ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Imoveis WHERE Id = @Id";
                
                ((SqlCommand)command).Parameters.AddWithValue("@Id", id);

                var rowsAffected = await ((SqlCommand)command).ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        private Imovel MapReaderToImovel(IDataReader reader)
        {
            return new Imovel
            {
                Id = (int)reader["Id"],
                Titulo = (string)reader["Titulo"],
                Descricao = (string)reader["Descricao"],
                Preco = (decimal)reader["Preco"],
                Localizacao = (string)reader["Localizacao"],
                DataCriacao = (DateTime)reader["DataCriacao"],
                DataAtualizacao = reader["DataAtualizacao"] as DateTime?
            };
        }
    }
}
