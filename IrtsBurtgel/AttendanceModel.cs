using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IrtsBurtgel
{
    public class AttendanceModel
    {
        string connectionString;

        public AttendanceModel()
        {
            connectionString = Constants.GetConnectionString();
        }

        public Attendance GetObj(SqlDataReader reader)
        {
            return new Attendance
            {
                id = (int)reader["attendance_id"],
                userId = (int)reader["attendance_id"],
                archivedMeetingId = (int)reader["attendance_id"],
                statusId = (int)reader["attendance_id"],
                regTime = (DateTime)reader["attendance_id"]
            };
        }
        public void Set(Attendance attendance)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    List<Object[]> list = attendance.ToKVStringList();
                    List<string> keyValueStr = new List<string>();

                    for (int i = 1; i < list.Count; i++)
                    {
                        keyValueStr.Add("\"" + list[i][0] + "\" = " + "@" + list[i][0]);
                    }
                    string sqlpart1 = String.Join(",", keyValueStr);
                    string sqlpart2 = "\"" + list[0][0] + "\" = " + "@" + list[0][0];
                    string sql = "UPDATE attendance SET " + sqlpart1 + " WHERE " + sqlpart2;

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

        public List<Attendance> GetByArchivedMeetingId(int archivedMeetingId)
        {
            List<Attendance> list = new List<Attendance>();
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    string sql = "SELECT * FROM attendance WHERE a_meeting_id = @a_meeting_id";

                    using (SqlCommand selectCommand = new SqlCommand(sql, conn))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("@a_meeting_id", archivedMeetingId));

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(GetObj(reader));
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

        public bool BulkAdd(List<Attendance> attendances)
        {
            bool result = true;
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connectionString;
                    conn.Open();

                    foreach (Attendance attendance in attendances)
                    {
                        List<Object[]> list = attendance.ToKVStringList();
                        List<string> columnnames = new List<string>();
                        List<string> paramnames = new List<string>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            columnnames.Add("\"" + list[i][0] + "\"");
                            paramnames.Add("@" + list[i][0]);
                        }
                        string sqlpart1 = String.Join(",", columnnames);
                        string sqlpart2 = String.Join(",", paramnames);
                        string sql = "INSERT INTO attendance(" + sqlpart1 + ") VALUES (" + sqlpart2 + ") ";

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
