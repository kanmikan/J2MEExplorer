using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace J2ME_Explorer
{
    internal class JarManager
    {

        public List<Item> elements = new List<Item>();
        public Action OnBefore;
        public Action<Item, int> OnItemAdded;
        public Action<int> OnCycle;
        
        public int jarCalls = 0;

        public void reset()
        {
            elements.Clear();
            jarCalls = 0;
        }

        public async Task readJarAsync(string filepath)
        {
            jarCalls++;
            await Task.Run(() =>
            {
                OnBefore?.Invoke();

                Dictionary<string, string> manifest = JarManager.ReadJarManifest(filepath);
                if (manifest != null)
                {
                    Image icon = Image.FromFile(@"Resources/app.png"); //icono default
                    string name = "noname"; //fallback name
                    string vendor = "novendor"; //fallback

                    //Intentar leer icono del midlet
                    if (manifest.ContainsKey("MIDlet-Icon"))
                    {
                        icon = JarManager.ReadJarIcon(filepath, manifest["MIDlet-Icon"]);
                    }
                    else if (manifest.ContainsKey("MIDlet-1"))
                    {
                        var parts = manifest["MIDlet-1"].Split(',');
                        string midlet1path = (parts.Length > 1) ? parts[1].Trim() : "";
                        icon = JarManager.ReadJarIcon(filepath, midlet1path);
                    }
                    else
                    {
                        Console.WriteLine("Manifest: no se ha detectado o no se puede cargar icono. usando default.");
                    }

                    //Intentar leer nombre del midlet
                    if (manifest.ContainsKey("MIDlet-Name"))
                    {
                        name = manifest["MIDlet-Name"];
                    }
                    else
                    {
                        Console.WriteLine("Manifest: no se ha detectado el nombre.");
                        //TODO: abandonar ciclo si no se detecta un nombre?
                    }

                    //Intentar leer vendor
                    if (manifest.ContainsKey("MIDlet-Vendor")) 
                    {
                        vendor = manifest["MIDlet-Vendor"];
                    }

                    Item item = new Item(icon, name, filepath, manifest);

                    elements.Add(item);
                    OnItemAdded?.Invoke(item, elements.Count);

                }
                else
                {
                    Console.WriteLine("Manifest: detectado como nulo, archivo corrupto o no es un jar.");
                }

                OnCycle?.Invoke(jarCalls);

            });
        }

        public static async Task<IEnumerable<string>> readDirectoryFilesAsync(string fullPath)
        {
            return await Task.Run(() => Directory.EnumerateFiles(fullPath, "*.jar", SearchOption.AllDirectories));
        }


        public static bool CheckImageDims(string jPath, List<string> target)
        {
            List<(int width, int height)> dimensionsList = new List<(int, int)>();

            foreach (var dimension in target)
            {
                string[] dimensions = dimension.Split('x');
                if (dimensions.Length == 2 && int.TryParse(dimensions[0], out int width) && int.TryParse(dimensions[1], out int height))
                {
                    dimensionsList.Add((width, height));
                }
                else
                {
                    return false;
                }
            }

            using (ZipArchive archive = ZipFile.OpenRead(jPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    try
                    {
                        if (entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            entry.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            entry.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                        {
                            using (Stream imageStream = entry.Open())
                            {
                                using (Bitmap img = new Bitmap(imageStream))
                                {
                                    foreach (var (targetWidth, targetHeight) in dimensionsList)
                                    {
                                        if (img.Width == targetWidth && img.Height == targetHeight)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) 
                    {
                        Console.WriteLine(e);
                        return false;
                    }
                }
            }
            return false;
        }

        public static List<JarItem> ReadJarFilelist(string path)
        {
            List<JarItem> fileNames = new List<JarItem>();
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    fileNames.Add(new JarItem(entry.FullName, entry.Length, entry.LastWriteTime.DateTime));
                }
            }

            return fileNames;
        }

        public static Dictionary<string, string> ReadJarManifest(string path)
        {
            Dictionary<string, string> manifestData = new Dictionary<string, string>();
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(path))
                {
                    ZipArchiveEntry manifestEntry = archive.Entries.FirstOrDefault(e => string.Equals(e.FullName, "META-INF/MANIFEST.MF", StringComparison.OrdinalIgnoreCase));
                    if (manifestEntry != null)
                    {
                        using (StreamReader reader = new StreamReader(manifestEntry.Open()))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                int cIndex = line.IndexOf(':');
                                if (cIndex != -1)
                                {
                                    string key = line.Substring(0, cIndex).Trim();
                                    string value = line.Substring(cIndex + 1).Trim();
                                    manifestData[key] = value;
                                }
                                else
                                {
                                    //Console.WriteLine("Manifest: algún Key-Value está mal formado.");
                                }
                            }
                        }
                        return manifestData;
                    }
                    else
                    {
                        Console.WriteLine("Manifest: no encontrado.");
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static Image ReadJarIcon(string path, string iconPath)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(path))
                {
                    //buscar archivo en el jar
                    iconPath = iconPath.TrimStart('/', '\\');
                    ZipArchiveEntry iconEntry = archive.Entries.FirstOrDefault(e => string.Equals(e.FullName, iconPath, StringComparison.OrdinalIgnoreCase));
                    
                    if (iconEntry != null)
                    {
                        using (Stream iconStream = iconEntry.Open())
                        {
                            try
                            {
                                return Image.FromStream(iconStream, true);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error al leer el icono, usando default.");
                                return Image.FromFile(@"Resources/app.png");
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Icono no encontrado: {iconPath}");
                    }

                }
            }
            catch (Exception e)
            {
                return Image.FromFile(@"Resources/app.png");
            }

            return Image.FromFile(@"Resources/app.png");

        }

    }
}
