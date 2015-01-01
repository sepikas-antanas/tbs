using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace TBS
{
    class Poller
    {
        private string PollAdress;
        private string TableId;
        private int PollTimeout;

        public List<string> RollList { get; set; }

        public Poller(string PollAdress, string TableId, int PollTimeout = 1000)
        {
            this.PollAdress = PollAdress;
            this.TableId = TableId;
            this.PollTimeout = PollTimeout;

            RefreshList(false);
        }

        public void Start()
        {
            //start thread
        }

        public void Stop()
        {
            //kill thread
        }

        public void RefreshList(bool first = true)
        {
            List<string> TmpList = new List<string>();

            if (RollList != null)
            {
                TmpList = RollList;
            }

            foreach (List<int> Item in GetData(first))
            {
                if (first == false) {
                    TmpList.Add(String.Join(" ", Item.ToArray()));
                }
                else
                {
                    TmpList.Insert(0, String.Join(" ", Item.ToArray()));
                }
                
            }

            RollList = TmpList;
        }

        private List<List<int>> GetData(bool first)
        {
            int[] date = { DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day };

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(PollAdress + string.Join("-", date) + "/1");

            HtmlNode table = document.GetElementbyId(TableId).SelectSingleNode("//tbody");

            List<List<int>> results = new List<List<int>>();

            foreach (HtmlNode tr in table.SelectNodes("tr"))
            {
                foreach (HtmlNode td in tr.SelectNodes("td").Skip(1))
                {
                    if (td.SelectSingleNode("span") != null)
                    {
                        List<int> roll = new List<int>();
                        
                        foreach (HtmlNode span in td.SelectNodes("span"))
                        {
                            roll.Add(System.Int32.Parse(span.SelectSingleNode("span").InnerText));
                        }

                        results.Add(roll);
                        
                        if (first == true)
                        {
                            return results;
                        }
                    }
                }
            }

            return results;
        }
    }
}
