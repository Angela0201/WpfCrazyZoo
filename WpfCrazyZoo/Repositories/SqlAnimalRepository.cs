using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using WpfCrazyZoo.Models;


namespace WpfCrazyZoo.Repositories
{
    public class SqlAnimalRepository : IRepository<Animal>
    {
        private readonly string cs;

        public SqlAnimalRepository(string connectionString)
        {
            cs = connectionString;
        }

        public void Add(Animal item)
        {
            int id;
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO dbo.Animals(Name,Age,Kind) OUTPUT INSERTED.Id VALUES(@n,@a,@k)";
                    cmd.Parameters.Add("@n", SqlDbType.NVarChar, 100).Value = item.Name;
                    cmd.Parameters.Add("@a", SqlDbType.Int).Value = item.Age;
                    cmd.Parameters.Add("@k", SqlDbType.Int).Value = (int)item.Kind;
                    id = (int)cmd.ExecuteScalar();
                }

                int enclosureId = GetMainEnclosureId(cn);
                using (var map = cn.CreateCommand())
                {
                    map.CommandText = "INSERT INTO dbo.EnclosureAnimals(EnclosureId,AnimalId) VALUES(@e,@i)";
                    map.Parameters.Add("@e", SqlDbType.Int).Value = enclosureId;
                    map.Parameters.Add("@i", SqlDbType.Int).Value = id;
                    map.ExecuteNonQuery();
                }
            }
        }

        public void Remove(Animal item)
        {
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "DELETE a FROM dbo.Animals a WHERE a.Name=@n AND a.Age=@a AND a.Kind=@k";
                    cmd.Parameters.Add("@n", SqlDbType.NVarChar, 100).Value = item.Name;
                    cmd.Parameters.Add("@a", SqlDbType.Int).Value = item.Age;
                    cmd.Parameters.Add("@k", SqlDbType.Int).Value = (int)item.Kind;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Animal> GetAll()
        {
            var list = new List<Animal>();
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name,Age,Kind FROM dbo.Animals ORDER BY Id";
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var name = r.GetString(0);
                            var age = r.GetInt32(1);
                            var kind = r.GetInt32(2);
                            list.Add(CreateAnimal(name, age, kind));
                        }
                    }
                }
            }
            return list;
        }

        public Animal Find(Func<Animal, bool> predicate)
        {
            foreach (var a in GetAll())
                if (predicate(a)) return a;
            return null;
        }

        private static Animal CreateAnimal(string name, int age, int kind)
        {
            if (kind == (int)AnimalKind.Cat) return new Cat(name, age);
            if (kind == (int)AnimalKind.Dog) return new Dog(name, age);
            return new Bird(name, age);
        }

        private static int GetMainEnclosureId(SqlConnection cn)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id FROM dbo.Enclosures WHERE Name=N'Main'";
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
