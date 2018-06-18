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
    public partial class Form1 : Form
    {
        // Connection string to MegaBookDB
        string connetionString = "Data Source=LAPTOP-EO2QHHSQ\\SQLEXPRESS;Initial Catalog=MegaBookDB;Integrated Security=SSPI;Persist Security Info=False";

        // Connnection and command instace for queries
        SqlConnection cnn;
        SqlCommand command;

        // Intialize book_id to automatically update
        static int book_id = 0;

        // Query to find and show fileds to the form.
        string queryString = "SELECT * FROM dbo.BOOKS";

        // In ordet to store the exsisting book_ids
        // Ultimately to get new book_id by comparing the existing book_ids
        List<string> book_ids;
        
        public Form1()
        {

            InitializeComponent();

        }

        // To pull and view the latest updated fields 
        public Dictionary<string, string> update()
        {
            // connection for the database
            cnn = new SqlConnection(connetionString);

            // intializing book_id collection
            book_ids = new List<string>();

            // In order to deliver the book_id and book name to Form 2
            //      without additionaly queries
            Dictionary<string, string> book_ids_names = new Dictionary<string, string>();

            try
            {

                cnn.Open();

                command = new SqlCommand(queryString, cnn);
                SqlDataReader reader = command.ExecuteReader();

                // Execute reading fields and rendering them to listbox in the form
                while (reader.Read())
                {

                    book_ids.Add(reader["book_id"].ToString());
                    book_ids_names.Add(reader["book_id"].ToString(), reader["book_name"].ToString());

                    this.listBox1.Items.Add(reader["book_name"].ToString() +
                        " : " + reader["author_name"].ToString() +
                        " : " + reader["publish_date"].ToString() +
                        " : " + reader["isbn"].ToString() +
                        " : " + reader["book_id"].ToString());

                }

            }
            // Exception control
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

            // returns the Dictionary obj value to Form2
            return book_ids_names;

        }

        private void idNumber()
        {
            if (book_ids.Count > 0)
            {
                // Get the last number of book_ids of List to generate new book_id
                int book_id_last_database = int.Parse(book_ids[book_ids.Count - 1]);

                // if the last number is equivalent to the length of List
                // there is no number ommited from 1 to the last element of List
                if (book_ids.Count == book_id_last_database)
                {

                    book_id = book_id_last_database + 1;

                }
                else
                {
                    // If the numbers are ommitted,
                    //      set up book_id with a min number of the ommitted numbers
                    // Then it can minimize confusion from the user
                    for (int i = 0; i < 1000; i++)

                    {
                        if (i + 1 != Int32.Parse(book_ids[i]))
                        {

                            book_id = i + 1;
                            break;

                        }
                    }

                }

            }
            else
            {

                book_id = 1;

            }

            // set up 3 digit book_id in a format of "001" ...."002"
            if (book_id < 10)
            {

                this.label8.Text = $"00{book_id.ToString()}";
                this.textBox5.Text = $"00{book_id.ToString()}";


            }
            else if (book_id < 100)
            {

                this.label8.Text = $"0{book_id.ToString()}";
                this.textBox5.Text = $"0{book_id.ToString()}";

            }
            else
            {

                this.label8.Text = book_id.ToString();
                this.textBox5.Text = book_id.ToString();
            }

        }

        // Loading form with elements when it starts to run
        private void Form1_Load(object sender, EventArgs e)
        {
            
            // Show the latest version of the list when it starts running.
            update();

            // Automatically populates a new book_id which is a minimum number
            idNumber();


            this.textBox5.Hide();
            this.label11.Hide();
            this.button3.Hide();
            this.button4.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            cnn = new SqlConnection(connetionString);

            try
            {

                cnn.Open();
                command = new SqlCommand("INSERT INTO dbo.BOOKS VALUES (@book_name, @author_name, @publish_date, @isbn, @book_id)", cnn);

                // Assign book_name, author_name, publish_date to Insert query
                command.Parameters.AddWithValue("@book_name", this.textBox1.Text);
                command.Parameters.AddWithValue("@author_name", this.textBox2.Text);
                command.Parameters.AddWithValue("@publish_date", this.textBox3.Text);

                // Define 7 character for ISBN
                if(this.textBox4.Text.Length >= 7)
                {
                    command.Parameters.AddWithValue("@isbn", this.textBox4.Text);

                }
                else
                {
                    MessageBox.Show("You typed less than 7 letters.");

                }

                // Assign book_id from label that has the new book_id
                // In order for the user to type invalid user_id
                //      use the label.
                command.Parameters.AddWithValue("@book_id", this.label8.Text); 

                // number of queries
                int r = command.ExecuteNonQuery();

                MessageBox.Show(r + " The new record successfully is added");

                cnn.Close();

                // Put the current book_id to eventually a new book_id below 
                book_ids.Add(this.label8.Text);

                // update a new book_id
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

        // invoke the new latest version fields list
        private void button2_Click(object sender, EventArgs e)
        {
            
            this.listBox1.Items.Clear();
            update();

        }

        // update the field value
        private void button3_Click(object sender, EventArgs e)
        {

            cnn = new SqlConnection(connetionString);

            try
            {

                cnn.Open();
                command = new SqlCommand("UPDATE dbo.BOOKS SET book_name=@book_name, author_name = @author_name, " +
                    "publish_date=@publish_date, isbn=@isbn WHERE book_id=@book_id", cnn);

                // define the user must fill out all text inputs.
                if(this.textBox1.Text == "" || this.textBox2.Text == "" ||
                    this.textBox3.Text == "" || this.textBox4.Text == "" )
                {
                    MessageBox.Show("You forgot entering vlaues");

                    return;

                }
                
                command.Parameters.AddWithValue("@book_name", this.textBox1.Text);
                command.Parameters.AddWithValue("@author_name", this.textBox2.Text);
                command.Parameters.AddWithValue("@publish_date", this.textBox3.Text);

                // same as above in Insert
                if (this.textBox4.Text.Length >= 7)
                {
                    command.Parameters.AddWithValue("@isbn", this.textBox4.Text);

                }
                else
                {
                    MessageBox.Show("You typed less than 7 letters in ISBN.");

                }

                // book_id validation. It must be 3 digit numbers and also start from 001, not from 000
                if (!this.textBox5.Text.All(char.IsDigit) || this.textBox5.Text == "000")
                {

                    MessageBox.Show("You must type 3 digit numbers greater tahn 000 in Book_ID.");

                }
                else
                {
                    // In order to prevent a new book_id that is not available in the fields
                    bool equivalent = false;

                    foreach(string id in book_ids)
                    {
                        // query only when the user utilizes the book_id that arleady exists
                        if(id == this.textBox5.Text)
                        {
                            command.Parameters.AddWithValue("@book_id", this.textBox5.Text);

                            int r = command.ExecuteNonQuery();
                            MessageBox.Show(r + " The update successfully is done!");
                            equivalent = true;
                            break;

                        }

                    }

                    if (!equivalent) MessageBox.Show("Unable to find same Book_ID. Please, check find the book_id in the list.");

                }

                cnn.Close();

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
        
        private void button4_Click(object sender, EventArgs e)
        {
            cnn = new SqlConnection(connetionString);

            try
            {

                cnn.Open();
                command = new SqlCommand("DELETE FROM BOOKS WHERE book_id=@book_id", cnn);
                
                // Same as above in update
                if (!this.textBox5.Text.All(char.IsDigit) || this.textBox5.Text == "000")
                {

                    MessageBox.Show("You must type 3 digit numbers greater tahn 000 in Book_ID.");

                }
                else
                {
                    // Same as above in update
                    if (!this.textBox5.Text.All(char.IsDigit) || this.textBox5.Text == "000")
                    {

                        MessageBox.Show("You must type 3 digit numbers greater tahn 000 in Book_ID.");

                    }
                    else
                    {
                        // Same as above in update
                        bool equivalent = false;

                        foreach (string id in book_ids)
                        {
                            if (id == this.textBox5.Text)
                            {
                                command.Parameters.AddWithValue("@book_id", this.textBox5.Text);

                                // Also, remove the book_id deleted from List to prevent confusion 
                                book_ids.Remove(this.textBox5.Text);

                                int r = command.ExecuteNonQuery();

                                MessageBox.Show(r + " The delete successfully is done!");

                                equivalent = true;

                                break;

                            }

                        }

                        if (!equivalent) MessageBox.Show("Unable to find same Book_ID. Please, check find the book_id in the list.");

                    }

                }

                cnn.Close();

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

        // Set up UI for "View"
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.label11.Hide();
            this.textBox5.Hide();
            this.label1.Show();
            this.label2.Show();
            this.label3.Show();
            this.label4.Show();
            this.label5.Show();
            this.label9.Show();
            this.label10.Show();
            this.textBox1.Show();
            this.textBox2.Show();
            this.textBox3.Show();
            this.textBox4.Show();
            this.textBox5.Hide();
            this.label8.Show();
            this.button3.Hide();
            this.button4.Hide();
            this.button1.Show();
        }

        // Set up UI for "Edit"
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
          
            this.label1.Show();
            this.label2.Show();
            this.label3.Show();
            this.label4.Show();
            this.label5.Show();
            this.label8.Hide();
            this.label9.Show();
            this.label10.Show();
            this.textBox1.Show();
            this.textBox2.Show();
            this.textBox3.Show();
            this.textBox4.Show();
            this.textBox5.Show();
            this.label11.Show();
            this.textBox5.Show();
            this.button1.Hide();
            this.button4.Hide();
            this.button3.Show();
        }

        // Set up UI for "Delete"
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.label11.Show();
            this.textBox5.Show();
            this.label1.Hide();
            this.label2.Hide();
            this.label3.Hide();
            this.label4.Hide();
            this.label5.Show();
            this.label9.Hide();
            this.label10.Hide();
            this.textBox1.Hide();
            this.textBox2.Hide();
            this.textBox3.Hide();
            this.textBox4.Hide();
            this.textBox5.Hide();
            this.label8.Hide();
            this.textBox5.Show();
            this.button1.Hide();
            this.button3.Hide();
            this.button4.Show();
        }
    }

}
