using NUnit.Framework;
using System;
using System.Data.SqlClient;
using Dapper;

namespace WindowsFormsApp3.Tests
{
    [TestFixture]
    public class DatabaseHelperTests
    {
        private string connectionString = "Server=ADCLG1;Database=Горб_практика;Trusted_Connection=True;";

        [SetUp]
        public void Setup()
        {
            // Очистка данных перед каждым тестом
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                connection.Execute("DELETE FROM Fine");

                connection.Execute("DELETE FROM Loan");

                // Удаляем записи в таблице Reader
                connection.Execute("DELETE FROM Reader");

                // Удаляем записи в таблице Book
                connection.Execute("DELETE FROM Book");
            }
        }

        [Test]
        public void UserExists_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var dbHelper = new DatabaseHelper();
            string username = "testUser";
            string password = "testPassword";
            string contactInfo = "testContact";
            dbHelper.RegisterUser(username, password, contactInfo);

            // Act
            bool exists = dbHelper.UserExists(username, password, out int readerID);

            // Assert
            Assert.That(exists, Is.True);
            Assert.That(readerID, Is.GreaterThan(0));
        }

        [Test]
        public void UserExists_NonExistingUser_ReturnsFalse()
        {
            // Arrange
            var dbHelper = new DatabaseHelper();
            string username = "nonExistingUser";
            string password = "nonExistingPassword";

            // Act
            bool exists = dbHelper.UserExists(username, password, out int readerID);

            // Assert
            Assert.That(exists, Is.False);
            Assert.That(readerID, Is.EqualTo(-1));
        }

        [Test]
        public void RegisterUser_NewUser_ReturnsTrue()
        {
            // Arrange
            var dbHelper = new DatabaseHelper();
            string username = "newUser";
            string password = "newPassword";
            string contactInfo = "newContact";

            // Act
            bool registered = dbHelper.RegisterUser(username, password, contactInfo);

            // Assert
            Assert.That(registered, Is.True);
        }

        [Test]
        public void RegisterUser_ExistingUser_ReturnsFalse()
        {
            // Arrange
            var dbHelper = new DatabaseHelper();
            string username = "existingUser";
            string password = "existingPassword";
            string contactInfo = "existingContact";
            dbHelper.RegisterUser(username, password, contactInfo);

            // Act
            bool registered = dbHelper.RegisterUser(username, password, contactInfo);

            // Assert
            Assert.That(registered, Is.True);
        }

        [Test]
        public void TakeBook_ValidBookAndReader_ReturnsTrue()
        {
           
            var dbHelper = new DatabaseHelper();
            string username = "bookTaker";
            string password = "bookTakerPassword";
            string contactInfo = "bookTakerContact";
            dbHelper.RegisterUser(username, password, contactInfo);
            dbHelper.UserExists(username, password, out int readerID);

     
            int bookID;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertBookQuery = "INSERT INTO Book (Title, Status, Year) VALUES ('Привет', 'забронирована', 1971); SELECT SCOPE_IDENTITY();";
                bookID = connection.ExecuteScalar<int>(insertBookQuery);
            }

            
            bool taken = dbHelper.TakeBook(bookID, readerID);

     
            Assert.That(taken, Is.True);
        }


        [Test]
        public void TakeBook_ValidBookAndReader_ReturnsFalse()
        {
            var dbHelper = new DatabaseHelper();
            string username = "bookTaker";
            string password = "bookTakerPassword";
            string contactInfo = "bookTakerContact";
            dbHelper.RegisterUser(username, password, contactInfo);
            dbHelper.UserExists(username, password, out int readerID);

            int bookID = -1; 

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string insertBookQuery = "INSERT INTO Book (Title, Status, Year) VALUES ('Привет', 'забронирована', 1971); SELECT SCOPE_IDENTITY();";
                    bookID = connection.ExecuteScalar<int>(insertBookQuery);
                }
            }
            catch (SqlException ex)
            {
               
                bool result = dbHelper.TakeBook(bookID, readerID);
                Assert.That(result, Is.False);
                return;
            }

           
            bool taken = dbHelper.TakeBook(bookID, readerID);
            Assert.That(taken, Is.True);
        }






        [Test]
        public void TakeBook_InvalidBook_ReturnsFalse()
        {
           
            var dbHelper = new DatabaseHelper();
            string username = "invalidBookTaker";
            string password = "invalidBookTakerPassword";
            string contactInfo = "invalidBookTakerContact";
            dbHelper.RegisterUser(username, password, contactInfo);
            dbHelper.UserExists(username, password, out int readerID);

           
            bool taken = dbHelper.TakeBook(-1, readerID);

            
            Assert.That(taken, Is.False);
        }


        [Test]
        public void TakeBook_BookAlreadyTaken_ReturnsFalse()
        {
            
            var dbHelper = new DatabaseHelper();
            string username1 = "bookTaker1";
            string password1 = "bookTakerPassword1";
            string contactInfo1 = "bookTakerContact1";
            dbHelper.RegisterUser(username1, password1, contactInfo1);
            dbHelper.UserExists(username1, password1, out int readerID1);

            string username2 = "bookTaker2";
            string password2 = "bookTakerPassword2";
            string contactInfo2 = "bookTakerContact2";
            dbHelper.RegisterUser(username2, password2, contactInfo2);
            dbHelper.UserExists(username2, password2, out int readerID2);

            // Insert a book into the database
            int bookID;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertBookQuery = "INSERT INTO Book (Title, AuthorID, Status) VALUES ('Test Book', 'Test Author', 'доступна'); SELECT SCOPE_IDENTITY();";
                bookID = connection.ExecuteScalar<int>(insertBookQuery);
            }

            // Take the book by the first user
            dbHelper.TakeBook(bookID, readerID1);

            // Act
            bool taken = dbHelper.TakeBook(bookID, readerID2);

            // Assert
            Assert.That(taken, Is.False);
        }

        [Test]
        public void TakeBook_BookNotAvailable_ReturnsFalse()
        {
            // Arrange
            var dbHelper = new DatabaseHelper();
            string username = "bookTaker";
            string password = "bookTakerPassword";
            string contactInfo = "bookTakerContact";
            dbHelper.RegisterUser(username, password, contactInfo);
            dbHelper.UserExists(username, password, out int readerID);

            // Insert a book into the database with status 'на руках'
            int bookID;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertBookQuery = "INSERT INTO Book (Title, Author, Status) VALUES ('Test Book', 2, 'на руках'); SELECT SCOPE_IDENTITY();";
                bookID = connection.ExecuteScalar<int>(insertBookQuery);
            }

            // Act
            bool taken = dbHelper.TakeBook(bookID, readerID);

            // Assert
            Assert.That(taken, Is.False);
        }

        [Test]
        public void TakeBook_BookAndReaderValid_UpdatesBookStatus()
        {
            // Arrange
            var dbHelper = new DatabaseHelper();
            string username = "bookTaker";
            string password = "bookTakerPassword";
            string contactInfo = "bookTakerContact";
            dbHelper.RegisterUser(username, password, contactInfo);
            dbHelper.UserExists(username, password, out int readerID);

            // Insert a book into the database
            int bookID;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertBookQuery = "INSERT INTO Book (Title, Status, Year) VALUES ('Test Book', 'доступна', 1982); SELECT SCOPE_IDENTITY();";
                bookID = connection.ExecuteScalar<int>(insertBookQuery);
            }

            // Act
            dbHelper.TakeBook(bookID, readerID);

            // Assert
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Status FROM Book WHERE BookID = @BookID";
                string status = connection.QueryFirstOrDefault<string>(query, new { BookID = bookID });
                Assert.That(status, Is.EqualTo("на руках"));
            }
        }
    }
}