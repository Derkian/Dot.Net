using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp1.Fundos
{
    public static class ETF
    {
        public static void Sync()
        {
            var Lista = new List<KeyValuePair<string, string>>();

            Lista.Add(new KeyValuePair<string, string>("GOVE11", "https://br.investing.com/etfs/it-now-igct-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("BOVA11", "https://br.investing.com/etfs/ishares-ibovespa-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("PIBB11", "https://br.investing.com/etfs/it-now-pibb-ibrx-50-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("BOVV11", "https://br.investing.com/etfs/it-now-ibovespa-fundo-de-indice-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("SMAL11", "https://br.investing.com/etfs/ishares-bm-fbovespa-small-cap-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("IVVB11", "https://br.investing.com/etfs/fundo-de-invest-ishares-sp-500-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("XPML11", "https://br.investing.com/etfs/xp-malls-fdo-inv-imob-fii-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("MALL11", "https://br.investing.com/etfs/fii-malls-bp-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("ISUS11", "https://br.investing.com/etfs/it-now-ise-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("ECOO11", "https://br.investing.com/etfs/ishares-carbon-efficient-brasil-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("DIVO11", "https://br.investing.com/etfs/it-now-idiv-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("UBSR11", "https://br.investing.com/etfs/fundo-inv-imob-fii-ubs-br-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("FIND11", "https://br.investing.com/etfs/it-now-ifnc-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("SPXI11", "https://br.investing.com/etfs/it-now-sp500-trn-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("BRAX11", "https://br.investing.com/etfs/ishares-ibrx-brazil---ibrx100-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("BBSD11", "https://br.investing.com/etfs/bb-sp-dividendos-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("MATB11", "https://br.investing.com/etfs/it-now-imat-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("BZLI11", "https://br.investing.com/etfs/brazil-realty-fundo-inv-fii-historical-data"));
            Lista.Add(new KeyValuePair<string, string>("XBOV11", "https://br.investing.com/etfs/caixa-ibovespa-historical-data"));

            Parallel.ForEach(Lista, (l) =>
            {
                Sync(l.Key, l.Value);
            });
        }

        private static void Sync(string Nome, string Url)
        {
            IWebDriver driver = null;

            try
            {
                UI.StartChrome(ref driver);

                driver.Url = Url;
                UI.WaitPageLoad(driver);

                // Clica no combo de período, troca de "Diário" para "Mensal"
                var combo = driver.FindElement(By.XPath("//*[@id=\"data_interval\"]"));
                var options = new SelectElement(combo);
                options.SelectByText("Mensal");
                UI.Wait(3);

                // Lê a tabela
                IList<IWebElement> trs = driver.FindElements(By.XPath("//*[@id=\"curr_table\"]/tbody/tr"));
                var lstHistorico = new List<Historico>();

                using (var context = new MoneyEntities())
                {
                    var Fundo = (from a in context.Fundo where a.Categoria == "ETF" && a.Nome == Nome select a).SingleOrDefault();

                    if (Fundo == null)
                    {
                        Fundo = new Database.Fundo();
                        Fundo.Categoria = "ETF";
                        Fundo.Nome = Nome;
                        Fundo.Tipo = "Renda Variável";
                        Fundo.URL = Url;
                        context.Fundo.Add(Fundo);
                        context.SaveChanges();
                    }
                    else
                    {
                        context.Database.ExecuteSqlCommand("DELETE FROM Historico WHERE FundoID = " + Fundo.FundoID.ToString());
                    }

                    foreach (var tr in trs)
                    {
                        var tds = tr.FindElements(By.TagName("td"));
                        var Historico = new Historico();
                        Historico.Fundo = Fundo;
                        Historico.Data = DateTime.ParseExact(tds[0].Text, "MMM yy", System.Globalization.CultureInfo.CurrentCulture);
                        Historico.Rendimento = Convert.ToDecimal(tds[6].Text.Replace("%", ""));
                        lstHistorico.Add(Historico);
                    }

                    context.Historico.AddRange(lstHistorico);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (driver != null)
                {
                    try { driver.Close(); } catch { }
                    driver.Quit();
                    driver.Dispose();
                    driver = null;
                }
            }
        }
    }
}
