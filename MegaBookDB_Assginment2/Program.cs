/*
    Author : Joon An
    ID : 991448483
    Date : June 12, 2018
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace MegaBookDB_Assginment2
{
    static class Program
    {
        
        [STAThread]
        static void Main()
        {
            // Connection object
            SqlConnection cnn;
            // Command Object
            SqlCommand command;

            // setup connection string to SQL server
            string connetionString = "Data Source=LAPTOP-EO2QHHSQ\\SQLEXPRESS;Initial Catalog=MegaBookDB;Integrated Security=SSPI;Persist Security Info=False";

            // setup creating BOOKS table
            string createBooksString = "CREATE TABLE dbo.BOOKS " +
              "(book_name VARCHAR(50) NOT NULL," +
              " author_name VARCHAR(50) NOT NULL," +
              "publish_date DATE NOT NULL," +
              "isbn VARCHAR(50) NOT NULL," +
             "book_id VARCHAR(50) PRIMARY KEY)";
      
            // setup creating REVIEWS table
            string createReviewsString = "CREATE TABLE dbo.REVIEWS " +
                "(reviewer_name VARCHAR(50) NOT NULL," +
                "review_text VARCHAR(50) NOT NULL," +
                "review_id VARCHAR(50) PRIMARY KEY," +
                "review_date DATE NOT NULL," +
                "rating VARCHAR(2) NULL," +
                "book_id VARCHAR(50) NOT NULL," +
                "CONSTRAINT FK_BOOKS_REVIEWS FOREIGN KEY(book_id) REFERENCES BOOKS(book_id))";

            // initializing con instance cotaining connectioon property
            cnn = new SqlConnection(connetionString);

            // Creating BOOKS table
            try
            {
                // open connection
                cnn.Open();
                
                // trying to connect to the database and input creating BOOK table command
                command = new SqlCommand(createBooksString, cnn);

                // executes creating BOOKS table command
                SqlDataReader reader_books = command.ExecuteReader();
                
            }
            catch (SqlException ex)
            {
                // SQL Error Message
                MessageBox.Show("Error in SQL! " + ex.Message);

            }
            finally
            {

                if (cnn.State == ConnectionState.Open)
                {

                    cnn.Close();

                }

            }

            // Creating REVIEWS table
            try
            {

                cnn.Open();

                command = new SqlCommand(createReviewsString, cnn);

                SqlDataReader reader_reviews = command.ExecuteReader();

            }
            catch (SqlException ex)
            {

                MessageBox.Show("Error in SQL! " + ex.Message);

            }
            finally
            {

                if (cnn.State == ConnectionState.Open)
                {

                    cnn.Close();

                }

            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // TO populate 3 forms consecutively
            Application.Run(new Form1());
            Application.Run(new Form2());
            Application.Run(new Form3());

        }

    }

}
