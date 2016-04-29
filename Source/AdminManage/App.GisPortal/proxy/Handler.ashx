<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Web.Caching;

/// <summary>
/// Forwards requests to an ArcGIS Server REST resource. Uses information in
/// the proxy.config file to determine properties of the server.
/// </summary>
public class Handler : IHttpHandler
{

    private string[] serverUrls = {
        "http://172.24.57.47/service/RSImage/wms",
        "http://10.246.60.16:6080/arcgis/rest/services"
    };

    public void ProcessRequest(HttpContext context)
    {

        HttpResponse response = context.Response;

        string uri = context.Request.Url.Query.Substring(1);

        context.Response.Headers.Add("Authorization", "Basic c3lsbGhqeHhqeDpzeWxsaGp4eGp4MTIz");


		bool allowed = false;

        string token = null;
        foreach (string surl in serverUrls)
        {
            string[] stokens = StringHelperClass.StringSplit(surl, "\\s*,\\s*", true);
            if (true)
            {
                allowed = true;
                if (stokens.Length >= 2 && stokens[1].Length > 0)
                {
                    token = stokens[1];
                }
                Console.WriteLine(token);
                break;
            }
        }
        if (token != null)
        {
            uri = uri + (uri.IndexOf("?") > -1 ? "&" : "?") + "token=" + token;
        }

        System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
        req.Method = context.Request.HttpMethod;
        req.ServicePoint.Expect100Continue = false;
        req.Referer = context.Request.Headers["referer"];

        // Set body of request for POST requests
        if (context.Request.InputStream.Length > 0)
        {
            byte[] bytes = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(bytes, 0, (int)context.Request.InputStream.Length);
            req.ContentLength = bytes.Length;

            string ctype = context.Request.ContentType;
            if (String.IsNullOrEmpty(ctype))
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }
            else
            {
                req.ContentType = ctype;
            }

            using (Stream outputStream = req.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
        }
        else
        {
            req.Method = "GET";
        }

        // Send the request to the server
        System.Net.WebResponse serverResponse = null;
        try
        {
            serverResponse = req.GetResponse();
        }
        catch (System.Net.WebException webExc)
        {
            response.StatusCode = 500;
            response.StatusDescription = webExc.Status.ToString();
            response.Write(webExc.Response);
            response.End();
            return;
        }

        // Set up the response to the client
        if (serverResponse != null)
        {
            response.ContentType = serverResponse.ContentType;
            using (Stream byteStream = serverResponse.GetResponseStream())
            {

                // Text response
                if (serverResponse.ContentType.Contains("text") ||
                    serverResponse.ContentType.Contains("json") ||
                    serverResponse.ContentType.Contains("xml"))
                {
                    using (StreamReader sr = new StreamReader(byteStream))
                    {
                        string strResponse = sr.ReadToEnd();
                        response.Write(strResponse);
                    }
                }
                else
                {
                    // Binary response (image, lyr file, other binary file)
                    BinaryReader br = new BinaryReader(byteStream);
                    byte[] outb = br.ReadBytes((int)serverResponse.ContentLength);
                    br.Close();

                    // Tell client not to cache the image since it's dynamic
                    response.CacheControl = "no-cache";

                    // Send the image to the client
                    // (Note: if large images/files sent, could modify this to send in chunks)
                    response.OutputStream.Write(outb, 0, outb.Length);
                }

                serverResponse.Close();
            }
        }
        response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
public static class StringHelperClass
{
	public static string[] StringSplit(string source, string regexDelimiter, bool trimTrailingEmptyStrings)
	{
		string[] splitArray = System.Text.RegularExpressions.Regex.Split(source, regexDelimiter);

		if (trimTrailingEmptyStrings)
		{
			if (splitArray.Length > 1)
			{
				for (int i = splitArray.Length; i > 0; i--)
				{
					if (splitArray[i - 1].Length > 0)
					{
						if (i < splitArray.Length)
							System.Array.Resize(ref splitArray, i);

						break;
					}
				}
			}
		}
		return splitArray;
	}
}
