using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class User
    {
        public int id;
        public string fname;
        public string lname;
        public string fingerprint;
        public bool isDeleted;

        public User() { }

        public User(int id, string fname, string lname, string fingerprint)
        {
            this.id = id;
            this.fname = fname;
            this.lname = lname;
            this.fingerprint = fingerprint;
        }

        public virtual List<Object[]> ToKVStringList()
        {
            List<Object[]> list = new List<Object[]>();
            if (id != -1)
            {
                list.Add(new Object[] { "user_id", id });
            }
            list.Add(new Object[] { "fname", fname });
            list.Add(new Object[] { "lname", lname });
            list.Add(new Object[] { "fingerprint", fingerprint });
            list.Add(new Object[] { "is_deleted", isDeleted });
            return list;
        }
    }
}
