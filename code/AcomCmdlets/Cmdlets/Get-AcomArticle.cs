using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Xml.Linq;
using Microsoft.AcomObjects;

namespace Microsoft.AcomCmdlets.Commands
{
    [Cmdlet(VerbsCommon.Get, "AcomArticle", DefaultParameterSetName = "Author")]
    public class GetAcomArticleCommand : Cmdlet
    {
        [Parameter(Mandatory = true, Position =0)]
        public string ArticleDirectory { get; set; }

        [Parameter(ParameterSetName ="Author", Mandatory = false)]
        [Parameter(ParameterSetName = "Stale", Mandatory = false)]
        [Parameter(ParameterSetName = "StaleWithin", Mandatory = false)]
        public string Author { get; set; } = string.Empty;

        [Parameter(ParameterSetName ="Stale", Mandatory =true)]
        public SwitchParameter Stale { get; set; }

        [Parameter(ParameterSetName = "StaleWithin", Mandatory = true)]
        [ValidateRange(1, 89)]
        [PSDefaultValue()]
        public int StaleWithin { get; set; } = 0;

        const int staleDays = 90;

        protected override void ProcessRecord()
        {
            try
            {
                foreach (string fileName in Directory.GetFiles(this.ArticleDirectory, "*.md", SearchOption.AllDirectories))
                {
                     ProcessFile(fileName);
                }
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, AcomCmdlets.Resources.UnexpectedError, ErrorCategory.InvalidOperation, this));
            }
        }

        protected void ProcessFile(string fileName)
        {
            try
            {
                AcomArticle article = new AcomArticle(fileName);

                // Check author
                if (this.Author != string.Empty && !article.Author.Contains(this.Author))
                {
                    return; // Failed author check; no need to go further with this article.
                }

                // Check staleness
                if (this.Stale || this.StaleWithin > 0)
                {
                    if (article.PubDate.AddDays(staleDays - this.StaleWithin) > DateTime.Now)
                    {
                        return; // User asked for stale/stalewithin and this article doesn't match criteria.
                    }
                }

                WriteObject(article);
            }
            catch(Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendFormat("[{0}] : {1}", fileName, ex.Message);
                ThrowTerminatingError(new ErrorRecord(new Exception(error.ToString(), ex), AcomCmdlets.Resources.UnexpectedError, ErrorCategory.InvalidOperation, this));
            }
        }
    }
}
