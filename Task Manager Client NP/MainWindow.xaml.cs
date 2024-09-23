using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;

namespace Task_Manager_Client_NP;


public partial class MainWindow : Window, INotifyPropertyChanged
{
    HttpClient Client = new();
    HttpRequestMessage message = new();
    private List<string> processes;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public List<string> Processes { get => processes; set { processes = value; NotifyPropertyChanged(); } }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Processes = new();
    }

    private async void GETButton_Click(object sender, RoutedEventArgs e)
    {
        message = new();
        Client = new();
        message.RequestUri = new Uri(@"http://localhost:27001/");
        message.Method = HttpMethod.Get;
        

        var response = await Client.GetAsync(message.RequestUri);
        var json = await response.Content.ReadAsStringAsync();
        Processes = JsonSerializer.Deserialize<List<string>>(json);

    }

    private async void RUNButton_Click(object sender, RoutedEventArgs e)
    {
        var SelectedItemName = RunBox.Text;
        message = new();
        Client = new();
        Client.DefaultRequestHeaders.Add("POST", "application.json");
        message.RequestUri = new Uri(@"http://localhost:27001/");
        message.Method = HttpMethod.Post;
        message.Headers.Add("POST", SelectedItemName);
        
              
       
        var response = await Client.PostAsync(message.RequestUri,message.Content);
        var json = await response.Content.ReadAsStringAsync();
        MessageBox.Show(json);
    }

    private async void KILLButton_Click(object sender, RoutedEventArgs e)
    {
        message = new();
        Client = new();
        var SelectedItemName = (ProcessBox.SelectedItem as Process).ProcessName;
        message = new();
        message.RequestUri = new Uri(@"http://localhost:27001/");

        message.Method = HttpMethod.Delete;
        message.Content = new StringContent(SelectedItemName);
        var response = await Client.DeleteAsync(message.RequestUri);
        var json = await response.Content.ReadAsStringAsync();
        MessageBox.Show(json);
    }
}