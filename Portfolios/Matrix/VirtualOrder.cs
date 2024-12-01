using System;

namespace M4.Matrix
{
    [Serializable]
    public class VirtualOrder
    {
        public string symbol;
        public double price;
        public string operation;
        public int quantity;
        public Guid id;
    }
}
