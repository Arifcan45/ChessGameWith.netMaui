﻿using Microsoft.EntityFrameworkCore;
using SatrancApi.Entities.Models;
using SatrancAPI.Datas;
using SatrancAPI.Entities.HamleSınıfları;
using SatrancAPI.Entities.Models;

namespace SatrancAPI.Services
{
    public class OyunYoneticisi
    {
        private readonly SatrancDbContext _dbContext;
        private readonly TahtaYoneticisi _tahtaYoneticisi;

        public OyunYoneticisi(SatrancDbContext dbContext, TahtaYoneticisi tahtaYoneticisi)
        {
            _dbContext = dbContext;
            _tahtaYoneticisi = tahtaYoneticisi;
        }

        // Yeni oyun oluşturur
        public async Task<Oyun> YeniOyunOlustur(Guid beyazOyuncuId, Guid siyahOyuncuId)
        {
            var beyazOyuncu = await _dbContext.Oyuncular.FindAsync(beyazOyuncuId);
            var siyahOyuncu = await _dbContext.Oyuncular.FindAsync(siyahOyuncuId);

            if (beyazOyuncu == null || siyahOyuncu == null)
                throw new Exception("Oyuncular bulunamadı");

            var oyun = new Oyun
            {
                OyunId = Guid.NewGuid(),
                BeyazOyuncuId = beyazOyuncuId,
                SiyahOyuncuId = siyahOyuncuId,
                BeyazOyuncu = beyazOyuncu,
                SiyahOyuncu = siyahOyuncu,
                BaslangicTarihi = DateTime.Now,
                Durum = Durum.Oynaniyor,
                BeyazSkor = 0,
                SiyahSkor = 0,
                SiraKimin = 0, // Beyaz başlar
                ToplamHamleSayisi = 0,
                SahDurumu = false
            };

            _dbContext.Oyunlar.Add(oyun);

            // Başlangıç taşlarını oluştur
            BaslangicTaslariniOlustur(oyun);

            await _dbContext.SaveChangesAsync();
            return oyun;
        }

        // Başlangıç taşlarını oluşturur
        private void BaslangicTaslariniOlustur(Oyun oyun)
        {
            // Beyaz piyonlar
            for (int y = 0; y < 8; y++)
            {
                _dbContext.Taslar.Add(new Tas
                {
                    TasId = Guid.NewGuid(),
                    OyunId = oyun.OyunId,
                    OyuncuId = oyun.BeyazOyuncuId,
                    renk = Renk.Beyaz,
                    turu = TasTuru.Piyon,
                    X = 6,
                    Y = y,
                    AktifMi = true,
                    HicHareketEtmediMi = true
                });
            }

            // Beyaz diğer taşlar
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.BeyazOyuncuId, renk = Renk.Beyaz, turu = TasTuru.Kale, X = 7, Y = 0, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.BeyazOyuncuId, renk = Renk.Beyaz, turu = TasTuru.At, X = 7, Y = 1, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.BeyazOyuncuId, renk = Renk.Beyaz, turu = TasTuru.Fil, X = 7, Y = 2, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.BeyazOyuncuId, renk = Renk.Beyaz, turu = TasTuru.Vezir, X = 7, Y = 3, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.BeyazOyuncuId, renk = Renk.Beyaz, turu = TasTuru.Şah, X = 7, Y = 4, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.BeyazOyuncuId, renk = Renk.Beyaz, turu = TasTuru.Fil, X = 7, Y = 5, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.BeyazOyuncuId, renk = Renk.Beyaz, turu = TasTuru.At, X = 7, Y = 6, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.BeyazOyuncuId, renk = Renk.Beyaz, turu = TasTuru.Kale, X = 7, Y = 7, AktifMi = true, HicHareketEtmediMi = true });

            // Siyah piyonlar
            for (int y = 0; y < 8; y++)
            {
                _dbContext.Taslar.Add(new Tas
                {
                    TasId = Guid.NewGuid(),
                    OyunId = oyun.OyunId,
                    OyuncuId = oyun.SiyahOyuncuId,
                    renk = Renk.Siyah,
                    turu = TasTuru.Piyon,
                    X = 1,
                    Y = y,
                    AktifMi = true,
                    HicHareketEtmediMi = true
                });
            }

