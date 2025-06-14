using SatranOyunumApp.Services;

namespace SatranOyunumApp.Views;

public partial class TestPage : ContentPage
{
    private readonly SatrancApiService _apiService;

    public TestPage(SatrancApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void OnTestClicked(object sender, EventArgs e)
    {
        StatusLabel.Text = "Test ediliyor...";

        bool isConnected = await _apiService.TestConnection();

        if (isConnected)
        {
            StatusLabel.Text = "? API Ba�lant�s� Ba�ar�l�!";
        }
        else
        {
            StatusLabel.Text = "? API Ba�lant�s� Ba�ar�s�z!";
        }
    }
}