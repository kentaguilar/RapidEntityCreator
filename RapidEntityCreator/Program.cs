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
            Console.WriteLine("ZioLib Class Creator v1.0");
            Console.WriteLine("Please answer prompts below to start.\n");

            Console.Write("DB Type(mysql/sqlserver): ");
            string dbType = Console.ReadLine();

            Console.Write("Namespace Name: ");
            string userNamespace = Console.ReadLine();

            Console.Write("Class Name: ");
            string userClass = Console.ReadLine();

            Console.Write("Table Name: ");
            string tableName = Console.ReadLine();

            string propertyName = string.Empty;
            string propertyType = string.Empty;
            string columnName = string.Empty;
            string primaryKey = string.Empty;
            string image = string.Empty;

            var propertyList = new List<Property>();

            bool isDone = false;
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

        public static void CreateClass(string dbType, string userNamespace, string userClass, string tableName, IEnumerable<Property> propertList)
        {
            string filePath = string.Format(@"{0}\{1}.cs", Environment.CurrentDirectory, userClass);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Create the file.
                using (FileStream fs = File.Create(filePath))
                {
                    string content = "using System;\n";
                    content += "using System.Collections.Generic;\n";
                    content += "using System.Linq;\n";
                    content += "using System.Text;\n";
                    content += "using System.Threading.Tasks;\n";
                    content += "using ZioLib.Data;\n";
                    content += "using ZioLib.Helpers;\n\n";
                    content += string.Format("namespace {0}\n", userNamespace);
                    content += "{\n";
                    content += "\t[TableName(\"" + tableName + "\")]\n";
                    content += string.Format("\tpublic class {0}\n", userClass);
                    content += "\t{\n";

                    int recordCount = 1;

                    content += "\t\t#region Data Access Properties\n";
                    foreach (var property in propertList)
                    {
                        if (property.IsPrimaryKey == "true")
                        {
                            content += "\t\t[IsPrimaryKey(" + property.IsPrimaryKey + ")]\n";
                        }

                        if (property.IsImage == "true")
                        {
                            content += "\t\t[IsImage(" + property.IsPrimaryKey + ")]\n";
                        }

                        content += "\t\t[ColumnName(\"" + property.ColumnName + "\")]\n";
                        content += "\t\tpublic " + property.PropertyType + " " + property.PropertyName + " { get; set; }\n";

                        if (recordCount < propertList.Count())
                        {
                            content += "\n";
                        }

                        recordCount++;
                    }
                    content += "\t\t#endregion\n\n";

                    //constructor                    
                    content += string.Format("\t\tprivate IDBEntity<{0}> dbEntity = null;\n\n", userClass);
                    content += "\t\t//Constructor\n";
                    content += "\t\tpublic " + userClass + "()\n";
                    content += "\t\t{\n";
                    content += string.Format("\t\t\tdbEntity = new {0}<{1}>();\n", dbType, userClass);
                    content += "\t\t}\n\n";

                    //data retrieval
                    content += "\t\t//Required: Data Retrieval\n";
                    content += "\t\tpublic " + userClass + "(Dictionary<string, object> args)\n";
                    content += "\t\t{\n";

                    foreach (var property in propertList)
                    {
                        content += "\t\t\t";
                        switch (property.PropertyType)
                        {
                            case "int":
                                content += property.PropertyName + " = Convert.ToInt32(args[\"" + property.ColumnName + "\"].ToString());";
                                break;
                            case "decimal":
                                content += property.PropertyName + " = Convert.ToDecimal(args[\"" + property.ColumnName + "\"].ToString());";
                                break;
                            case "double":
                                content += property.PropertyName + " = Convert.ToDouble(args[\"" + property.ColumnName + "\"].ToString());";
                                break;
                            case "datetime":
                                content += property.PropertyName + " = Convert.ToDateTime(args[\"" + property.ColumnName + "\"].ToString());";
                                break;
                            case "char":
                                content += property.PropertyName + " = Convert.ToChar(args[\"" + property.ColumnName + "\"].ToString());";
                                break;
                            case "byte":
                                content += property.PropertyName + " = Convert.ToByte(args[\"" + property.ColumnName + "\"].ToString());";
                                break;
                            default:
                                content += property.PropertyName + " = args[\"" + property.ColumnName + "\"].ToString();";
                                break;
                        }
                        content += "\n";
                    }

                    content += "\t\t}\n\n";

                    content += "\t\t#region Class Methods\n\n";
                    content += "\t\t#endregion\n";

                    content += "\t}\n";
                    content += "}";

                    Byte[] info = new UTF8Encoding(true).GetBytes(content);
                    fs.Write(info, 0, info.Length);
                }

                Console.WriteLine("\nEntity successfully created");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
