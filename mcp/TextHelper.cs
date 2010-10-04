using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;

namespace mcp
{
   public static class TextHelper
   {
      public static string lineSuffix = "\r\n";

      public static string tailString(string toTail, int lines)
      {
         string[] textArray = toTail.Split('\n');
         ArrayList textList = new ArrayList(textArray);
         string returnText = "";

         if (textList.Count > lines)
         {
            textList.RemoveRange(0, (textList.Count - lines));
            textArray = (string[])textList.ToArray(typeof(string));
            returnText = String.Join("\n", textArray);
         }
         else
         {
            returnText = toTail;
         }
         return returnText;
      }

      public static void UpdateTextBox(TextBox box, string displayText, int linesToshow)
      {
         if (box != null)
         {
            box.Text = TextHelper.tailString(box.Text, linesToshow) + displayText + TextHelper.lineSuffix;
            TextHelper.PositionTextDisplay(box);
         }
      }

      public static void UpdateLabel(Label label, string text)
      {
         if (label != null)
         {
            label.Text = text;
         }
      }


      /// <summary>
      /// Position the carat at the end of the text to keep the last bit into view
      /// </summary>
      public static void PositionTextDisplay(TextBox box)
      {
         int length = box.Text.Length;
         if (Regex.IsMatch(box.Text, "(\r\n)$"))
         {
            length -= 2;
         }
         box.Select(length, 0);
         box.ScrollToCaret();
      }

      public static string DeStreamString(MemoryStream ms, Encoding encoding)
      {
         byte[] stringLengthBytes = new byte[sizeof(int)];
         ms.Read(stringLengthBytes, 0, sizeof(int));
         int stringLength = BitConverter.ToInt32(stringLengthBytes, 0);
         byte[] stringBytes = new byte[stringLength];
         ms.Read(stringBytes, 0, stringLength);
         string stringToReturn = encoding.GetString(stringBytes, 0, stringLength);
         return stringToReturn;
      }

      public static void StreamString(MemoryStream ms, string item , Encoding encoding)
      {
         byte[] itemBytes = encoding.GetBytes(item);
         int itemBytesLength = itemBytes.Length;
         byte[] lengthBytes = BitConverter.GetBytes(itemBytesLength);

         ms.Write(lengthBytes, 0, sizeof(int));
         ms.Write(itemBytes, 0, itemBytesLength);
      }
   }
}
