using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace LyricsTranslator
{
    /// <summary>
    /// Summary description for LyricsProcessor:
    /// This service provides a number of functions to process lyrics:
    /// AddLineBreaksHTML strives to, based on the placement of capital letters in a text, add html breaks
    /// to display the text as lyrics on a webpage/
    /// Transform linebreaks is a function that transforms "\n" and "\r" into htlm breaks.
    /// TranslateEnglishToDutch translates a provided ENGLISH text to DUTCH
    /// </summary>
    /// [WebService(Namespace = "http://tempuri.org/")]
    [WebService(Namespace = "https://lyricstranslator.azurewebsites.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class LyricsProcessor : System.Web.Services.WebService
    {

        [WebMethod]
        public string AddLineBreaksHTML(string lyrics)
        {
            //char[] lyricsChar = lyrics.ToCharArray();
            int length = lyrics.Count();
            int i = 0;
            List<int> breaks = new List<int>();

            //Keep track of average sentence length
            List<int> sentenceLengths = new List<int>();
            //double averageSentenceLength = 0;
            while (i < length)
            {
                if (IsCapital(lyrics[i]))
                {
                    //If the capital is the first letter of the word
                    if (i > 0 && lyrics[i - 1].Equals(' '))
                    {
                        if (breaks.Count >= 2)
                        {
                            int currentSentenceLength = (i - 1) - breaks[breaks.Count - 2];
                            double avg = ((sentenceLengths.Count > 0) ? sentenceLengths.Average() : 0.0);
                            if (currentSentenceLength > 0.75 * avg)
                            {
                                //System.Diagnostics.Debug.WriteLine("test");
                                sentenceLengths.Add(currentSentenceLength);
                                breaks.Add(i - 1);
                            }
                        }
                        else
                        {
                            breaks.Add(i - 1);
                        }

                    }
                }
                i++;
            }

            //return lyrics;
            return AddBreaks(lyrics, breaks, length);
        }

        [WebMethod]
        public string TransformLineBreaks(string lyrics)
        {
            string lyricsHTML = "";
            int i = 0;
            foreach (char letter in lyrics)
            {
                if (!letter.Equals('\\'))
                {
                    if (i > 0 && !(lyrics[i - 1].Equals('\\') && letter.Equals('n')))
                    {
                        lyricsHTML = String.Concat(lyricsHTML, letter);
                    }
                    else if (i == 0)
                    {
                        lyricsHTML = String.Concat(lyricsHTML, letter);
                    }
                }
                else
                {
                    lyricsHTML = String.Concat(lyricsHTML, "<br>");
                }
                i++;
            }
            return lyricsHTML;
        }


        [WebMethod]
        public string TranslateEnglishToDutch(string text)
        {
            //string translation = TranslateEnglishToDutchAPICall(text);
            //translation = translation.Replace("[", "").Replace("]", "").Replace("\"", "").TrimStart('\r', '\n', ' ').TrimEnd('\r', '\n', ' ');
            //translation = translation.Replace("]", "");
            //translation = translation.Replace("\"", "");
            //translation = translation.TrimStart('\r','\n',' ').TrimEnd('\r','\n',' ');
            //translation = translation.Remove(translation.Length - 1);
            string additionalText = "<br><a href=\"https://translate.yandex.com/\">Powered by Yandex.Translate</a>";
            return TranslateEnglishToDutchAPICall(text).Replace("[", "").Replace("]", "").Replace("\"", "").TrimStart('\r', '\n', ' ').TrimEnd('\r', '\n', ' ') + additionalText;
        }


        /// <summary>  
        ///  Translate the given English input string to Dutch, which is returned as a string.
        /// </summary> 
        public string TranslateEnglishToDutchAPICall(string text)
        {
            //Original code source (note that the code has been slightly altered):
            //https://code-maze.com/different-ways-consume-restful-api-csharp/#HttpClient
            //https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client

            string url = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20190517T221238Z.a66674a8ced94c60.0211c3eaa0048c93fed460a712b1f70bd2c419ef&text=" + text + "&lang=en-nl";
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetStringAsync(new Uri(url)).Result;
                return JObject.Parse(response)["text"].ToString();
            }
        }

        private Boolean IsCapital(char c)
        {
            if (c.ToString() == c.ToString().ToLower())
            {
                return false;
            }
            return true;
        }

        private string AddBreaks(string lyrics, List<int> breaks, int length)
        {
            //char[] updatedCharArray = new char[length + breaks.Count * 4];
            string updatedLyrics = "";
            int i = 0; //For the original charArray
            //int j = 0; //For the updated  charArray
            int b = 0; //For the list with breaks
            while (i < length)
            {
                updatedLyrics = String.Concat(updatedLyrics, lyrics[i]);
                //updatedCharArray[j] = lyrics[i];
                if (breaks[b] == i)
                {
                    updatedLyrics = String.Concat(updatedLyrics, "<br>");
                    /*
                    j++;
                    updatedCharArray[j] = '<';
                    j++;
                    updatedCharArray[j] = 'b';
                    j++;
                    updatedCharArray[j] = 'r';
                    j++;
                    updatedCharArray[j] = '>';
                    */
                    if (b < breaks.Count - 1) b++;
                }
                i++;
                //j++;
            }
            //Console.WriteLine(updatedCharArray);
            //return updatedCharArray.ToString();
            return updatedLyrics;
        }
    }
}
