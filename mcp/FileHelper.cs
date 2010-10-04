using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace mcp
{
   public static class FileHelper
   {
      /// <summary>
      /// Chack for and create a given directory if it doesn't exist
      /// </summary>
      /// <param name="path">The path for the directory to be checked</param>
      /// <returns>True if the folder was created</returns>
      public static bool CheckAndCreateFolder(string path)
      {
         bool wasCreated = false;

         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
            wasCreated = true;
         }
         return wasCreated;
      }

      /// <summary>
      /// Checks a given location for the existance of a file and copies it there if not preasent.
      /// n.b. it does not check the contents of the file, just that a so-named file exists in that location.
      /// </summary>
      /// <param name="source">The location of the source file</param>
      /// <param name="path">The location to check exists</param>
      /// <returns>True if file was copied to location</returns>
      public static bool CheckAndCopyFile(string source, string path)
      {
         bool wasCopied = false;
         if (!File.Exists(path))
         {
            File.Copy(source, path);
            wasCopied = true;
         }
         return wasCopied;
      }
   }
}
