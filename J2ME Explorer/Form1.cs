﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace J2ME_Explorer
{
    public partial class Form1 : Form
    {
        private JarManager jarManager = new JarManager();
        private DBManager dbManager = new DBManager();
        private CustomListView listView;

        public Form1()
        {
            InitializeComponent();

            //setup de componentes
            MainList.View = View.LargeIcon;

            //leer carpeta predeterminada
            string fullPath = Path.Combine(Application.StartupPath, "jars");
            ReadFolder(fullPath);

            //eventos de drag and drop
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            //eventos del menu
            abrirCarpeta.Click += new EventHandler(abrirCarpeta_Click);
            salir.Click += new EventHandler(salir_Click);
            acercaDe.Click += new EventHandler(acercaDe_Click);
            sortNombre.Click += new EventHandler(sortNombre_Click);
            sortFile.Click += new EventHandler(sortFile_Click);
            sortVendor.Click += new EventHandler(sortVendor_Click);
            importarLista.Click += new EventHandler(importarLista_Click);
            exportarLista.Click += new EventHandler(exportarLista_Click);

            //eventos de tecla
            this.KeyPreview = true;

        }

        private void importarLista_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Application.StartupPath;
                openFileDialog.Filter = "Database files (*.db)|*.db";
                openFileDialog.Title = "Elige un archivo para importar";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    List<Item> elements = dbManager.DB2Items(filePath);
                    ReadFromList(elements);
                }
            }
        }

        private void exportarLista_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Application.StartupPath;
                saveFileDialog.Filter = "Database files (*.db)|*.db";
                saveFileDialog.Title = "Elige donde guardar el archivo";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    Console.WriteLine(filePath);

                    List<Item> elements = jarManager.elements;

                    // Save the list to the selected database file.
                    dbManager.Items2DB(filePath, elements);
                }
            }
        }

        private void sortVendor_Click(object sender, EventArgs e) 
        {
            listView.SortByVendor(SortOrder.Ascending);
        }

        private void sortFile_Click(object sender, EventArgs e)
        {
            listView.SortByFile(SortOrder.Ascending);
        }

        private void sortNombre_Click(object sender, EventArgs e) 
        {
            listView.SortByName(SortOrder.Ascending);
        }

        private void salir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void acercaDe_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void abrirCarpeta_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fDialog = new OpenFileDialog())
            {
                fDialog.ValidateNames = false;
                fDialog.CheckFileExists = false;
                fDialog.CheckPathExists = true;
                fDialog.FileName = "Selecciona la carpeta";

                if (fDialog.ShowDialog() == DialogResult.OK)
                {
                    string fpath = Path.GetDirectoryName(fDialog.FileName);
                    ReadFolder(fpath);
                }
            }

        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] droppedItems = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string path in droppedItems)
            {
                if (Directory.Exists(path))
                {
                    ReadFolder(path);

                }
                else
                {
                    Console.WriteLine(path);
                }
            }
        }

        private void ReadFromList(List<Item> elements)
        {
            MainList.Controls.Clear();
            jarManager.reset();

            listView = new CustomListView
            {
                Dock = DockStyle.Fill,
                View = View.LargeIcon
            };

            MainList.Controls.Add(listView);

            loadingBar.Maximum = elements.Count();
            loadingBar.Value = 0;
            loadingBar.Visible = true;
            detectedjars.Text = $"{elements.Count()} Detectados.";

            Task.Run(() =>
            {
                for (int i = 0; i < elements.Count(); i++)
                {
                    Console.WriteLine($"Leyendo: {elements[i].Name} ...");

                    string imageKey = Path.GetFileNameWithoutExtension(elements[i].Path);
                    if (!listView.ImageList.Images.ContainsKey(imageKey))
                    {
                        Invoke((Action)(() => listView.ImageList.Images.Add(imageKey, elements[i].Icon)));
                    }

                    Invoke((Action)(() =>
                    {
                        listView.AddItem(elements[i]);
                        validjars.Text = $"{i + 1} Válidos.";
                        jarcount.Text = $"{i + 1} Jars Leidos.";
                        loadingBar.Increment(1);
                    }));
                }

                Invoke((Action)(() =>
                {
                    loadingBar.Value = loadingBar.Maximum;
                    loadingBar.Visible = false;
                }));
            });
        }


        private async void ReadFolder(string fullPath)
        {
            MainList.Controls.Clear();
            jarManager.reset();
            this.Text = $"J2ME Explorer - {Path.GetFileName(fullPath)}";

            listView = new CustomListView
            {
                Dock = DockStyle.Fill,
                View = View.LargeIcon
            };

            MainList.Controls.Add(listView);

            if (Directory.Exists(fullPath))
            {
                IEnumerable<string> files = await JarManager.readDirectoryFilesAsync(fullPath);

                loadingBar.Maximum = files.Count();
                loadingBar.Value = 0;
                loadingBar.Visible = true;
                detectedjars.Text = $"{files.Count()} Detectados.";

                int count = 0;
                foreach (string file in files)
                {
                    Console.WriteLine($"Leyendo: {file} ...");

                    //leer jar
                    await jarManager.readJarAsync(file);

                    //al añadir un item.
                    jarManager.OnItemAdded = (item, vcount) =>
                    {
                        try
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                string imageKey = Path.GetFileNameWithoutExtension(file);
                                if (!listView.ImageList.Images.ContainsKey(imageKey))
                                {
                                    listView.ImageList.Images.Add(imageKey, item.Icon);
                                }
                                listView.AddItem(item);
                                validjars.Text = $"{vcount} Válidos.";
                            });
                        }
                        catch (Exception e) 
                        { } //ignorar crash al cerrar muy rapido la ventana
                    };

                    //al hacer un ciclo, haya añadido el item o no.
                    jarManager.OnCycle = (pcount) =>
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            jarcount.Text = $"{pcount} Jars Leidos.";
                            loadingBar.Increment(1);
                            count = pcount;
                        });
                    };
                }

                if (count == files.Count())
                {
                    loadingBar.Value = loadingBar.Maximum;
                    loadingBar.Visible = false;
                }
            }
            else
            {
                Console.WriteLine("Carpeta inexistente.");
            }
        }
    }
}
