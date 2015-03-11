using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace OfficalWCF.Entities
{
    [DataContract]
    public class OrderDetails
    {
        [DataMember]
        public int OrderID;
        [DataMember]
        public int ProductID;
        [DataMember]
        public decimal UnitPrice;
        [DataMember]
        public int Quantity;
        [DataMember]
        public float Discount;
    }
}