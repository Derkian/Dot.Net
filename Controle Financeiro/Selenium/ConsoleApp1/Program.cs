using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using ConsoleApp1.Models;
using System.Linq;
using System.Net;
using System.Text;
using System.IO.Compression;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Mata processos zumbis
                foreach (var process in Process.GetProcessesByName("chromedriver"))
                {
                    process.Kill();
                }

                // Leitura dos arquivos
                var contas = new List<Conta>();
                var contas_path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\Extratos\contas.txt";
                var extrato_path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\Extratos\extrato.txt";

                if (!File.Exists(contas_path)) throw new Exception("Arquivo contas.txt não encontrado");
                if (File.Exists(extrato_path)) File.Delete(extrato_path);

                using (var reader = new StreamReader(contas_path))
                {
                    while (reader.Peek() >= 0)
                    {
                        var arr = reader.ReadLine().Trim().Split(Convert.ToChar("|"));
                        if (arr.Length == 6)
                        {
                            var conta = new Conta();
                            conta.nome = arr[0];
                            conta.agencia = arr[1];
                            conta.conta = arr[2];
                            conta.senha = arr[3];
                            conta.ultima_atulizacao = Convert.ToDateTime(arr[4]);
                            conta.classe = arr[5];
                            contas.Add(conta);
                        }
                    }
                }

                // Inicia as classes
                var extratos = new List<Extrato>();
                var classes = new List<Type>();

                foreach (var classe in (from a in contas select a.classe).Distinct())
                {
                    classes.Add(Type.GetType(classe));
                }

                var aParams = new object[] { contas };
                object locker = new object();

                foreach (var classe in classes)
                {
                    var imported = (List<Extrato>)classe.GetMethod("Sync").Invoke(null, aParams);

                    extratos.AddRange(imported);
                }

                if (extratos.Count > 0)
                {
                    // Apaga o arquivo extrato.txt
                    if (File.Exists(extrato_path)) File.Delete(extrato_path);

                    // Cria o arquivo extrato.txt
                    using (var writer = new StreamWriter(extrato_path, false))
                    {
                        foreach (var extrato in extratos)
                        {
                            writer.WriteLine(extrato.conta + "|" +
                                             extrato.data.ToString("dd/MM/yyyy") + "|" +
                                             extrato.descricao + "|" +
                                             extrato.credito.ToString() + "|" +
                                             extrato.debito.ToString() + "|" +
                                             extrato.saldo.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateChromeDrive();

                Console.WriteLine("Message => " + ex.Message);
                Console.WriteLine("StackTrace =>" + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Message => " + ex.InnerException.Message);
                    Console.WriteLine("StackTrace =>" + ex.InnerException.StackTrace);
                }
                Console.ReadKey();
            }
        }

        private static void UpdateChromeDrive()
        {
            try
            {
                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe")))
                {
                    FileVersionInfo chormeVersion = FileVersionInfo.GetVersionInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe"));

                    string versaoChrome = chormeVersion.FileVersion;
                    string urlAddress = "https://chromedriver.storage.googleapis.com/?delimiter=/&prefix=";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;

                        if (String.IsNullOrWhiteSpace(response.CharacterSet))
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                        System.Data.DataSet dsTeste = new System.Data.DataSet();
                        dsTeste.ReadXml(readStream);

                        if (dsTeste.Tables.Contains("CommonPrefixes"))
                        {
                            foreach (System.Data.DataRow dataRow in dsTeste.Tables["CommonPrefixes"].Rows)
                            {
                                if (dataRow["Prefix"].ToString().Contains(versaoChrome.Substring(0, (versaoChrome.LastIndexOf(".") - 1))))
                                {
                                    using (var download = new WebClient())
                                    {
                                        string urlDownload = $"https://chromedriver.storage.googleapis.com/{dataRow["Prefix"].ToString()}chromedriver_win32.zip";
                                        string pathDownload = Path.Combine(Path.GetTempFileName().Replace(".tmp", ".zip"));
                                        download.DownloadFile(urlDownload, pathDownload);

                                        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromedriver.exe")))
                                        {
                                            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromedriver.exe"));
                                        }

                                        ZipFile.ExtractToDirectory(pathDownload, AppDomain.CurrentDomain.BaseDirectory);
                                    }
                                }
                            }
                        }

                        response.Close();
                        readStream.Close();
                    }
                }
            }
            catch
            {
            }
        }
    }
}
