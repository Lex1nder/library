using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class BReturn : Form
    {
        private string connectionString = "Server=ADCLG1;Database=Горб_практика;Trusted_Connection=True;";
        private int readerID;

        public BReturn(int readerID)
        {
            InitializeComponent();
            this.readerID = readerID;
            LoadBooks();
        }

        private void LoadBooks()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        B.BookID, 
                        B.Title, 
                        A.Name AS AuthorName, 
                        L.ReturnDate
                    FROM 
                        Loan L
                    JOIN 
                        Book B ON L.BookID = B.BookID
                    JOIN 
                        Author A ON B.AuthorID = A.AuthorID
                    WHERE 
                        L.ReaderID = @ReaderID AND L.Status = 'активна'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReaderID", readerID);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    int y = 0;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        CreateBookPanel(row, y);
                        y += 60;
                    }
                }
            }
        }

        private void CreateBookPanel(DataRow book, int y)
        {
            Panel panel = new Panel
            {
                Size = new Size(850, 50),
                Location = new Point((pictureBox1.Width - 850) / 2, 131 + y),
                BackColor = Color.White
            };

            Label titleLabel = new Label
            {
                Text = book["Title"].ToString(),
                Location = new Point(10, 15),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            Label authorLabel = new Label
            {
                Text = book["AuthorName"].ToString(),
                Location = new Point(220, 15),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 10)
            };

            Label returnDateLabel = new Label
            {
                Text = "До " + ((DateTime)book["ReturnDate"]).ToShortDateString(),
                Location = new Point(430, 15),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 10)
            };

            Button returnButton = new Button
            {
                Text = "Вернуть книгу",
                Location = new Point(650, 5),
                Size = new Size(180, 40),
                BackColor = Color.MediumAquamarine
            };

            returnButton.Click += (sender, e) => ReturnBook(book["BookID"].ToString());

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(authorLabel);
            panel.Controls.Add(returnDateLabel);
            panel.Controls.Add(returnButton);

            pictureBox1.Controls.Add(panel);
        }

        private void ReturnBook(string bookID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string updateBookQuery = "UPDATE Book SET Status = 'в наличии' WHERE BookID = @BookID";
                using (SqlCommand updateCommand = new SqlCommand(updateBookQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@BookID", bookID);
                    updateCommand.ExecuteNonQuery();
                }

                string updateLoanQuery = "UPDATE Loan SET Status = 'завершена' WHERE BookID = @BookID AND ReaderID = @ReaderID AND Status = 'активна'";
                using (SqlCommand updateLoanCommand = new SqlCommand(updateLoanQuery, connection))
                {
                    updateLoanCommand.Parameters.AddWithValue("@BookID", bookID);
                    updateLoanCommand.Parameters.AddWithValue("@ReaderID", readerID);
                    updateLoanCommand.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Книга возвращена.");
            pictureBox1.Controls.Clear();
            LoadBooks();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewTabForm newTabForm = new NewTabForm(readerID);
            newTabForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}