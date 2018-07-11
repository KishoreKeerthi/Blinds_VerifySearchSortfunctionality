using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Blinds_VerifySearchSort_RoomDarkening
{
    class Blinds_VerifySearchSort_RoomDarkening_Class
    {
        //Declaring driver of IWebDriver at class level to use it across all methods in Class
        static public IWebDriver driver;

        //Declaring Variable to capture all verification errors from all methods in Calss
        static public String verificationErrors="";

        static void Main(string[] args)
        {
            //Creating instance of Class 'Blinds_VerifySearchSort_RoomDarkening_Class' 
            Blinds_VerifySearchSort_RoomDarkening_Class cls = new Blinds_VerifySearchSort_RoomDarkening_Class();

            //Variable to pass URL of Blinds
            String BlindsURL = "https://www.blinds.com";

            //Method call to launch Blinds website
            cls.LaunchBlinds(BlindsURL);

            //Declaring string variable for 'text to search',verify text on search results page and assigning values 
            String str_SearchText="room darkening blinds", str_SearchPageHeadertext = "Room Darkening Shades";

            //Method call to verify search results page header text
            cls.VerifySearchResultPage(str_SearchText,str_SearchPageHeadertext);

            //Declaring variables for total count of items for Brands and for Opacity Items
            int TotalCount_BrandItems,TotalCount_AO_OpacityItems;

            //Method call to Get the total count of items for all the brands shown under Brands category on left menu after search 
            TotalCount_BrandItems = cls.GetTotalCountBrands();

            //Method call to Get the total count of items for Opacity Blackout option under Available options shown on left menu after search
            TotalCount_AO_OpacityItems = cls.GetTotalOpacityBlackoutItems();

            //--Assertion to verify total count retrieved under Brand Category is same as total count of items shown under Available Otpions -> Opacity:Blackout
            try
            {
                Assert.AreEqual(TotalCount_AO_OpacityItems, TotalCount_BrandItems);
                Console.WriteLine("\n STEP PASS : Total count of all items under Brand Category is same as total count of items shown under Available Otptions as '" + TotalCount_BrandItems+"' matches with '"+ TotalCount_AO_OpacityItems+"'\n");
            }
            catch (AssertFailedException e)
            {
                verificationErrors = verificationErrors + e.Message;
                Console.WriteLine("\n STEP FAIL : Total count of all items under Brand Category is not same as total count of items shown under Available Otptions as '" + TotalCount_BrandItems + "' does not match with '" + TotalCount_AO_OpacityItems+"'\n");
            }

            //Method call to verify the checked status of Opacity Blackout checkbox
            cls.VerifyOpacityBlackOutCheckbox();

            //Click on 'See More Products' button till it's visible to show all the search result items
            while (driver.PageSource.Contains("see-more-products-btn"))
            {
                try
                {
                    driver.FindElement(By.Id("see-more-products-btn")).Click();//Click on the button if it's visible
                }
                catch
                {
                    continue;
                }
            }

            //Wait for all results to load
            System.Threading.Thread.Sleep(5000);

            //Declaring variable to store count of all search items on search results page
            int AllSearchItems_Count = 0;

            //Method call to verify total count of items retrieved in search results is same as total count of items shown under Available Options -> Opacity:Blackout on left menu
            AllSearchItems_Count = cls.VerifySearchItemsCount(TotalCount_AO_OpacityItems);

            //Method call to verify names in search results match with search criteria provided
            cls.VerifySearchResultsNames(AllSearchItems_Count);

            //--Click on 'Price Low-High' link to sort products from low to high price range
            driver.FindElement(By.LinkText("Price Low-High")).Click();

            //Wait for page to load
            System.Threading.Thread.Sleep(5000);

            //Declaring variable to store count of all sorted items on search results page after sort
            int AllSortedSearchItems_Count =0;

            //Method call to verify total count retrieved after sorting (from Low to High price) is same as total count of items retrieved during search results
            AllSortedSearchItems_Count = cls.VerifyCountSortedSearchResults(AllSearchItems_Count);

            //Method call to verify correctness of sorted search results based on prices from Low to High
            cls.VerifySortedSearchResults(AllSortedSearchItems_Count);

            //--Output all failure errors at the end 
            Console.WriteLine(verificationErrors);

            //Terminate Chrome browser
            foreach (Process P in Process.GetProcessesByName("chrome"))
            P.Kill();
            
        }

        /**************************************************************************************
         * Purpose/Description : This method is launch Blinds website.
         * Arguments to Pass   : URL of Blinds website.
         * Returns             : None.
         * ************************************************************************************/
        public void LaunchBlinds(String URL)
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArguments("disable-infobars");

            //Launch Chrome brower with option
            driver = new ChromeDriver(option);

            //Enter Blinds.com URL 
            driver.Url = URL; // "https://www.blinds.com";

            //Maximize the browser window after page is loaded
            driver.Manage().Window.Maximize();

            //Get title of the page 
            String str_Pagetitle = driver.Title;

            //Assertion to verify Blinds page loaded correctly
            try
            {
                StringAssert.Contains(str_Pagetitle, "Blinds");
                Console.WriteLine("\n STEP PASS : Blinds page is loaded correctly as text 'Blinds' exists in '" + str_Pagetitle + "'\n");

            }
            catch (AssertFailedException e)
            {
                verificationErrors = verificationErrors + e.Message;
                Console.WriteLine("\n STEP FAIL: Blinds page is not loaded correctly as text 'Blinds' is not present in '" + str_Pagetitle + "'\n");
                
            }
        }

        /**************************************************************************************
         * Purpose/Description : This method is verify the top header of the search results page.
         * Arguments to Pass   : search criteria text, text to verify on top header.
         * Returns             : None.
         * ************************************************************************************/
        public void VerifySearchResultPage(String str_toSearch, String str_SearchPageHeader)
        {

            //Enter room darkening blinds and select the first match item in Search text field
            driver.FindElement(By.Id("gcc-inline-search")).SendKeys(str_toSearch + Keys.Down + Keys.Return);

            //Get the title of top section under BLINDS.COM 
            String str_SearchPage = driver.FindElement(By.XPath("//div[contains(@class,'shop-by-header-content')]/h1")).GetAttribute("innerHTML");

            //Assertion to verify search results page loaded correctly
            try
            {
                StringAssert.Contains(str_SearchPage, str_SearchPageHeader); 
                Console.WriteLine("\n STEP PASS : Search Results page loaded correctly as text '"+ str_SearchPageHeader + "' presents in '" + str_SearchPage + "'\n");
            }
            catch (AssertFailedException e)
            {
                verificationErrors = verificationErrors + e.Message;
                Console.WriteLine("\n STEP FAIL: Search Results page not loaded correctly as text '" + str_SearchPageHeader + "' is not present in '" + str_SearchPage + "'\n");
            }


        }

        /*************************************************************************************************************************************
        * Purpose/Description : This method is get the total count of items for all the brands under Brand Category on left menu after search.
        * Arguments to Pass   : None.
        * Returns             : Total count.
        * ************************************************************************************************************************************/
        public int GetTotalCountBrands()
        {
            //--Get the total count of items for all the brands shown under Brands category on left menu after search
            IList<IWebElement> Catergory_Brand = driver.FindElements(By.XPath("//div/ul[@class='list m0 pr2-m pr3-l']/li[@data-id='Brand']/ul[@class='near-black list m0 mb3']/li[@data-id='gcc-search-filter-option']"));
            int Count_Brands = Catergory_Brand.Count;
            String Count_BrandItems = "";
            int TotalCount_BrandItems = 0;

            //Loop through all brands to get the total count of items under Brands category
            for (int i = 0; i < Count_Brands; i++)
            {

                Count_BrandItems = Catergory_Brand[i].FindElement(By.XPath(".//*[@class='f6 dark-gray']")).GetAttribute("textContent");//Retrieve item count beside each brand under Brands Category
                Count_BrandItems = Count_BrandItems.Replace("(", string.Empty);//Remove '(' from the string retrieved 
                Count_BrandItems = Count_BrandItems.Replace(")", string.Empty);//Remove ')' from the string retrieved
                TotalCount_BrandItems = TotalCount_BrandItems + Int32.Parse(Count_BrandItems);//Convert string to int data type and add to total count
            }

            return TotalCount_BrandItems;
        }

        /******************************************************************************************************************************************************
       * Purpose/Description : This method is get the total count of items for Opacity Blackout option under Available options shown on left menu after search.
       * Arguments to Pass   : None.
       * Returns             : Total count.
       * *****************************************************************************************************************************************************/
        public int GetTotalOpacityBlackoutItems()
        {
            //--Get the total count of items for Opacity Blackout option under Available options shown on left menu after search
            IList<IWebElement> Category_AO_Opacity = driver.FindElements(By.XPath("//div/ul[@class='list m0 pr2-m pr3-l']/li[@data-id='Available Options']//ul[@class='near-black list m0 mb3']/li[@data-id='gcc-search-filter-option']//input[@id='availableOptions_Opacity: Blackout']"));
            int Count_AO_Opacity = Category_AO_Opacity.Count;
            String Count_AO_OpacityItems = "";
            int TotalCount_AO_Opacity = 0;

            //Loop through all 'Opacity Blackout' items to get the total count of items under Available Otpions -> Opacity:Blackout
            for (int j = 0; j < Count_AO_Opacity; j++)
            {

                Count_AO_OpacityItems = Category_AO_Opacity[j].FindElement(By.XPath("..//*[@class='f6 dark-gray']")).GetAttribute("textContent"); //Get count of items beside Opacity Blackout under Available Options
                Count_AO_OpacityItems = Count_AO_OpacityItems.Replace("(", string.Empty); //Remove '(' from the string retrieved 
                Count_AO_OpacityItems = Count_AO_OpacityItems.Replace(")", string.Empty); //Remove ')' from the string retrieved
                TotalCount_AO_Opacity = TotalCount_AO_Opacity + Int32.Parse(Count_AO_OpacityItems);//Convert string to int data type and add to total count
            }
            return TotalCount_AO_Opacity;
        }

        /*********************************************************************************************
        * Purpose/Description : This method is verify the checked status of Opacity Blackout Checkbox.
        * Arguments to Pass   : None.
        * Returns             : None.
        * *******************************************************************************************/
        public void VerifyOpacityBlackOutCheckbox()
        {
            //Get the checked status of Opacity Blackout checkbox 
            String OpacityBlackout_CheckStatus = driver.FindElement(By.XPath("//div/ul[@class='list m0 pr2-m pr3-l']/li[@data-id='Available Options']//ul[@class='near-black list m0 mb3']/li[@data-id='gcc-search-filter-option']//input[@id='availableOptions_Opacity: Blackout']")).GetAttribute("checked");

            //Assertion to verify Opacity Blackout checkbox is toggled ON or not under Available Options
            try
            {
                StringAssert.Contains(OpacityBlackout_CheckStatus, "true");
                Console.WriteLine("\n STEP PASS : Opacity Blackout checkbox is toggled ON as expected to show 'Room Darkening blinds' items as 'checked' status of it is '" + OpacityBlackout_CheckStatus + "'\n");
            }
            catch (AssertFailedException e)
            {
                verificationErrors = verificationErrors + e.Message;
                Console.WriteLine("\n STEP FAIL: Opacity Blackout checkbox is not toggled ON as 'checked' status of it is '" + OpacityBlackout_CheckStatus + "'\n");
            }
        }

        /*********************************************************************************************
       * Purpose/Description : This method is verify the total count of items shown as search results.
       * Arguments to Pass   : Total Count of Opacity Blackout items.
       * Returns             : Total Count of items shown as search results .
       * ********************************************************************************************/
        public int VerifySearchItemsCount(int TotalCount_AO_OpacityItems)
        {
            //--Get the total count of items shown as search results
            IWebElement GetParent_SearchItems = driver.FindElement(By.XPath("//div[@class='mt0 mb3-ns flex flex-wrap justify-center justify-start-l']"));
            IList<IWebElement> List_GetAllSearchItems = GetParent_SearchItems.FindElements(By.XPath("//div[@class='pa3 w-100 w-50-ns flex flex-wrap flex-column']"));
            int AllSearchItems_Count = List_GetAllSearchItems.Count;


            //--Assertion to verify total count of items retrieved as search results is same as total count of items shown under Available Options -> Opacity:Blackout on left menu
            try
            {
                Assert.AreEqual(AllSearchItems_Count, TotalCount_AO_OpacityItems);
                Console.WriteLine("\n STEP PASS : Total count of items retrieved as search results is same as total count of items shown under Available Options->Opacity:Blackout as '" + AllSearchItems_Count + "' matches with '" + TotalCount_AO_OpacityItems + "'\n");
            }
            catch (AssertFailedException e)
            {
                verificationErrors = verificationErrors + e.Message;
                Console.WriteLine("\n STEP FAIL: Total count of items retrieved as search results is not same as total count of items shown under Available Options->Opacity:Blackout as '" + AllSearchItems_Count + "' does not match with '" + TotalCount_AO_OpacityItems + "'\n");
            }
            return AllSearchItems_Count;
        }

        /******************************************************************************************************
      * Purpose/Description : This method is verify names in search results match with search criteria provided.
      * Arguments to Pass   : Total Count of items shown as search results.
      * Returns             : None.
      * *******************************************************************************************************/
        public void VerifySearchResultsNames(int AllSearchItems_Count)
        {
            //Declaring variables to verify search results correctness
            String str_GetSearchItemName;
            bool SearchResult = false;
            int FailedSearchItem = 0;

            //--Get the total list of items shown as search results
            IWebElement GetParent_SearchItems = driver.FindElement(By.XPath("//div[@class='mt0 mb3-ns flex flex-wrap justify-center justify-start-l']"));
            IList<IWebElement> List_GetAllSearchItems = GetParent_SearchItems.FindElements(By.XPath("//div[@class='pa3 w-100 w-50-ns flex flex-wrap flex-column']"));

            //Loop across all search result items to verify the results are accurate
            for (int l = 0; l < AllSearchItems_Count; l++)
            {
                //Get title of each item
                str_GetSearchItemName = List_GetAllSearchItems[l].FindElement(By.XPath(".//*[@data-testid='gcc-product-title']")).GetAttribute("href");

                if (str_GetSearchItemName.Contains("shade") || str_GetSearchItemName.Contains("blackout") || str_GetSearchItemName.Contains("draper") || str_GetSearchItemName.Contains("pleated") || str_GetSearchItemName.Contains("fabric") || str_GetSearchItemName.Contains("panel"))
                {
                    SearchResult = true;
                }
                else
                {
                    FailedSearchItem = l;
                    SearchResult = false;
                    break;
                }
            }

            //--Assertion to verify correctness of published search results 
            if (SearchResult)
            {
                Console.WriteLine("\n STEP PASS : Search results are published correctly\n");
            }
            else
            {
                Console.WriteLine("\n STEP FAIL: Search results are not published correctly\n");
            }
        }

        /**************************************************************************************
       * Purpose/Description : This method is verify the total count of items shown after sorting the search results.
       * Arguments to Pass   : Total Count of items shown as search results.
       * Returns             : Total Count of items shown after sorting the search results .
       * ************************************************************************************/
        public int VerifyCountSortedSearchResults(int AllSearchItems_Count)
        {


            //--Get the total count of items shown as search results after sorting from Low to High price range
            IWebElement GetParent_SortedItems = driver.FindElement(By.XPath("//div[@class='mt0 mb3-ns flex flex-wrap justify-center justify-start-l']"));
            System.Threading.Thread.Sleep(2000);
            IList<IWebElement> List_GetAllSortedItems = GetParent_SortedItems.FindElements(By.XPath("//div[@class='pa3 w-100 w-50-ns flex flex-wrap flex-column']"));
            int AllSortItems_Count = List_GetAllSortedItems.Count;


            //--Assertion to verify total count retrieved after sorting (from Low to High price) is same as total count of items retrieved during search results
            try
            {
                Assert.AreEqual(AllSortItems_Count, AllSearchItems_Count);
                Console.WriteLine("\n STEP PASS : Total count of items retrieved after sorting from low to high price is same as total count of items shown before sorting as '" + AllSortItems_Count + "' matches with '" + AllSearchItems_Count + "'\n");
            }
            catch (AssertFailedException e)
            {
                verificationErrors = verificationErrors + e.Message;
                Console.WriteLine("\n STEP FAIL: Total count of items retrieved after sorting from low to high price is not same as total count of items shown before sorting as '" + AllSortItems_Count + "' does not match with '" + AllSearchItems_Count + "'\n");
            }

            return AllSortItems_Count;
        }

        /**************************************************************************************
        * Purpose/Description : This method is verify Search results are sorted correctly or not from low to high price.
        * Arguments to Pass   : Total Count of items shown after sorting the search results.
        * Returns             : None.
        * ************************************************************************************/
        public void VerifySortedSearchResults(int AllSortItems_Count)
        {
            //Declaring variables to verify sorted results correctness with price range from low to high
            String str_GetPriceFirstItem, str_GetPrice;
            decimal dec_GetPriceFirstItem = 0, dec_GetPrice = 0;
            bool SortingResult = false;
            int FailedItem = 0;

            //--Get the total list of items shown as search results after sorting from Low to High price range
            IWebElement GetParent_SortedItems = driver.FindElement(By.XPath("//div[@class='mt0 mb3-ns flex flex-wrap justify-center justify-start-l']"));
            System.Threading.Thread.Sleep(2000);
            IList<IWebElement> List_GetAllSortedItems = GetParent_SortedItems.FindElements(By.XPath("//div[@class='pa3 w-100 w-50-ns flex flex-wrap flex-column']"));

            //Loop across all sorted items to verify the prices sorted from low to high
            for (int k = 0; k < AllSortItems_Count; k++)
            {

                //Get the price value of first item in sorted search results
                if (k == 0)
                {
                    str_GetPriceFirstItem = List_GetAllSortedItems[0].FindElement(By.XPath(".//*[@data-testid='gcc-product-discount-price']")).GetAttribute("textContent");

                    //Remove extra spaces beside N/A
                    if (str_GetPriceFirstItem.Contains("N/A"))
                    {
                        str_GetPriceFirstItem = str_GetPriceFirstItem.Replace(" ", string.Empty);

                    }
                    //Remove $ symbol beside price
                    if (str_GetPriceFirstItem.Contains("$"))
                    {
                        str_GetPriceFirstItem = str_GetPriceFirstItem.Replace("$", string.Empty);

                    }
                    //Convert string to decimal value if price is not N/A
                    if (str_GetPriceFirstItem != "N/A")
                    {
                        dec_GetPriceFirstItem = decimal.Parse(str_GetPriceFirstItem);
                    }

                }
                //Get the price values of remaining items in sorted search results and compare them with previous item's price
                else
                {
                    //Get price value of sorted item
                    str_GetPrice = List_GetAllSortedItems[k].FindElement(By.XPath(".//*[@data-testid='gcc-product-discount-price']")).GetAttribute("textContent");

                    //Remove extra spaces from price value
                    str_GetPrice = str_GetPrice.Replace(" ", string.Empty);

                    //Remove $ sign from price value
                    if (str_GetPrice.Contains("$"))
                    {
                        str_GetPrice = str_GetPrice.Replace("$", string.Empty);
                    }
                    //If price is not N/A, compare it with previous item price value
                    if (str_GetPrice != "N/A")
                    {
                        dec_GetPrice = decimal.Parse(str_GetPrice);
                        if (dec_GetPriceFirstItem <= dec_GetPrice)
                        {
                            SortingResult = true;
                            dec_GetPriceFirstItem = dec_GetPrice;
                        }
                        else
                        {
                            FailedItem = k;
                            SortingResult = false;
                            break;
                        }
                    }

                }


            }

            //--Assertion to verify correctness of sorted search results based on prices
            if (SortingResult)
            {
                Console.WriteLine("\n STEP PASS : Search results are sorted correctly from low to high price as expected as price value of previous item is smaller than price value of current item for all sorted items\n");
            }
            else
            {
                Console.WriteLine("\n STEP FAIL: Search results are not sorted correctly from low to high price as price value of previous item is not smaller than price value of current item for item number : '" + FailedItem + "'\n");
            }

            
        }

    }
}
