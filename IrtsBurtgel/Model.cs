using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IrtsBurtgel
{
    public class Model<T> where T : Entity, new()
    {
        string connectionString;
        T staticObj;

        public Model()
        {
            connectionString = Constants.GetConnectionString();
            staticObj = new T();
        }

        public int Add(T obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<Object[]> list = obj.ToKVStringList();
                    List<string> columnnames = new List<string>();
                    List<string> paramnames = new List<string>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        columnnames.Add("\"" + list[i][0] + "\"");
                        paramnames.Add("@" + list[i][0]);
                    }
                    string sqlpart1 = String.Join(",", columnnames);
                    string sqlpart2 = String.Join(",", paramnames);
                    string sql = "INSERT INTO \"" + obj.TableName + "\"(" + sqlpart1 + ") OUTPUT INSERTED." + obj.IDName + " VALUES (" + sqlpart2 + ") ";

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

        public void Set(T obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<Object[]> list = obj.ToKVStringList();
                    List<string> keyValueStr = new List<string>();

                    for (int i = 1; i < list.Count; i++)
                    {
                        keyValueStr.Add("\"" + list[i][0] + "\" = " + "@" + list[i][0]);
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);
                    string sqlpart2 = "\"" + list[0][0] + "\" = " + "@" + list[0][0];
                    string sql = "UPDATE \"" + obj.TableName + "\" SET " + sqlpart1 + " WHERE " + sqlpart2;

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

        public List<T> GetAll()
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT * FROM \"" + staticObj.TableName + "\"";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add((T)staticObj.GetObj(reader));
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

        public T Get(int id)
        {
            T obj = null;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT TOP (1) * FROM \"" + staticObj.TableName + "\" WHERE " + staticObj.IDName + " = @id";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@id", id));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                obj = (T)staticObj.GetObj(reader);
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

            return obj;
        }

        public List<T> Get(int[] list)
        {
            List<T> result = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();
                    
                    List<string> keyValueStr = new List<string>();

                    for (int i = 1; i < list.Length; i++)
                    {
                        keyValueStr.Add("@id" + i.ToString());
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);

                    string sql = "SELECT TOP (1) * FROM \"" + staticObj.TableName + "\" WHERE " + staticObj.IDName + " IN (" + sqlpart1 + ")";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        for(int i = 1; i < list.Length; i++)
                        {
                            selectCommand.Parameters.Add(new SqlParameter("@id" + i.ToString(), list[i]));
                        }

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add((T)staticObj.GetObj(reader));
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

            return result;
        }

        public List<T> SelectBare(string sql, List<Object[]> parameters)
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        foreach (Object[] item in parameters)
                        {
                            selectCommand.Parameters.Add(new SqlParameter("@" + (string)item[0], item[1]));
                        }

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add((T)staticObj.GetObj(reader));
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

            return list;
        }

        public List<T> GetByFK(string fkName, int fkId)
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT * FROM \"" + staticObj.TableName + "\" WHERE " + fkName + " = @id";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                            selectCommand.Parameters.Add(new SqlParameter("@id", fkId));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add((T)staticObj.GetObj(reader));
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

            return list;
        }

        public List<T> GetByFK(string[] fkNames, int[] fkIds)
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();
                    
                    string sql = "SELECT * FROM \"" + staticObj.TableName + "\" WHERE " + fkNames + " = @id";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@id", fkIds));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add((T)staticObj.GetObj(reader));
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

            return list;
        }

        public bool BulkAdd(List<T> objs)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    foreach (T obj in objs)
                    {
                        List<Object[]> list = obj.ToKVStringList();
                        List<string> columnnames = new List<string>();
                        List<string> paramnames = new List<string>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            columnnames.Add("\"" + list[i][0] + "\"");
                            paramnames.Add("@" + list[i][0]);
                        }
                        string sqlpart1 = String.Join(",", columnnames);
                        string sqlpart2 = String.Join(",", paramnames);
                        string sql = "INSERT INTO \"" + staticObj.TableName + "\"(" + sqlpart1 + ") VALUES (" + sqlpart2 + ") ";

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


        public bool Remove(int id)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "DELETE FROM \"" + staticObj.TableName + "\" WHERE \"" + staticObj.IDName + "\" = @id ";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
                        insertCommand.Parameters.Add(new SqlParameter("@id", id));
                        insertCommand.ExecuteNonQuery();
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

        public bool MarkAsDeleted(int id)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "UPDATE \"" + staticObj.TableName + "\" SET is_deleted = 1 WHERE \"" + staticObj.IDName + "\" = @id ";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
                        insertCommand.Parameters.Add(new SqlParameter("@id", id));
                        insertCommand.ExecuteNonQuery();
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
