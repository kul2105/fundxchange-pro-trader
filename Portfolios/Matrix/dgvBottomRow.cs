using System;

namespace M4.Matrix
{
    public class dgvBottomRow
    {
        double m_p_l;
        double m_order;
        double m_bidsize;
        double m_asksize;
        double m_price;
        double m_volume;
        public dgvBottomRow(double p_l, double order, double bid, double ask, double volume, double price)
        {
            m_asksize = ask;
            m_bidsize = bid;
            m_order = order;
            m_p_l = p_l;
            m_price = price;
            m_volume = volume;
        }
    }
}
