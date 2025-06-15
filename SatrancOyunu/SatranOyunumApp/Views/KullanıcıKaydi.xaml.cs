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
        // *** YEN�: Email do�rulamas� ***
        if (!EmailDogrula(EmailEntry.Text))
        {
            await DisplayAlert("Ge�ersiz Email",
                "L�tfen ge�erli bir Gmail adresi giriniz.\n�rnek: kullaniciadi@gmail.com",
                "Tamam");
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
    /// <summary>
    /// Email adresinin @gmail.com ile bitip bitmedi�ini kontrol eder
    /// </summary>
    /// <param name="email">Kontrol edilecek email adresi</param>
    /// <returns>Ge�erli ise true, de�ilse false</returns>
    private bool EmailDogrula(string email)
    {
        // Null veya bo� kontrol
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Trim ile ba��ndaki ve sonundaki bo�luklar� temizle
        email = email.Trim().ToLower();

        // @ i�areti var m� kontrol et
        if (!email.Contains("@"))
            return false;

        // @gmail.com ile bitiyor mu kontrol et
        if (!email.EndsWith("@gmail.com"))
            return false;

        // @ i�aretinden �nce en az 1 karakter var m� kontrol et
        string kullaniciAdi = email.Substring(0, email.IndexOf("@"));
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return false;

        // Kullan�c� ad�nda ge�ersiz karakterler var m� kontrol et
        if (kullaniciAdi.Contains(" ") || kullaniciAdi.Contains(".."))
            return false;

        return true;
    }
}