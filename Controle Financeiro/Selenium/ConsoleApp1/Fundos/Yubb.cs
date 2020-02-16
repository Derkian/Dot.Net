//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Database;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Support.UI;

//namespace ConsoleApp1.Fundos
//{
//    public static class Yubb
//    {
//        public static void Sync(ref int erros)
//        {
//            IWebDriver driver = null;

//            var Tabs = new List<KeyValuePair<string, string>>();
//            var Lista = new List<KeyValuePair<string, string>>();

//            Tabs.Add(new KeyValuePair<string, string>("Renda Fixa", "//*[@id=\"categoryTabs\"]/li[1]/a"));
//            Tabs.Add(new KeyValuePair<string, string>("Renda Variável", "//*[@id=\"categoryTabs\"]/li[2]/a"));
//            Tabs.Add(new KeyValuePair<string, string>("Fundo de Investimento", "//*[@id=\"categoryTabs\"]/li[3]/a"));

//            foreach (var tab in Tabs)
//            {
//                GetURL(ref driver, ref Lista, tab.Key, tab.Value);
//            }

//            //Parallel.ForEach(Tabs, (l) =>
//            //{
//            //    GetURL(ref Lista, l.Key, l.Value);
//            //});

//            //foreach (var l in Lista)
//            //{
//            //    Sync(l.Key, l.Value);
//            //}

//            //Parallel.ForEach(Lista, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (l) =>
//            //{
//            //    Sync(l.Key, l.Value);
//            //});


//            var Blacklist = new List<string>();

//            //Blacklist.Add("sorocred-financeira-ourinvest-116-cdi-lc-365-dias-1000-minimo");
//            //Blacklist.Add("sorocred-financeira-daycoval-116-cdi-lc-361-dias-10000-minimo");



//            try
//            {
//                using (var context = new MoneyEntities())
//                {
//                    foreach (var l in Lista)
//                    {
//                        try
//                        {
//                            string Tipo = l.Key;
//                            string Url = l.Value;

//                            if ((from a in Blacklist where Url.Contains(a) select a).FirstOrDefault() != null) continue;
//                            if ((from a in context.Fundo where a.URL == Url select a).SingleOrDefault() != null) continue;

//                            if (driver == null) UI.StartChrome(ref driver);
//                            driver.Url = Url;
//                            UI.WaitPageLoad(driver);
//                            //UI.Wait(4);

//                            var Nome = driver.FindElement(By.ClassName("pageHeader__titleHolder")).FindElement(By.TagName("h1")).Text;
//                            var Categoria = driver.FindElement(By.ClassName("pageHeader__titleHolder")).FindElement(By.TagName("h2")).Text;

//                            Nome = Nome.Replace("Fundo De Investimento Em Cotas De Fundos De Investimento Em Ações", "");
//                            Nome = Nome.Replace("Fundo De Investimento Em Cotas De Fundos De Investimento De Ações", "");
//                            Nome = Nome.Replace("Fundo De Investimento Em Ações", "");
//                            Nome = Nome.Replace("Fundo De Investimento Em Cotas De Fundos De Investimento Multimercado", "");
//                            Nome = Nome.Replace("Fundo De Investimento Em Ações", "");
//                            Nome = Nome.Replace("Fundo De Investimento Em Cotas De Fundos De Investimento Multimercado Access", "");
//                            Nome = Nome.Replace("Fundo De Investimento Em Quotas De Fundos De Investimento De Ações", "");
//                            Nome = Nome.Replace("Fundo De Investimento De Ações", "");
//                            Nome = Nome.Replace("Fiq De Fundos De Investimento De Ações", "");
//                            Nome = Nome.Replace("Fundo De Investimento Em Quotas De Fundos De Investimento Multimercado", "");
//                            Nome = Nome.Replace("Fundo De Investimento Multimercado", "");
//                            Nome = Nome.Trim();

//                            Categoria = Categoria.Replace("Certificado de Depósito Bancário - CDB", "CDB");
//                            Categoria = Categoria.Replace("Letra de Crédito Imobiliário - LCI", "LCI");
//                            Categoria = Categoria.Replace("Letra de Câmbio - LC", "LC");
//                            Categoria = Categoria.Trim();

