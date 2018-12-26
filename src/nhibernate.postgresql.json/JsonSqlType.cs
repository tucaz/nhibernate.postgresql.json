using NHibernate.SqlTypes;
using System;
using System.Data;

namespace nhibernate.postgresql.json
{
    [Serializable]
    public class JsonSqlType : SqlType
    {
        public JsonSqlType() : base(DbType.Object)
        {
        }
    }
}
