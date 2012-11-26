using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

namespace NetworkRegistrationToolScraper
{
public class RequestState
{
  // This class stores the State of the request.
  const int BUFFER_SIZE = 1024;
  public StringBuilder requestData;
  public byte[] BufferRead;
  public HttpWebRequest request;
  public HttpWebResponse response;
  public Stream streamResponse;

  public RequestState()
  {
    BufferRead = new byte[BUFFER_SIZE];
    requestData = new StringBuilder("");
    request = null;
    streamResponse = null;
  }
}

public class Example
{

  public static ManualResetEvent allDone= new ManualResetEvent(false);
  const int BUFFER_SIZE = 1024;

  public static void Demo()
  {
      try
      {
          System.Uri uri = new Uri("http://www.contoso.com");

          // Create a HttpWebrequest object to the desired URL.
          HttpWebRequest myHttpWebRequest1= (HttpWebRequest)WebRequest.Create(uri);

          // Create an instance of the RequestState and assign the previous myHttpWebRequest1
          // object to it's request field.  
          RequestState myRequestState = new RequestState();  
          myRequestState.request = myHttpWebRequest1;

          // Start the asynchronous request.
          IAsyncResult result=
          (IAsyncResult) myHttpWebRequest1.BeginGetResponse(new AsyncCallback(RespCallback),myRequestState);

          allDone.WaitOne();

          // Release the HttpWebResponse resource.
          myRequestState.response.Close();
      }
      catch(WebException e)
      {
          outputBlock.Text += "\nException raised!\n";
          outputBlock.Text += "Message: ";
          outputBlock.Text += e.Message;
          outputBlock.Text += "\nStatus: ";
          outputBlock.Text += e.Status;
          outputBlock.Text += "\n";
      }
      catch(Exception e)
      {
          outputBlock.Text += "\nException raised!\n";
          outputBlock.Text += "\nMessage: ";
          outputBlock.Text += e.Message;
          outputBlock.Text += "\n";
      }
  }

  private static void RespCallback(IAsyncResult asynchronousResult)
  {  
      try
      {
          // State of request is asynchronous.
          RequestState myRequestState=(RequestState) asynchronousResult.AsyncState;
          HttpWebRequest  myHttpWebRequest2=myRequestState.request;
          myRequestState.response = (HttpWebResponse) myHttpWebRequest2.EndGetResponse(asynchronousResult);

          // Read the response into a Stream object.
          Stream responseStream = myRequestState.response.GetResponseStream();
          myRequestState.streamResponse=responseStream;

          // Begin the Reading of the contents of the HTML page and print it to the console.
          IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
      }
      catch(WebException e)
      {
          // Need to handle the exception
          // ...

          Debug.WriteLine(e.Message);
      }
  }

  private static  void ReadCallBack(IAsyncResult asyncResult)
  {
      try
      {
          RequestState myRequestState = (RequestState)asyncResult.AsyncState;
          Stream responseStream = myRequestState.streamResponse;
          int read = responseStream.EndRead( asyncResult );

          // Read the HTML page and then do something with it
          if (read > 0)
          {
              myRequestState.requestData.Append(Encoding.UTF8.GetString(myRequestState.BufferRead, 0, read));
              IAsyncResult asynchronousResult = responseStream.BeginRead( myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
          }
          else
          {
              if(myRequestState.requestData.Length>1)
              {
                  string stringContent;
                  stringContent = myRequestState.requestData.ToString();
                  // do something with the response stream here
              }

              responseStream.Close();
              allDone.Set();     
          }
      }
      catch(WebException e)
      {
          // Need to handle the exception
          // ...

          Debug.WriteLine(e.Message);
      }
  }

}