//                            var Fundo = (from a in context.Fundo where a.Nome == Nome && a.Tipo == Tipo && a.Categoria == Categoria select a).SingleOrDefault();

//                            if (Fundo == null)
//                            {
//                                Fundo = new Database.Fundo();
//                                Fundo.Nome = Nome;
//                                Fundo.Tipo = Tipo;
//                                Fundo.Categoria = Categoria;
//                                context.Fundo.Add(Fundo);
//                                context.SaveChanges();
//                            }

//                            if (UI.ElementExist(driver, By.ClassName("tableDetails__grossAnnualYield")))
//                                Fundo.RentabilidadeBruta1Ano = Convert.ToDecimal(driver.FindElement(By.ClassName("tableDetails__grossAnnualYield")).Text.Replace("%", "").Replace("ao ano", ""));
//                            else if (UI.ElementExist(driver, By.ClassName("tableDetails__grossHistoricYield")))
//                                Fundo.RentabilidadeBruta1Ano = Convert.ToDecimal(driver.FindElement(By.ClassName("tableDetails__grossHistoricYield")).Text.Replace("%", "").Replace("ao ano", ""));

//                            if (UI.ElementExist(driver, By.ClassName("tableDetails__netAnnualYield")))
//                                Fundo.RentabilidadeLiquida1Ano = Convert.ToDecimal(driver.FindElement(By.ClassName("tableDetails__netAnnualYield")).Text.Replace("%", "").Replace("ao ano", ""));
//                            else if (UI.ElementExist(driver, By.ClassName("tableDetails__netHistoricYield")))
//                                Fundo.RentabilidadeLiquida1Ano = Convert.ToDecimal(driver.FindElement(By.ClassName("tableDetails__netHistoricYield")).Text.Replace("%", "").Replace("ao ano", ""));

//                            Fundo.IR = Convert.ToDecimal(driver.FindElement(By.ClassName("tableDetails__incomeTaxRate")).Text.Replace("%", "").Replace("ao ano", ""));
//                            Fundo.InvestimentoMinimo = Convert.ToDecimal(driver.FindElement(By.XPath("//*[contains(text(),'Investimento Mínimo')]")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.TagName("td")).Text.Replace("R$", ""));
//                            Fundo.Liquidez = driver.FindElement(By.XPath("//*[contains(text(),'Liquidez')]")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.TagName("td")).Text;
//                            Fundo.Distribuidor = driver.FindElement(By.XPath("//*[contains(text(),'Distribuidor')]")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.TagName("td")).Text;

//                            if (UI.ElementExist(driver, By.ClassName("tableDetails__administrationTaxRate")))
//                                Fundo.TaxaAdmAno = Convert.ToDecimal(driver.FindElement(By.ClassName("tableDetails__administrationTaxRate")).Text.Replace("%", "").Replace("ao ano", ""));

//                            if (UI.ElementExist(driver, By.XPath("//*[contains(text(),'Emissor')]")))
//                                Fundo.Emissor = driver.FindElement(By.XPath("//*[contains(text(),'Emissor')]")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.TagName("td")).Text;

//                            if (UI.ElementExist(driver, By.XPath("//*[contains(text(),'Gestor')]")))
//                                Fundo.Gestor = driver.FindElement(By.XPath("//*[contains(text(),'Gestor')]")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.TagName("td")).Text;

//                            if (UI.ElementExist(driver, By.XPath("//*[contains(text(),'Administrador')]")))
//                                Fundo.Administrador = driver.FindElement(By.XPath("//*[contains(text(),'Administrador')]")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.TagName("td")).Text;

//                            if (UI.ElementExist(driver, By.XPath("//*[contains(text(),'Corretora')]")))
//                            {
//                                Fundo.Corretora = driver.FindElement(By.XPath("//*[contains(text(),'Corretora')]")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.TagName("td")).Text;
//                                if (Fundo.Corretora.Contains("(")) Fundo.Corretora = Fundo.Corretora.Substring(0, Fundo.Corretora.IndexOf("(")).Trim();
//                            }

//                            if (UI.ElementExist(driver, By.ClassName("tableDetails__custodyTaxRate")))
//                                Fundo.TaxaCustodiaAno = Convert.ToDecimal(driver.FindElement(By.ClassName("tableDetails__custodyTaxRate")).Text.Replace("%", "").Replace("ao ano", ""));

