using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(string code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘EST’"));
            string status = GetStatusLine(code);

            // TODO: Create the request string
            if(code=="301")
            {
                headerLines.Add(redirectoinPath);
                responseString = status + "\r\n" + "Content-Type: " + headerLines[0] + "\r\n"
                    + "Content-Length: " + headerLines[1] + "\r\n" + "Date" + headerLines[2] + "\r\n" + "Location" + headerLines[3] + "\r\n" + "\r\n" + "Content: " + content;
            }
            else
            {
                responseString = status + "\r\n" + "Content-Type: " + headerLines[0] + "\r\n"
                    + "Content-Length: " + headerLines[1] + "\r\n" + "Date" + headerLines[2] + "\r\n" + "\r\n" + "Content: " + content;
            }
        }

        private string GetStatusLine(string code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            string httpver = "HTTP/1.1";
            if (code=="200")
            {
                statusLine = httpver + " " + code + "OK";
            }
            else if(code=="301")
            {
                statusLine = httpver + " " + code + "Redirect";
            }
            else if(code=="400")
            {
                statusLine = httpver + " " + code + "BadRequest";
            }
            else if(code=="404")
            {
                statusLine = httpver + " " + code + "NotFound";
            }
            else if(code=="500")
            {
                statusLine = httpver + " " + code + "InternalServerError";
            }
            return statusLine;
        }
    }
}
