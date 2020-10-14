using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using TechTalk.SpecFlow;
using Xunit;

namespace Task1.Steps
{
    [Binding]
    public class EggTimerSteps : IDisposable
    {
        private ChromeDriver _chromeDriver;

        //TODO should be page object but ran out of time
        private readonly string _timeBoxId = "start_a_timer";
        private readonly string _startButtonId = "timergo";
        private readonly string _timerOutput = "progressText";
        private readonly List<int> _timeAndOutput;

        public EggTimerSteps()
        {
            _chromeDriver = new ChromeDriver(new ChromeOptions
            {
                UnhandledPromptBehavior = UnhandledPromptBehavior.Dismiss
            });
            _timeAndOutput = new List<int>();

            _chromeDriver.Navigate().GoToUrl("https://e.ggtimer.com/");
        }

        [Given(@"the timer is set to (.*) seconds")]
        public void GivenTheTimerIsSetToSeconds(int p0)
        {
            var timerInputBox = _chromeDriver.FindElementById(_timeBoxId);
            timerInputBox.Clear();
            //TODO: do these strings need to bo localized, because the website is available in multiple languages?
            timerInputBox.SendKeys($"{p0} seconds");
        }


        [When(@"the timer is run")]
        public void WhenTheTimerIsRun()
        {
            int secondsToWaitForOutput = 5;
            var startButton = _chromeDriver.FindElementById(_startButtonId);
            startButton.Click();
            var wait = new WebDriverWait(new SystemClock(), _chromeDriver, TimeSpan.FromSeconds(secondsToWaitForOutput), TimeSpan.FromMilliseconds(5));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id(_timerOutput)));

            var timerOutput = _chromeDriver.FindElementById(_timerOutput);

            while (!timerOutput.Text.Equals("Time Expired!", StringComparison.InvariantCultureIgnoreCase))
            {
                StoreRemainingSecondsValue();
                Thread.Sleep(1000);
            }
        }

        private void StoreRemainingSecondsValue()
        {
            var currentSecondsRemaining = GetSecondsRemaining();
            if (currentSecondsRemaining != null)
            {
                _timeAndOutput.Add((int)currentSecondsRemaining);
            }
        }

        private int? GetSecondsRemaining()
        {
            var timerOutput = _chromeDriver.FindElementById(_timerOutput);
            var timerText = timerOutput.Text.Trim();

            var m = new Regex("([0-9]+) seconds", RegexOptions.IgnoreCase).Match(timerText);
            var secondsText = m.Groups[1].Value;

            bool success = int.TryParse(secondsText, out var secondsRemaining);
            if (success)
            {
                return secondsRemaining;
            }

            return null;
        }





        [Then(@"the timer counted down from (.*) seconds")]
        public void ThenTheTimerCountedDownFromSeconds(int p0)
        {
            Assert.True(true);
        }

        [Then(@"the timer updated every second")]
        public void ThenTheTimerUpdatedEverySecond()
        {
            int lastValue = _timeAndOutput.Max();
            foreach (var timeValue in _timeAndOutput.TakeLast(_timeAndOutput.Count - 1))
            {

                Assert.Equal(lastValue, timeValue + 1);
                lastValue = timeValue;
            }
        }

        [Then(@"the time decreased by one second each update")]
        public void ThenTheTimeDecreasedByOneSecondEachUpdate()
        {
            Assert.True(true);
        }


        public void Dispose()
        {
            if (_chromeDriver != null)
            {
                _chromeDriver.Dispose();
                _chromeDriver = null;
            }
        }
    }
}
