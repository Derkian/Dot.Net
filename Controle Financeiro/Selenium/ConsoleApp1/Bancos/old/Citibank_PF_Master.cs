//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Support.UI;
//using OpenQA.Selenium.Firefox;

//public static class Citibank_PF_Master
//{
//    public static List<Extrato> Sync(Dictionary<string, DateTime> contas, IWebDriver driver)
//    {
//        var conta = "Citibank Master";
//        var extratos = new List<Extrato>();
//        var cultureBR = new CultureInfo("pt-BR");

//        // Clica no menu "Cartões"
//        UI.ClickWhenClickable(driver, By.Id("link_liCartoesCreditoCBOL"));

//        // Espera o iframe carregar
//        UI.WaitIFrameLoad(driver, "cc_iframe");

//        // Clica na lupa "Ver fatura detalhada"
//        UI.Click(driver, By.LinkText("Ver fatura detalhada"));
//        UI.WaitPageLoad(driver);
//        UI.ScrollDown(driver);

//        // Captura os registros da última fatura
//        ReadRecords(driver, conta, ref extratos, cultureBR);

//        // Loopa as faturas no combobox
//        DateTime fromExcel = contas[conta];
//        DateTime from = new DateTime(fromExcel.Year, fromExcel.Month, 18);
//        DateTime to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 18);

//        while (from <= to)
//        {
//            // Clica no combo de data
//            var combo = driver.FindElement(By.Id("selSrcAcct"));
//            UI.Click(driver, combo);
//            UI.Wait(1);

//            // Verifica se existe o select
//            var options = new SelectElement(combo);
//            var sData = from.ToString("dd/MM/yyyy");

//            // Clica na data e lê os registros
//            if (UI.ComboContains(options, sData))
//            {
//                options.SelectByText(sData);
//                UI.ScrollDown(driver);
//                ReadRecords(driver, conta, ref extratos, cultureBR);
//            }

//            // Incrementa 1 mês
//            from = from.AddMonths(1);
//        }

//        // Retorna os registros
//        return extratos;
//    }

//    private static void ReadRecords(IWebDriver driver, string conta, ref List<Extrato> extratos, CultureInfo cultureBR)
//    {
//        try
//        {
//            UI.WaitUntilVisible(driver, By.ClassName("appCellBorder1"));
//        }
//        catch
//        {
//            return;
//        }

//        UI.Wait(2);
//        IList<IWebElement> trs = driver.FindElement(By.ClassName("appCellBorder1")).FindElements(By.TagName("tr"));

//        foreach (var tr in trs)
//        {
//            IList<IWebElement> tds = tr.FindElements(By.TagName("td"));

//            if (tds.Count >= 3)
//            {
//                var sData = tds[0].Text + "/" + DateTime.Today.Year.ToString();
//                var sDesc = tds[2].Text;

//                if (UI.IsDate(sData))
//                {
//                    var extrato = new Extrato();
//                    extrato.conta = conta;
//                    extrato.data = DateTime.ParseExact(sData, "dd/MM/yyyy", CultureInfo.InvariantCulture);
//                    extrato.descricao = ExtratoHelper.GetDescricao(tds[1].Text);
//                    if (extrato.descricao == null) continue;

//                    if (sDesc.Contains("-"))
//                        extrato.debito = -1 * (decimal?)decimal.Parse(sDesc.Replace("-", ""), NumberStyles.Currency, cultureBR);
//                    else
//                        extrato.credito = (decimal?)decimal.Parse(sDesc.Replace("-", ""), NumberStyles.Currency, cultureBR);

//                    ExtratoHelper.AddExtrato(ref extratos, extrato);
//                }
//            }
//        }
//    }

//}
