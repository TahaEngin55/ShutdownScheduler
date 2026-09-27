# Shutdown Scheduler

Modern, koyu temalı bir Windows masaüstü uygulaması. Bilgisayarını belirlediğin süre sonunda **kapatmak**, **yeniden başlatmak**, **uyku moduna almak** veya **oturumu kapatmak** için zamanlayıcı kurmanı sağlar.

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D6?logo=windows&logoColor=white)

## Özellikler

- 🔌 **4 işlem**: Kapat · Yeniden Başlat · Uyku · Oturumu Kapat
- ⏱️ Dakika cinsinden süre girişi + hızlı ön ayarlar (5 / 15 / 30 / 60)
- 🔢 Canlı geri sayım (saat:dakika:saniye)
- ❌ Çalışan zamanlamayı istediğin an **iptal etme**
- 🎨 Modern koyu tema, özel yuvarlatılmış sürüklenebilir başlık çubuğu
- ✅ Sadece rakam kabul eden giriş, doğrulama ve durum mesajları

## Ekran

Özel başlık çubuğu, işlem seçici, süre kartı, geri sayım ve Başlat / İptal butonları.

## Gereksinimler

- Windows 10/11
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (derlemek için)

## Çalıştırma

```bash
dotnet run --project ShutdownScheduler
```

## Derleme (Release)

```bash
dotnet build ShutdownScheduler/ShutdownScheduler.csproj -c Release
```

Çıktı: `ShutdownScheduler/bin/Release/net10.0-windows/ShutdownScheduler.exe`

## Nasıl çalışır

İşlemler Windows'un yerleşik komutlarıyla yürütülür:

| İşlem | Komut |
|-------|-------|
| Kapat | `shutdown /s /t 0` |
| Yeniden Başlat | `shutdown /r /t 0` |
| Oturumu Kapat | `shutdown /l` |
| Uyku | `rundll32 powrprof.dll,SetSuspendState 0,1,0` |

Geri sayım uygulama içinde tutulur; bu sayede her mod için iptal çalışır. **Not:** Geri sayım sürerken uygulamayı kapatırsan işlem gerçekleşmez.

## Proje yapısı

```
ShutdownScheduler/
├─ ShutdownScheduler.csproj   # net10.0-windows, WPF
├─ App.xaml / App.xaml.cs     # renk paleti + buton/textbox stilleri
├─ MainWindow.xaml            # arayüz
├─ MainWindow.xaml.cs         # mantık (timer, geri sayım, komutlar)
└─ assets/
   ├─ icon.svg                # kaynak ikon
   └─ icon.ico                # uygulama/pencere ikonu
```

## Lisans

MIT
