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
            string propertyName = string.Empty;
            string propertyType = string.Empty;
            string columnName = string.Empty;
            string primaryKey = string.Empty;
            string image = string.Empty;
            var propertyList = new List<Property>();
            bool isDone = false;

            Console.WriteLine("RapidORM Class Creator v1.0");
            Console.WriteLine("Please answer prompts below to start.\n");

            Console.Write("DB Type(mysql/sqlserver): ");
            string dbType = Console.ReadLine();

            Console.Write("Namespace Name: ");
            string userNamespace = Console.ReadLine();

            Console.Write("Class Name: ");
            string userClass = Console.ReadLine();

            Console.Write("Table Name: ");
            string tableName = Console.ReadLine();
          
            while (!isDone)
            {
                Console.WriteLine("\n--------------------");

                Console.Write("Property Type: ");
                propertyType = Console.ReadLine();

                Console.Write("Property Name: ");
                propertyName = Console.ReadLine();

                Console.Write("Table Column Name: ");
                columnName = Console.ReadLine();

                Console.Write("Is Primary Key?: ");
                primaryKey = Console.ReadLine();

                Console.Write("Is Image?(for blob): ");
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

        public static void CreateClass(string dbType, string userNamespace, string userClass, string tableName, 
            IEnumerable<Property> propertList)
        {
            StringBuilder content = new StringBuilder();
            string filePath = string.Format(@"{0}\{1}.cs", Environment.CurrentDirectory, userClass);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                using (FileStream fs = File.Create(filePath))
                {
                    content.Append("using System;\n");
                    content.Append("using System.Collections.Generic;\n");
                    content.Append("using System.Linq;\n");
                    content.Append("using System.Text;\n");
                    content.Append("using System.Threading.Tasks;\n");
                    content.Append("using RapidORM.Data;\n");
                    content.Append("using RapidORM.Helpers;\n\n");
                    content.Append("using RapidORM.Attributes;\n\n");
                    content.Append("using RapidORM.Interfaces;\n\n");
                    content.Append("using RapidORM.Common;\n\n");

                    //--- Depending on your DB, you can add the following ---
                    //content.Append("using RapidORM.Client.MySQL;\n\n");
                    //content.Append("using RapidORM.Client.SQL;\n\n");

                    content.AppendFormat("namespace {0}\n", userNamespace);
                    content.Append("{\n");
                    content.AppendFormat("\t[TableName(\"{0}\")]\n", tableName);
                    content.AppendFormat("\tpublic class {0}\n", userClass);
                    content.Append("\t{\n");

                    int recordCount = 1;

                    content.Append("\t\t#region Data Access Properties\n");
                    foreach (var property in propertList)
                    {
                        if (property.IsPrimaryKey == "true")
                        {                            
                            content.AppendFormat("\t\t[IsPrimaryKey({0})]\n", property.IsPrimaryKey);
                        }

                        if (property.IsImage == "true")
                        {
                            content.AppendFormat("\t\t[IsImage({0})]\n", property.IsPrimaryKey);
                        }
                                                
                        content.AppendFormat("\t\t[ColumnName(\"{0}\")]\n", property.ColumnName);                        
                        content.AppendFormat("\t\tpublic {0} {1} { get; set; }\n", 
                            property.PropertyType,
                            property.PropertyName);

                        if (recordCount < propertList.Count())
                        {                            
                            content.Append("\n");
                        }

                        recordCount++;
                    }                    
                    content.Append("\t\t#endregion\n\n");

                    //constructor                                        
                    content.AppendFormat("\t\tprivate IDBEntity<{0}> dbEntity = null;\n\n", userClass);                    
                    content.Append("\t\t//Constructor\n");                    
                    content.AppendFormat("\t\tpublic {0}()\n", userClass);                    
                    content.Append("\t\t{\n");                    
                    content.AppendFormat("\t\t\tdbEntity = new {0}<{1}>();\n", dbType, userClass);                    
                    content.Append("\t\t}\n\n");

                    //data retrieval                    
                    content.Append("\t\t//Required: Data Retrieval\n");                    
                    content.AppendFormat("\t\tpublic {0}(Dictionary<string, object> args)\n", userClass);                    
                    content.Append("\t\t{\n");

                    foreach (var property in propertList)
                    {                        
                        content.Append("\t\t\t");
                        switch (property.PropertyType)
                        {
                            case "int":                                
                                content.AppendFormat("{0} = Convert.ToInt32(args[\"{1}\"].ToString());",
                                    property.PropertyName, property.ColumnName);
                                break;
                            case "decimal":                                
                                content.AppendFormat("{0} = Convert.ToDecimal(args[\"{1}\"].ToString());",
                                    property.PropertyName, property.ColumnName);
                                break;
                            case "double":                                
                                content.AppendFormat("{0} = Convert.ToDouble(args[\"1\"].ToString());",
                                    property.PropertyName, property.ColumnName);
                                break;
                            case "datetime":                                
                                content.AppendFormat("{0} = Convert.ToDateTime(args[\"{1}\"].ToString());",
                                    property.PropertyName, property.ColumnName);
                                break;
                            case "char":                                
                                content.AppendFormat("{0} = Convert.ToChar(args[\"{1}\"].ToString());",
                                    property.PropertyName, property.ColumnName);
                                break;
                            case "byte":                                
                                content.AppendFormat("{0} = Convert.ToByte(args[\"{1}\"].ToString());",
                                    property.PropertyName, property.ColumnName);
                                break;
                            default:                                
                                content.AppendFormat("{0} = args[\"{1}\"].ToString();",
                                    property.PropertyName, property.ColumnName);
                                break;
                        }                        
                        content.Append("\n");
                    }
                                        
                    content.Append("\t\t}\n\n");

                    content.Append("\t\t#region Class Methods\n\n");                    
                    content.Append("\t\t#endregion\n");

                    content.Append("\t}\n");                    
                    content.Append("}");

                    Byte[] info = new UTF8Encoding(true).GetBytes(content.ToString());
                    fs.Write(info, 0, info.Length);
                }

                Console.WriteLine("\Model successfully created");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
