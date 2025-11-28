using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Account : BaseModel
    {
        protected override DocumentList CreateDemo()
        {
            return new DocumentList {
                new Document { ObjectId = "tung", Phone = "0989154248", Role = "Customer", },
                new Document { ObjectId = "tho", Phone = "0902186628", Role = "Customer", },
            };
        }
        public Account() : base("accounts")
        {
        }
    }
}
