using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace WpfCrazyZoo.Data
{
    public static class ZooDb
    {
        public static void EnsureCreated()
        {
            var dataDir = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (string.IsNullOrWhiteSpace(dataDir))
            {
                dataDir = AppDomain.CurrentDomain.BaseDirectory;
                AppDomain.CurrentDomain.SetData("DataDirectory", dataDir);
            }

            var dbPath = Path.Combine(dataDir, "ZooDatabase.mdf");
            if (!File.Exists(dbPath))
            {
                using (var master = new SqlConnection(ConfigurationManager.ConnectionStrings["ZooMaster"].ConnectionString))
                {
                    master.Open();
                    using (var cmd = master.CreateCommand())
                    {
                        var safePath = dbPath.Replace("'", "''");
                        cmd.CommandText = "CREATE DATABASE [ZooDatabase] ON (NAME = N'ZooDatabase', FILENAME = N'" + safePath + "')";
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["ZooDb"].ConnectionString))
            {
                db.Open();

                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText =
@"IF OBJECT_ID(N'dbo.Animals') IS NULL
BEGIN
  CREATE TABLE dbo.Animals(
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    Kind INT NOT NULL
  )
END";
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText =
@"IF OBJECT_ID(N'dbo.Enclosures') IS NULL
BEGIN
  CREATE TABLE dbo.Enclosures(
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
  )
END";
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText =
@"IF OBJECT_ID(N'dbo.EnclosureAnimals') IS NULL
BEGIN
  CREATE TABLE dbo.EnclosureAnimals(
    EnclosureId INT NOT NULL,
    AnimalId INT NOT NULL,
    CONSTRAINT PK_EnclosureAnimals PRIMARY KEY (EnclosureId, AnimalId),
    CONSTRAINT FK_EA_Enclosures FOREIGN KEY(EnclosureId) REFERENCES dbo.Enclosures(Id) ON DELETE CASCADE,
    CONSTRAINT FK_EA_Animals FOREIGN KEY(AnimalId) REFERENCES dbo.Animals(Id) ON DELETE CASCADE
  )
END";
                    cmd.ExecuteNonQuery();
                }

                EnsureMainEnclosure(db);
            }
        }

        private static void EnsureMainEnclosure(SqlConnection db)
        {
            using (var check = db.CreateCommand())
            {
                check.CommandText = "SELECT Id FROM dbo.Enclosures WHERE Name=N'Main'";
                var id = check.ExecuteScalar();
                if (id == null || id == DBNull.Value)
                {
                    using (var ins = db.CreateCommand())
                    {
                        ins.CommandText = "INSERT INTO dbo.Enclosures(Name) VALUES(N'Main')";
                        ins.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
