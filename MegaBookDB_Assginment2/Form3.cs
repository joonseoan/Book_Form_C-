/*
    Author : Joon An
    ID : 991448483
    Date : June 12, 2018
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MegaBookDB_Assginment2
{
    public partial class Form3 : Form
    {
        // Connection Setup
        string connetionString = "Data Source=LAPTOP-EO2QHHSQ\\SQLEXPRESS;Initial Catalog=MegaBookDB;Integrated Security=SSPI;Persist Security Info=False";

        SqlConnection cnn;
        SqlCommand command;

        // Use Join Select query to get book_name which is criteria for searching review fields
        string queryString = "SELECT reviewer_name, review_text, review_id, review_date, rating, dbo.REVIEWS.book_id, book_name " +
            "FROM dbo.REVIEWS " +
            "JOIN dbo.BOOKS " +
            "ON dbo.REVIEWS.book_id = dbo.BOOKS.book_id ";

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Nothing necessary to be loaded before the events run
        }

        // find functions
        private void findReview(string factors)
        {

            cnn = new SqlConnection(connetionString);

            // Validate book_id which is criteria to find REVIEWS fields
            if (!this.textBox1.Text.All(char.IsDigit) || this.textBox1.Text == "000")
            {

                MessageBox.Show("You must type 3 digit numbers greater than 000 in Review_ID.");

            }

            try
            {
                // query declared to be used for different environemt setups
                string query;

                if (factors == "book_id")
                {
                    
                    // based oN book_id
                    query = queryString + $"WHERE dbo.BOOKS.book_id = '{this.textBox1.Text}'";

                }
                else if (factors == "book_name")
                {
                    // based on book_name
                    query = queryString + $"WHERE dbo.BOOKS.book_name = '{this.textBox2.Text}'";

                }
                else
                {

                    // based on reviewer_name
                    Console.WriteLine("else");
                    query = queryString + $" WHERE reviewer_name = '{this.textBox3.Text}'";

                }

                cnn.Open();

                command = new SqlCommand(query, cnn);
                SqlDataReader reader = command.ExecuteReader();

                bool contents = false;
                
                while (reader.Read())
                {

                    this.listBox1.Items.Add(reader["reviewer_name"].ToString() +
                         " : " + reader["review_text"].ToString() +
                         " : " + reader["review_id"].ToString() +
                         " : " + reader["review_date"].ToString() +
                         " : " + reader["rating"].ToString() +
                         " : " + reader["book_id"].ToString());

                    contents = true;

                }
                
                // only when no data is available or when the user inputs invalid data 
                if(!contents) MessageBox.Show("Unable to find any reviews. Please double check your spelling.");

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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // invoke function with book_in parameter to search REVIEWS fields based on book_id 
            findReview("book_id");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            // invoke function with book_name parameter to search REVIEWS fields based on book_name 
            findReview("book_name");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // In order to search REVIEWS fields based on reviewer_name
            findReview("reviewer");

        }

    }

}
