using System;
using System.Collections.Generic;
using FundXchange.Domain.Entities;

namespace FundXchange.Model.ViewModels.MarketDepth
{
    public class CompressedDatas
    {
        List<VolumeAndCount> m_Bid = new List<VolumeAndCount>();
        List<VolumeAndCount> m_Ask = new List<VolumeAndCount>();

        public void UpdateBid(List<Bid> bids, double blockWeight)
        {
            m_Bid.Clear();
            VolumeAndCount temp = null;
            for (int i = 0; i < bids.Count; i++)
            {

                if (temp != null && bids[i].Price > temp.Price - blockWeight)
                {
                    temp.Size += bids[i].Size;
                    temp.Count++;
                }
                else
                {
                    if (m_Bid.Count == 6) return;
                    temp = new VolumeAndCount(bids[i].Price, bids[i].Size);
                    m_Bid.Add(temp);
                }
            }
        }

        public void UpdateAsk(List<Offer> offers, double blockWeight)
        {
            m_Ask.Clear();
            VolumeAndCount temp = null;
            for (int i = 0; i < offers.Count; i++)
            {
                if (temp != null && offers[i].Price < temp.Price + blockWeight)
                {
                    temp.Size += offers[i].Size;
                    temp.Count++;
                }
                else
                {
                    if (m_Ask.Count == 6) return;
                    temp = new VolumeAndCount(offers[i].Price, offers[i].Size);
                    m_Ask.Add(temp);
                }
            }

        }

        public int GetCount()
        {
            return m_Ask.Count < m_Bid.Count ? m_Bid.Count : m_Ask.Count;
        }

        public string NewData(int rowIndex, int cellIndex)
        {
            string retVal = string.Empty;

            if (m_Bid.Count > rowIndex && m_Ask.Count > rowIndex)
            {
                switch (cellIndex)
                {
                    case 0:

                        if (m_Bid.Count > rowIndex)
                            retVal = m_Bid[rowIndex].Price.ToString();
                        else
                            retVal = "0";
                        retVal += " * ";

                        if (m_Ask.Count > rowIndex)
                            retVal += m_Ask[rowIndex].Price.ToString();
                        else
                            retVal += "0";

                        //retVal = (m_Bid.Count > rowIndex ? m_Bid[rowIndex].Price.ToString() : "0") 
                        //    + " * " + (m_Ask.Count > rowIndex ? m_Ask[rowIndex].Price.ToString() : "0");
                        break;
                    case 1:

                        if (m_Bid.Count > rowIndex)
                            retVal = m_Bid[rowIndex].Count.ToString();
                        else
                            retVal = "0";
                        retVal += " * ";

                        if (m_Ask.Count > rowIndex)
                            retVal += m_Ask[rowIndex].Count.ToString();
                        else
                            retVal += "0";

                        //retVal = (m_Bid.Count > rowIndex ? m_Bid[rowIndex].Count.ToString() : "0") 
                        //    + " * " + (m_Ask.Count > rowIndex ? m_Ask[rowIndex].Count.ToString() : "0");
                        break;
                    case 2:

                        if (m_Bid.Count > rowIndex)
                            retVal = m_Bid[rowIndex].Size.ToString();
                        else
                            retVal = "0";
                        retVal += " * ";

                        if (m_Ask.Count > rowIndex)
                            retVal += m_Ask[rowIndex].Size.ToString();
                        else
                            retVal += "0";

                        //retVal = (m_Bid.Count > rowIndex ? m_Bid[rowIndex].Size.ToString() : "0") 
                        //    + " * " + (m_Ask.Count > rowIndex ? m_Ask[rowIndex].Size.ToString() : "0");
                        break;
                    case 3:

                        retVal = Math.Abs(m_Bid[rowIndex].Price - m_Ask[rowIndex].Price).ToString();

                        //retVal = (m_Bid.Count > rowIndex ? m_Bid[rowIndex].Price : 0 - m_Ask.Count > rowIndex ? m_Ask[rowIndex].Price : 0).ToString();
                        break;
                }
            }

            return retVal;
        }

    }
}
