using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CHECKFB
{
    class Program
    {            
        public static List<string> ResultParsing = new List<string>(); 
        public static string Email;
        public static string Password;
        static void Main(string[] args)
        {
            Console.WriteLine("Введите почту");
            Email = Console.ReadLine();
            Console.WriteLine("Введите пароль");
            Password = Console.ReadLine();
            Console.WriteLine("Какой алфавит?(ru,en)");
            Pint();
            Console.ReadKey();
        }

        
        public static void Save()
        {
            string ParsedString = string.Join("\n", ResultParsing);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.WriteLine(ParsedString);
            }
        }
        public static void Check(string text,string mainWord, string watchSearchCache)
        {
            if(String.IsNullOrEmpty(text))
            {
                return;
            }
            string Text = text;
            Text = Text.ToLower();
            Text = Text.Replace("\r\n", "");
            Text = Text.Replace($"все аккаунты с названием «{watchSearchCache.ToLower()}»", "");
            ResultParsing.AddRange(Text.Split(new string[] { mainWord.ToLower() }, StringSplitOptions.None).Select(x => mainWord + " " + x.Trim()).ToList()); 
        }
        public static void Pint()
        {
            List<char> Alphabet = new List<char> { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'щ', 'ы', 'э', 'ю', 'я' };
            IWebDriver Driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
            Driver.Navigate().GoToUrl($"https://www.pinterest.ru/");
            var Ele = Driver.FindElement(By.XPath("//*[@id='__PWS_ROOT__']/div[1]/div/div/div/div[1]/div[1]/div[2]/div[2]/button"));
            Ele.Click();
            Ele = Driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/div/div[1]/div[2]/div[2]/div/div/div/div/div/div/div/div[4]/form/div[1]/fieldset/span/div/input"));
            Ele.SendKeys(Email);
            Ele = Driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/div/div[1]/div[2]/div[2]/div/div/div/div/div/div/div/div[4]/form/div[2]/fieldset/span/div/input"));
            Ele.SendKeys(Password);
            Thread.Sleep(500);

            Ele = Driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/div/div[1]/div[2]/div[2]/div/div/div/div/div/div/div/div[4]/form/div[5]/button"));
            Ele.Click();
            List<string> Find = File.ReadAllLines("WhatSearch.txt").ToList();
            Thread.Sleep(2000);
            string WhatSearch="";
            foreach(var ell in Find)
            {
                WhatSearch = ell;
                foreach (var el in Alphabet)
                {
                    var WhatSearchChache = WhatSearch + " " + el;
                    try
                    {
                        Ele = Driver.FindElement(By.Name("searchBoxInput"));
                    }
                    catch (Exception)
                    {
                    }
                    Ele.SendKeys(Keys.Control + "a" + Keys.Delete);
                    Ele.SendKeys(WhatSearchChache);
                    Thread.Sleep(1000);
                    var a = Driver.PageSource;
                    Thread.Sleep(1000);
                    try
                    {
                        Ele = Driver.FindElement(By.Id("SuggestionsMenu"));
                        Check(Ele.Text, WhatSearch, WhatSearchChache);
                    }
                    catch (Exception)
                    {                  
                    }
                }
            }
            Save();
        }
    }
}

