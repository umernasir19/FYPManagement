using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYPAUtOMATION.Models
{
    public class AdminDocument
    {
        public int ID { get; set; }

        public string Document_Name { get; set; }

        public string DocumentPath { get; set; }

        public HttpPostedFileWrapper Document { get; set; }
    }
}