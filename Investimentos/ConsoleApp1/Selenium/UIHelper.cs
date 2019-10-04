using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;
using System.Drawing;

public static class UI
{
    public static void StartChrome(ref IWebDriver driver, int index = 0)
    {
        var options = new ChromeOptions();
        options.AddArguments("--incognito --start-maximized --no-sandbox");
        driver = new ChromeDriver(options);

        //options.AddArguments("--start-maximized");
        //var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        //driver.Manage().Window.Size = new Size(screen.Width, screen.Height);
        //driver.Manage().Window.Position = new Point(0, 0);
        //var w = screen.Width / 4;
        //driver.Manage().Window.Size = new Size(w, screen.Height);
        //driver.Manage().Window.Position = new Point(w  * ( index-1), 0);
    }

    public static void ChangeAttribute(IWebDriver driver, IJavaScriptExecutor executor, IWebElement el, string attribute, string value)
    {
        string[] textArray1 = new string[] { "arguments[0].setAttribute('", attribute, "', '", value, "')" };
        object[] args = new object[] { el };
        executor.ExecuteScript(string.Concat(textArray1), args);
    }

    public static void Click(IWebDriver driver, By by)
    {
        driver.FindElement(by).Click();
        UI.Wait(2);
    }

    public static void Click(IWebDriver driver, IWebElement el)
    {
        el.Click();
    }

    public static void ClickWhenClickable(IWebDriver driver, By by, int seconds = 15)
    {
        WaitForClickable(driver, by, seconds).Click();
    }

    public static IWebElement ClickWhenDisplayed(IWebDriver driver, By by, int seconds = 15)
    {
        IWebElement element = WaitForDisplayed(driver, by, seconds);
        element.Click();
        return element;
    }

    public static void ClickWithJavascript(IWebDriver driver, IJavaScriptExecutor executor, By by)
    {
        IWebElement btnF = driver.FindElement(by);
        ClickWithJavascript(driver, executor, btnF);
    }

    public static void ClickWithJavascript(IWebDriver driver, IJavaScriptExecutor executor, IWebElement btnF)
    {
        object[] args = new object[] { btnF };
        executor.ExecuteScript("arguments[0].click();", args);
    }

    public static void Blur(IWebDriver driver, IJavaScriptExecutor executor, IWebElement btnF)
    {
        object[] args = new object[] { btnF };
        executor.ExecuteScript("document.getElementById('" + btnF.GetAttribute("id") + "').blur();", args);
    }

    public static bool ComboContains(SelectElement select, string expectedValue)
    {
        IList<IWebElement> options = select.Options;
        List<string> optionsText = options.Select(a => a.Text).ToList();
        return optionsText.Contains(expectedValue);
    }

    public static void ClickWithPerform(IWebDriver driver, IWebElement el)
    {
        Actions actions = new Actions(driver);
        actions.MoveToElement(el).Click().Build().Perform();
    }

