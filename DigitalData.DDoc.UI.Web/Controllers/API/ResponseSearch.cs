using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalData.Open.WebUI.Controllers.API
{
    public class Doc
    {
        public string author { get; set; }
        public string authorS { get; set; }
        public List<string> content { get; set; }
        public List<string> contentType { get; set; }
        public object description { get; set; }
        public string id { get; set; }
        public object keywords { get; set; }
        public string resourcename { get; set; }
        public object subject { get; set; }
        public double version { get; set; }
    }

    public class Response
    {
        public List<Doc> docs { get; set; }
        public int maxScore { get; set; }
        public int numFound { get; set; }
        public int start { get; set; }
    }

    public class Params
    {
        public string Fq { get; set; }
        public string Q { get; set; }
    }

    public class ResponseHeader
    {
        public Params Params { get; set; }
        public int QTime { get; set; }
        public int status { get; set; }
        public bool zkConnected { get; set; }
    }

    public class ResponseSearch
    {
        public Response response { get; set; }
        public ResponseHeader responseHeader { get; set; }
    }


}