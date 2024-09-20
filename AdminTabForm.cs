using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class AdminTabForm : Form
    {
        private string connectionString = "Server=ADCLG1;Database=Горб_практика;Trusted_Connection=True;";
        public AdminTabForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Books books = new Books();
            books.Show();
            this.Hide();
        }

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        R.ReaderID, 
                        R.Name, 
                        R.Password, 
                        R.ContactInfo, 
                        CASE 
                            WHEN F.LoanID IS NOT NULL THEN 'Да' 
                            ELSE 'Нет' 
                        END AS HasFine,
                        F.Amount,
                        F.Status
                    FROM 
                        Reader R
                    LEFT JOIN 
                        Loan L ON R.ReaderID = L.ReaderID
                    LEFT JOIN 
                        Fine F ON L.LoanID = F.LoanID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}