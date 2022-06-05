using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        IMemoryCache Cache;

      
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            Cache = memoryCache;
        }

        [HttpGet]
        public Dictionary<string, string> Get()
        {
            List<string> sondakikaHurriyet = new List<string>();
            List<string> sondakikaMilliyet = new List<string>();
            Dictionary<string, string> benzer = new Dictionary<string, string>();
            Thread thrHurriyet = new Thread(() => GetFromHurriyet(sondakikaHurriyet));
            Thread thrMilliyet = new Thread(() => GetFromMilliyet(sondakikaMilliyet));

            thrHurriyet.Start();
            thrMilliyet.Start();

            thrHurriyet.Join();
            thrMilliyet.Join();

            Cache.Set("hurriyet", sondakikaHurriyet);
            Cache.Set("milliyet", sondakikaMilliyet);

           // sondakikaHurriyet = Cache.Get<List<string>>("hurriyet");
            //GetFromHurriyet(sondakikaHurriyet);
            //GetFromMilliyet(sondakikaMilliyet);



            //foreach (var item in sondakikaHurriyet)
            //{
            //    var keywordlist = item.Split(" ");

            //    foreach (var itemMil in sondakikaMilliyet)
            //    {
            //        var keywordMil = itemMil.Split(" ");
            //        double benzerCount = 0;
            //        double totalCount = keywordMil.Count();
            //        foreach (var key in keywordlist)
            //        {
            //            if (keywordMil.Contains(key))
            //            {
            //                benzerCount++;
            //                //  break;

            //            }
            //        }
            //        if ((benzerCount / totalCount) > 0.1)
            //        {
            //            benzer.Add(item, itemMil);
            //        }

            //    }
            //}

            return benzer;
        }

        private Dictionary<string, string> benzer;

        private int ThreadCount=0;

        [HttpGet("Sync")]
        public Dictionary<string, string> GetSync()
        {
            
            List<string> sondakikaHurriyet = Cache.Get<List<string>>("hurriyet");
            List<string> sondakikaMilliyet = Cache.Get<List<string>>("milliyet");
            benzer = new Dictionary<string, string>();

            foreach(var haberHurriyet in sondakikaHurriyet)
            {
                foreach(var haberMilliyet in sondakikaMilliyet)
                {
                    while(ThreadCount>8)
                    {
                        Thread.Sleep(500);
                    }
                    Thread thr1 = new Thread(() => BenzerHaber(haberHurriyet, haberMilliyet));

                    lock (new object())
                        ThreadCount++;

                    thr1.Start();
                   // BenzerHaber(haberHurriyet, haberMilliyet);


                }

            }

            while(ThreadCount>0)
            {
                Thread.Sleep(500);
            }
          

            return benzer;
        }

        private void BenzerHaber(string haberHurriyet, string haberMilliyet)
        {
            var hurKeywords = haberHurriyet.Split(" ");
            var milKeywords = haberMilliyet.Split(" ");

            double ayniKelimeSayisi = 0;
            double toplam = hurKeywords.Count();
            foreach(var item in hurKeywords)
            {
                if(milKeywords.Contains(item))
                {
                    ayniKelimeSayisi++;
                }

            }

            if(ayniKelimeSayisi/ toplam *100 >49)
            {
                benzer.Add(haberHurriyet, haberMilliyet);
            }
            else
            {
                
            }

            lock (new object())
            {
                ThreadCount--;
            }
        }

        private static void GetFromMilliyet(List<string> sondakikaMilliyet)
        {

            using (var driver = new ChromeDriver())
            {
                
                    int tryCount = 0;

                    while (tryCount < 3)
                    {
                        try
                        {
                            driver.Navigate().GoToUrl("https://www.milliyet.com.tr/son-dakika-haberleri/");
                        }
                        catch (Exception)
                        {
                            tryCount++;
                            Thread.Sleep(1000);
                            continue;
                        }
                        break;
                    }
                    Thread.Sleep(1000);
                    IWebElement containerDivMil = driver.FindElement(By.ClassName("timeline"));
                    var headCollectionMil = containerDivMil.FindElements(By.ClassName("timeline__text"));

                    foreach (var h3 in headCollectionMil)
                    {
                        sondakikaMilliyet.Add(h3.Text);
                    }
                
            }


        }

        private static void GetFromHurriyet(List<string> sondakikaHurriyet)
        {
            using (var driver = new ChromeDriver())
            {

                int tryCount = 0;
                while (tryCount < 3)
                {

                    try
                    {
                        driver.Navigate().GoToUrl("https://www.hurriyet.com.tr/son-dakika-haberleri/");
                    }
                    catch (Exception)
                    {
                        tryCount++;
                        Thread.Sleep(1000);
                        continue;
                    }
                    break;
                }

                Thread.Sleep(1000);
                IWebElement containerDiv = driver.FindElement(By.ClassName("son-dakika-contain"));

                var headCollection = containerDiv.FindElements(By.ClassName("son-dakika-headline"));

                foreach (var h3 in headCollection)
                {
                    sondakikaHurriyet.Add(h3.Text);
                }
            }

        }
    }
}
