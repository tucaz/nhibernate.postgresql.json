using Newtonsoft.Json;
using NHibernate.Engine;
using NHibernate.Type;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data.Common;

namespace nhibernate.postgresql.json
{
    [Serializable]
    public class JsonType<TSerializable> : MutableType
    {
        private readonly Type serializableClass;

        public JsonType() : base(new JsonSqlType())
        {
            serializableClass = typeof(TSerializable);
        }

        public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (!(cmd is NpgsqlCommand))
            {
                throw new ArgumentOutOfRangeException(nameof(cmd), $"Can't handle cmd {cmd.GetType().ToString()}. This is only ready for \"NpgsqlCommand\".");
            }

            var parameter = cmd.Parameters[index] as NpgsqlParameter;
            parameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
            parameter.Value = value;
        }

        public override object Get(DbDataReader rs, string name, ISessionImplementor session)
        {
            if (rs is NpgsqlDataReader)
            {
                var value = rs.GetFieldValue<TSerializable>(0);
                return value;
            }

            throw new ArgumentOutOfRangeException(nameof(rs), $"Can't handle DbDataReader {rs.GetType().ToString()}. This is only ready for \"NpgsqlDataReader\".");
        }

        public override object Get(DbDataReader rs, int index, ISessionImplementor session)
        {
            if (rs is NpgsqlDataReader)
            {
                var value = rs.GetFieldValue<TSerializable>(0);
                return value;
            }

            throw new ArgumentOutOfRangeException(nameof(rs), $"Can't handle DbDataReader {rs.GetType().ToString()}. This is only ready for \"NpgsqlDataReader\".");
        }

        public override Type ReturnedClass => serializableClass;

        public override bool IsEqual(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null | y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public override int GetHashCode(object x, ISessionFactoryImplementor factory)
        {
            return x.GetHashCode();
        }

        public static string Alias => string.Concat("json_", typeof(TSerializable).Name);

        public override string Name => Alias;

        public override object DeepCopyNotNull(object value)
        {
            return Deserialize(Serialize(value));
        }

        private string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                throw new SerializationException("Could not serialize a serializable property: ", e);
            }
        }

        public object Deserialize(string dbValue)
        {
            try
            {
                return JsonConvert.DeserializeObject(dbValue, serializableClass);
            }
            catch (Exception e)
            {
                throw new SerializationException("Could not deserialize a serializable property: ", e);
            }
        }

        public override object Assemble(object cached, ISessionImplementor session, object owner)
        {
            return (cached == null) ? null : Deserialize((string)cached);
        }

        public override object Disassemble(object value, ISessionImplementor session, object owner)
        {
            return (value == null) ? null : Serialize(value);
        }
    }
}
