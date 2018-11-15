using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NUnitSeleniumTestProject
{
    class NUnitTest
    {
        IWebDriver driver;
        //String productId;

        [SetUp]
        public void Initialize()
        {
            //temel tanımlar, init işlemleri burda

            //Google Chrome’un açılması için yapıyoruz. Aynı zamanda driver diye nesne tanımlamış olduk. Bu nesne üzerinden işlemleri yapacağız.
            driver = new ChromeDriver();

            //Geniş ekranda görünmesi için
            driver.Manage().Window.Maximize();

            //timeout süresi 20 saniye
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            //Frefox ile test etmek istenirse bu şekilde olacak
            //driver = new FirefoxDriver();
        }

        [Test]
        public void AllInAllTest()
        {

            //Gitmek istediğimiz sayfa
            driver.Navigate().GoToUrl("https://www.n11.com/");
            Console.WriteLine("n11.com ana sayfa açıldı.");

            //Login bağlantısına tıklanıyor
            driver.FindElement(By.LinkText("Giriş Yap")).Click();
            //driver.FindElement(By.ClassName("btnSignIn")).Click(); //alternatif element seçme yöntemlerinden bir tanesi bu, direk https://www.n11.com/giris-yap adresi de kullanılabilir
            Console.WriteLine("Login sayfası açıldı.");

            //Form elemanları dolduruluyor
            driver.FindElement(By.Id("email")).SendKeys("ayfer.gurus@gmail.com");
            Console.WriteLine("Email girildi.");

            driver.FindElement(By.Id("password")).SendKeys("xyz!1234");
            Console.WriteLine("Şifre girildi.");

            driver.FindElement(By.Id("loginButton")).Click();
            Console.WriteLine("Login düğmesine tıklandı ve login olunuyor.");


            //Arama kutusunu seçtik ve 'samsung' kelimesini yerleştirdik
            driver.FindElement(By.Id("searchData")).SendKeys("samsung");

            //Arama işlemini başlatıyoruz
            driver.FindElement(By.ClassName("searchBtn")).Click();
            Console.WriteLine("Arama sonucu ekrana geldi.");

            //Sonuç sayısı barındıran strong etiketini seçip ürün olup olmaması durumunu 0 ile karşılaştırıyoruz
            String resultCount = driver.FindElement(By.ClassName("resultText ")).FindElement(By.TagName("strong")).Text;
            Console.WriteLine(resultCount + " souç var");


            Assert.IsFalse(resultCount.Equals("0"));
            Console.WriteLine("Sonucun var olduğunu onaylandı.");


            //ikinci sayfa için '2' düğmesine de tıklanabilir ya da ileri düğmesine de, burda biz 2 ye tıkladık.
            driver.FindElement(By.ClassName("pagination")).FindElement(By.LinkText("2")).Click();
            //driver.FindElement(By.ClassName("next navigation")).Click(); 
            Console.WriteLine("2. sayfa için tıklandı.");

            //2. sayfada olduğumuzu sayfalamada 2 nolu sayfanın seçili olduğunu ('active' seçili) ve Dispayed özelliği ile test ettik.
            String page = driver.FindElement(By.CssSelector("div.productArea > div.pagination > a.active")).Text;
            bool isDisplated = driver.FindElement(By.CssSelector("div.productArea > div.pagination > a.active")).Displayed;
            Assert.IsTrue(page.Equals("2") && isDisplated); //sayfa 2 aktif ve doğrulandı
            Console.WriteLine("2. sayfanın aktif ve gösterimde olduğu doğrulandı.");


            //Listedeki 3. ürünü favori listesine ekliyoruz
            var element = driver.FindElement(By.CssSelector("#view > ul > li:nth-child(3) div.proDetail > span.textImg.followBtn"));
            //var a = driver.FindElement(By.CssSelector("#p-287284607 > div.proDetail > span.textImg.followBtn"));
            String productId = element.GetAttribute("data-productid"); //Ürünün productionid sini daha sonra kullanmak üzere değişkene atıyoruz
            element.Click();
            Console.WriteLine(productId, " nolu favorilere eklendi.");

            //Favorilerim sayfasını açmak için fare ile Hesabım bağlantısı üzerine gelip 'İstek Listem / Favorilerim' bağlantısı tıklanmalı
            element = driver.FindElement(By.LinkText("Hesabım"));
            Console.WriteLine("Hesabım bağlantısı üzerine gelindi.");
            //Create object 'action' of an Actions class
            Actions action = new Actions(driver);
            //Mouseover on an element
            action.MoveToElement(element).Perform();
            IWebElement subLink = driver.FindElement(By.LinkText("İstek Listem / Favorilerim"));
            action.MoveToElement(subLink);
            action.Click();
            Console.WriteLine("Hesabım bağlantısı altındaki Favorilerim bağkantısına tıklandı.");
            action.Perform();

            Console.WriteLine("Favorilere ekli ürünlerden en son eklediğimiz var mı test ediliyor.");
            var list = driver.FindElements(By.CssSelector(".wishGroupListItem ul > li > a"));
            bool hasFavoriteElementAdded = false;
            foreach (var item in list)
            {
                //Console.WriteLine("HREF: " + item.GetAttribute("href"));
                hasFavoriteElementAdded = item.GetAttribute("href").Contains("p-" + productId);

                if (hasFavoriteElementAdded)
                    break;
            }

            Assert.IsTrue(hasFavoriteElementAdded);
            Console.WriteLine("Favorilere ekli elemanın favori listesinde var olduğu doğrulandı.");

            //Favorite ürün detay sayfasını açıyoruz.
            driver.FindElement(By.CssSelector(".wishGroupListItem.favorites > div > a")).Click();
            Console.WriteLine("Favorite ürün detay sayfasını açıldı.");

            //Favorite listesinden eklediğimiz ürünü kaldırıyoruz (Sil'e basarak).
            driver.FindElement(By.CssSelector("#p-" + productId + " > div.wishProBtns > span")).Click();
            Console.WriteLine("Daha önce eklenen favorite ürün listeden silindi.");


            //2 saniye verdik silme işlemi sonrası popup bildirimin çıkması için
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            element = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".lightBox .btn.btnBlack.confirm")));

            //silme sonrası popup bildirim açılıyor, 'Tamam' düğmesine tıklatıyoruz.
            driver.FindElement(By.CssSelector(".lightBox .btn.btnBlack.confirm")).Click();

            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20); //2 saniye bekle, popup da Tamam düğmesine bastıktan sonra listeden ürünün tam olarak kalkması için
            Thread.Sleep(2000); //2 saniye bekle bişey yapmadan

            var itemList = driver.FindElements(By.CssSelector("#p-" + productId + " > div.wishProBtns > span"));
            Console.WriteLine("Opps! Sildiğimiz ürün halen listede!.");
            Assert.IsTrue(itemList.Count == 0); //bu tam doğru çalışmıyor dikkat!!!
            Console.WriteLine("Favorilere ekli elemanın favori listesinden silindiği doğrulandı.");

        }

        /*
        [Test, Order(1)]
        public void OpenMainPageTest()
        {               
            //Gitmek istediğimiz sayfa
            driver.Navigate().GoToUrl("https://www.n11.com/");
            //driver.Url = "https://www.n11.com/";
            Console.WriteLine("n11.com anasayfaya yönlendi.");

            //Anasayfa görüntülendi mi testi
            bool isVisible = driver.FindElement(By.TagName("body")).Displayed;
            Assert.IsTrue(isVisible);
            Console.WriteLine("Anasayfanın görüntülendiği doğrulandı.");
        }


        [Test, Order(2)]
      
        public void LoginTest()
        {           
            //Login bağlantısına tıklanıyor
            driver.FindElement(By.LinkText("Giriş Yap")).Click();
            //driver.FindElement(By.ClassName("btnSignIn")).Click(); //alternatif element seçme yöntemlerinden bir tanesi bu, direk https://www.n11.com/giris-yap adresi de kullanılabilir
            Console.WriteLine("Login sayfası açıldı.");

            //Form elemanları dolduruluyor
            driver.FindElement(By.Id("email")).SendKeys("ayfer.gurus@gmail.com");
            Console.WriteLine("Email girildi.");

            driver.FindElement(By.Id("password")).SendKeys("721988.Grs");
            Console.WriteLine("Şifre girildi.");

            driver.FindElement(By.Id("loginButton")).Click();
            Console.WriteLine("Login düğmesine tıklandı.");


            //Anasayfa görüntülendi mi testi
            bool isVisible = driver.FindElement(By.CssSelector("div.myAccount > a.menuLink.user")).Displayed;
            Assert.IsTrue(isVisible);
            Console.WriteLine("Login olunduğu doğrulandı.");
        }

        [Test, Order(3)]
        public void hasResultForSearchKeywordTest()
        {
            //Arama kutusunu seçtik ve 'samsung' kelimesini yerleştirdik
            driver.FindElement(By.Id("searchData")).SendKeys("samsung");

            //Arama işlemini başlatıyoruz
            driver.FindElement(By.ClassName("searchBtn")).Click();
            Console.WriteLine("Arama yapması için tıkladık.");

            //Sonuç sayısı barındıran strong etiketini seçip ürün olup olmaması durumunu 0 ile karşılaştırıyoruz
            String resultCount = driver.FindElement(By.ClassName("resultText ")).FindElement(By.TagName("strong")).Text;
            Console.WriteLine(resultCount + " souç var");


            //result textte '0' dan farklı bir değer varsa aranan ile ilgili sonuç var demektir
            Assert.IsFalse(resultCount.Equals("0"));
            Console.WriteLine("Sonucun var olduğunu onaylandı.");
        }

        [Test, Order(4)]
        public void OpenSecondPageTest()
        {
            //ikinci sayfa için '2' düğmesine de tıklanabilir ya da ileri düğmesine de, burda biz 2 ye tıkladık.
            driver.FindElement(By.ClassName("pagination")).FindElement(By.LinkText("2")).Click();
            //driver.FindElement(By.ClassName("next navigation")).Click(); 
            Console.WriteLine("2. sayfa için tıklandı.");

            //2. sayfada olduğumuzu sayfalamada 2 nolu sayfanın seçili olduğunu ('active' seçili) ve Dispayed özelliği ile test ettik.
            var element = driver.FindElement(By.CssSelector("div.productArea > div.pagination > a.active"));
            Assert.IsTrue(element.Text.Equals("2") && element.Displayed); //sayfa 2 aktif ve doğrulandı
            Console.WriteLine("2. sayfanın gösterimde olduğu doğrulandı.");
        }

        [Test, Order(5)]
        public void AddThirdProductToFavoriteListTest()
        {
            //Listedeki 3. ürünü favori listesine ekliyoruz, normalde li:nth-child(4) 3. ürüne karşılık geliyor ama 3. ürünle ilgili sorun old. için başka bir ürün ile test ettik
            var element = driver.FindElement(By.CssSelector("#view > ul > li:nth-child(3) div.proDetail > span.textImg.followBtn"));
            //var a = driver.FindElement(By.CssSelector("#p-287284607 > div.proDetail > span.textImg.followBtn"));
            productId = element.GetAttribute("data-productid"); //Ürünün productionid sini daha sonra kullanmak üzere değişkene atıyoruz
            element.Click();
            Console.WriteLine(productId, " nolu ürünü favori listesine eklemek için tıkladık.");

            //Favorilerim sayfasını açmak için fare ile Hesabım bağlantısı üzerine gelip 'İstek Listem / Favorilerim' bağlantısı tıklanmalı
            element = driver.FindElement(By.LinkText("Hesabım"));
            Console.WriteLine("Hesabım bağlantısı üzerine gelindi.");
            //Create object 'action' of an Actions class
            Actions action = new Actions(driver);
            //Mouseover on an element
            action.MoveToElement(element).Perform();
            var subLink = driver.FindElement(By.LinkText("İstek Listem / Favorilerim"));
            action.MoveToElement(subLink);
            action.Click();
            Console.WriteLine("Hesabım bağlantısı altındaki Favorilerim bağkantısına tıklandı.");
            action.Perform();

            //ürün favorilere eklendi mi testi...
            Console.WriteLine("Favorilere ekli ürünlerden en son eklediğimiz var mı test ediliyor.");
            var list = driver.FindElements(By.CssSelector(".wishGroupListItem ul > li > a"));
            bool hasFavoriteElementForProductId = false;
            foreach (var item in list)
            {
                //Console.WriteLine("HREF: " + item.GetAttribute("href"));
                hasFavoriteElementForProductId = item.GetAttribute("href").Contains("p-" + productId);

                if (hasFavoriteElementForProductId)
                    break;
            }

            Assert.IsTrue(hasFavoriteElementForProductId);
            Console.WriteLine("Favorilere ekli elemanın favori listesinde var olduğu doğrulandı.");

        }



        [Test, Order(6)]
        public void RemoveNewlyAddedProductFromFavoriteListTest()
        {
            //Favorite ürün detay sayfasını açıyoruz.
            driver.FindElement(By.CssSelector(".wishGroupListItem.favorites > div > a")).Click();
            Console.WriteLine("Favorite ürün detay sayfasını açıldı.");

            //Favorite listesinden eklediğimiz ürünü kaldırıyoruz (Sil'e basarak).
            driver.FindElement(By.CssSelector("#p-" + productId + " > div.wishProBtns > span")).Click();
            Console.WriteLine("Daha önce eklenen favorite ürün listeden silindi.");


            //2 saniye verdik silme işlemi sonrası popup bildirimin çıkması için
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            var element = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".lightBox .btn.btnBlack.confirm")));

            //silme sonrası popup bildirim açılıyor, 'Tamam' düğmesine tıklatıyoruz.
            driver.FindElement(By.CssSelector(".lightBox .btn.btnBlack.confirm")).Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3); //2 saniye bekle, popup da Tamam düğmesine bastıktan sonra listeden ürünün tam olarak kalkması için
            var itemList = driver.FindElements(By.CssSelector("#p-" + productId + " > div.wishProBtns > span"));
            Assert.IsTrue(itemList.Count == 0); //bu tam doğru çalışmıyor dikkat!!!
            Console.WriteLine("Favorilere ekli elemanın favori listesinden silindiği doğrulandı.");
        }
        */

        [TearDown]
        public void EndTest()
        {
            //testler bitti kapatıyoruz.        
            driver.Close();
        }

    }
}
