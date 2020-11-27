using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using System.Linq;
using System.Net.Http;
using ConsoleApp1.Models;

public static class Personalite_MasterCard
{
    public static List<Extrato> Sync(List<Conta> contas)
    {
        var extratos = new List<Extrato>();
        var cultureBR = new CultureInfo("pt-BR");

        foreach (var conta in (from a in contas where a.classe.ToUpper(CultureInfo.InvariantCulture) == "PERSONALITE_MASTERCARD" select a).ToList())
        {
            IWebDriver driver = null;

            try
            {
                UI.StartChrome(ref driver, 4);

                driver.Url = "https://www.itau.com.br/";
                UI.WaitPageLoad(driver);


                // ------------------------------------------------------------------------------------------------------------------------------
                // Tela: Home
                // ------------------------------------------------------------------------------------------------------------------------------

                UI.SetTextBoxValue(driver, By.XPath("//*[@id=\"agencia\"]"), conta.agencia);
                UI.SetTextBoxValue(driver, By.XPath("//*[@id=\"conta\"]"), conta.conta);
                UI.Wait(1.5);
                UI.Click(driver, By.Id("btnLoginSubmit"));
                UI.WaitPageLoad(driver);
                UI.Wait(2);

                // ------------------------------------------------------------------------------------------------------------------------------
                // Tela: Login
                // ------------------------------------------------------------------------------------------------------------------------------

                var teclado = UI.WaitForDisplayed(driver, By.XPath(".//div[@class='teclas clearfix']"), 20);

                IList<IWebElement> teclas = teclado.FindElements(By.TagName("a"));
                var digitos = new List<KeyValuePair<string, IWebElement>>();
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;

                foreach (char c in conta.senha)
                {
                    foreach (var tecla in teclas)
                    {
                        if (tecla.Text.Contains(" ou ") && tecla.Text.Contains(c.ToString()))
                        {
                            UI.ClickWithJavascript(driver, executor, tecla);
                        }
                    }
                }

                UI.ClickWithJavascript(driver, executor, By.Id("acessar"));
                UI.WaitPageLoad(driver);


                UI.Wait(10);

                // ------------------------------------------------------------------------------------------------------------------------------
                // Recupera o JSON dos lançamentos
                // ------------------------------------------------------------------------------------------------------------------------------

                var url = driver.Url.Replace("#30horas", "");
                var cookies = UI.GetCookies(executor);
                var xauthtoken = cookies.First(x => x.Key == "X-AUTH-TOKEN").Value;
                var op = driver.FindElement(By.XPath("/html/body/header/div[3]/nav/ul/li/div/div/div[2]/ul[2]/li[2]/a")).GetAttribute("data-op");

                string html = UI.GetRequestHtml(HttpMethod.Post, url, xauthtoken, op);
                html = html.Substring(html.IndexOf("self.init", StringComparison.InvariantCulture));
                html = html.Substring(html.IndexOf("urlContingencia", StringComparison.InvariantCulture) + 6);
                html = html.Substring(html.IndexOf("urlContingencia", StringComparison.InvariantCulture) + 19);
                var op2 = html.Substring(0, html.IndexOf("\n", StringComparison.InvariantCulture));
                op2 = op2.Substring(0, op2.LastIndexOf(";", StringComparison.InvariantCulture)).Replace("tingencia = \"", "").Replace("\"", ""); // 15 dias, não vai usar

                var json = UI.GetRequestJson(HttpMethod.Post, url, xauthtoken, op2);
                var dados = json["object"]["data"];

                foreach (var faturas in dados)
                {
                    var fatura = faturas["faturas"];

                    foreach (var faturaFechada in fatura)
                    {
                        if (!UI.JsonNullOrEmpty(faturaFechada["status"]))
                        {
                            if (faturaFechada["status"].ToString().Equals("fechada", StringComparison.InvariantCulture) ||
                                faturaFechada["status"].ToString().Equals("aberta", StringComparison.InvariantCulture))
                            {
                                var dataFechamentoFatura = DateTime.ParseExact(faturaFechada["dataFechamentoFatura"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                var dataVencimentoFatura = DateTime.ParseExact(faturaFechada["dataVencimento"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                                //if (DateTime.Now > dataFechamentoFatura && DateTime.Now < dataVencimentoFatura)
                                //{
                                var titularidades = faturaFechada["lancamentosNacionais"]["titularidades"];

                                foreach (var titularidade in titularidades)
                                {
                                    var lancamentos = titularidade["lancamentos"];

                                    foreach (var lancamento in lancamentos)
                                    {
                                        var data = DateTime.ParseExact(lancamento["data"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                                        var extrato = new Extrato();
                                        extrato.conta = conta.nome;
                                        extrato.data = data;
                                        extrato.descricao = ExtratoHelper.GetDescricao(lancamento["descricao"].ToString());

                                        if (!string.IsNullOrEmpty(extrato.descricao))
                                        {
                                            var valor = decimal.Parse(lancamento["valor"].ToString(), NumberStyles.Currency, cultureBR);

                                            if (lancamento["sinalValor"].ToString().ToUpper(CultureInfo.InvariantCulture) == "+")
                                                extrato.debito = Math.Abs(valor) * -1;
                                            else
                                                extrato.credito = valor;

                                            ExtratoHelper.AddExtrato(ref extratos, extrato);
                                        }
                                    }
                                }
                                //}
                            }
                        }
                    }
                }

                UI.ClickWithJavascript(driver, executor, By.ClassName("mfp-close"));

                UI.Click(driver, By.Id("linkSairHeader")); // Sair
                UI.WaitPageLoad(driver);
                UI.Wait(2);

                UI.Click(driver, By.XPath("//*[@id=\"telaSair\"]/div/div/div[1]/section/div/div[2]/a")); // Confirmar
                UI.WaitPageLoad(driver);
            }
            catch (Exception ex)
            {
                UI.SaveHtmlAndPrint(driver, "Itau", ex);
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

        // Retorna os registros
        return extratos;
    }


}