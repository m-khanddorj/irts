using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IrtsBurtgel
{
    public class ModifiedMeetingModel
    {
        string connectionString;

        public ModifiedMeetingModel()
        {
            connectionString = Constants.GetConnectionString();
        }

        public void Add(ModifiedMeeting meeting)
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
                    string sql = "INSERT INTO modified_meeting(" + sqlpart1 + ") VALUES (" + sqlpart2 + ") ";

                    using (SqlCommand insertCommand = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            insertCommand.Parameters.Add(new SqlParameter("@" + list[i][0], list[i][1]));
                        }
                        insertCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Successfully added meeting.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Set(ModifiedMeeting meeting)
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
                    string sql = "UPDATE modified_meeting SET " + sqlpart1 + " WHERE " + sqlpart2;

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

        public List<ModifiedMeeting> GetAll()
        {
            List<ModifiedMeeting> list = new List<ModifiedMeeting>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT * FROM modified_meeting order by cast(start_datetime as time)";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new ModifiedMeeting
                                {
                                    id = (int)reader["m_meeting_id"],
                                    name = (string)reader["name"],
                                    startDatetime = (DateTime)reader["start_datetime"],
                                    endDate = (DateTime)reader["end_date"],
                                    duration = (int)reader["duration"],
                                    reason = (string)reader["reason"],
                                    isDeleted = (bool)reader["is_deleted"],
                                    event_id = reader["event_id"].GetType() != typeof(int) ? -1 : (int)reader["event_id"],
                                    meeting_id = (int)reader["meeting_id"]
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

        public ModifiedMeeting Get(int id)
        {
            ModifiedMeeting meeting = null;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT TOP (1) * FROM modified_meeting WHERE meeting_id = @meeting_id";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@meeting_id", id));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                meeting = new ModifiedMeeting
                                {
                                    id = (int)reader["m_meeting_id"],
                                    name = (string)reader["name"],
                                    startDatetime = (DateTime)reader["start_datetime"],
                                    endDate = (DateTime)reader["end_date"],
                                    duration = (int)reader["duration"],
                                    reason = (string)reader["reason"],
                                    isDeleted = (bool)reader["is_deleted"],
                                    event_id = reader["event_id"].GetType() != typeof(int) ? -1 : (int)reader["event_id"],
                                    meeting_id = (int)reader["meeting_id"]
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

        public ModifiedMeeting FindByDateAndMid(DateTime date, int meeting_id)
        {
            ModifiedMeeting meeting = null;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT * FROM modified_meeting WHERE meeting_id = @meeting_id AND cast(start_datetime as date) < '" + date.Date.ToString("yyyyMMdd") + "' AND  cast(start_datetime as date)  >'" + date.Date.ToString("yyyyMMdd") + "'";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@meeting_id", meeting_id));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                meeting = new ModifiedMeeting
                                {
                                    id = (int)reader["m_meeting_id"],
                                    name = (string)reader["name"],
                                    startDatetime = (DateTime)reader["start_datetime"],
                                    endDate = (DateTime)reader["end_date"],
                                    duration = (int)reader["duration"],
                                    reason = (string)reader["reason"],
                                    isDeleted = (bool)reader["is_deleted"],
                                    event_id = reader["event_id"].GetType() != typeof(int) ? -1 : (int)reader["event_id"],
                                    meeting_id = (int)reader["meeting_id"]
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

            return meeting;
        }
        
    }
}
