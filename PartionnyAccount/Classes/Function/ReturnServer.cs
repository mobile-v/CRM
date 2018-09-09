using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartionnyAccount.Classes.Function
{
    internal class ReturnServer
    {
        internal dynamic Return(bool pSuccess, object msg)
        {
            dynamic collectionWrapper = new
            {
                success = pSuccess,
                data = msg,
                //MaxJsonLength = Int32.MaxValue

                /*Accept = "application/json", //, text/javascript
                ContentType = "application/json", //; charset=utf-8
                mimetype = "application/json"*/
            };

            return collectionWrapper;
        }
    }
}