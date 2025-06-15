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
                if (loginSonucu.Basarili)
                {
                    // *** YEN�: Ba�ar�l� giri�ten sonra ho�geldin ekran�n� g�ster ***
                    HosgeldinEkraniniGoster(loginSonucu);
                }

                //Application.Current.MainPage = new AppShell();
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

    private void HosgeldinEkraniniGoster(LoginSonucu loginSonucu)
    {
        // Login formunu gizle
        LoginFormFrame.IsVisible = false;

        // Ho�geldin ekran�n� g�ster
        HosgeldinFrame.IsVisible = true;

        // Kullan�c� ad�n� g�ster
        HosgeldinLabel.Text = $"Ho�geldiniz {loginSonucu.KullaniciAdi}!";
        Preferences.Set("KullaniciAdi", loginSonucu.KullaniciAdi ?? "Kullan�c�");
        Preferences.Set("KullaniciEmail", EmailEntry.Text);
    }

    private void OnCikisYapClicked(object sender, EventArgs e)
    {
        // Ho�geldin ekran�n� gizle
        HosgeldinFrame.IsVisible = false;

        // Login formunu g�ster
        LoginFormFrame.IsVisible = true;

        // Form alanlar�n� temizle
        EmailEntry.Text = "";
        SifreEntry.Text = "";
    }

    private async void OnSifreDegistirTapped(object sender, EventArgs e)
    {
        // �ifre de�i�tirme sayfas�na git
        await Shell.Current.GoToAsync("//SifreDegistirPage");
    }

    private async void OnOyunaGitTapped(object sender, EventArgs e)
    {
        try
        {
            // GamePage'e git
            await Shell.Current.GoToAsync("//GamePage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Sayfa y�nlendirme hatas�: {ex.Message}", "Tamam");
        }
    }

    private async void OnKayitOlTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Kullan�c�Kaydi");
    }
}