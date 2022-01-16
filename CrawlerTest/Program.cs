using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace CrawlerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            Actions action = new Actions(driver);

            string url = "https://www.lme.com/en/Metals/EV/LME-Molybdenum-Platts#Price+graph";
            //  string url = "https://www.lme.com/en/Metals/EV/LME-Cobalt-Fastmarkets-MB#Price+graph";

            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //10秒內載完網頁所有元素

            driver.Manage().Window.Maximize();
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 1100);");

            // 等chart loading 結束
            new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                .Until(drv => drv.FindElement(By.XPath("//*[@id='dataset-tab-3']//canvas")));

            Thread.Sleep(2000);
            // 關掉廣告
            try
            {
                var closeTab = driver.FindElement(By.XPath("//*[@id='onetrust-close-btn-container']//button"));
                action = new Actions(driver);
                action.MoveToElement(closeTab).Click().Build().Perform();
            }
            catch (Exception ex) { }

            // Key區間日期
            //Thread.Sleep(1000);
            //var input = driver.FindElements(By.Name("start-date"))[2];
            //input.Click();
            //Thread.Sleep(5000);
            //action = new Actions(driver);
            //action.SendKeys(input, "20220101").Perform();
            //Thread.Sleep(50);
            //input = driver.FindElements(By.Name("end-date"))[2];
            //input.Click();
            //action = new Actions(driver);
            //action.SendKeys(input, "20220106").Build().Perform();

            // select 
            new SelectElement(driver.FindElement(By.XPath("//*[@class='chart-controls__label-text']/following-sibling::select")))
                .SelectByText("Month 4", true);

            // 按下update
            //Thread.Sleep(500);
            //driver.FindElements(By.XPath("//*[@type='submit']"))[4].Click();
            //Thread.Sleep(500);

            //var chartElement = driver.FindElement(By.ClassName("chart__main"));
            var chartElement = driver.FindElement(By.XPath("//*[@id='dataset-tab-3']//canvas"));

            // 按下update後 等待 chart 更新完成
            for (int i = 0; i < 20; ++i)
            {
                Thread.Sleep(500);
                try
                {
                    action = new Actions(driver);
                    action.MoveToElement(chartElement, 60, 22, MoveToElementOffsetOrigin.TopLeft).Build().Perform();
                    break;
                }
                catch (Exception ex) { }
            }

            Console.WriteLine($"chartElement => X:{chartElement.Location.X} Y:{chartElement.Location.Y}");
            Console.WriteLine($"chartElement => Width:{chartElement.Size.Width} Height:{chartElement.Size.Height}");
            Console.WriteLine($"labe = {chartElement.GetAttribute("aria-label")} ");

            //hover 唯一的點 跳出tooltip info
            for (int i = 1; i < 80; ++i)
            {
                action = new Actions(driver);
                // 第一個點 較少位數
                //action.MoveToElement(chartElement, 60, i * 5, MoveToElementOffsetOrigin.TopLeft).Build().Perform();
                // 第一個點 較多位數
                //action.MoveToElement(chartElement, 81, i * 5, MoveToElementOffsetOrigin.TopLeft).Build().Perform();
                // 最後一個點
                action.MoveToElement(chartElement, 677, i * 5, MoveToElementOffsetOrigin.TopLeft).Build().Perform();

                int theadChildCount = Convert.ToInt32(driver.FindElement(By.XPath("//*[@class='chart-tooltip__inner']//thead")).GetAttribute("childElementCount"));
                if (theadChildCount > 0)
                {
                    var dateElement = driver.FindElement(By.XPath("//*[@class='chart-tooltip__inner']//th"));
                    var priceElement = driver.FindElements(By.XPath("//*[@class='chart-tooltip__cell']//span"))[1];
                    Console.WriteLine($"date = { dateElement.Text }  preice= { priceElement.Text }");
                    break;
                }

                Console.WriteLine($"y座標移動到 { i }");
            }

            Console.WriteLine("Finish");
        }
    }
}
