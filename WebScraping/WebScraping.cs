using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading;
using OpenQA.Selenium.Safari;
using System.Collections.Generic;
using System.Web;

namespace WebScraping
{
    public class ScrapingTest
    {
        String test_url_1 = "https://www.youtube.com/c/LambdaTest/videos";
        String test_url_2 = "https://www.lambdatest.com/blog/";

        static Int32 vcount = 1;
        public IWebDriver driver;

        /* LambdaTest Credentials and Grid URL */
        String username = "user-name";
        String accesskey = "access-key";
        String gridURL = "@hub.lambdatest.com/wd/hub";

        [SetUp]
        public void start_Browser()
        {
            /* Local Selenium WebDriver */
            /* driver = new ChromeDriver(); */
            DesiredCapabilities capabilities = new DesiredCapabilities();

            capabilities.SetCapability("user", username);
            capabilities.SetCapability("accessKey", accesskey);
            capabilities.SetCapability("build", "[C#] Demo of Web Scraping in Selenium");
            capabilities.SetCapability("name", "[C#] Demo of Web Scraping in Selenium");
            capabilities.SetCapability("platform", "Windows 10");
            capabilities.SetCapability("browserName", "Chrome");
            capabilities.SetCapability("version", "latest");

            driver = new RemoteWebDriver(new Uri("https://" + username + ":" + accesskey + gridURL), capabilities,
                TimeSpan.FromSeconds(600));
            driver.Manage().Window.Maximize();
        }

        [Test(Description = "Web Scraping LambdaTest YouTube Channel"), Order(1)]
        public void YouTubeScraping()
        {
            driver.Url = test_url_1;
            /* Explicit Wait to ensure that the page is loaded completely by reading the DOM state */
            var timeout = 10000; /* Maximum wait time of 10 seconds */
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            Thread.Sleep(5000);

            /* Once the page has loaded, scroll to the end of the page to load all the videos */
            /* Get scroll height */
            Int64 last_height = (Int64)(((IJavaScriptExecutor)driver).ExecuteScript("return document.documentElement.scrollHeight"));
            while (true)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.documentElement.scrollHeight);");
                /* Wait to load page */
                Thread.Sleep(2000);
                /* Calculate new scroll height and compare with last scroll height */
                Int64 new_height = (Int64)((IJavaScriptExecutor)driver).ExecuteScript("return document.documentElement.scrollHeight");
                if (new_height == last_height)
                    /* If heights are the same it will exit the function */
                    break;
                last_height = new_height;
            }

            By elem_video_link = By.CssSelector("ytd-grid-video-renderer.style-scope.ytd-grid-renderer");
            ReadOnlyCollection<IWebElement> videos = driver.FindElements(elem_video_link);
            Console.WriteLine("Total number of videos in " + test_url_1 + " are " + videos.Count);

            /* Go through the Videos List and scrap the same to get the attributes of the videos in the channel */
            foreach (IWebElement video in videos)
            {
                string str_title, str_views, str_rel;
                IWebElement elem_video_title = video.FindElement(By.CssSelector("#video-title"));
                str_title = elem_video_title.Text;

                IWebElement elem_video_views = video.FindElement(By.XPath(".//*[@id='metadata-line']/span[1]"));
                str_views = elem_video_views.Text;

                IWebElement elem_video_reldate = video.FindElement(By.XPath(".//*[@id='metadata-line']/span[2]"));
                str_rel = elem_video_reldate.Text;

                Console.WriteLine("******* Video " + vcount + " *******");
                Console.WriteLine("Video Title: " + str_title);
                Console.WriteLine("Video Views: " + str_views);
                Console.WriteLine("Video Release Date: " + str_rel);
                Console.WriteLine("\n");
                vcount++;
            }
            Console.WriteLine("Scraping Data from LambdaTest YouTube channel Passed");
        }

        [Test(Description = "Web Scraping LambdaTest Blog Page"), Order(2)]
        public void LTBlogScraping()
        {
            driver.Url = test_url_2;
            /* Explicit Wait to ensure that the page is loaded completely by reading the DOM state */
            var timeout = 10000; /* Maximum wait time of 10 seconds */
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            Thread.Sleep(5000);

            /* Find total number of blogs on the page */
            By elem_blog_list = By.CssSelector("div.col-xs-12.col-md-12.blog-list");
            ReadOnlyCollection<IWebElement> blog_list = driver.FindElements(elem_blog_list);
            Console.WriteLine("Total number of videos in " + test_url_2 + " are " + blog_list.Count);

            /* Reset the variable from the previous test */
            vcount = 1;

            /* Go through the Blogs List and scrap the same to get the attributes of the blogs on the page */
            foreach (IWebElement blog in blog_list)
            {
                string str_blog_title, str_blog_author, str_blog_views, str_blog_link;

                IWebElement elem_blog_title = blog.FindElement(By.ClassName("blog-titel"));
                str_blog_title = elem_blog_title.Text;

                IWebElement elem_blog_link = blog.FindElement(By.ClassName("blog-titel"));
                IWebElement elem_blog_alink = elem_blog_link.FindElement(By.TagName("a"));
                str_blog_link = elem_blog_alink.GetAttribute("href");

                IWebElement elem_blog_author = blog.FindElement(By.ClassName("user-name"));
                str_blog_author = elem_blog_author.Text;

                IWebElement elem_blog_views = blog.FindElement(By.ClassName("comm-count"));
                str_blog_views = elem_blog_views.Text;

                Console.WriteLine("******* Blog " + vcount + " *******");
                Console.WriteLine("Blog Title: " + str_blog_title);
                Console.WriteLine("Blog Perm Link: " + str_blog_link);
                Console.WriteLine("Blog Author: " + str_blog_author);
                Console.WriteLine("Blog Views: " + str_blog_views);
                Console.WriteLine("\n");
                vcount++;
            }
            Console.WriteLine("Scraping of data from the LambdaTest Blog Complete");
        }

        [TearDown]
        public void close_Browser()
        {
            driver.Quit();
        }
    }
}