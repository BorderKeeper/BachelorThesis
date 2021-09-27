using System;
using MySql.Data.MySqlClient;

namespace BachelorThesis.Network
{
    public class SqlDataProvider : IDisposable
    {
        private readonly MySqlConnection _dal;

        public SqlDataProvider()
        {
            var connectionString = "server=localhost;user=bachelor_acc;database=Bachelor;port=3306;password=gom";

            _dal = new MySqlConnection(connectionString);

            try
            {
                _dal.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Mysql connection could not be established: {e.Message}");
            }
        }

        public MySqlDataReader Read(string command)
        {
            MySqlCommand mysqlCommand = new MySqlCommand(command, _dal);
            MySqlDataReader reader = null;

            try
            {
                reader = mysqlCommand.ExecuteReader();

                return reader;
            }
            catch (Exception e)
            {
                reader?.Close();
                _dal.Close();
                throw new Exception("Error in DB", e);
            }
        }

        public void Update(string command)
        {
            MySqlCommand mysqlCommand = new MySqlCommand(command, _dal);
            MySqlDataReader reader = null;

            try
            {
                reader = mysqlCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                reader?.Close();
                _dal.Close();
                throw new Exception("Error in DB", e);
            }

            reader.Close();
        }

        public void Dispose()
        {
            _dal?.Dispose();
        }
    }
}