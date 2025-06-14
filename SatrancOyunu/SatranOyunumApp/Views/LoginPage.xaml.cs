using SatranOyunumApp.Services;

namespace SatranOyunumApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly SatrancApiService _apiService;

    public LoginPage(SatrancApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void OnGirisYapClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Hata", "Email alan� bo� olamaz", "Tamam");
            return;
        }

        if (string.IsNullOrWhiteSpace(SifreEntry.Text))
        {
            await DisplayAlert("Hata", "�ifre alan� bo� olamaz", "Tamam");
            return;
        }

        // Loading g�ster
        GirisButton.Text = "Giri� yap�l�yor...";
        GirisButton.IsEnabled = false;

        try
        {
            // API'den login kontrol� yap
            var loginSonucu = await _apiService.Login(EmailEntry.Text, SifreEntry.Text);

            if (loginSonucu.Basarili)
            {
                // Ba�ar�l� giri�
                await DisplayAlert("Ba�ar�l�", $"Ho�geldin! {EmailEntry.Text}", "Tamam");

                //// Ana sayfaya y�nlendir (�imdilik TestPage'e gidelim)
                //await Shell.Current.GoToAsync("//TestPage");
                // Ana sayfaya y�nlendir (�imdilik MainPage)
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                // Hatal� giri�
                await DisplayAlert("Hata", loginSonucu.Mesaj, "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Bir hata olu�tu: " + ex.Message, "Tamam");
        }
        finally
        {
            // Loading'i kapat
            GirisButton.Text = "G�R�� YAP";
            GirisButton.IsEnabled = true;
        }
    }
    private async void OnSifreDegistirTapped(object sender, EventArgs e)
    {
        // �ifre de�i�tirme sayfas�na git
        await Shell.Current.GoToAsync("//SifreDegistirPage");
    }

    private async void OnOyunaGitTapped(object sender, EventArgs e)
    {
        // Direkt oyun sayfas�na git (hangi sayfa oldu�unu s�ylerseniz do�ru route'u yazar�m)
        await Shell.Current.GoToAsync("//MainPage"); // Bu k�sm� oyun sayfan�z�n route'una g�re de�i�tirin
    }

    private async void OnKayitOlTapped(object sender, EventArgs e)
    {
        // *** YANLI�: "//Kullan�c�Kaydi" yerine "Kullan�c�Kaydi" olmal� ***
        await Shell.Current.GoToAsync("Kullan�c�Kaydi");
    }
}