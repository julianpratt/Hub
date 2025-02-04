using System.Collections.Generic;
using Mistware.Utils;

namespace Hub
{
	public class MenuItem
	{
		/* MenuItem refers to a page - used for site navigation */	
		public string Action  { get; set; }
		public string Title   { get; set; }
	}

	public class Site
	{
		/* Site has an address (URL) and a list of content items */
		public string         Address      { get; set; } 
		public string         Port         { get; set; }
		public string         Base         { get; set; }
		public string         Styles       { get; set; }
		public string         Images       { get; set; }		
		public string         Scripts      { get; set; }		
		public string         Attachments  { get; set; }		
		public bool           UseBootstrap { get; set; }
		public List<MenuItem> Items        { get; set; } 
	
		public static Site Load(string filepath)
		{
			Site site = new Site();
			site.Address      = "";
			site.Port         = "5001";
			site.Base         = "./content/";
			site.UseBootstrap = true;
			site.Items        = new List<MenuItem>();

			bool inContent    = false;  
			
			if (!File.Exists(filepath)) Log.Me.Fatal("Could not load " + filepath);
			
			using (var reader = new StreamReader(filepath))
			{
				Log.Me.Info("Opened " + filepath + " OK.");
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					if (line == null) Log.Me.Fatal("Failed to read line in: " + filepath);
					line = line.Trim();
					if (line.Length > 0)
					{
						List<string> words = line.Wordise();
					
						if (!inContent) 
						{
							string key   = words[0].ToUpper();
							if      (key == "SITE"  )      site.Address     = words[1];
							else if (key == "PORT"  )      site.Port        = words[1];
							else if (key == "BASE"  )      site.Base        = words[1];
							else if (key == "STYLES")      site.Styles      = words[1];
							else if (key == "IMAGES")      site.Images      = words[1];
							else if (key == "SCRIPTS")     site.Scripts     = words[1];
							else if (key == "ATTACHMENTS") site.Attachments = words[1];
							else if (key == "CONTENT")     inContent = true;
							else Log.Me.Fatal("Illegal statement in " + filepath + ". Statement is: " + line); 	
						}
						else 
						{
							if (words.Count != 2) Log.Me.Fatal("Missing values in " + filepath + ". Statement is: " + line); 	
							MenuItem item = new MenuItem();	
							item.Action = words[0];
							item.Title  = words[1].StripQuotes();
							site.Items.Add(item);
						}
					}		
				}
			}
			
			return site;	
		}
		
		public void Debug()
		{
			Log.Me.Info("=========================");
			Log.Me.Info("SITE Address:      " + this.Address);
			Log.Me.Info("     Port:         " + this.Port);
			Log.Me.Info("     Base:         " + this.Base);
			Log.Me.Info("     Styles:       " + this.Styles);
			Log.Me.Info("     Images:       " + this.Images);
			Log.Me.Info("     Scripts:      " + this.Scripts);
			Log.Me.Info("     Attachments:  " + this.Attachments);
			Log.Me.Info("     UseBootStrap: " + this.UseBootstrap.ToString());

			foreach (MenuItem item in this.Items)
			{
				Log.Me.Info(item.Action + " = [" + item.Title + "]");
			}
			Log.Me.Info("");			
		}
	
		
	}
}
