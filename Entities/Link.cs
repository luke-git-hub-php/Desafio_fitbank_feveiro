using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace pokemon.Entities
{
    public class Links : ImageConversion
    {
        public IDictionary<string, string> images;
        public List<string> links;

        public Links() { }

        public static Links Cards(int num)
        {
            WebClient webClient = new WebClient();
            List<string> links = new List<string>();
            Dictionary<string, string> images = new Dictionary<string, string>();
            for (int id = 1; id <= num; id++)
            {
                string pag = webClient.DownloadString("https://www.pokemon.com/us/pokemon-tcg/pokemon-cards/" + id + "?cardName=&cardText=&evolvesFrom=&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=300&retreatCostMin=0&retreatCostMax=5");
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pag);
                HtmlNodeCollection lineDom = htmlDocument.DocumentNode.SelectNodes("//ul[@class='cards-grid clear']/li");
                Parallel.ForEach(lineDom,
                    dom =>
                    {
                        var tag = dom.Element("a");
                        var img = tag.ChildNodes[1].ChildNodes[1];
                        var atr = tag.Attributes["href"].Value;
                        var linktxt = img.Attributes["src"].Value;
                        if (!images.ContainsKey(atr))
                        {
                            images[atr] = GetImageBase64ByUrlEncode(linktxt);
                        }
                        links.Add(atr);
                    });
            }
            return new Links
            {
                links = links,
                images = images
            };
        }
        public static void CardsData(Links links, int numpages, int options)
        {
            List<Card> cardsList = new List<Card>();
            List<List<string>> listMultiCards = new List<List<string>>();
            _ = Parallel.ForEach(links.links,
                link =>
                {
                    var webClient = new WebClient();
                    string page = webClient.DownloadString("https://www.pokemon.com" + link);
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(page);
                    _ = Parallel.ForEach(htmlDocument.DocumentNode.SelectNodes("//div[@class='color-block color-block-gray']"),
                    dom =>
                    {
                        var tag = dom.ChildNodes["h1"];
                        if (tag != null && link != null)
                        {
                            Card cards = new Card
                            {
                                Name = tag.InnerText,
                                Numbering = tag.InnerText,
                                Expansion = tag.InnerText,
                                Url = "https://www.pokemon.com" + link,
                                Image = links.images.FirstOrDefault(x => x.Key == link).Value
                            };
                            tag = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='stats-footer']/span");
                            tag = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='stats-footer']/h3");
                            cardsList.Add(cards);
                        }
                    });
                });
            if (options == 2)
            {
                for (var i = 0; i < numpages; i++)
                {
                    var count = 1;
                    string path = @"C:Pokemon\" + i + ".json";
                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo directory = Directory.CreateDirectory(path);
                    }
                    FileInfo filePrimary = new FileInfo(path);
                    string json = string.Empty;
                    foreach (var card in cardsList)
                    {
                        if (count <= 12)
                        {
                            json += JsonConvert.SerializeObject(card) + ",";
                            count++;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (!filePrimary.Exists /*!string.IsNullOrEmpty(json*/)
                    {
                        StreamWriter file = filePrimary.CreateText();
                        json = json.Remove(json.Length - 1);
                        file.WriteLine("[" + json + "]");
                        file.WriteLine();
                        file.Close();
                    }
                }
            }
            else
            {
                string path = @"C:\Pokemon\";
                string path2 = @"C:\Pokemon\cards.json";
                if (!Directory.Exists(path))
                {
                    DirectoryInfo directory = Directory.CreateDirectory(path);
                }
                FileInfo fi = new FileInfo(path2);
                string json = string.Empty;
                foreach (var card in cardsList)
                {
                    json += JsonConvert.SerializeObject(card) + ",";
                }

                if (!fi.Exists && !string.IsNullOrEmpty(json))
                {
                    using (StreamWriter file = fi.CreateText())
                    {
                        json = json.Remove(json.Length - 1);
                        file.WriteLine("[" + json + "]");
                        file.WriteLine();
                        file.Close();
                    }
                }
            }
        }
    }
}
