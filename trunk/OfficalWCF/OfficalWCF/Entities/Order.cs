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
    public class Order
    {
        [DataMember]
        public int OrderID;
        [DataMember]
        public int CustomerID;
        [DataMember]
        public DateTime OrderDate;
        [DataMember]
        public DateTime RequireDate;
        [DataMember]
        public DateTime ShippedDate;
        [DataMember]
        public int ShipVia;
        [DataMember]
        public decimal Freight;
        [DataMember]
        public string ShipName;
        [DataMember]
        public string ShipAddress;
        [DataMember]
        public string ShipCity;
        [DataMember]
        public string ShipRegion;
        [DataMember]
        public string ShipPostalCode;
        [DataMember]
        public string ShipCountry;
    }
}