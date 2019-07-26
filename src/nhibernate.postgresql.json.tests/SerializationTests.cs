using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NpgsqlTypes;
using NUnit.Framework;

namespace nhibernate.postgresql.json.tests
{
    public class SerializationTests
    {
        [Test]
        public void NpgsqlExtendedSqlType()
        {
            var sut = new NpgsqlExtendedSqlType(DbType.Binary, NpgsqlDbType.Jsonb);
            var mem = new MemoryStream();
            var b = new BinaryFormatter();
            
            try
            {
                b.Serialize(mem, sut);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}