using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DatabaseContext _context;

        public ClienteRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            var clientes = new List<Cliente>();
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Nome, Email, Telefone, DataCriacao, DataAtualizacao FROM Clientes";

                using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        clientes.Add(MapReaderToCliente(reader));
                    }
                }
            }
            return clientes;
        }

        public async Task<Cliente> GetByIdAsync(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Nome, Email, Telefone, DataCriacao, DataAtualizacao FROM Clientes WHERE Id = @Id";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToCliente(reader);
                    }
                }
            }
            return null;
        }

        public async Task<Cliente> AddAsync(Cliente cliente)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Clientes (Nome, Email, Telefone, DataCriacao) 
                    OUTPUT INSERTED.Id
                    VALUES (@Nome, @Email, @Telefone, @DataCriacao)";

                ((SqlCommand)command).Parameters.AddWithValue("@Nome", cliente.Nome);
                ((SqlCommand)command).Parameters.AddWithValue("@Email", cliente.Email);
                ((SqlCommand)command).Parameters.AddWithValue("@Telefone", cliente.Telefone);
                ((SqlCommand)command).Parameters.AddWithValue("@DataCriacao", cliente.DataCriacao);

                var newId = (int)await ((SqlCommand)command).ExecuteScalarAsync();
                cliente.Id = newId;
                return cliente;
            }
        }

        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Clientes 
                    SET Nome = @Nome, 
                        Email = @Email, 
                        Telefone = @Telefone, 
                        DataAtualizacao = @DataAtualizacao
                    WHERE Id = @Id";

                ((SqlCommand)command).Parameters.AddWithValue("@Id", cliente.Id);
                ((SqlCommand)command).Parameters.AddWithValue("@Nome", cliente.Nome);
                ((SqlCommand)command).Parameters.AddWithValue("@Email", cliente.Email);
                ((SqlCommand)command).Parameters.AddWithValue("@Telefone", cliente.Telefone);
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
                command.CommandText = "DELETE FROM Clientes WHERE Id = @Id";
                
                ((SqlCommand)command).Parameters.AddWithValue("@Id", id);

                var rowsAffected = await ((SqlCommand)command).ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        private Cliente MapReaderToCliente(IDataReader reader)
        {
            return new Cliente
            {
                Id = (int)reader["Id"],
                Nome = (string)reader["Nome"],
                Email = (string)reader["Email"],
                Telefone = (string)reader["Telefone"],
                DataCriacao = (DateTime)reader["DataCriacao"],
                DataAtualizacao = reader["DataAtualizacao"] as DateTime?
            };
        }
    }
}
