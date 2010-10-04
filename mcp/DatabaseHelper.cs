using System;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data.SQLite;

namespace mcp
{
   #region helper methods
   public static class SQLHelper
   {
      public static string DateFormatString = "yyyy-MM-dd H:mm:ss";

      public static string QuoteSQL(string st)
      {
         return Regex.Replace(st, "'", "''");
      }

      public static string NullOrFormatAndAddQuotes(string value)
      {
         string return_value = "";
         if ((value == null) || (value == ""))
         {
            return_value = "NULL";
         }
         else
         {
            return_value = FormatAndAddQuotes(value);
         }
         return return_value;
      }

      public static object ValueIfNull(SQLiteDataReader result, int index, object defaultValue)
      {
         object valueToReturn = null;
         if (result.IsDBNull(index))
         {
            valueToReturn = defaultValue;
         }
         else
         {
            valueToReturn = result.GetValue(index);
         }
         return valueToReturn;
      }

      public static string FormatAndAddQuotes(string value)
      {
         return "'" + SQLHelper.QuoteSQL(value) + "'";
      }

      public static string GenerateColumnsAndValues(Hashtable data)
      {
         string columns = "";
         string values = "";

         int i = 1;
         foreach (string key in data.Keys)
         {
            columns += String.Format("{0}", key);
            values += String.Format("{0}", data[key]);
            if (i < data.Count)
            {
               columns += ", ";
               values += ", ";
            }
            i++;
         }
         return String.Format("({0}) VALUES ({1})", columns, values);
      }

      public static void DeleteIDFromTable(Guid id, string tableName, SQLiteConnection database)
      {
         string query = String.Format("DELETE FROM {0} WHERE id='{1}'", tableName, id);
         Debug.WriteLine(query);

         lock (database)
         {
            using (SQLiteCommand command = new SQLiteCommand(query, database))
            {
               if (command.Connection.State == ConnectionState.Closed)
               {
                  command.Connection.Open();
               }
               command.ExecuteNonQuery();
            }
            database.Close();
         }
      }

      public static Guid GetLastIdentity(SQLiteConnection database)
      {
         Guid id = Guid.Empty;
         string query = "SELECT @@IDENTITY";
         Debug.WriteLine(query);

         lock (database)
         {
            using (SQLiteCommand command = new SQLiteCommand(query, database))
            {
               if (command.Connection.State == ConnectionState.Closed)
               {
                  command.Connection.Open();
               }
               //Once the session is saved, get the ID from the DB and store it
               SQLiteDataReader resultReader = command.ExecuteReader();
               if (resultReader.Read())
               {
                  id = resultReader.GetGuid(0);
               }
               else
               {
                  id = Guid.Empty;
               }
               resultReader.Close();
               Debug.WriteLine(String.Format("@@IDENTITY: {0}", id));
            }
            database.Close();
         }
         return id;
      }

      public static SQLiteConnection ConnectToSQL(string connectionString)
      {
         return new SQLiteConnection(connectionString);
      }

      public static void ExecuteQuery(SQLiteConnection connection, string txtQuery)
      {
         lock (connection)
         {
            if (connection.State != ConnectionState.Open)
            {
               connection.Open();
            }
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = txtQuery;
            Debug.WriteLine(txtQuery);
            command.ExecuteNonQuery();
            connection.Close();
         }
      }

   }
   #endregion
}