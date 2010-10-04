using System;
using System.Windows.Forms;

namespace SensorShare
{
   public class NumString
   {
      private int num;

      public NumString(int num)
      {
         this.num = num;
      }

      public int Num
      {
         get { return this.num; }
      }

      public string NumStr
      {
         get { return String.Format("{0}", this.num); }
      }

   }
}


