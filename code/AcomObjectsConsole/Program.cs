using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Microsoft.AcomObjects;

namespace Microsoft.AcomObjectsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //ProcessFile(@"C:\Users\tomar\Source\Repos\azure-content-pr\articles\vpn-gateway\vpn-gateway-about-forced-tunneling.md");
                ProcessDirectory(@"c:\users\tomar\source\repos\azure-content-pr\articles");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey(true);
            }
        }

        static void ProcessFile(string fileName)
        {
            AcomArticle article = new AcomArticle(fileName);
            Console.WriteLine(article);
        }

        static void ProcessDirectory(string dirName)
        {
            try
            {
                // Process current directory
                foreach (string fileName in Directory.GetFiles(dirName, "*.md"))
                {
                    Console.WriteLine("Processing {0}", fileName);
                    AcomArticle article = new AcomArticle(fileName);
                    Console.WriteLine(article.Author);
                }

                foreach (string subDir in Directory.GetDirectories(dirName))
                {
                    ProcessDirectory(subDir);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }
}
