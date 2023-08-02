using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FormularioWPF
{
    public class DAL
    {
        private NpgsqlConnection connection;
        private string connectionString;

        public DAL()
        {
            // Coloque aqui a sua string de conexão com o banco de dados / PostgreSQL
            connectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=123;";
        }

        public DataTable RetornaTabela(string query, params NpgsqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public void ExcluirEndereco(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "DELETE FROM tbl_endereco WHERE Id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        public void ComandoSql(string query, Dictionary<string, object> parametros = null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                if (parametros != null)
                {
                    foreach (var parametro in parametros)
                    {
                        command.Parameters.AddWithValue(parametro.Key, parametro.Value);
                    }
                }

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        public void ComandoSql(string comandoSQL)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(comandoSQL, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void OpenConnection()
        {
            try
            {
                connection = new NpgsqlConnection(connectionString);
                connection.Open();
                MessageBox.Show("Conexão estabelecida com sucesso!", "Conexão", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Trate exceções ou registre os erros conforme necessário
                MessageBox.Show(ex.Message, "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<string> GetSomeData()
        {
            List<string> data = new List<string>();

            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM pessoa";

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string value = reader.GetString(0); // Substitua 0 pelo índice da coluna desejada
                            data.Add(value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return data;
        }
    }
}
