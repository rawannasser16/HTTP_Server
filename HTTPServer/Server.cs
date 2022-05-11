using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 1000);

            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(endPoint); 

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] receivedData = new byte[65536];
                    int receivedLength = clientSocket.Receive(receivedData);
                    string data = Encoding.ASCII.GetString(receivedData);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0) break;

                    // TODO: Create a Request object using received request string
                    Request clientRequest = new Request(data);

                    // TODO: Call HandleRequest Method that returns the response
                    Response serverResponse = HandleRequest(clientRequest);
                    string res = serverResponse.ResponseString;
                    byte[] response = Encoding.ASCII.GetBytes(res);

                    // TODO: Send Response back to client
                    clientSocket.Send(response);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
        }

        Response HandleRequest(Request request)
        {
            
            string content = "", code = "";
            try
            {
                //throw new Exception();
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    code = "400";
                    content = "<!DOCTYPE html><html><body><h1>400 Bad Request</h1><p>400 Bad Request</p></body></html>";
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string[] pagename = request.relativeURI.Split('/');
                string physical_path = Configuration.RootPath + '\\' + pagename[1];

                //TODO: check for redirect
                for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
                {
                    if(pagename[1] == Configuration.RedirectionRules.Keys.ElementAt(i).ToString())
                    {
                        string redirectedPath = Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                        physical_path = Configuration.RootPath + '\\' + redirectedPath;
                    }
                }

                //TODO: check file exists
                if(!File.Exists(physical_path))
                {
                    physical_path = Configuration.RootPath + '\\' + "Notfound.html";
                    code = "404";
                    content = File.ReadAllText(physical_path);
                }
                //TODO: read the physical file
                else
                {
                    code = "200";
                    content = File.ReadAllText(physical_path);
                }

                // Create OK response
                Response rs = new Response(code, "text/html", content, physical_path);
                return rs;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                string physical_path = Configuration.RootPath + '\\' + "InternalError.html";
                code = "500";
                content = File.ReadAllText(physical_path);

                Response rs = new Response(code, "text/html", content, physical_path);
                return rs;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            
            // else read file and return its content
            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                FileStream fs = new FileStream("redirectionRules.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs); 

                // then fill Configuration.RedirectionRules dictionary 
                while(sr.Peek() != -1)
                {
                    string line = sr.ReadLine();
                    string[] data = line.Split(',');
                    if (data[0] == "") break;

                    Configuration.RedirectionRules.Add(data[0], data[1]);
                }

                fs.Close(); 
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
