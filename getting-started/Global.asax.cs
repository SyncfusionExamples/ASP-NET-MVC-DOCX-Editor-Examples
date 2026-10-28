using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Newtonsoft.Json;
using Syncfusion.EJ2.SpellChecker;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DocumentEditor
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InitializeSpellChecker();
        }

        // Loads the dictionaries listed in App_Data/spellcheck.json once at startup.
        private static void InitializeSpellChecker()
        {
            string dataPath = HostingEnvironment.MapPath("~/App_Data/");
            string configPath = Path.Combine(dataPath, "spellcheck.json");
            if (!File.Exists(configPath))
            {
                return;
            }

            var configured = JsonConvert.DeserializeObject<List<DictionaryData>>(File.ReadAllText(configPath));
            var dictionaries = new List<DictionaryData>();
            string personalDictPath = null;
            foreach (var item in configured)
            {
                dictionaries.Add(new DictionaryData(item.LanguadeID, Path.Combine(dataPath, item.DictionaryPath), Path.Combine(dataPath, item.AffixPath)));
                personalDictPath = Path.Combine(dataPath, item.PersonalDictPath);
            }
            SpellChecker.InitializeDictionaries(dictionaries, personalDictPath, 3);
        }
    }
}
