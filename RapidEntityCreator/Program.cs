using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RapidEntityCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            ManageClassCreation();   
        }

        protected static void ManageClassCreation()
        {
            string propertyName = string.Empty;
            string propertyType = string.Empty;
            string columnName = string.Empty;
            string primaryKey = string.Empty;
            string image = string.Empty;
            var propertyList = new List<Property>();
            bool isDone = false;

            Console.WriteLine("RapidORM Class Creator v1.1");
            Console.WriteLine("Please answer prompts below to start.\n");

            Console.Write("DB Type(mysql/sql): ");
            string dbType = Console.ReadLine();

            Console.Write("Namespace Name: ");
            string userNamespace = Console.ReadLine();

            Console.Write("Class Name: ");
            string userClass = Console.ReadLine();

            Console.Write("Table Name: ");
            string tableName = Console.ReadLine();

            while (!isDone)
            {
                Console.WriteLine("\n----------------------------");

                Console.Write("Property Type(int,decimal,double,datetime,char,byte,string): ");
                propertyType = Console.ReadLine();

                Console.Write("Property Name: ");
                propertyName = Console.ReadLine();

                Console.Write("Table Column Name: ");
                columnName = Console.ReadLine();

                Console.Write("Is Primary Key(y/n)?: ");
                primaryKey = Console.ReadLine();

                Console.Write("Is Image(y/n)? - For Blob: ");
                image = Console.ReadLine();

                propertyList.Add(new Property
                {
                    PropertyName = string.IsNullOrEmpty(propertyName) ? "MyProperty" : propertyName,
                    PropertyType = string.IsNullOrEmpty(propertyType) ? "string" : propertyType,
                    ColumnName = string.IsNullOrEmpty(columnName) ? "table_column" : columnName,
                    IsPrimaryKey = (primaryKey.ToLower() == "y") ? "true" : "false",
                    IsImage = (image.ToLower() == "y") ? "true" : "false",
                });

                Console.Write("\nAdd another property? ");
                var answer = Console.ReadLine();

                if (answer.ToLower() == "n")
                {
                    isDone = true;
                }
            }

            userClass = string.IsNullOrEmpty(userClass) ? "MyClass" : userClass;

            CreateClass(dbType == "mysql" ? "MySqlEntity" : "SqlEntity",
                string.IsNullOrEmpty(userNamespace) ? "MyNamespace" : userNamespace,
                userClass,
                string.IsNullOrEmpty(tableName) ? userClass.ToLower() : tableName,
                propertyList);
        }

        protected static void CreateClass(string dbType, string userNamespace, 
            string userClass, string tableName, IEnumerable<Property> propertyList)
        {
            StringBuilder content = new StringBuilder();
            string filePath = string.Format(@"{0}\{1}.cs", Environment.CurrentDirectory, userClass);
            ContentBuilder contentBuilder = new ContentBuilder();            

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                using (FileStream fs = File.Create(filePath))
                {
                    contentBuilder.MakeHeader(ref content, userNamespace, tableName, userClass);
                    contentBuilder.MakeProperties(ref content, propertyList);
                    contentBuilder.MakeConstructor(ref content, userClass, dbType);
                    contentBuilder.MakeDataRetrieval(ref content, userClass, propertyList);
                    contentBuilder.MakeFooter(ref content);                                                           

                    Byte[] info = new UTF8Encoding(true).GetBytes(content.ToString());
                    fs.Write(info, 0, info.Length);
                }

                Console.WriteLine("\nModel successfully created");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