    public static bool ElementExist(IWebDriver driver, By by)
    {
        //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

        try
        {
            driver.FindElement(by);
            return true;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
        finally
        {
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        }
    }

    public static bool IsDate(string s)
    {
        DateTime time;
        return DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
    }

    public static bool IsDisplayed(IWebDriver driver, By by)
    {
        try
        {
            return driver.FindElement(by).Displayed;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsDisplayed(IWebDriver driver, IWebElement el, By by)
    {
        try
        {
            return el.FindElement(by).Displayed;
        }
        catch
        {
            return false;
        }
    }

    public static bool Exists(IWebDriver driver, string elementID)
    {
        return driver.PageSource.Contains(elementID); ;
    }

    public static void SaveHtmlAndPrint(IWebDriver driver, string filename, Exception ex = null)
    {
        if (ex != null)
        {
            string str2 = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\Error\" + filename + "_[error].txt";
            
            if (File.Exists(str2)) { File.Delete(str2); }
            using (StreamWriter writer2 = new StreamWriter(str2, false))
            {
                writer2.WriteLine(ex.Message);
                writer2.WriteLine(ex.StackTrace);
            }
        }
        if (driver == null) return;

        if (!string.IsNullOrEmpty(driver.PageSource))
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\Error\" + filename + ".html";
            if (File.Exists(path)) { File.Delete(path); }
            using (StreamWriter writer = new StreamWriter(path, false)) { writer.Write(driver.PageSource); }
        }

        string str3 = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\Error\" + filename + ".png";
        if (File.Exists(str3)) { File.Delete(str3); }
        Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
        string str4 = screenshot.AsBase64EncodedString;
        byte[] asByteArray = screenshot.AsByteArray;
        screenshot.SaveAsFile(str3, ScreenshotImageFormat.Png);
        screenshot.ToString();
    }

    public static void ScrollDown(IWebDriver driver)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", Array.Empty<object>());
    }

    public static void SetTextBoxValue(IWebDriver driver, By elementLocator, string text)
    {
        IWebElement element = driver.FindElement(elementLocator);
        element.Clear();
        if (!string.IsNullOrEmpty(text))
        {
            element.SendKeys(text);
        }
    }

    public static void Wait(double seconds)
    {
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
    }

    public static void WaitAndClick(IWebDriver driver, double seconds, By elementLocator)
    {
        Wait(seconds);
        driver.FindElement(elementLocator).Click();
    }

    public static IWebElement WaitForClickable(IWebDriver driver, By by, int seconds = 15)
    {
        WaitForDisplayed(driver, by, seconds);
        WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, seconds));
        Type[] exceptionTypes = new Type[] { typeof(NoSuchElementException), typeof(ElementNotVisibleException) };
        wait.IgnoreExceptionTypes(exceptionTypes);
        IWebElement element = wait.Until<IWebElement>(ExpectedConditions.ElementExists(by));
        wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, seconds));
        return wait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable(element));
    }

    //public static void WaitAndClick(IWebDriver driver, IWebElement el, int seconds = 15)
    //{
    //    WaitForDisplayed(driver, by, seconds);
    //    WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, seconds));
    //    Type[] exceptionTypes = new Type[] { typeof(NoSuchElementException), typeof(ElementNotVisibleException) };
    //    wait.IgnoreExceptionTypes(exceptionTypes);
    //    IWebElement element = wait.Until<IWebElement>(ExpectedConditions.ElementExists(by));
    //    wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, seconds));
    //    return wait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable(element));
    //}

    public static IWebElement WaitForExists(IWebDriver driver, By by, int seconds = 15)
    {
        WaitForDisplayed(driver, by, seconds);
        WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, seconds));
        Type[] exceptionTypes = new Type[] { typeof(NoSuchElementException), typeof(ElementNotVisibleException) };
        wait.IgnoreExceptionTypes(exceptionTypes);
        return wait.Until<IWebElement>(ExpectedConditions.ElementExists(by));
    }

    public static IWebElement WaitForDisplayed(IWebDriver driver, By by, int seconds = 15)
    {
        try
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, seconds));
            Type[] exceptionTypes = new Type[] { typeof(NoSuchElementException), typeof(ElementNotVisibleException) };
            wait.IgnoreExceptionTypes(exceptionTypes);
            return wait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable(by));
        }
        catch (Exception ex)
        {
            throw new Exception("Elemento não encontrado: " + by.ToString(), ex);
        }
    }

    public static void WaitUntilDisappear(IWebDriver driver, By by, int seconds = 5)
    {
        WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, seconds));
        Type[] exceptionTypes = new Type[] { typeof(NoSuchElementException), typeof(ElementNotVisibleException) };
        wait.IgnoreExceptionTypes(exceptionTypes);
        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(by));
    }

    public static void WaitIFrameLoad(IWebDriver driver, string frameName, int seconds = 15)
    {
        try
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until<IWebDriver>(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(frameName));
        }
        catch (Exception ex)
        {
            throw new Exception("iFrame não localizado: " + frameName, ex);
        }
    }

    public static void WaitPageLoad(IWebDriver driver)
    {
        Wait(3);
        //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15.0);
    }

    public static IWebElement WaitUntilElementClickable(IWebDriver driver, By elementLocator, int timeout = 15)
    {
        IWebElement e = null;
        new WebDriverWait(driver, TimeSpan.FromSeconds((double)timeout)).Until<IWebElement>(delegate (IWebDriver d)
        {
            IWebElement element = d.FindElement(elementLocator);
            if (element.Displayed && element.Enabled)
            {
                e = element;
                return element;
            }
            return null;
        });
        return e;
    }

    public static IWebElement WaitUntilVisible(IWebDriver driver, By elementLocator, int timeout = 15)
    {
        IWebElement e = null;
        new WebDriverWait(driver, TimeSpan.FromSeconds((double)timeout)).Until<IWebElement>(delegate (IWebDriver d)
        {
            IWebElement element = d.FindElement(elementLocator);
            if (element.Displayed)
            {
                e = element;
                return element;
            }
            return null;
        });
        return e;
    }

    public static string RemoveDoubleSpaces(string s)
    {
        RegexOptions options = RegexOptions.None;
        Regex regex = new Regex("[ ]{2,}", options);
        s = regex.Replace(s, " ");
        return s;
    }

    public static string RemoveLineEndings(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            return value;
        }
        string lineSeparator = ((char)0x2028).ToString();
        string paragraphSeparator = ((char)0x2029).ToString();

        return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
    }
}