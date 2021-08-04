using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace EpricaExportFalsificat
{
    static class Resource
    {

        public static string ReadResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            // if (!name.StartsWith(nameof(SignificantDrawerCompiler)))
            //{
            resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(name));
            //}

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
