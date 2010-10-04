using System.IO;
using System.Threading;

namespace mcp
{
    public class FileWriter
    {
        private string fileName;
        private string fileHeader;
        private Mutex writeMutex = new Mutex();
        /// <param name="fileName">The name of the file to be written to</param>
        public FileWriter(string fileName, string fileHeader)
        {
            this.fileName = fileName;
            this.fileHeader = fileHeader;
            this.Start();
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
        }
        /// <summary>
        /// Create new file for writing
        /// </summary>
        public void Start()
        {
            if (fileName.Length > 0)
            {
                FileStream file = new FileStream(fileName, FileMode.OpenOrCreate);
                if (file.Length == 0)
                {
                    file.Close();
                    Append(fileHeader);
                }
                else
                {
                    file.Close();
                }
            }
            

        }

        /// <param name="text">Add the string to the end of the file</param>
        public void Append(string text)
        {
            writeMutex.WaitOne(3000, false);
            try
            {
                using (StreamWriter strWriter = File.AppendText(fileName))
                {
                    strWriter.WriteLine(text);
                    strWriter.Close();
                }
            }
            finally
            {
                writeMutex.ReleaseMutex();
            }
        }
    }
}
