using System.Collections.Generic;
using static Microsoft.AspNetCore.Http.Results;
using Mistware.Utils;
using Markdig;
using System.IO;

namespace Hub
{
	public class Page
	{
		public string Title  { get; set; }
		public string Body   { get; set; }
		public int    Status { get; set; }
			
		public static IResult Build(string pagename, Site site) 
		{
			/* If page doesn't exist, but page/index does then use that */
			if (!System.IO.File.Exists(site.Base + pagename + ".md")) 
			{
				pagename += "/index";
				if (!System.IO.File.Exists(site.Base + pagename + ".md")) return NotFound(site);
			}	
							
			Log.Me.Info("Sending: " + pagename);
			
			string html = MarkdownToHtml(site.Base + pagename + ".md");
			
			Page page = new Page();
			page.Title  = pagename;
			page.Body   = "<article class=\"markdown-body\">\n" + html + "</article>\n";
			page.Status = 200; 

			return Build(page, site);		
		}
		
		private static string MarkdownToHtml(string filepath) 
		{
			var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
			
			return Markdown.ToHtml(System.IO.File.ReadAllText(filepath), pipeline) ;
		}
		
		public static IResult NotFound(Site site)
		{
			Page page = new Page();
			page.Title  = "Not Found";
			page.Body   = "<b>Not Found</b>";
			page.Status = 404;
			
			return Build(page, site); 		
		}
		
		public static IResult Build(Page page, Site site) 
		{
			string html;
			
			html =  Header(page.Title, site); 
			html += MainMenu(site);
			html += page.Body;
			html += Footer(site);

			return Content( html, "text/html", default, page.Status);		
		}

		private static string Header(string title, Site site)
		{
			//string style      = "site.css";
			string stylesheet = "/css/site.css";
			
			string html;

			html =  "<!DOCTYPE html>\n";
			html += "<HTML>\n";
			html += "  <HEAD>\n";
			html += "    <TITLE>" + title + "</TITLE>\n";
			html += "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n";
			html += "    <meta charset=\"UTF-8\">\n";
			if (site.UseBootstrap) html += BootstrapHtml("css");    
			html += "    <link rel=\"stylesheet\" type=\"text/css\" href=\"" + stylesheet + "\">\n";
			html += "  </head>\n";
			html += "  <body class=\"container\">\n";

			return html;
		}

		private static string MainMenu(Site site)
		{
			string html; 
		
			html  = "      <nav class=\"navbar navbar-expand-lg navbar-light bg-light\">\n";
			html += "        <a class=\"navbar-brand\" href=\"/\">Home</a>\n";
			html += "        <button class=\"navbar-toggler\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#menuItems\" aria-controls=\"menuItems\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">\n";
			html += "          <span class=\"navbar-toggler-icon\"></span>\n";
			html += "        </button>\n";
			html += "        <div class=\"collapse navbar-collapse\" id=\"menuItems\">\n";
			html += "          <ul class=\"navbar-nav me-auto mb-2 mb-lg-0\">\n";

			foreach (MenuItem item in site.Items)
			{
				html += "          <li class=\"nav-item\"><a class=\"nav-link\" href=\"/" + item.Action + "\">" + item.Title + "</a></li>\n";
			}
			
			html += "        </ul>\n";
			html += "      </div>\n";
			html += "  </nav>\n";
			html += "  <br/>\n";  

			return html;
		}
	  
		private static string Footer(Site site)
		{
			string html = "";
			 
			if (site.UseBootstrap) html += BootstrapHtml("js");
			html += "  </BODY>\n";
			html += "</HTML>";

			return html;
		}
		
		private static string BootstrapHtml(string urltype)
		{
			if (urltype != "css" && urltype != "js") Log.Me.Fatal("Invalid URL type '" + urltype + "' in BootstrapRef"); 
			
			string url = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/" + urltype + "/bootstrap.min." + urltype;

			string sha = "";
			if      (urltype == "css") sha = "sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH";
			else if (urltype == "js" ) sha = "sha384-0pUGZvbkm6XF6gxjEnlmuGrJXVbNuzT9qBBavbLwCsOGabYfZo0T0to5eqruptLy";
			
			string middle = url + "\" INTEGRITY=\"" + sha + "\" CROSSORIGIN=\"anonymous\">";
			
			string html = "    ";
			if      (urltype == "css") html += "<LINK   REL=\"stylesheet\" HREF=\"" + middle;
			else if (urltype == "js" ) html += "<SCRIPT SRC=\"" + middle + "</script>";
			
			return html;		
		}  	
	}	
}
