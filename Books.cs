using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Books : Form
    {
        private string connectionString = "Server=ADCLG1;Database=Горб_практика;Trusted_Connection=True;";

        public Books()
        {
            InitializeComponent();
            LoadData();
            LoadAuthors();
        }

        private void LoadData()
        {
            LoadBookData();
        }

        private void LoadBookData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        B.BookID, 
                        B.Title, 
                        A.Name AS AuthorName, 
                        B.Year,
                        B.Status
                    FROM 
                        Book B
                    JOIN 
                        Author A ON B.AuthorID = A.AuthorID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void LoadAuthors()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT AuthorID, Name FROM Author";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminTabForm adminTabForm = new AdminTabForm();
            adminTabForm.Show();
            this.Hide();
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            string title = txtBookTitle.Text;
            string authorName = txtAuthorName.Text;
            string year = txtYear.Text;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(authorName) || string.IsNullOrEmpty(year))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            int authorID = GetAuthorID(authorName);
            if (authorID == -1)
            {
                DialogResult result = MessageBox.Show("Автор не найден. Хотите добавить нового автора?", "Автор не найден", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (AddAuthor(authorName))
                    {
                        authorID = GetAuthorID(authorName);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении автора.");
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            if (AddBook(title, authorID, year, "в наличии"))
            {
                MessageBox.Show("Книга успешно добавлена.");
                LoadBookData();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении книги.");
            }
        }

        private int GetAuthorID(string authorName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT AuthorID FROM Author WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", authorName);
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            return -1;
        }

        private bool AddAuthor(string authorName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Author (Name) VALUES (@Name)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", authorName);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        private bool AddBook(string title, int authorID, string year, string status)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Book (Title, AuthorID, Year, Status) VALUES (@Title, @AuthorID, @Year, @Status)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@AuthorID", authorID);
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Status", status);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}