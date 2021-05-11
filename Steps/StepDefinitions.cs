using System.IO;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace E2E_Selenium_Testing.Steps
{
    [Binding]
    public sealed class StepDefinitions
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js;
        private readonly ScenarioContext _scenarioContext;
        private int[] rowData;
        private string arrayChallengeIndex;
        static Process p;

        public StepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeFeature]
        static void IStartTheApplication()
        { 
            string projectDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.Parent.Parent.FullName;
            Directory.SetCurrentDirectory(projectDir);
            ProcessStartInfo ps = new ProcessStartInfo("cmd.exe");
            ps.Arguments = "/K yarn && yarn start";
            p = Process.Start(ps);   
        }

        [BeforeScenario]
        public void GetStarted()
        {
            if (driver == null)
                driver = new ChromeDriver();
            if (js == null)
                js = (IJavaScriptExecutor)driver;

        }
        
 

        [Given("the test page (.*) is showing")]
        public void TheTestPageIsShowing(string testPageURL)
        {
            //if(driver == null)
            //     driver = new ChromeDriver();
            if (driver.Url != "http://localhost:3000/")
                driver.Navigate().GoToUrl("http://localhost:3000/");

            WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("home")));
        }

        [When("I click on the Render The Challenge button")]
        public void WhenIClickRenderChallenge()
        {
            // click the Render The Challenge Button
            driver.FindElement(By.XPath("//*[@id='home']/div/div/button")).Click();
        }

        [Then("I see the Arrays Challenge")]
        public void ThenISeeTheArraysChallenge()
        {
            //WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            //waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.(By.ClassName("challenge")));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            long oldPosition = (long)js.ExecuteScript("return pageYOffset");
            long newPosition = 0;
            bool atEnd = false;
            while(atEnd == false)
            {
                System.Threading.Thread.Sleep(1000);

                newPosition = (long)js.ExecuteScript("return pageYOffset");
                if (oldPosition == newPosition)
                    atEnd = true;
                else
                    oldPosition = newPosition;                
            }
            
        }


        [When("I get the centre cell of row (\\d+) of the array challenge")]
        public void WhenIGetTheCentreCell(int rowNumber)
        {
            if (driver.Url != "http://localhost:3000/")
            {
                driver.Navigate().GoToUrl("http://localhost:3000/");

                WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("home")));
                driver.FindElement(By.XPath("//*[@id='home']/div/div/button")).Click();
               
                waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("challenge")));
            }
            if(driver.FindElements(By.ClassName("challenge")).Count == 0)
            {
                driver.FindElement(By.XPath("//*[@id='home']/div/div/button")).Click();
                WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("challenge")));
            }

            rowData = new int[9];

            arrayChallengeIndex = "Null";
            
            var rows = driver.FindElements(By.TagName("tr"));

            var rowCells = rows[rowNumber - 1].FindElements(By.TagName("td"));
            for (int index = 0; index < rowCells.Count; index++)
            {
                rowData[index] = Convert.ToInt32(rowCells[index].Text);
            }

            int leftTotal;
            int rightTotal;

            for (int indexPosition = 1; indexPosition < 8; indexPosition++)
            {
                leftTotal = 0;
                rightTotal = 0;
                for (int currentCell = 0; currentCell < indexPosition; currentCell++)
                {
                    leftTotal = leftTotal + Convert.ToInt32(rowData[currentCell]);
                }

                for (int currentCell = 8; currentCell > indexPosition; currentCell--)
                {
                    rightTotal = rightTotal + Convert.ToInt32(rowData[currentCell]);
                    if (rightTotal > leftTotal)
                        break;
                }

                if (rightTotal == leftTotal)
                {
                    arrayChallengeIndex = Convert.ToString(indexPosition);
                    break;
                }
            }
        }

        [When("I enter the index in field (\\d+)")]
        public void WhenIEnterIndexInTextField(int fieldNumber)
        {
            if (arrayChallengeIndex != "")
                driver.FindElement(By.XPath("//input[@data-test-id = 'submit-" + fieldNumber.ToString() + "']")).SendKeys(arrayChallengeIndex);
            else
                driver.FindElement(By.XPath("//input[@data-test-id = 'submit-" + fieldNumber.ToString() + "']")).SendKeys("null");
        }

        [When("I enter (.*) into the name field")]
        public void WhenIPopulateNameField(string name)
        {
            driver.FindElement(By.XPath("//input[@data-test-id = 'submit-4']")).SendKeys(name);
        }

        [When("I submit the answers")]
        public void WhenISubmitTheAnswers()
        {
            driver.FindElement(By.XPath("//*[@id='challenge']/div/div/div[2]/div/div[2]/button/div/div/span")).Click();
        }

        [Then("I see a completion message with the words (.*)")]
        public void ThenISeeCompletionMessage(string completionMessage)
        {
            driver.SwitchTo().ActiveElement();

            WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[text()='" + completionMessage + "']")));
            
            Assert.Greater(driver.FindElements(By.XPath("//div[text()='" + completionMessage + "']")).Count, 0, "Congratulations message wasn't found");

            driver.FindElement(By.XPath("/html/body/div[2]/div/div[1]/div/div/div[2]/button")).Click();

            driver.SwitchTo().ActiveElement();
        }

        [AfterScenario]
        public void TidyUp()
        {
            driver.Quit();
        }

        [AfterFeature]
        static void IStopTheApplication()
        {
            p.Kill();
        }
    }
}
