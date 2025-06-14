using SatranOyunumApp.Services;

namespace SatranOyunumApp.Views;

public partial class Kullan�c�Kaydi : ContentPage
{
    private readonly SatrancApiService _apiService;

    public Kullan�c�Kaydi(SatrancApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void OnKayitOlClicked(object sender, EventArgs e)
    {
        // Basit validation
        if (string.IsNullOrWhiteSpace(KullaniciAdiEntry.Text))
        {
            await DisplayAlert("Hata", "Kullan�c� ad� bo� olamaz!", "Tamam");
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Hata", "Email bo� olamaz!", "Tamam");
            return;
        }

        if (string.IsNullOrWhiteSpace(SifreEntry.Text))
        {
            await DisplayAlert("Hata", "�ifre bo� olamaz!", "Tamam");
            return;
        }

        // Loading g�ster
        var loadingButton = sender as Button;
        loadingButton.Text = "Kaydediliyor...";
        loadingButton.IsEnabled = false;

        try
        {
            // API'ye kay�t iste�i g�nder
            var sonuc = await _apiService.KullaniciKaydet(
                KullaniciAdiEntry.Text,
                EmailEntry.Text,
                SifreEntry.Text
            );

            if (sonuc.Basarili)
            {
                await DisplayAlert("Ba�ar�l�", sonuc.Mesaj, "Tamam");

                // Form'u temizle
                KullaniciAdiEntry.Text = "";
                EmailEntry.Text = "";
                SifreEntry.Text = "";

                // *** DE���T�R�LEN: Route ismiyle geri d�n ***
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                await DisplayAlert("Hata", sonuc.Mesaj, "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Beklenmeyen hata: {ex.Message}", "Tamam");
        }
        finally
        {
            // Button'u eski haline getir
            loadingButton.Text = "KAYIT OL";
            loadingButton.IsEnabled = true;
        }
    }
}