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
    public class User
    {
        [DataMember]
        public int CustomerID;
        [DataMember]
        public string UserName;
        [DataMember]
        public string PassWord;
        [DataMember]
        public bool Role;
        [DataMember]
        public bool Remember;
    }
}