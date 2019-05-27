[![Build status](https://ci.appveyor.com/api/projects/status/qq2ldw5bfoe2f148?svg=true)](https://ci.appveyor.com/project/tucaz/nhibernate-postgresql-json)

# nhibernate.postgresql.json

NHibernate custom type to handle PostgreSQL JSON and JSONB column types

# Installation

This library depends on the following NuGet packages:

- Direct
  - [NHibernate](https://www.nuget.org/packages/NHibernate)
  - [NpgSql.Json.Net](https://www.nuget.org/packages/Npgsql.Json.NET/) 
- Indirect
  - [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
  - [NpgSql](https://www.nuget.org/packages/Npgsql/)

These will be installed as dependencies along with this page.

To install this download package `nhibernate.postgresql.json` or run:

```
Install-Package nhibernate.postgresql.json
```

## Compatibility

This library has been tested with .NET Core 2.1 and NHibernate 5.x.

# Configuration

All this package does is offer an NHibernate custom type that you can use to handle json/jsonb PostgreSql data types. It relies on the configuration provided by NpgSql.Json.Net.

The way it works is by telling the extension which .NET types you want it to handle for you. This is done in the `Startup` class:

```csharp
NpgsqlConnection.GlobalTypeMapper.UseJsonNet(new[] { typeof(AnotherType) });
```

It is important to note that you must specify each of the types that you want the library to handle for you and not their parents.

# Usage

After the configuration is complete, you can just use it in your mapping as a custom type:

```csharp
Map(x => x.PropertyWithMyType).CustomSqlType("jsonb").CustomType<JsonType<AnotherType>>().Column("db_column");
```

It also works with native CLR types such as `Dictionary<string,string>` which is perfect to store unstructured data. In my particular case, I use it to store raw data from web requests along with parsed data.

```csharp
Map(x => x.Raw).CustomSqlType("jsonb").CustomType<JsonType<Dictionary<string,string>>>().Column("raw");
```

# Sample

A more complete sample can be found below:

**Startup.cs**

```csharp
public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            NpgsqlConnection.GlobalTypeMapper.UseJsonNet(new[] { typeof(MyType) });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
```

**MyEntity.cs**

```csharp
public class AnotherType
{
	public AnotherType() {}
	
	public virtual string AnotherProperty {get;set;}
}

public class MyType
{
	public MyType() {}

	public virtual string Name { get; set; }
	public virtual int Age { get; set; }
	public virtual AnotherType Other { get; set; }	
}

public class MyTypeMap : ClassMap<AnotherType>
{

	public MyType()
	{
		Table("my_table");

		Map(x => x.Name).Column("name");
		Map(x => x.Age).Column("age");
		Map(x => x.Other).CustomSqlType("jsonb").CustomType<JsonType<Transaction>>().Column("other").Not.Nullable();
	}
}
```
