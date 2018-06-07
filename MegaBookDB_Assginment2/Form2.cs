﻿using System;
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
        string connetionString = "Data Source=LAPTOP-EO2QHHSQ\\SQLEXPRESS;Initial Catalog=MegaBookDB;Integrated Security=SSPI;Persist Security Info=False";
        SqlConnection cnn;
        SqlCommand command;

        static int review_id = 0;

        string queryString = "SELECT * FROM dbo.REVIEWS";

        List<string> review_ids;


        public Form2()
        {

            InitializeComponent();

        }

        private void update()
        {
            cnn = new SqlConnection(connetionString);

            review_ids = new List<string>();

            try
            {

                cnn.Open();

                command = new SqlCommand(queryString, cnn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    review_ids.Add(reader["review_id"].ToString());

                    this.listBox1.Items.Add(reader["reviewer_name"].ToString() +
                        " : " + reader["review_text"].ToString() +
                        " : " + reader["review_id"].ToString() +
                        " : " + reader["review_date"].ToString() +
                        " : " + reader["rating"].ToString());
                        //" : " + reader["book_id"].ToString());
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

        private void Form2_Load(object sender, EventArgs e)
        {

            update();

            for (int i = 0; i < review_ids.Count; i++)
            {
                Console.WriteLine(Int32.Parse(review_ids[i].Substring(0, 2)));
            }

            

            if (review_ids.Count > 0)
            {
                int review_id_last_database = Int32.Parse(review_ids[review_ids.Count - 1]);

                if (review_ids.Count == review_id_last_database)
                {

                    review_id = review_id_last_database + 1;

                }
                else
                {

                    for (int i = 0; i < 1000; i++)

                    {
                        if (i + 1 != Int32.Parse(review_ids[i]))
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

            Console.WriteLine("book_id: " + review_id);


            if (review_id < 10)
            {

                this.textBox3.Text = $"00{review_id.ToString()}_R";


            }
            else if (review_id < 100)
            {

                this.textBox3.Text = $"0{review_id.ToString()}_R";

            }
            else
            {

                this.textBox3.Text = review_id.ToString() + "_R";

            }

        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {

            cnn = new SqlConnection(connetionString);

            try
            {

                cnn.Open();
                command = new SqlCommand("INSERT INTO dbo.BOOKS VALUES (@book_name, @author_name, @publish_date, @isbn, @book_id)", cnn);

                command.Parameters.AddWithValue("@book_name", this.textBox1.Text);
                command.Parameters.AddWithValue("@author_name", this.textBox2.Text);
                command.Parameters.AddWithValue("@publish_date", this.textBox3.Text);
                command.Parameters.AddWithValue("@isbn", this.textBox4.Text);
                command.Parameters.AddWithValue("@book_id", this.textBox5.Text);

                int r = command.ExecuteNonQuery();

                MessageBox.Show(r + " The new record successfully is added");

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

        private void button2_Click(object sender, EventArgs e)
        {

            this.listBox1.Items.Clear();
            update();

        }

        private void button3_Click(object sender, EventArgs e)
        {

            cnn = new SqlConnection(connetionString);

            try
            {

                cnn.Open();
                command = new SqlCommand("UPDATE dbo.BOOKS SET book_name=@book_name, author_name = @author_name, " +
                    "publish_date=@publish_date, isbn=@isbn WHERE book_id=@book_id", cnn);

                if (this.textBox1.Text == "" || this.textBox2.Text == "" ||
                    this.textBox3.Text == "" || this.textBox4.Text == "" || this.textBox5.Text == "")
                {
                    MessageBox.Show("You forgot entering vlaues");

                    return;

                }

                command.Parameters.AddWithValue("@book_name", this.textBox1.Text);
                command.Parameters.AddWithValue("@author_name", this.textBox2.Text);
                command.Parameters.AddWithValue("@publish_date", this.textBox3.Text);
                command.Parameters.AddWithValue("@isbn", this.textBox4.Text);
                command.Parameters.AddWithValue("@book_id", this.textBox5.Text);

                int r = command.ExecuteNonQuery();

                MessageBox.Show(r + " The update successfully is done!");

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

                command.Parameters.AddWithValue("@book_id", this.textBox5.Text);

                book_ids.Remove(this.textBox5.Text);

                int r = command.ExecuteNonQuery();

                MessageBox.Show(r + " The delete successfully is done!");

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
        }*/
    }
}