//                            Fundo.URL = Url;
//                            context.SaveChanges();


//                            // Histórico
//                            context.Database.ExecuteSqlCommand("DELETE FROM Historico WHERE FundoID = " + Fundo.FundoID.ToString());
//                            decimal RendimentoMensal = Convert.ToDecimal(Fundo.RentabilidadeBruta1Ano) / 12;

//                            for (int i = 1; i <= 24; i++)
//                            {
//                                var Historico = new Database.Historico();
//                                Historico.Fundo = Fundo;
//                                Historico.Data = new DateTime(DateTime.Today.AddMonths(-i).Year, DateTime.Today.AddMonths(-i).Month, 1);
//                                Historico.Rendimento = RendimentoMensal;
//                                context.Historico.Add(Historico);
//                            }

//                            context.SaveChanges();

//                        }
//                        catch (Exception ex)
//                        {
//                            if (driver != null)
//                            {
//                                try { driver.Close(); } catch { }
//                                driver.Quit();
//                                driver.Dispose();
//                                driver = null;
//                            }

//                            erros += 1;
//                        }
//                        finally
//                        {
//                            //if (driver != null)
//                            //{
//                            //    try
//                            //    {
//                            //        driver.Close();
//                            //    }
//                            //    catch (Exception ex2)
//                            //    {
//                            //        var aa = false;
//                            //    }
//                            //    driver.Quit();
//                            //    driver.Dispose();
//                            //    driver = null;
//                            //}
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            finally
//            {
//                if (driver != null)
//                {
//                    try { driver.Close(); } catch { }
//                    driver.Quit();
//                    driver.Dispose();
//                    driver = null;
//                }
//            }

//        }


//        private static void GetURL(ref IWebDriver driver, ref List<KeyValuePair<string, string>> Lista, string Tipo, string XPathTab)
//        {
//            try
//            {
//                if (driver == null)
//                    UI.StartChrome(ref driver);

//                driver.Url = "https://yubb.com.br";
//                UI.WaitPageLoad(driver);

//                // Valor e meses
//                UI.SetTextBoxValue(driver, By.XPath("//*[@id=\"principal\"]"), "1000000");
//                UI.SetTextBoxValue(driver, By.XPath("//*[@id=\"months\"]"), "12");
//                UI.Click(driver, By.XPath("/html/body/div[3]/div/section[2]/div/div[2]/div/form/button"));

//                // ------------------------------------------------------------------------------------------------------------------------------
//                // Tela: Busca
//                // ------------------------------------------------------------------------------------------------------------------------------

//                UI.WaitPageLoad(driver);
//                UI.Wait(1);

//                // Clica na aba
//                UI.Click(driver, By.XPath(XPathTab));
//                UI.Wait(1);

//                // Lê as URLs
//                var Paginacao = driver.FindElements(By.XPath("//*[@id=\"collection__container\"]/div[3]/nav/span"));
//                IWebElement Ultima = null;

//                if (Paginacao.Count > 0)
//                    Ultima = Paginacao[Paginacao.Count - 2];

//                var TemPaginacao = Ultima != null && Ultima.Displayed;
//                var Paginas = TemPaginacao ? Convert.ToInt32(Ultima.Text) : 1;
//                int Atual = 1;

//                do
//                {
//                    var divs = driver.FindElements(By.XPath("//*[@id=\"collection__container\"]/div[2]/div"));

//                    foreach (var div in divs)
//                    {
//                        var url = div.FindElement(By.TagName("a")).GetAttribute("href");
//                        Lista.Add(new KeyValuePair<string, string>(Tipo, url));
//                    }

//                    Atual += 1;
//                    //Atual += 999999;

//                    if (TemPaginacao && Atual <= Paginas)
//                    {
//                        UI.Click(driver, By.XPath("//*[@aria-label='próximo']"));
//                        UI.Wait(2);
//                    }

//                }
//                while (Atual <= Paginas);

//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            finally
//            {
//                //if (driver != null)
//                //{
//                //    try { driver.Close(); } catch { }
//                //    driver.Quit();
//                //    driver.Dispose();
//                //    driver = null;
//                //}
//            }
//        }

//        //private static void Sync(string Tipo, string Url)
//        //{

//        //}

//    }
//}
