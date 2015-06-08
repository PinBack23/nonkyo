using Nancy;
using Nancy.Conventions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOnkyo.Web
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        private byte[] favicon;

        protected override byte[] FavIcon
        {
            get { return this.favicon ?? (this.favicon = LoadFavIcon()); }
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("web", "web"));
            base.ConfigureConventions(nancyConventions);
        }

        private byte[] LoadFavIcon()
        {
            using (MemoryStream loMemoryStream = new MemoryStream())
            {
                NOnkyo.Web.Properties.Resources.NOnkyo.Save(loMemoryStream);
                loMemoryStream.Flush();
                return loMemoryStream.ToArray();
            }
        }
    }
}
