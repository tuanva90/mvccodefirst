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
    public class Customer
    {
        [DataMember]
        public int CustomerID;
        [DataMember]
        public string ContactName;
        [DataMember]
        public string ContactTitle;
        [DataMember]
        public string Address;
        [DataMember]
        public string City;
        [DataMember]
        public string Region;
        [DataMember]
        public string PostalCode;
        [DataMember]
        public string Country;
        [DataMember]
        public string Phone;
        [DataMember]
        public string Fax;
    }
}