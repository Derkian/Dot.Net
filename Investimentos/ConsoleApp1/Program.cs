using System;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using System.IO;
using ConsoleApp1.Models;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ReadInvestimentos();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro => " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }
        }

        private static void ReadInvestimentos()
        {
            var investimentos_path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\investimentos.txt";

            // Inicia a importação via Selenium
            var investimentos = new List<Investimento>();
            var classes = new List<Type>();

            classes.Add(typeof(Poupanca));
            classes.Add(typeof(Inflacao));
            classes.Add(typeof(TesouroDireto)); // TODO Derkian

            object locker = new object();

            Parallel.ForEach(classes, classe =>
            {
                var imported = (List<Investimento>)classe.GetMethod("Sync").Invoke(null, null);

                lock (locker)
                {
                    investimentos.AddRange(imported);
                }
            });

            if (investimentos.Count > 0)
            {
                // Apaga o arquivo
                if (File.Exists(investimentos_path)) File.Delete(investimentos_path);

                investimentos = investimentos.OrderByDescending(o => o.mes).ThenBy(o => o.tipo).ThenBy(o => o.nome).ToList();

                // Cria o arquivo
                using (var writer = new StreamWriter(investimentos_path, false))
                {
                    foreach (var investimento in investimentos)
                    {
                        writer.WriteLine(investimento.risco + "|" +
                                         investimento.tipo + "|" + 
                                         investimento.nome + "|" +
                                         investimento.mes.ToString("dd/MM/yyyy") + "|" +
                                         investimento.valor.ToString());
                    }
                }
            }
        }






        //private static void ReadExtrato()
        //{
        //    var contas_path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\Extratos\contas.txt";
        //    var extrato_path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\Extratos\extrato.txt";

        //    if (!File.Exists(contas_path)) throw new Exception("Arquivo contas.txt não encontrado");
        //    if (File.Exists(extrato_path)) File.Delete(extrato_path);

        //    // Lê o arquivo contas.txt e armazena em variável
        //    var contas = new Dictionary<string, DateTime>();

        //    using (var reader = new StreamReader(contas_path))
        //    {
        //        while (reader.Peek() >= 0)
        //        {
        //            var linha = reader.ReadLine();

        //            if (linha.Trim() != "")
        //            {
        //                var arr = linha.Split(Convert.ToChar("|"));
        //                if (arr[1] != "") contas.Add(arr[0], Convert.ToDateTime(arr[1]));
        //            }
        //        }
        //    }


        //    // Inicia a importação via Selenium
        //    var extratos = new List<Extrato>();
        //    var classes = new List<Type>();

        //    classes.Add(typeof(Itau_PF_CC));
        //    classes.Add(typeof(ItauPersonnalite_PF_CC));
        //    classes.Add(typeof(Bradesco_PF));
        //    classes.Add(typeof(Master_Credicard_Itau));

        //    var aParams = new object[] { contas };
        //    object locker = new object();

        //    Parallel.ForEach(classes, classe =>
        //    {
        //        var imported = (List<Extrato>)classe.GetMethod("Sync").Invoke(null, aParams);

        //        lock (locker)
        //        {
        //            extratos.AddRange(imported);
        //        }
        //    });

        //    if (extratos.Count > 0)
        //    {
        //        // Apaga o arquivo extrato.txt
        //        if (File.Exists(extrato_path)) File.Delete(extrato_path);

        //        // Cria o arquivo extrato.txt
        //        using (var writer = new StreamWriter(extrato_path, false))
        //        {
        //            foreach (var extrato in extratos)
        //            {
        //                writer.WriteLine(extrato.conta + "|" +
        //                                 extrato.data.ToString("dd/MM/yyyy") + "|" +
        //                                 extrato.descricao + "|" +
        //                                 extrato.credito.ToString() + "|" +
        //                                 extrato.debito.ToString() + "|" +
        //                                 extrato.saldo.ToString());
        //            }
        //        }
        //    }
        //}

        //private static void ReadInvestimentosYubb()
        //{
        //    var investimentos_path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\Extratos\yubb.txt";

        //    // Inicia a importação via Selenium
        //    var fundos = new List<Yubb>();
        //    var classes = new List<Type>();

        //    // Renda Fixa
        //    classes.Add(typeof(Yubb_Tesouro_Direto));

        //    // Renda Variável
        //    //classes.Add(typeof(Yubb_Renda_Variavel));

        //    //// Fundos de Investimento
        //    //classes.Add(typeof(Yubb_Fundo_Investimento));

        //    //object locker = new object();

        //    //Parallel.ForEach(classes, classe =>
        //    //{
        //    //    var imported = (List<Investimento>)classe.GetMethod("Sync").Invoke(null, null);

        //    //    lock (locker)
        //    //    {
        //    //        fundos.AddRange(imported);
        //    //    }
        //    //});

        //    //if (fundos.Count > 0)
        //    //{
        //    //    // Apaga o arquivo
        //    //    if (File.Exists(investimentos_path)) File.Delete(investimentos_path);

        //    //    fundos = fundos.OrderByDescending(o => o.mes).ThenBy(o => o.tipo).ThenBy(o => o.nome).ToList();

        //    //    // Cria o arquivo
        //    //    using (var writer = new StreamWriter(investimentos_path, false))
        //    //    {
        //    //        foreach (var investimento in fundos)
        //    //        {
        //    //            writer.WriteLine(investimento.tipo + "|" +
        //    //                             investimento.nome + "|" +
        //    //                             investimento.mes.ToString("dd/MM/yyyy") + "|" +
        //    //                             investimento.valor.ToString());
        //    //        }
        //    //    }
        //    //}
        //}

        //private static void ReadInvestimento_Old()
        //{
        //    var fundos_path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\Extratos\fundos.txt";
        //    var historico_path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\Extratos\historico.txt";

        //    if (!File.Exists(fundos_path)) throw new Exception("Arquivo fundos.txt não encontrado");
        //    if (File.Exists(historico_path)) File.Delete(historico_path);

        //    // Lê o arquivo fundos.txt e armazena em variável
        //    var fundos = new Dictionary<string, DateTime>();

        //    using (var reader = new StreamReader(fundos_path))
        //    {
        //        while (reader.Peek() >= 0)
        //        {
        //            var linha = reader.ReadLine();

        //            if (linha.Trim() != "")
        //            {
        //                var arr = linha.Split(Convert.ToChar("|"));
        //                if (arr[1] != "") fundos.Add(arr[0], Convert.ToDateTime(arr[1]));
        //            }
        //        }
        //    }


        //    // Inicia a importação via Selenium
        //    var historicos = new List<Fundo>();
        //    var classes = new List<Type>();

        //    classes.Add(typeof(Itau_PF_CC));
        //    classes.Add(typeof(ItauPersonnalite_PF_CC));
        //    classes.Add(typeof(Bradesco_PF));
        //    classes.Add(typeof(Master_Credicard_Itau));

        //    var aParams = new object[] { fundos };
        //    object locker = new object();

        //    Parallel.ForEach(classes, classe =>
        //    {
        //        var imported = (List<Fundo>)classe.GetMethod("Sync").Invoke(null, aParams);

        //        lock (locker)
        //        {
        //            historicos.AddRange(imported);
        //        }
        //    });

        //    if (historicos.Count > 0)
        //    {
        //        // Apaga o arquivo historico.txt
        //        if (File.Exists(historico_path)) File.Delete(historico_path);

        //        // Cria o arquivo historico.txt
        //        using (var writer = new StreamWriter(historico_path, false))
        //        {
        //            foreach (var historico in historicos)
        //            {
        //                writer.WriteLine(historico.conta + "|" +
        //                                 historico.data.ToString("dd/MM/yyyy") + "|" +
        //                                 historico.descricao + "|" +
        //                                 historico.credito.ToString() + "|" +
        //                                 historico.debito.ToString() + "|" +
        //                                 historico.saldo.ToString());
        //            }
        //        }
        //    }
        //}



    }
}
