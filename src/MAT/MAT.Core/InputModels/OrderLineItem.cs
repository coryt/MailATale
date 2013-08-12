using System;

namespace MAT.Core.InputModels
{
    public class OrderLineItem
    {
        public string schedule { get; set; }
        public decimal price { get; set; }
        public string displayPrice { get; set; }
        public string name { get; set; }
        public bool isDoubleBox { get; set; }
    }
}