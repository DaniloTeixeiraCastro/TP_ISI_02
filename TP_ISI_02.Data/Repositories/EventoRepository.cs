using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.Data.Repositories
{
    public class EventoRepository : IEventoRepository
    {
        private readonly DatabaseContext _context;

        public EventoRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Evento>> GetAllAsync()
        {
            var eventos = new List<Evento>();
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT e.Id, e.Data, e.Descricao, e.ImovelId, e.DataCriacao, e.DataAtualizacao,
                           i.Titulo as ImovelTitulo
                    FROM Eventos e
                    LEFT JOIN Imoveis i ON e.ImovelId = i.Id";

                using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        eventos.Add(MapReaderToEvento(reader));
                    }
                }
            }
            return eventos;
        }

        public async Task<Evento> GetByIdAsync(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT e.Id, e.Data, e.Descricao, e.ImovelId, e.DataCriacao, e.DataAtualizacao,
                           i.Titulo as ImovelTitulo
                    FROM Eventos e
                    LEFT JOIN Imoveis i ON e.ImovelId = i.Id
                    WHERE e.Id = @Id";
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToEvento(reader);
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<Evento>> GetByImovelIdAsync(int imovelId)
        {
            var eventos = new List<Evento>();
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT e.Id, e.Data, e.Descricao, e.ImovelId, e.DataCriacao, e.DataAtualizacao,
                           i.Titulo as ImovelTitulo
                    FROM Eventos e
                    LEFT JOIN Imoveis i ON e.ImovelId = i.Id
                    WHERE e.ImovelId = @ImovelId";

                ((SqlCommand)command).Parameters.AddWithValue("@ImovelId", imovelId);

                using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        eventos.Add(MapReaderToEvento(reader));
                    }
                }
            }
            return eventos;
        }

        public async Task<Evento> AddAsync(Evento evento)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Eventos (Data, Descricao, ImovelId, DataCriacao) 
                    OUTPUT INSERTED.Id
                    VALUES (@Data, @Descricao, @ImovelId, @DataCriacao)";

                ((SqlCommand)command).Parameters.AddWithValue("@Data", evento.Data);
                ((SqlCommand)command).Parameters.AddWithValue("@Descricao", evento.Descricao);
                ((SqlCommand)command).Parameters.AddWithValue("@ImovelId", evento.ImovelId);
                ((SqlCommand)command).Parameters.AddWithValue("@DataCriacao", evento.DataCriacao);

                var newId = (int)await ((SqlCommand)command).ExecuteScalarAsync();
                evento.Id = newId;
                return evento;
            }
        }

        public async Task<bool> UpdateAsync(Evento evento)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Eventos 
                    SET Data = @Data, 
                        Descricao = @Descricao, 
                        ImovelId = @ImovelId, 
                        DataAtualizacao = @DataAtualizacao
                    WHERE Id = @Id";

                ((SqlCommand)command).Parameters.AddWithValue("@Id", evento.Id);
                ((SqlCommand)command).Parameters.AddWithValue("@Data", evento.Data);
                ((SqlCommand)command).Parameters.AddWithValue("@Descricao", evento.Descricao);
                ((SqlCommand)command).Parameters.AddWithValue("@ImovelId", evento.ImovelId);
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
                command.CommandText = "DELETE FROM Eventos WHERE Id = @Id";
                
                ((SqlCommand)command).Parameters.AddWithValue("@Id", id);

                var rowsAffected = await ((SqlCommand)command).ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        private Evento MapReaderToEvento(IDataReader reader)
        {
            var evento = new Evento
            {
                Id = (int)reader["Id"],
                Data = (DateTime)reader["Data"],
                Descricao = (string)reader["Descricao"],
                ImovelId = (int)reader["ImovelId"],
                DataCriacao = (DateTime)reader["DataCriacao"],
                DataAtualizacao = reader["DataAtualizacao"] as DateTime?
            };

            // Map Imovel title if available (from JOIN)
            if (reader["ImovelTitulo"] != DBNull.Value)
            {
                evento.Imovel = new Imovel
                {
                    Id = evento.ImovelId,
                    Titulo = (string)reader["ImovelTitulo"]
                };
            }

            return evento;
        }
    }
}