            // Siyah diğer taşlar
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.SiyahOyuncuId, renk = Renk.Siyah, turu = TasTuru.Kale, X = 0, Y = 0, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.SiyahOyuncuId, renk = Renk.Siyah, turu = TasTuru.At, X = 0, Y = 1, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.SiyahOyuncuId, renk = Renk.Siyah, turu = TasTuru.Fil, X = 0, Y = 2, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.SiyahOyuncuId, renk = Renk.Siyah, turu = TasTuru.Vezir, X = 0, Y = 3, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.SiyahOyuncuId, renk = Renk.Siyah, turu = TasTuru.Şah, X = 0, Y = 4, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.SiyahOyuncuId, renk = Renk.Siyah, turu = TasTuru.Fil, X = 0, Y = 5, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.SiyahOyuncuId, renk = Renk.Siyah, turu = TasTuru.At, X = 0, Y = 6, AktifMi = true, HicHareketEtmediMi = true });
            _dbContext.Taslar.Add(new Tas { TasId = Guid.NewGuid(), OyunId = oyun.OyunId, OyuncuId = oyun.SiyahOyuncuId, renk = Renk.Siyah, turu = TasTuru.Kale, X = 0, Y = 7, AktifMi = true, HicHareketEtmediMi = true });
        }

        // Bir oyunun tahta durumunu veritabanından alır ve tahta yöneticisine yükler
        public async Task TahtayiYukle(Guid oyunId)
        {
            var taslar = await _dbContext.Taslar
                .Where(t => t.OyunId == oyunId && t.AktifMi)
                .ToListAsync();

            _tahtaYoneticisi.TahtayiOlustur(taslar);
        }

        // ✅ Tahta döndürür
        public async Task<Tas[,]> TahtayiGetir(Guid oyunId)
        {
            await TahtayiYukle(oyunId);
            return _tahtaYoneticisi.TahtayiGetir();
        }

        // ✅ Şah tehdit durumunu kontrol et
        public bool SahTehditAltindaMi(Renk sahRengi)
        {
            return _tahtaYoneticisi.SahTehditAltindaMi(sahRengi);
        }

        // ✅ Şah mat durumunu kontrol et
        public bool SahMatMi(Renk sahRengi)
        {
            return _tahtaYoneticisi.SahMatMi(sahRengi);
        }

        // Hamle yapar - ✅ TEK KİŞİLİK OYUN DESTEĞİ EKLENDİ
        public async Task<bool> HamleYap(Guid oyunId, Guid tasId, int hedefX, int hedefY)
        {
            var oyun = await _dbContext.Oyunlar.FindAsync(oyunId);
            if (oyun == null || oyun.Durum != Durum.Oynaniyor)
                return false;

            await TahtayiYukle(oyunId);

            var tas = await _dbContext.Taslar
                .FirstOrDefaultAsync(t => t.TasId == tasId && t.AktifMi);

            if (tas == null)
                return false;

            // ✅ TEK KİŞİLİK OYUN KONTROLÜ
            bool tekKislikOyun = oyun.BeyazOyuncuId == oyun.SiyahOyuncuId;

            if (!tekKislikOyun)
            {
                // Sıra kontrolü - sadece çok oyunculu oyunlarda
                var sonHamle = await _dbContext.Hamleler
                    .Where(h => h.OyunId == oyunId)
                    .OrderByDescending(h => h.HamleTarihi)
                    .FirstOrDefaultAsync();

                bool beyazinSirasi = sonHamle == null || sonHamle.OyuncuId == oyun.SiyahOyuncuId;

                if ((beyazinSirasi && tas.renk != Renk.Beyaz) || (!beyazinSirasi && tas.renk != Renk.Siyah))
                    return false;
            }

            // Hedef konumdaki taş (varsa)
            var hedefTas = await _dbContext.Taslar
                .FirstOrDefaultAsync(t => t.OyunId == oyunId && t.X == hedefX && t.Y == hedefY && t.AktifMi);

            // Hamle yap
            if (!_tahtaYoneticisi.HamleYap(tas, hedefX, hedefY))
                return false;

            // Taşın konumunu güncelle
            int eskiX = tas.X;
            int eskiY = tas.Y;
            tas.X = hedefX;
            tas.Y = hedefY;
            tas.HicHareketEtmediMi = false;
            tas.SonHareketTarihi = DateTime.Now;

            // Hedef konumdaki taş varsa ele al
            if (hedefTas != null)
            {
                hedefTas.AktifMi = false;
            }

            // Hamle sayısını artır
            oyun.ToplamHamleSayisi++;

            // Hamle kaydet
            var hamle = new Hamle
            {
                HamleId = Guid.NewGuid(),
                OyunId = oyunId,
                OyuncuId = tas.OyuncuId,
                TasId = tas.TasId,
                turu = tas.turu,
                BaslangicX = eskiX,
                BaslangicY = eskiY,
                HedefX = hedefX,
                HedefY = hedefY,
                HamleTarihi = DateTime.Now
            };

            _dbContext.Hamleler.Add(hamle);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Geçerli hamleleri getir - ✅ DÜZELTME: Koordinat listesi döndürür
        public async Task<List<Koordinat>> GecerliHamleleriGetir(Guid oyunId, Guid tasId)
        {
            // Tahtayı yükle
            await TahtayiYukle(oyunId);
            var tas = await _dbContext.Taslar.FirstOrDefaultAsync(t => t.TasId == tasId && t.AktifMi);
            if (tas == null)
                return new List<Koordinat>();

            // TahtaYoneticisi'nden tuple listesi al ve Koordinat listesine çevir
            var hamleler = _tahtaYoneticisi.GecerliHamleleriGetir(tas);
            return hamleler.Select(h => new Koordinat(h.x, h.y)).ToList();
        }

        // ✅ Oyunu bitir
        public async Task<bool> OyunuBitir(Guid oyunId, string bitisNedeni, string kazanan = null)
        {
            var oyun = await _dbContext.Oyunlar.FindAsync(oyunId);
            if (oyun == null)
                return false;

            oyun.Durum = Durum.Bitiyor;
            oyun.BitisTarihi = DateTime.Now;
            oyun.BitisNedeni = bitisNedeni;

            if (!string.IsNullOrEmpty(kazanan))
            {
                oyun.KazananOyuncu = kazanan;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}