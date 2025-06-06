﻿using SatrancAPI.Entities.HamleSınıfları;
using SatrancAPI.Entities.Models;

namespace SatrancAPI.Services
{
    public class TahtaYoneticisi
    {
        private Tas[,] _tahta;

        public TahtaYoneticisi()
        {
            _tahta = new Tas[8, 8]; // 8x8 boş tahta
        }

        // Veritabanındaki taşlardan tahta oluşturur
        public void TahtayiOlustur(List<Tas> taslar)
        {
            _tahta = new Tas[8, 8]; // Tahtayı temizle

            foreach (var tas in taslar)
            {
                if (tas.X >= 0 && tas.X < 8 && tas.Y >= 0 && tas.Y < 8 && tas.AktifMi)
                {
                    _tahta[tas.X, tas.Y] = tas;
                }
            }
        }

        // Bir taşın geçerli hamlelerini hesaplar
        public List<(int x, int y)> GecerliHamleleriGetir(Tas tas)
        {
            if (tas == null) return new List<(int x, int y)>();

            var hamle = HamleFactory.HamleSinifiGetir(tas.turu);
            return hamle.getGecerliHamleler(tas, _tahta);
        }

        // Hamlenin geçerli olup olmadığını kontrol eder
        public bool HamleGecerliMi(Tas tas, int hedefX, int hedefY)
        {
            var gecerliHamleler = GecerliHamleleriGetir(tas);
            return gecerliHamleler.Contains((hedefX, hedefY));
        }

        // Hamle yapar ve tahtayı günceller
        public bool HamleYap(Tas tas, int hedefX, int hedefY)
        {
            if (!HamleGecerliMi(tas, hedefX, hedefY))
                return false;

            // Eğer hedef konumda rakip taş varsa, onu ele
            if (_tahta[hedefX, hedefY] != null)
            {
                _tahta[hedefX, hedefY].AktifMi = false; // Taş ele alındı
            }

            // Taşın konumunu güncelle
            _tahta[tas.X, tas.Y] = null;
            _tahta[hedefX, hedefY] = tas;

            // Taş nesnesinin konumunu güncelle
            tas.X = hedefX;
            tas.Y = hedefY;

            return true;
        }        // Şah tehdit altında mı kontrolü
        public bool SahTehditAltindaMi(Renk sahRengi)
        {
            // Şahın konumunu bul
            Tas sah = null;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (_tahta[x, y]?.turu == TasTuru.Şah && _tahta[x, y].renk == sahRengi)
                    {
                        sah = _tahta[x, y];
                        break;
                    }
                }
                if (sah != null) break;
            }

            if (sah == null) return false;

            // Rakip taşların şahı tehdit edip etmediğini kontrol et
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var tas = _tahta[x, y];
                    if (tas != null && tas.renk != sahRengi)
                    {
                        var hamle = HamleFactory.HamleSinifiGetir(tas.turu);
                        var gecerliHamleler = hamle.getGecerliHamleler(tas, _tahta);

                        if (gecerliHamleler.Contains((sah.X, sah.Y)))
                            return true;
                    }
                }
            }

            return false;
        }
        
        // Şah mat kontrolü - Şah tehdit altında ve kurtuluş hamlesi yok
        public bool SahMatMi(Renk sahRengi)
        {
            // Önce şahın tehdit altında olup olmadığını kontrol et
            if (!SahTehditAltindaMi(sahRengi))
                return false;
            
            // Şah tehdit altındaysa, oyuncunun yapabileceği tüm hamleler şahı kurtarıyor mu kontrol et
            List<Tas> oyuncuTaslari = new List<Tas>();
            
            // Oyuncunun tüm taşlarını bul
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (_tahta[x, y] != null && _tahta[x, y].renk == sahRengi)
                    {
                        oyuncuTaslari.Add(_tahta[x, y]);
                    }
                }
            }
            
            // Her bir taş için tüm olası hamleleri dene ve şahı kurtarıp kurtarmadığını kontrol et
            foreach (var tas in oyuncuTaslari)
            {
                var hamle = HamleFactory.HamleSinifiGetir(tas.turu);
                var gecerliHamleler = hamle.getGecerliHamleler(tas, _tahta);
                
                foreach (var (hedefX, hedefY) in gecerliHamleler)
                {
                    // Hamleyi geçici olarak yap ve şahın tehdit altında olup olmadığını kontrol et
                    var geciciTahta = KopyalaTahta();
                    var geciciTas = geciciTahta[tas.X, tas.Y];
                    
                    // Hedefte taş varsa geçici olarak kaldır
                    Tas hedefTas = null;
                    if (geciciTahta[hedefX, hedefY] != null)
                    {
                        hedefTas = geciciTahta[hedefX, hedefY];
                        geciciTahta[hedefX, hedefY] = null;
                    }
                    
                    // Taşı hedef konuma taşı
                    geciciTahta[hedefX, hedefY] = geciciTas;
                    geciciTahta[tas.X, tas.Y] = null;
                    
                    // Hamle yapıldıktan sonra şahın tehdit altında olup olmadığını kontrol et
                    bool sahHalaTehditAltinda = SahTehditAltindaMiTahtaIle(sahRengi, geciciTahta);
                    
                    // Şah hala tehdit altında değilse, bu kurtuluş hamlesi demektir
                    if (!sahHalaTehditAltinda)
                        return false; // Şah mat değil
                }
            }
            
            // Hiçbir kurtuluş hamlesi bulunamadıysa şah mat
            return true;
        }
        
        // Belirli bir tahta durumuna göre şah tehdit kontrolü
        private bool SahTehditAltindaMiTahtaIle(Renk sahRengi, Tas[,] tahta)
        {
            // Şahın konumunu bul
            Tas sah = null;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (tahta[x, y]?.turu == TasTuru.Şah && tahta[x, y].renk == sahRengi)
                    {
                        sah = tahta[x, y];
                        break;
                    }
                }
                if (sah != null) break;
            }
            
            if (sah == null) return false;
            
            // Rakip taşların şahı tehdit edip etmediğini kontrol et
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var tas = tahta[x, y];
                    if (tas != null && tas.renk != sahRengi)
                    {
                        var hamle = HamleFactory.HamleSinifiGetir(tas.turu);
                        var gecerliHamleler = hamle.getGecerliHamleler(tas, tahta);
                        
                        if (gecerliHamleler.Contains((sah.X, sah.Y)))
                            return true;
                    }
                }
            }
            
            return false;
        }
        
        // Tahtanın kopyasını oluştur (şah mat kontrolü için)
        private Tas[,] KopyalaTahta()
        {
            Tas[,] kopyaTahta = new Tas[8, 8];
            
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    kopyaTahta[x, y] = _tahta[x, y];
                }
            }
            
            return kopyaTahta;
        }

        // Mevcut tahta durumunu döndürür
        public Tas[,] TahtayiGetir()
        {
            return _tahta;
        }
    }
}