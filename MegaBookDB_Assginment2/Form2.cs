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
    public partial class Form2 : Form
    {
        // Set up for database connection
        string connetionString = "Data Source=LAPTOP-EO2QHHSQ\\SQLEXPRESS;Initial Catalog=MegaBookDB;Integrated Security=SSPI;Persist Security Info=False";

        // Same as in Form1
        SqlConnection cnn;
        SqlCommand command;

        // In order to store the existing review_id.
        // Then by utilizing this review_ids, generates a new review_id
        static int review_id = 0;

        // Setup for select join table to additionally get book_name and book_id for Form 2
        string queryString = "SELECT reviewer_name, review_text, review_id, review_date, rating, dbo.REVIEWS.book_id, " +
            "dbo.BOOKS.book_id, book_name " +
            "FROM dbo.REVIEWS " +
            "JOIN dbo.BOOKS " +
            "ON dbo.REVIEWS.book_id = dbo.BOOKS.book_id";

        // to store review_id and then generates a new review_id
        List<string> review_ids;

        // To get book_id and book_name from Form1
        Dictionary<string, string> book_ids_names;

        // Initialize count for a real time update of a number of insert actions
        int count = 0;

        public Form2()
        {

            InitializeComponent();

            // To show a number of counts of Inserts
            this.label12.Text = count.ToString();

        }

        private void update()
        {

            cnn = new SqlConnection(connetionString);

            // initialize review_ids
            review_ids = new List<string>();

            // To have book_id and book_name Form1 pulled out
            book_ids_names = new Dictionary<string, string>();
            
            // Creating Form1 instance 
            Form1 books = new Form1();

            // Get the return value, Dictionary from Form1
            book_ids_names = books.update();

            try
            {

                cnn.Open();

                command = new SqlCommand(queryString, cnn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    review_ids.Add(reader["review_id"].ToString());

                    Console.WriteLine(review_ids);

                    this.listBox1.Items.Add(reader["reviewer_name"].ToString() +
                        " : " + reader["review_text"].ToString() +
                        " : " + reader["review_id"].ToString() +
                        " : " + reader["review_date"].ToString() +
                        " : " + reader["rating"].ToString() +
                        " : " + reader["book_id"].ToString());
                }
                
                // List up and show book_id and book_name on Form 2
                foreach (string key in book_ids_names.Keys)
                {

                    this.listBox2.Items.Add($"{key} : {book_ids_names[key]}");
                  
                }

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

        // Same as in Form1
        private void idNumber()
        {


            if (review_ids.Count > 0)
            {

                int review_id_last_database = Int32.Parse(review_ids[review_ids.Count - 1].Substring(0, 3));

                // When there is no a number ommitted from 001 to a number of the last element
                if (review_ids.Count == review_id_last_database)
                {

                    review_id = review_id_last_database + 1;

                }
                else
                {
                    // When there is an ommitted number
                    for (int i = 0; i < 1000; i++)

                    {
                        if (i + 1 != Int32.Parse(review_ids[i].Substring(0, 3)))
                        {

                            review_id = i + 1;
                            break;

                        }
                    }

                }

            }
            else
            {

                review_id = 1;

            }

            if (review_id < 10)
            {

                this.textBox3.Text = $"00{review_id.ToString()}";
                this.label16.Text = $"00{review_id.ToString()}";

            }
            else if (review_id < 100)
            {

                this.textBox3.Text = $"0{review_id.ToString()}";
                this.label16.Text = $"0{review_id.ToString()}";

            }
            else
            {

                this.textBox3.Text = review_id.ToString();
                this.label16.Text = review_id.ToString();

            }

        }

        // Load a form before the events are initialized
        private void Form2_Load(object sender, EventArgs e)
        {

            // get the latest version of the REVIEWS fields
            update();

            // generates REVIEWS's review_id
            idNumber();

            this.label10.Hide();
            this.textBox3.Hide();
            this.button3.Hide();
            this.button4.Hide();

        }
        
        // Insert data to REVIEWS table
        private void button1_Click_1(object sender, EventArgs e)
        {
            cnn = new SqlConnection(connetionString);

            // Same as in Form1
            try
            {

                cnn.Open();
                command = new SqlCommand("INSERT INTO dbo.REVIEWS VALUES (@reviewer_name, @review_text, @review_id, @review_date, @rating, @book_id)", cnn);

                // sending data to SQL server
                command.Parameters.AddWithValue("@reviewer_name", this.textBox1.Text);
                command.Parameters.AddWithValue("@review_text", this.textBox2.Text);
                command.Parameters.AddWithValue("@review_id", this.label16.Text + "_R");

                // Auto generated date by implementing DateTime object
                command.Parameters.AddWithValue("@review_date", DateTime.Today.ToShortDateString());

                // rating can be null from the user because it is optional
                // Therefore, the user skip typing the rating.
                // At this moment, it must not cause the exception.
                if(!string.IsNullOrEmpty(this.textBox5.Text))
                {

                    // validate the string from the user
                    if (!this.textBox5.Text.All(char.IsDigit) || Int32.Parse(this.textBox5.Text) < 1)
                    {

                        MessageBox.Show("You must type one (1, 2..) or two digit number, (10 only) in Rating.");

                    }
                    else
                    {
                        // No error from the validation
                        command.Parameters.AddWithValue("@rating", this.textBox5.Text);
                    }
                }
                else
                {
                        // input null value to database. No error
                        command.Parameters.AddWithValue("@rating", DBNull.Value);
                    
                }

                // Same as book_id in Form1
                if (!this.label13.Text.All(char.IsDigit))
                {

                    MessageBox.Show("You must select a book in the book list");

                }
                else
                {
                    command.Parameters.AddWithValue("@book_id", this.label13.Text);
                }
                
                int r = command.ExecuteNonQuery();

                MessageBox.Show(r + " The new record successfully is added");
                
                cnn.Close();

                // Whenever Insert is correctly done, increase the count
                count++;
                this.label12.Text = count.ToString();

                this.textBox1.Text = "";
                this.textBox2.Text = "";
                this.textBox3.Text = "";
                this.textBox5.Text = "";

                // Add the current review_id
                review_ids.Add(this.label16.Text);

               // Based on the current review_id, generates a new review_id
                idNumber();
                
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

        // Veiw the lastes version of the fields
        private void button2_Click_1(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();
            update();

        }

        // Update fields. Same as in Form 1
        private void button3_Click(object sender, EventArgs e)
        {
            cnn = new SqlConnection(connetionString);

            try
            {

                cnn.Open();
                command = new SqlCommand("UPDATE dbo.REVIEWS SET reviewer_name=@reviewer_name, review_text=@review_text, " +
                    "review_date=@review_date, rating=@rating, book_id=@book_id WHERE review_id=@review_id", cnn);

                // must fill out all text inputs
                if (this.textBox1.Text == "" || this.textBox2.Text == "" ||
                    this.textBox3.Text == "" || this.textBox5.Text == "")
                {
                    MessageBox.Show("You forgot entering vlaues");

                    return;

                }

                command.Parameters.AddWithValue("@reviewer_name", this.textBox1.Text);
                command.Parameters.AddWithValue("@review_text", this.textBox2.Text);
                
                // Validate review_id. It must be a 3 digit number but real review_id has surfix of "_R"
                if (!this.textBox3.Text.All(char.IsDigit) || this.textBox3.Text == "000")
                {

                    MessageBox.Show("You must type 3 digit numbers greater tahn 000 in Review_ID.");

                }
                else
                {

                    command.Parameters.AddWithValue("@review_id", this.textBox3.Text + "_R");

                }

                // Automatically generates Date info
                command.Parameters.AddWithValue("@review_date", DateTime.Today.ToShortDateString());

                // Same as above in Insert
                if (!string.IsNullOrEmpty(this.textBox5.Text))
                {

                    if (!this.textBox5.Text.All(char.IsDigit) || Int32.Parse(this.textBox5.Text) < 1)
                    {

                        MessageBox.Show("You must type one (1, 2..) or two digit number, (10 only) in Rating.");

                    }
                    else
                    {

                        command.Parameters.AddWithValue("@rating", this.textBox5.Text);
                    }
                }
                else
                {

                    command.Parameters.AddWithValue("@rating", DBNull.Value);

                }

                // Must select a book list to Insert or update REVIEW Table to apply Foreign Key.
                // because it must use Primary Key of BOOKS table
                if (!this.label13.Text.All(char.IsDigit))
                {

                    MessageBox.Show("You must select a book in the book list");

                }
                else
                {

                    command.Parameters.AddWithValue("@book_id", this.label13.Text);

                }
                
                int r = command.ExecuteNonQuery();

                MessageBox.Show(r + " The update successfully is done!");

                cnn.Close();

                this.textBox1.Text = "";
                this.textBox2.Text = "";
                this.textBox3.Text = "";
                this.textBox5.Text = "";

            }

            catch (SqlException ex)
            {

                MessageBox.Show("Error in SQL! " + "Please, make sure you select a book in the booklist." +
                    "Also, put digit numbers.");
            }

            finally
            {

                if (cnn.State == ConnectionState.Open)
                {

                    cnn.Close();

                }

            }
        }

        // Delete fields
        private void button4_Click_1(object sender, EventArgs e)
        {
            cnn = new SqlConnection(connetionString);

            try
            {

                cnn.Open();
                command = new SqlCommand("DELETE FROM REVIEWS WHERE review_id=@review_id", cnn);

                // Validate review_id. Same as above Update
                if (!this.textBox3.Text.All(char.IsDigit) || this.textBox3.Text == "000")
                {

                    MessageBox.Show("You must type 3 digit numbers greater tahn 000 in Review_ID.");

                }
                else
                {

                    command.Parameters.AddWithValue("@review_id", this.textBox3.Text + "_R");

                }

                review_ids.Remove(this.textBox3.Text);

                int r = command.ExecuteNonQuery();

                MessageBox.Show(r + " The delete successfully is done!");

                cnn.Close();

                this.textBox1.Text = "";
                this.textBox2.Text = "";
                this.textBox3.Text = "";
                this.textBox5.Text = "";

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

        // Setup UI for View
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
            this.label10.Hide();
            this.textBox3.BackColor = Color.White;
            this.label1.Show();
            this.textBox1.Show();
            this.label2.Show();
            this.textBox2.Show();
            this.label4.Show();
            this.label9.Show();
            this.label5.Show();
            this.textBox5.Show();
            this.label6.Show();
            this.textBox3.Hide();
            this.label16.Show();
            this.label13.Show();
            this.button1.Show();
            this.button3.Hide();
            this.button4.Hide();

        }

        // Set up UI for Update
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            this.textBox3.Show();
            this.textBox3.BackColor = Color.Yellow;
            this.label10.Show();
            this.label1.Show();
            this.textBox1.Show();
            this.label2.Show();
            this.textBox2.Show();
            this.label4.Show();
            this.label9.Show();
            this.label5.Show();
            this.textBox5.Show();
            this.label6.Show();
            this.label10.Show();
            this.label16.Hide();
            this.label13.Show();
            this.button1.Hide();
            this.button3.Show();
            this.button4.Hide();



        }

        // Set up UI for Delete
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox3.Show();
            this.label1.Hide();
            this.textBox1.Hide();
            this.label2.Hide();
            this.textBox2.Hide();
            this.label4.Hide();
            this.label9.Hide();
            this.label5.Hide();
            this.textBox5.Hide();
            this.label6.Hide();
            this.label10.Hide();
            this.textBox3.BackColor = Color.White;
            this.label14.Hide();
            this.label13.Hide();
            this.label16.Hide();
            this.button1.Hide();
            this.button3.Hide();
            this.button4.Show();


        }

        // Select book_name and book_id from listbox
        // Then it delivers the book_id to label
        private void button5_Click(object sender, EventArgs e)
        {
            Console.WriteLine(listBox2.SelectedItem.ToString());
            
            this.label13.Text = listBox2.SelectedItem.ToString().Substring(0, 3);

        }

    }

}
