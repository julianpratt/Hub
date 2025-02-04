using System.Collections;
using static Microsoft.AspNetCore.Http.Results;
using Mistware.Utils;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Builder;

namespace Hub
{
	public class Hub
	{ 
		public static void Main(string[] args)
		{
			IDictionary eVars = Environment.GetEnvironmentVariables();
			
			Site site = Site.Load("site.txt");
			site.Debug();  
			string sitebase = eVars["PWD"] + "/" + site.Base; 
			
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
			builder.Logging.ClearProviders();
			WebApplication app = builder.Build();
			//var app = WebApplication.Create(args);			
	
			app.UseHttpsRedirection();

			Static(app, "/css",          sitebase + site.Styles);
			Static(app, "/img",          sitebase + site.Images);
			//Static(app, "/js",           sitebase + site.Scripts);
			Static(app, "/attachments",  sitebase + site.Attachments);	
			
			string[] folders = System.IO.Directory.GetDirectories(site.Base); 
			
			foreach (string folder in folders)
			{
				string f = Path.GetFileName(folder.TrimEnd(Path.DirectorySeparatorChar));
				if (f != "css" && f != "img" && f != "attachments" && f != "js")
				{
					app.MapGet("/" + f,                ()                     => Page.Build(f, site));
					app.MapGet("/" + f + "/{p1}",      (string p1)            => Page.Build(f + "/" + p1, site));		
					app.MapGet("/" + f + "/{p1}/{p2}", (string p1, string p2) => Page.Build(f + "/" + p1 + "/" + p2, site));				
				}
			}
			
			app.MapGet("/page/{p1}", (string p1) => Page.Build(p1, site));		
	
			app.UseExceptionHandler("/oops");

			app.MapGet("/oops", () => "Oops! An error happened.");

			app.MapGet("/", () => Page.Build("home", site));

			app.Run();
		}
		
		private static void Static(WebApplication app, string requestpath, string physicalpath)
		{
			Console.WriteLine("Static: " + requestpath + "@" + physicalpath);
			
			app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(physicalpath), RequestPath = requestpath });
		}

	}
}
