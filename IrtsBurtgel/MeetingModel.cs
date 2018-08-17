using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IrtsBurtgel
{
    public class MeetingModel
    {
        string connectionString;

        public MeetingModel()
        {
            connectionString = Constants.GetConnectionString();
        }

        public int Add(Meeting meeting)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<Object[]> list = meeting.ToKVStringList();
                    List<string> columnnames = new List<string>();
                    List<string> paramnames = new List<string>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        columnnames.Add("\"" + list[i][0] + "\"");
                        paramnames.Add("@" + list[i][0]);
                    }
                    string sqlpart1 = String.Join(",", columnnames);
                    string sqlpart2 = String.Join(",", paramnames);
                    string sql = "INSERT INTO meeting(" + sqlpart1 + ") OUTPUT INSERTED.meeting_id VALUES (" + sqlpart2 + ") ";

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

        public void Set(Meeting meeting)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<Object[]> list = meeting.ToKVStringList();
                    List<string> keyValueStr = new List<string>();

                    for (int i = 1; i < list.Count; i++)
                    {
                        keyValueStr.Add("\"" + list[i][0] + "\" = " + "@" + list[i][0]);
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);
                    string sqlpart2 = "\"" + list[0][0] + "\" = " + "@" + list[0][0];
                    string sql = "UPDATE meeting SET " + sqlpart1 + " WHERE " + sqlpart2;

                    using (SqlCommand updateCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            updateCommand.Parameters.Add(new SqlParameter("@" + list[i][0], list[i][1]));
                        }
                        updateCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Successfully updated meeting.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public List<Meeting> GetAll()
        {
            List<Meeting> list = new List<Meeting>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT * FROM meeting order by cast(start_datetime as time)";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Meeting
                                {
                                    id = (int)reader["meeting_id"],
                                    name = (string)reader["name"],
                                    startDatetime = (DateTime)reader["start_datetime"],
                                    endDate = reader["end_date"].GetType() != typeof(DateTime) ? new DateTime() : (DateTime)reader["end_date"],
                                    duration = (int)reader["duration"],
                                    intervalDay = (int)reader["interval_day"],
                                    isDeleted = (bool)reader["is_deleted"]
                                });
                            }
                        }
                    }
                    MessageBox.Show("Successfully retreived meetings.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return list;
        }

        public Meeting Get(int id)
        {
            Meeting meeting = null;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT TOP (1) * FROM meeting WHERE meeting_id = @meeting_id";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@meeting_id", id));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                meeting = new Meeting
                                {
                                    id = (int)reader["meeting_id"],
                                    name = (string)reader["name"],
                                    startDatetime = (DateTime)reader["start_datetime"],
                                    endDate = reader["end_date"].GetType() != typeof(DateTime) ? new DateTime() : (DateTime)reader["end_date"],
                                    duration = (int)reader["duration"],
                                    intervalDay = (int)reader["interval_day"],
                                    isDeleted = (bool)reader["is_deleted"]
                                };
                                break;
                            }
                        }
                    }
                    MessageBox.Show("Successfully retreived meetings.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return meeting;
        }

        public int[] GetUsers(int id)
        {
            List<int> list = new List<int>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT user_id FROM meeting_and_user WHERE meeting_id=@mid";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@mid", id));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add((int)reader[0]);
                                break;
                            }
                        }
                    }
                    MessageBox.Show("Successfully retreived meetings.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return list.ToArray();
        }

        public int[] GetGroups(int id)
        {
            List<int> list = new List<int>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT group_id FROM meeting_and_group WHERE meeting_id=@mid";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@mid", id));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add((int)reader[0]);
                                break;
                            }
                        }
                    }
                    MessageBox.Show("Successfully retreived meetings.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return list.ToArray();
        }

        public void SetUsers(int id, int[] uids)
        {
            Object[] difference = GetDifference(GetUsers(id), uids);
            int[] toUpdate = (int[]) difference[0];
            int[] toInsert = (int[]) difference[1];
            int[] toDelete = (int[]) difference[2];

            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<string> sqlpart = new List<string>();

                    for(int i=0; i<toDelete.Length; i++)
                    {
                        sqlpart.Add("@uid" + i.ToString());
                    }

                    string sql = "DELETE FROM meeting_and_user WHERE meeting_id=@mid AND user_id in (" + String.Join(",", sqlpart) + ")";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@mid", id));

                        for (int i = 0; i < uids.Length; i++)
                        {
                            cmd.Parameters.Add(new SqlParameter("@uid" + i.ToString(), uids[i]));
                        }

                        cmd.ExecuteNonQuery();
                    }

                    sqlpart.Clear();

                    for (int i = 0; i < toInsert.Length; i++)
                    {
                        sqlpart.Add("(@uid" + i.ToString() + ", @mid)");
                    }

                    sql = "INSERT INTO meeting_and_useruser_id, meeting_id) VALUES " + String.Join(",", sqlpart);

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@mid", id));

                        for (int i = 0; i < uids.Length; i++)
                        {
                            cmd.Parameters.Add(new SqlParameter("@uid" + i.ToString(), uids[i]));
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetGroups(int id, int[] gids)
        {
            Object[] difference = GetDifference(GetGroups(id), gids);
            int[] toUpdate = (int[])difference[0];
            int[] toInsert = (int[])difference[1];
            int[] toDelete = (int[])difference[2];

            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<string> sqlpart = new List<string>();

                    for (int i = 0; i < toDelete.Length; i++)
                    {
                        sqlpart.Add("@gid" + i.ToString());
                    }

                    string sql = "DELETE FROM meeting_and_group WHERE meeting_id=@mid AND group_id in (" + String.Join(",", sqlpart) + ")";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@mid", id));

                        for (int i = 0; i < gids.Length; i++)
                        {
                            cmd.Parameters.Add(new SqlParameter("@gid" + i.ToString(), gids[i]));
                        }

                        cmd.ExecuteNonQuery();
                    }

                    sqlpart.Clear();

                    for (int i = 0; i < toInsert.Length; i++)
                    {
                        sqlpart.Add("(@gid" + i.ToString() + ", @mid)");
                    }

                    sql = "INSERT INTO meeting_and_group(group_id, meeting_id) VALUES " + String.Join(",", sqlpart);

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@mid", id));

                        for (int i = 0; i < gids.Length; i++)
                        {
                            cmd.Parameters.Add(new SqlParameter("@gid" + i.ToString(), gids[i]));
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static Object[] GetDifference(int[] oldIds, int[] newIds)
        {
            List<int> toInsert = new List<int>();
            List<int> toDelete = new List<int>();
            List<int> toUpdate = new List<int>();
            toInsert.AddRange(newIds);
            toDelete.AddRange(oldIds);

            if (newIds.Length == 0)
            {
                toDelete.AddRange(oldIds);
            }
            else
            {
                foreach (int value1 in newIds)
                {
                    foreach (int value2 in oldIds)
                    {
                        if (value1 == value2)
                        {
                            toUpdate.Add(value1);
                            toInsert.Remove(value1);
                            toDelete.Remove(value1);
                        }
                    }
                }
            }
            return new Object[]{toUpdate, toInsert, toDelete};
        }

    }
}
