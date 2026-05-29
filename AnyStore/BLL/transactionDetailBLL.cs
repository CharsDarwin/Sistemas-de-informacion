using System;

namespace AnyStore.BLL
{
    public class transactionDetailBLL
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public decimal rate { get; set; }
        public decimal qty { get; set; }
        public decimal total { get; set; }
        public int dea_cust_id { get; set; }
        public DateTime added_date { get; set; }
        public int added_by { get; set; }
    }
}
