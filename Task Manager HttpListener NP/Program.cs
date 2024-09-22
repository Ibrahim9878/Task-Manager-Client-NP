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
    StreamWriter writer = new StreamWriter(response.OutputStream);
    if (request.HttpMethod == "GET")
    {
        var processes = Process.GetProcesses().ToList().Select(p => p.ProcessName).ToList();
        string json = JsonSerializer.Serialize(processes);
        writer.WriteLine(json);
    }
    else if (request.HttpMethod == "DELETE")
    {
        var killProcessName = response.Headers["DELETE"];

        var pr = Process.GetProcesses().FirstOrDefault(p => p.ProcessName == killProcessName).Id;
        Process.GetProcessById(pr).Kill();

        writer.WriteLine("ProcessKilled");
    }
    else if (request.HttpMethod == "POST")
    {
        var runProcessName = response.Headers["POST"];

        Process.Start(runProcessName);

        writer.WriteLine("Process Started");
    }
    writer.Close();
}



