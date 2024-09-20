using System;
using System.Data.SqlClient;

namespace WindowsFormsApp3
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=ADCLG1;Database=Горб_практика;Trusted_Connection=True;";

        public bool UserExists(string username, string password, out int readerID)
        {
            readerID = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ReaderID FROM Reader WHERE Name = @Username AND CAST(Password AS NVARCHAR) = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        readerID = Convert.ToInt32(result);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool RegisterUser(string username, string password, string contactInfo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Reader (Name, Password, ContactInfo) VALUES (@Username, @Password, @ContactInfo)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@ContactInfo", contactInfo);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool TakeBook(int bookID, int readerID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string updateBookQuery = "UPDATE Book SET Status = 'на руках' WHERE BookID = @BookID";
                using (SqlCommand updateCommand = new SqlCommand(updateBookQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@BookID", bookID);
                    updateCommand.ExecuteNonQuery();
                }

                string insertLoanQuery = @"
                    INSERT INTO Loan (BookID, ReaderID, LoanDate, ReturnDate, Status)
                    VALUES (@BookID, @ReaderID, @LoanDate, @ReturnDate, @Status)";
                using (SqlCommand insertCommand = new SqlCommand(insertLoanQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@BookID", bookID);
                    insertCommand.Parameters.AddWithValue("@ReaderID", readerID);
                    insertCommand.Parameters.AddWithValue("@LoanDate", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@ReturnDate", DateTime.Now.AddDays(7));
                    insertCommand.Parameters.AddWithValue("@Status", "активна");
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}