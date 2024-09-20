using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class NewTabForm : Form
    {
        private string connectionString = "Server=ADCLG1;Database=Горб_практика;Trusted_Connection=True;";
        private string imagePath = @"C:\Users\429199-3\Downloads\WindowsFormsApp3\sefsafawda.png";
        private int readerID;
        private DatabaseHelper dbHelper;

        public NewTabForm(int readerID)
        {
            InitializeComponent();
            this.readerID = readerID;
            dbHelper = new DatabaseHelper();
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
                        B.Status
                    FROM 
                        Book B
                    JOIN 
                        Author A ON B.AuthorID = A.AuthorID
                    WHERE 
                        B.Status = 'в наличии'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        CreateBookPanel(row);
                    }
                }
            }
        }

        private void CreateBookPanel(DataRow book)
        {
            Panel panel = new Panel
            {
                Size = new Size(250, 250),
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(250, 150),
                Location = new Point(0, 0),
                Image = Image.FromFile(imagePath),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            Label titleLabel = new Label
            {
                Text = book["Title"].ToString(),
                Location = new Point(0, 150),
                Size = new Size(250, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            Label authorLabel = new Label
            {
                Text = book["AuthorName"].ToString(),
                Location = new Point(0, 180),
                Size = new Size(250, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10)
            };

            Button takeButton = new Button
            {
                Text = "Взять книгу",
                Location = new Point(0, 210),
                Size = new Size(250, 40),
                BackColor = Color.MediumAquamarine
            };

            takeButton.Click += (sender, e) => TakeBook(book["BookID"].ToString());

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(titleLabel);
            panel.Controls.Add(authorLabel);
            panel.Controls.Add(takeButton);

            flowLayoutPanel1.Controls.Add(panel);
        }

        private void TakeBook(string bookID)
        {
            if (dbHelper.TakeBook(Convert.ToInt32(bookID), readerID))
            {
                MessageBox.Show("Книга взята.");
                flowLayoutPanel1.Controls.Clear();
                LoadBooks();
            }
            else
            {
                MessageBox.Show("Ошибка при взятии книги.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BReturn breturn = new BReturn(readerID);
            breturn.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string searchQuery = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(searchQuery))
            {
                LoadBooks();
                return;
            }

            SearchBooks(searchQuery);
        }

        private void SearchBooks(string searchQuery)
        {
            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        B.BookID, 
                        B.Title, 
                        A.Name AS AuthorName, 
                        B.Status
                    FROM 
                        Book B
                    JOIN 
                        Author A ON B.AuthorID = A.AuthorID
                    WHERE 
                        B.Status = 'в наличии' AND (B.Title LIKE @SearchQuery OR A.Name LIKE @SearchQuery)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        CreateBookPanel(row);
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}