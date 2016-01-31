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
        /// <summary>
        /// Specifies the ACOM article file name the user wants to process.
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true
        )]
        [ValidateNotNullOrEmpty]
        public string FileName { get; set; }

        [Parameter(ParameterSetName ="Author", Mandatory = false)]
        [Parameter(ParameterSetName = "Stale", Mandatory = false)]
        [Parameter(ParameterSetName = "StaleWithin", Mandatory = false)]
        public string Author { get; set; } = string.Empty;

        [Parameter(ParameterSetName ="Stale", Mandatory =true)]
        public SwitchParameter Stale { get; set; }

        [Parameter(ParameterSetName = "StaleWithin", Mandatory = true)]
        [ValidateRange(1, 89)]
        public int StaleWithin { get; set; } = 0;

        const int staleDays = 90;

        protected override void ProcessRecord()
        {
            if (this.FileName != null)
            {
                AcomArticle article = new AcomArticle(this.FileName);

                // Check author
                if (this.Author != string.Empty && !article.Author.Contains(this.Author)) 
                {
                    return; // Failed author check; no need to go further.
                }

                // Check staleness
                if (this.Stale || this.StaleWithin > 0)
                {
                    if (article.PubDate.AddDays(staleDays - this.StaleWithin) > DateTime.Now)
                    {
                        return; // Article is fresh
                    }
                }

                WriteObject(article);
            }
        }
    }
}
