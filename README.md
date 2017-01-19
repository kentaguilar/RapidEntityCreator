# Rapid Entity Creator

Tool to help RapidORM users to generate a model class easily.
Simply execute this program and automatically you'll have your formatted class model instantly. You don't need to create a class manually and configure it - that's error prone.

## Getting Started

- You can run the program and respond to the displayed prompts. Choices are available on selected prompts to guide you. Also, a default value will be created should an "Enter" key is invoked.
- The screen should look like this.

<img src="http://deepmirage.com/git/rapidentitycreator.png" alt="RapidORM" width="400px"/>

- Once all the information is provided. A neatly organized class file will be generated on your projects execution folder or on the location you run the RapidEntityCreator program.
- Please note that the newly created class is not instantly added on Visual Studio. You need to add it up. To do so, hit right-click on your project, then click on "Add > Existing Item" (Shift + Alt + A).

## Additional Note

You can also add RapidEntityCreator to your system's environment variables(if you're using Windows), to execute the program anywhere via the command prompt. Simply add the actual location of the RapidEntityCreator executable file to the "PATH" variable under Environment Variables.

## Sample Output

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidORM.Data;
using RapidORM.Helpers;
using RapidORM.Attributes;
using RapidORM.Interfaces;
using RapidORM.Common;

namespace MyNamespace
{
	[TableName("mytable")]
	public class MyClass
	{
		#region Properties
		[IsPrimaryKey(true)]
		[ColumnName("id")]
		public int Id { get; set; }

		[ColumnName("column1")]
		public string Column1 { get; set; }
		#endregion

		private IDBEntity<MyClass> dbEntity = null;

		//Constructor
		public MyClass()
		{
			dbEntity = new MySqlEntity<MyClass>();
		}

		//Required: Data Retrieval
		public MyClass(Dictionary<string, object> args)
		{
			Id = Convert.ToInt32(args["id"].ToString());
			Column1 = args["column1"].ToString();
		}

		#region Class Methods

		#endregion
	}
}
```
