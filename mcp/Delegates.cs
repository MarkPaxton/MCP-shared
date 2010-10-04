using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;
using System.Drawing;

namespace mcp
{
   public delegate void VoidDelegate();
   public delegate void TextBoxDelegate(TextBox box, string text, int linesToShow);
   public delegate void TextDelegate(string text);
   public delegate void LabelTextDelegate(Label label, string text);
   public delegate void BoolDelegate(bool boo);
   public delegate void ButtonBoolDelegate(Button button, bool boo);
   public delegate void ButtonStringBoolDelegate(Button button, string str, bool boo);
   public delegate void BitmapDelegate(Bitmap bmp);
   public delegate void DialogResultDelegeate(DialogResult result);
   public delegate void FourDoubleDelegate(double data1, double data2, double data3, double data4);
   public delegate void GuidDelegate(Guid id);
   public delegate void FormDelegate(Form form);
}
