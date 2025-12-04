using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Username, PasswordHash, Email, DataCriacao FROM Users WHERE Username = @Username";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Username";
                parameter.Value = username;
                command.Parameters.Add(parameter);

                using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = (int)reader["Id"],
                            Username = (string)reader["Username"],
                            PasswordHash = (string)reader["PasswordHash"],
                            Email = (string)reader["Email"],
                            DataCriacao = (DateTime)reader["DataCriacao"]
                        };
                    }
                }
            }
            return null;
        }

        public async Task<User> AddAsync(User user)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Users (Username, PasswordHash, Email, DataCriacao) 
                    OUTPUT INSERTED.Id
                    VALUES (@Username, @PasswordHash, @Email, @DataCriacao)";

                ((SqlCommand)command).Parameters.AddWithValue("@Username", user.Username);
                ((SqlCommand)command).Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                ((SqlCommand)command).Parameters.AddWithValue("@Email", user.Email);
                ((SqlCommand)command).Parameters.AddWithValue("@DataCriacao", user.DataCriacao);

                var newId = (int)await ((SqlCommand)command).ExecuteScalarAsync();
                user.Id = newId;
                return user;
            }
        }
    }
}
