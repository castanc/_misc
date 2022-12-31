using StringUtils;
using System.IO.Compression;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace FileUtil
{
    public static class FileUtil
    {
        public static void OrganizeFolder(this string folderName, string targetFolder = "", string mask = "*.*", 
            int maxFolders = 21, params string[] removeTexts)
        {
            if ( !targetFolder.IsValidString())
                targetFolder = folderName;

            string[] files = Directory.GetFiles(folderName,mask,SearchOption.AllDirectories);
            SortedDictionary<string, string> sorted = new SortedDictionary<string, string>();

            foreach(string file in files)
            {
                if (!sorted.ContainsKey(Path.GetFileNameWithoutExtension(file)))
                    sorted.Add(Path.GetFileNameWithoutExtension(file), file);
            }

            int count = 0;
            int countFolder = 1;
            foreach (var kvp in sorted)
            {
                if ( count < maxFolders)
                {
                    string newFolder = $"{targetFolder}\\{countFolder}";
                    string newName = Path.GetFileName(kvp.Value);
                    if ( removeTexts != null )
                    {
                        foreach(string rt in removeTexts)
                        {
                            newName = newName.Replace(rt, "");
                        }
                    }
                    newName = newName.Replace(" ", "-");
                    newName = newName.Replace("_", "-");
                    newName = newName.Replace(";", "-");
                    newName = newName.Replace("&", "-");
                    newName = newName.Replace("--", "-");
                    newName = newName.Replace("-.", ".");
                    newName = newName.RemoveBetween("(", ")");



                    if (Path.GetDirectoryName(kvp.Value) != newFolder)
                    {
                        GetCreateFolder(newFolder);
                        File.Move(kvp.Value, $"{newFolder}\\{newName}", true);
                        DeleteEmptyFolder(Path.GetDirectoryName(kvp.Value));
                    }

                    count++;
                }
                else
                {
                    countFolder++;
                    count = 0;

                }
            }
        }

        public static void DeleteEmptyFolder(string folderName)
        {
            try
            {
                string[] files = Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    try
                    {
                        Directory.Delete(folderName, true);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch(Exception ex)
            {
                string s = "";
            }
        }

        public static void OrganizeFolders(this string rootFolder, string mask = "*.*",
    int maxFolders = 30, params string[] removeTexts)
        {
            SortedDictionary<string, string> sorted = new SortedDictionary<string, string>();
            string[] folders = Directory.GetDirectories(rootFolder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder, mask, SearchOption.TopDirectoryOnly);
                if (files.Length > maxFolders)
                {
                    sorted = new SortedDictionary<string, string>();
                    foreach (string file in files)
                    {
                        if (!sorted.ContainsKey(Path.GetFileNameWithoutExtension(file)))
                            sorted.Add(Path.GetFileNameWithoutExtension(file), file);
                    }
                    int count = 1;
                    int countFolder = 1;
                    foreach (var kvp in sorted)
                    {
                        if (count < maxFolders)
                        {
                            string newFolder = $"{folder}\\{countFolder}";
                            string newName = Path.GetFileName(kvp.Value);
                            if (removeTexts != null)
                            {
                                foreach (string rt in removeTexts)
                                {
                                    newName = newName.Replace(rt, "");
                                }
                            }
                            newName = newName.Replace(" ", "-");
                            newName = newName.Replace("_", "-");
                            newName = newName.Replace(";", "-");
                            newName = newName.Replace("&", "-");
                            newName = newName.Replace("--", "-");
                            newName = newName.Replace("-.", ".");
                            newName = newName.RemoveBetween("(", ")");

                            GetCreateFolder(newFolder);
                            File.Move(kvp.Value, $"{newFolder}\\{newName}", true);
                            count++;
                        }
                        else
                        {
                            countFolder++;
                            count = 1;

                        }
                    }

                }

            }





        }


        public static void GetCreateFolder(this string folderName)
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }

        public static int Unzip(this string sourceFolder, string targetFolder  = "")
        {
            if (targetFolder == "")
                targetFolder = sourceFolder;

            int count = 0;
            string[] files = Directory.GetFiles(sourceFolder,"*.zip", SearchOption.AllDirectories);
            Parallel.ForEach(files, zipFileName =>
            {
                string newFolder = $"{targetFolder}\\{Path.GetFileNameWithoutExtension(zipFileName)}";
                newFolder.GetCreateFolder();
                ZipFile.ExtractToDirectory(zipFileName, newFolder, true);

            });
            return count;
        }
    }
}
