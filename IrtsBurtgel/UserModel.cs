using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IrtsBurtgel
{
    public class UserModel
    {
        string connectionString;

        public UserModel()
        {
            connectionString = Constants.GetConnectionString();
        }

        public int Add(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<Object[]> list = user.ToKVStringList();
                    List<string> columnnames = new List<string>();
                    List<string> paramnames = new List<string>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        columnnames.Add("\"" + list[i][0] + "\"");
                        paramnames.Add("@" + list[i][0]);
                    }
                    string sqlpart1 = String.Join(",", columnnames);
                    string sqlpart2 = String.Join(",", paramnames);
                    string sql = "INSERT INTO \"user\"(" + sqlpart1 + ") OUTPUT INSERTED.user_id VALUES (" + sqlpart2 + ") ";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            insertCommand.Parameters.Add(new SqlParameter("@" + list[i][0], list[i][1]));
                        }
                        return (int)insertCommand.ExecuteScalar();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return -1;
            }
        }

        public void Set(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<Object[]> list = user.ToKVStringList();
                    List<string> keyValueStr = new List<string>();

                    for (int i = 1; i < list.Count; i++)
                    {
                        keyValueStr.Add("\"" + list[i][0] + "\" = " + "@" + list[i][0]);
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);
                    string sqlpart2 = "\"" + list[0][0] + "\" = " + "@" + list[0][0];
                    string sql = "UPDATE \"user\" SET " + sqlpart1 + " WHERE " + sqlpart2;

                    using (SqlCommand updateCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            updateCommand.Parameters.Add(new SqlParameter("@" + list[i][0], list[i][1]));
                        }
                        updateCommand.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public List<User> GetAll()
        {
            List<User> list = new List<User>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT * FROM \"user\"";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                list.Add(new User
                                {
                                    id = (int)reader["user_id"],
                                    fname = (string)reader["fname"],
                                    lname = (string)reader["lname"],
                                    fingerprint = (string)reader["fingerprint"],
                                    isDeleted = (bool)reader["is_deleted"]
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return list;
        }

        public User Get(int id)
        {
            User user = null;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT TOP (1) * FROM \"user\" WHERE user_id = @user_id";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@user_id", id));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user = new User
                                {
                                    id = (int)reader["user_id"],
                                    fname = (string)reader["fname"],
                                    lname = (string)reader["lname"],
                                    fingerprint = (string)reader["fingerprint"],
                                    isDeleted = (bool)reader["is_deleted"]
                                };
                                break;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return user;
        }

        public bool BulkAdd(List<User> users)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    foreach (User user in users)
                    {
                        List<Object[]> list = user.ToKVStringList();
                        List<string> columnnames = new List<string>();
                        List<string> paramnames = new List<string>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            columnnames.Add("\"" + list[i][0] + "\"");
                            paramnames.Add("@" + list[i][0]);
                        }
                        string sqlpart1 = String.Join(",", columnnames);
                        string sqlpart2 = String.Join(",", paramnames);
                        string sql = "INSERT INTO \"user\"(" + sqlpart1 + ") VALUES (" + sqlpart2 + ") ";

                        using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                insertCommand.Parameters.Add(new SqlParameter("@" + list[i][0], list[i][1]));
                            }
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                result = false;
            }
            return result;
        }
    }
}
