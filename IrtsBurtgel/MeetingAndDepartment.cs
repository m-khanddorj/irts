using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class MeetingAndDepartment : Entity
    {
        public override string TableName => "meeting_and_department";
        public override string IDName => "mdid";

        public int id;
        public int meetingId;
        public int departmentId;

        public MeetingAndDepartment()
        {
            id = -1;
            meetingId = -1;
            departmentId = -1;
        }

        public override List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "mdid", id });
            }
            list.Add(new Object[] { "meeting_id", meetingId });
            list.Add(new Object[] { "department_id", departmentId });
            return list;
        }

        public override Entity GetObj(SqlDataReader reader)
        {
            return new MeetingAndDepartment
            {
                id = (int)reader["mdid"],
                meetingId = (int)reader["meeting_id"],
                departmentId = (int)reader["department_id"]
            };
        }
    }
}
