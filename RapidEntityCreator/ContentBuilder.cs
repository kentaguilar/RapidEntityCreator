using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidEntityCreator
{
    class ContentBuilder
    {
        public void MakeHeader(ref StringBuilder content, string userNamespace,
            string tableName, string userClass)
        {
            content.Append("using System;\n");
            content.Append("using System.Collections.Generic;\n");
            content.Append("using System.Linq;\n");
            content.Append("using System.Text;\n");
            content.Append("using System.Threading.Tasks;\n");
            content.Append("using RapidORM.Data;\n");
            content.Append("using RapidORM.Helpers;\n");
            content.Append("using RapidORM.Attributes;\n");
            content.Append("using RapidORM.Interfaces;\n");
            content.Append("using RapidORM.Common;\n\n");

            //--- Depending on your DB, you can add the following ---
            //content.Append("using RapidORM.Client.MySQL;\n\n");
            //content.Append("using RapidORM.Client.SQL;\n\n");

            content.AppendFormat("namespace {0}\n", userNamespace);
            content.Append("{\n");
            content.AppendFormat("\t[TableName(\"{0}\")]\n", tableName);
            content.AppendFormat("\tpublic class {0}\n", userClass);
            content.Append("\t{\n");
        }

        public void MakeConstructor(ref StringBuilder content, string userClass, string dbType)
        {
            content.AppendFormat("\t\tprivate IDBEntity<{0}> dbEntity = null;\n\n", userClass);
            content.Append("\t\t//Constructor\n");
            content.AppendFormat("\t\tpublic {0}()\n", userClass);
            content.Append("\t\t{\n");
            content.AppendFormat("\t\t\tdbEntity = new {0}<{1}>();\n", dbType, userClass);
            content.Append("\t\t}\n\n");
        }

        public void MakeProperties(ref StringBuilder content, IEnumerable<Property> propertList)
        {
            int recordCount = 1;

            content.Append("\t\t#region Properties\n");
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
                content.Append("\t\tpublic " + property.PropertyType + " " + property.PropertyName + " { get; set; }\n");

                if (recordCount < propertList.Count())
                {
                    content.Append("\n");
                }

                recordCount++;
            }
            content.Append("\t\t#endregion\n\n");
        }

        public void MakeDataRetrieval(ref StringBuilder content, string userClass, 
            IEnumerable<Property> propertyList)
        {
            content.Append("\t\t//Required: Data Retrieval\n");
            content.AppendFormat("\t\tpublic {0}(Dictionary<string, object> args)\n", userClass);
            content.Append("\t\t{\n");

            foreach (var property in propertyList)
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
        }

        public void MakeFooter(ref StringBuilder content)
        {
            content.Append("\t\t}\n\n");

            content.Append("\t\t#region Class Methods\n\n");
            content.Append("\t\t#endregion\n");

            content.Append("\t}\n");
            content.Append("}");
        }
    }
}
