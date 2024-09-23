using System.Diagnostics;
using System.Net;
using System.Text.Json;

var listener = new HttpListener();
listener.Prefixes.Add(@"http://localhost:27001/");
listener.Start();


while (true)
{

    var context = listener.GetContext();
    var request = context.Request;
    var response = context.Response;
    StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
    StreamWriter writer = new StreamWriter(response.OutputStream);
    if (request.HttpMethod == "GET")
    {
        var processes = Process.GetProcesses().Select(p => p.ProcessName).ToList();
        string json = JsonSerializer.Serialize(processes);
        writer.WriteLine(json);
    }
    else if (request.HttpMethod == "DELETE")
    {
        var killProcessName = reader.ReadToEnd();

        var pr = Process.GetProcesses().FirstOrDefault(p => p.ProcessName == killProcessName).Id;

        Process.GetProcessById(pr).Kill();

        writer.WriteLine($"ProcessKilled {pr}");
    }
    else if (request.HttpMethod == "POST")
    { 
        var runProcessName = reader.ReadToEnd();


        Process.Start(runProcessName);

        writer.WriteLine("Process Started");
    }
    writer.Close();
    reader.Close();
}



