using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Microsoft.AcomObjects
{
    public class AcomArticleProperty
    {
        public int StartingAt { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class AcomArticle
    {
        const string nodeEnd = "/>";

        // Local file name of article
        public string FileName { get; protected set; }

        // ACOM Properties fields
        public string PageTitle { get; protected set; }
        public string Description { get; protected set; }
        public string Services { get; protected set; }
        public string DocumentationCenter { get; protected set; }
        public string Authors { get; protected set; }
        public string Manager { get; protected set; }
        public string Editor { get; protected set; }

        // MS Tags fields
        public string Service { get; protected set; }
        public string Workload { get; protected set; }
        public string TargetPlatform { get; protected set; }
        public string Devlang { get; protected set; }
        public string Topic { get; protected set; }
        public DateTime PubDate { get; protected set; }
        public string Author { get; protected set; }

        // Article content
        public string Body { get; set; }

        public AcomArticle(string fileName)
        {
            this.FileName = fileName;
            LoadArticle();
        }

        void LoadArticle()
        {
            using (StreamReader stream = new StreamReader(this.FileName))
            {
                String fileContents = stream.ReadToEnd();

                // Code assumes that Properties section ...
                int propsDelimiterIndex = fileContents.IndexOf(nodeEnd);
                if (propsDelimiterIndex >= 0)
                {
                    // ... comes before Tags section ...
                    int tagsDelimiterIndex = fileContents.IndexOf(nodeEnd, propsDelimiterIndex + nodeEnd.Length);
                    if (tagsDelimiterIndex >= 0)
                    {
                        string propsSection = fileContents.Substring(0, propsDelimiterIndex + nodeEnd.Length);
                        string tagsSection = fileContents.Substring(propsDelimiterIndex + nodeEnd.Length, tagsDelimiterIndex - propsDelimiterIndex);

                        // ... which precedes the body of the article.
                        this.Body = fileContents.Substring(tagsDelimiterIndex + nodeEnd.Length);

                        // Now that we know where the Properties and Tags sections are, 
                        // get the values from the respective sections.
                        GetProperties(propsSection);
                        GetTags(tagsSection);
                    }
                }
            }
        }

        void GetProperties(string propsSection)
        {
            this.PageTitle = GetValue<string>(propsSection, "pageTitle");
            this.Description = GetValue<string>(propsSection, "description");
            this.Services = GetValue<string>(propsSection, "services");
            this.DocumentationCenter = GetValue<string>(propsSection, "documentationCenter");
            this.Authors = GetValue<string>(propsSection, "authors");
            this.Manager = GetValue<string>(propsSection, "manager");
            this.Editor = GetValue<string>(propsSection, "editor");
        }
        void GetTags(string tagsSection)
        {
            this.Service = GetValue<string>(tagsSection, "ms.service");
            this.Workload = GetValue<string>(tagsSection, "ms.workload");
            this.TargetPlatform = GetValue<string>(tagsSection, "ms.tgt_pltfrm");
            this.Devlang = GetValue<string>(tagsSection, "ms.devlang");
            this.Topic = GetValue<string>(tagsSection, "ms.topic");
            this.PubDate = GetValue<DateTime>(tagsSection, "ms.date");
            this.Author = GetValue<string>(tagsSection, "ms.author");
        }
 
        T GetValue<T>(string source, string key)
        {
            T value = default(T);

            string searchFor = string.Format("{0}=", key);

            int idxStart = source.IndexOf(searchFor);
            if (idxStart > -1)
            {
                idxStart = idxStart + searchFor.Length + 1;
                if (idxStart < source.Length)
                {
                    int idxStop = source.IndexOf('=', idxStart);
                    if (idxStop == -1)
                    {
                        idxStop = source.Length - 1;
                    }
                    string temp = source.Substring(idxStart, idxStop - idxStart);
                    int idxEndingQuote = temp.LastIndexOf('\"');
                    if (idxEndingQuote > -1)
                    {
                        temp = temp.Substring(0, idxEndingQuote);
                        value = (T)Convert.ChangeType(temp, typeof(T));
                    }
                }
            }

            return value;
        }
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.AppendFormat("Filename = {0}\n\n", this.FileName);

            ret.AppendFormat("PageTitle = {0}\nDescription = {1}\nServices = {2}\nDocumentationCenter = {3}\nAuthors = {4}\nManager = {5}\nEditor = {6}\n\n",
                this.PageTitle,
                this.Description,
                this.Services,
                this.DocumentationCenter,
                this.Authors,
                this.Manager,
                this.Editor);

            ret.AppendFormat("Service = {0}\nWorkload = {1}\nTgt_pltfrm = {2}\nDevlang = {3}\nTopic = {4}\nDate = {5}\nAuthor = {6}\n\n",
                            this.Service,
                            this.Workload,
                            this.TargetPlatform,
                            this.Devlang,
                            this.Topic,
                            this.PubDate,
                            this.Author);

            ret.AppendFormat("Content = {0}", this.Body);

            return ret.ToString();
        }
    }
}