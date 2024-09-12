using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace J2ME_Explorer
{
    internal class DBManager
    {
        //guardar items en base de datos especificada
        public void Items2DB(string path, List<Item> items) 
        {
            string cstr = $"Data Source={path};Version=3;";
            using (SQLiteConnection db = new SQLiteConnection(cstr))
            {
                db.Open();
                //crear tabla para guardar los items.
                string tableScheme = @"
                    CREATE TABLE IF NOT EXISTS Items (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                        Name TEXT NOT NULL, 
                        Path TEXT NOT NULL,  
                        Manifest TEXT, 
                        Icon BLOB
                    )";


                using (SQLiteCommand command = new SQLiteCommand(tableScheme, db))
                {
                    command.ExecuteNonQuery();
                }

                //insertar elementos a la tabla
                foreach (Item item in items)
                {
                    string iQuery = @"
                        INSERT INTO Items (Name, Path, Manifest, Icon) 
                        VALUES (@name, @path, @manifest, @icon)";
                    using (SQLiteCommand c = new SQLiteCommand(iQuery, db))
                    {
                        c.Parameters.AddWithValue("@name", item.Name);
                        c.Parameters.AddWithValue("@path", item.Path);
                        c.Parameters.AddWithValue("@manifest", SerializeDictionary(item.Manifest));
                        c.Parameters.AddWithValue("@icon", ImageToByteArray(item.Icon));
                        c.ExecuteNonQuery();
                    }
                }

            }
        }

        //leer items de base de datos especificada
        public List<Item> DB2Items(string path) {
            List<Item> items = new List<Item>();

            //string cstr = $"Data Source={path};Version=3;";
            string cstr = $"Data Source={path};Version=3;Journal Mode=Memory;Synchronous=Off;Cache Size=5000;";
            using (SQLiteConnection db = new SQLiteConnection(cstr)) {
                db.Open();
                
                //selecciona tabla y lee cada elemento
                string selectQuery = "SELECT * FROM Items";
                using (SQLiteCommand c = new SQLiteCommand(selectQuery, db)) 
                {
                    using (SQLiteDataReader reader = c.ExecuteReader()) 
                    {
                        while (reader.Read()) 
                        {
                            Image icon = ByteArrayToImage((byte[])reader["Icon"]);
                            string name = reader["Name"].ToString();
                            string qpath = reader["Path"].ToString();
                            Dictionary<string, string> manifest = DeserializeDictionary(reader["Manifest"].ToString());

                            //añade los items a la lista
                            items.Add(new Item(icon, name, qpath, manifest));
                        }
                    }
                }

            }

            return items;

        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private string SerializeDictionary(Dictionary<string, string> manifest)
        {
            return JsonConvert.SerializeObject(manifest);
        }

        private Dictionary<string, string> DeserializeDictionary(string manifestJson)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(manifestJson);
        }

    }
}
