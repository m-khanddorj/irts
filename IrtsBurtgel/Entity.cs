using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace IrtsBurtgel
{
    public class Entity
    {
        public virtual string TableName
        {
            get { return ""; }
        }

        public virtual string IDName
        {
            get { return ""; }
        }

        public virtual List<Object[]> ToKVStringList()
        {
            return new List<Object[]>();
        }

        public string GetTableName()
        {
            return TableName;
        }

        public string GetIDName()
        {
            return IDName;
        }

        public virtual Entity GetObj(SqlDataReader reader)
        {
            return new Entity();
        }
    }
}
