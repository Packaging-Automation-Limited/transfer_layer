using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using System.Net;
using System.Security.Authentication;
using FluentFTP.Helpers;
using Microsoft.VisualBasic.FileIO;

namespace ConsoleRecipe00
{
    public class FileWatcher
    {
        FileSystemWatcher watcher = new();
        Json json = new();
        String FileName = "";
        string IP = "10.10.0.108";
        //string RecipeFile = "RecipeImp.csv";

        public string RecipeFile
        {
            get { return Environment.CurrentDirectory + "\\RecipeImp.csv"; }
            //get { return Path.Combine(DataDirectory, @"users.db"); }
        }

        public void FileWatcherStart(string path, string Ip)
        {
            IP = Ip;
            watcher.Path = @path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.csv";
            watcher.Changed += new FileSystemEventHandler(OnFileChanged);
            watcher.EnableRaisingEvents = true;
            //RecipeFile = Path.Combine(path, "RecipeImp.csv");
        }

        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            FileInfo fi = new FileInfo(e.FullPath);
            if (fi.Length > 1 & e.Name != FileName & e.Name != "RecipeImp.csv")
            {
                Console.WriteLine("File changed: " + e.Name + " Size: " + fi.Length);
                json.JsonPost("SoftImportRecipe", 0, IP);
                var result = ParseRecipe(e.FullPath);
                
                if(result) 
                    result = SendToHmiFluentFtp(RecipeFile, "RecipeImp.csv");
                if(result)
                    json.JsonPost("SoftImportRecipe", 1, IP);
                FileName = e.Name;
            }
        }

        private bool ParseRecipe(string FileN)
        {
            List<Recipe> recipes = new();

            using (TextFieldParser parser = new TextFieldParser(FileN))
            {
                try
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    if (!parser.EndOfData)
                        parser.ReadLine();
                    while (!parser.EndOfData)
                    {
                        //Process row
                        Recipe recipe = new();
                        string[] fields = parser.ReadFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            fields[i] = fields[i].Replace("\"", "");
                        }
                        recipe.FieldName = fields[2];
                        recipe.Product_e = Convert.ToInt16(fields[3]);
                        recipe.ProductCostPerKg = Convert.ToSingle(fields[8]);
                        recipe.ProductExtraCost = Convert.ToSingle(fields[9]);
                        recipe.ProductT1SP = Convert.ToSingle(fields[10]);
                        recipe.ProductTargetSpeed = Convert.ToInt16(fields[11]);
                        recipe.ProductTolerance = Convert.ToInt16(fields[12]);
                        recipe.ProductWeight = Convert.ToInt16(fields[7]);
                        recipe.CountThreshold = Convert.ToInt16(fields[14]);
                        recipe.AdjMax = Convert.ToInt16(fields[15]);
                        recipe.AdjMin = Convert.ToInt16(fields[16]);
                        recipe.AdjQtPercStart = Convert.ToInt16(fields[17]);
                        recipe.AutoAdjustON = StrBool(fields[18]);
                        recipe.AdjStartWeigh = Convert.ToInt16(fields[19]);
                        recipe.CwProgramNumber = Convert.ToInt16(fields[20]);
                        recipe.CwBypass = StrBool(fields[21]);
                        recipe.Tare = Convert.ToInt16(fields[22]);
                        recipe.WeightSP = Convert.ToInt16(fields[13]);
                        recipe.Moisture = Convert.ToSingle(fields[5]);
                        recipe.TolMin = recipe.ProductTolerance;
                        recipes.Add(recipe);
                        Console.WriteLine("Parse OK, Recipe: " + recipe.FieldName);
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    return false;
                }
            }
            string RecipeStr = "FieldName,Product_e,ProductCostPerKg,ProductExtraCost,ProductT1SP,ProductTargetSpeed,ProductTolerance,ProductWeight,CountThreshold,AdjMax,AdjMin,AdjQtPercStart,AutoAdjustON,AdjStartWeigh,CwProgramNumber,CwBypass,Tare,ProductDeclaredWeight,ProductMoistureTare" + Environment.NewLine;
            foreach (var recipe in recipes)
            {
                RecipeStr = RecipeStr +
                    recipe.FieldName + "," +
                    recipe.Product_e + "," +
                    recipe.ProductCostPerKg + "," +
                    recipe.ProductExtraCost + "," +
                    recipe.ProductT1SP + "," +
                    recipe.ProductTargetSpeed + "," +
                    recipe.ProductTolerance + "," +
                    recipe.ProductWeight + "," +
                    recipe.CountThreshold + "," +
                    recipe.AdjMax + "," +
                    recipe.AdjMin + "," +
                    recipe.AdjQtPercStart + "," +
                    recipe.AutoAdjustON + "," +
                    recipe.AdjStartWeigh + "," +
                    recipe.CwProgramNumber + "," +
                    recipe.CwBypass + "," +
                    recipe.Tare + "," +
                    recipe.WeightSP + "," +
                    recipe.Moisture + "," + Environment.NewLine;
            }
            if (File.Exists(RecipeFile))
                File.Delete(RecipeFile);
            File.WriteAllText(RecipeFile, RecipeStr);

            return true;
        }

        bool StrBool(string str)
        {
            if(str == "1")
                return true;
            else
                return false;
        }

        private bool SendToHmiFluentFtp(string LocalFullPath, string RemoteFullPath)
        {

            FtpClient client = new();
            FtpStatus ftpStatus;
            client.RetryAttempts = 3;
            try
            {
                client.LoadProfile(new FtpProfile
                {
                    Host = IP,
                    Credentials = new NetworkCredential("PAL", "756502"),
                    Encryption = FtpEncryptionMode.None,
                    Protocols = SslProtocols.None,
                    DataConnection = FtpDataConnectionType.PASV,
                    Encoding = Encoding.UTF8,
                });
                client.Port = 21;
                client.Connect();
                ftpStatus = client.UploadFile(LocalFullPath, "/"+ RemoteFullPath);
            }
            catch (Exception ex)
            {
                client.Disconnect();
                Console.WriteLine("FTP error: "+ex.ToString());
                return false;
            }
            client.Disconnect();
            if (ftpStatus.IsSuccess())
            {
                Console.WriteLine("FTP OK");
                return true;
            }
            else
            {
                Console.WriteLine("FTP Error");
                return false;
            }
        }
    }
}
