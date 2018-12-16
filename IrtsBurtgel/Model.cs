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
        public T staticObj;
        bool identityInsert = false;

        public Model()
        {
            connectionString = Constants.GetConnectionString();
            staticObj = new T();
        }

        public Model(bool identityInsert)
        {
            this.identityInsert = identityInsert;
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
                        if (list[i][1] == null)
                        {
                            paramnames.Add("NULL");
                        }
                        else
                        {
                            paramnames.Add("@" + list[i][0]);
                        }
                    }
                    string sqlpart1 = String.Join(",", columnnames);
                    string sqlpart2 = String.Join(",", paramnames);
                    string sql = "INSERT INTO \"" + obj.TableName + "\"(" + sqlpart1 + ") OUTPUT INSERTED." + obj.IDName + " VALUES (" + sqlpart2 + ") ";

                    if (identityInsert)
                    {
                        SetIdentityInsert(conn, true);
                    }

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i][1] != null)
                            {
                                insertCommand.Parameters.Add(new SqlParameter("@" + list[i][0], list[i][1]));
                            }
                        }
                        int id = (int)insertCommand.ExecuteScalar();

                        if (identityInsert)
                        {
                            SetIdentityInsert(conn, false);
                        }

                        return id;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return -1;
            }
        }

        public bool Set(T obj)
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
                        if (list[i][1] == null)
                        {
                            keyValueStr.Add("\"" + list[i][0] + "\" = NULL");
                        }
                        else
                        {
                            keyValueStr.Add("\"" + list[i][0] + "\" = " + "@" + list[i][0]);
                        }
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);
                    string sqlpart2 = "\"" + list[0][0] + "\" = " + "@" + list[0][0];
                    string sql = "UPDATE \"" + obj.TableName + "\" SET " + sqlpart1 + " WHERE " + sqlpart2;

                    using (SqlCommand updateCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i][1] != null)
                            {
                                updateCommand.Parameters.Add(new SqlParameter("@" + list[i][0], list[i][1]));
                            }
                        }
                        return updateCommand.ExecuteNonQuery() > 0;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
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

                    for (int i = 0; i < list.Length; i++)
                    {
                        keyValueStr.Add("@id" + i.ToString());
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);

                    string sql = "SELECT * FROM \"" + staticObj.TableName + "\" WHERE " + staticObj.IDName + " IN (" + sqlpart1 + ")";
                    Console.WriteLine(sql);
                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            selectCommand.Parameters.Add(new SqlParameter("@id" + i.ToString(), list[i]));
                        }

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add((T)staticObj.GetObj(reader));
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

        public List<T> SelectBare(string sql)
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

        //public List<T> GetByFilter()
        //{

        //}

        //public List<Object> GetByJoin<U>() where U : Entity
        //{

        //}

        public List<T> GetByFK(string[] fkNames, int[] fkIds)
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT * FROM \"" + staticObj.TableName + "\" WHERE " + String.Join(",", fkNames) + " = @id";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@id", fkIds));

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

        public List<T> GetByFK(string fkName, int[] fkIds)
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<string> keyValueStr = new List<string>();
                    for (int i = 0; i < fkIds.Length; i++)
                    {
                        keyValueStr.Add("@id" + i.ToString());
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);

                    string sql = "SELECT * FROM \"" + staticObj.TableName + "\" WHERE " + fkName + " in (" + sqlpart1 + ")";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < fkIds.Length; i++)
                        {
                            selectCommand.Parameters.Add(new SqlParameter("@id" + i.ToString(), fkIds[i]));
                        }

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

        public bool BulkAdd(List<T> objs)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    if (identityInsert)
                    {
                        SetIdentityInsert(conn, true);
                    }

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

                    if (identityInsert)
                    {
                        SetIdentityInsert(conn, false);
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

        public bool Remove(int[] list)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<string> keyValueStr = new List<string>();

                    for (int i = 0; i < list.Length; i++)
                    {
                        keyValueStr.Add("@id" + i.ToString());
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);

                    string sql = "DELETE FROM \"" + staticObj.TableName + "\" WHERE \"" + staticObj.IDName + "\" = @id ";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            insertCommand.Parameters.Add(new SqlParameter("@id" + i.ToString(), list[i]));
                        }

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

        public bool RemoveExcept(int[] list)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<string> keyValueStr = new List<string>();

                    for (int i = 0; i < list.Length; i++)
                    {
                        keyValueStr.Add("@id" + i.ToString());
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);

                    string sql = "DELETE FROM \"" + staticObj.TableName + "\" WHERE \"" + staticObj.IDName + "\" NOT IN (" + sqlpart1 + ") ";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            insertCommand.Parameters.Add(new SqlParameter("@id" + i.ToString(), list[i]));
                        }

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

        public bool RemoveAll()
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "DELETE FROM \"" + staticObj.TableName + "\"; DBCC CHECKIDENT (\"" + staticObj.TableName + "\", RESEED, 0);";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
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

        public bool MarkAsDeletedExcept(int[] list)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<string> keyValueStr = new List<string>();

                    for (int i = 0; i < list.Length; i++)
                    {
                        keyValueStr.Add("@id" + i.ToString());
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);

                    string sql = "UPDATE \"" + staticObj.TableName + "\"  SET is_deleted = 1 WHERE \"" + staticObj.IDName + "\" NOT IN (" + sqlpart1 + ") ";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            insertCommand.Parameters.Add(new SqlParameter("@id" + i.ToString(), list[i]));
                        }

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

        public bool MarkAllAsDeleted()
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<string> keyValueStr = new List<string>();

                    string sql = "UPDATE \"" + staticObj.TableName + "\"  SET is_deleted = 1 WHERE 1=1 ";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
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

        // Set connection identity insert
        private bool SetIdentityInsert(SqlConnection connection, bool onoff)
        {
            bool result = true;
            try
            {
                string sql = "SET IDENTITY_INSERT \"" + staticObj.TableName + "\" " + (onoff ? "ON" : "OFF");

                using (SqlCommand insertCommand = new SqlCommand(sql, connection))
                {
                    result = -1 != insertCommand.ExecuteNonQuery();
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
