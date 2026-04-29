using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDIL.DAL
{
    public class DatabaseLayer
    {
        private string connectionString;
        public DatabaseLayer()
        {
            // Default constructor - use Web.config connection string
            connectionString = ConfigurationManager.ConnectionStrings["TestSuitConnenction"]?.ConnectionString;
        }

        public DatabaseLayer(string connectionString)
        {
            // Constructor with custom connection string
            this.connectionString = connectionString;
        }
       
        // Authentication Methods
        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password AND is_active = 1";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password); // In production, use hashed passwords

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating user: " + ex.Message);
            }
        }

        public DataSet GetUserDetails(string username)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT user_id, username, full_name, email, role, last_login FROM users WHERE username = @username AND is_active = 1";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user details: " + ex.Message);
            }
        }

        public void UpdateLastLogin(string username)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE users SET last_login = @last_login WHERE username = @username";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@last_login", DateTime.Now);
                        command.Parameters.AddWithValue("@username", username);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating last login: " + ex.Message);
            }
        }

        public bool CreateUser(string username, string password, string fullName, string email, string role = "User")
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO users (username, password, full_name, email, role, is_active, created_date) 
                                   VALUES (@username, @password, @full_name, @email, @role, 1, @created_date)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password); // In production, use hashed passwords
                        command.Parameters.AddWithValue("@full_name", fullName);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@created_date", DateTime.Now);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating user: " + ex.Message);
            }
        }
    }
}
