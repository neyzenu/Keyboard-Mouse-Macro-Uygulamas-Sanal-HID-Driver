# Keyboard-Mouse-Macro-Uygulamas-Sanal-HID-Driver
Keyboard/Mouse Macro Uygulaması + Sanal HID Driver
# Virtual HID Keyboard & Mouse Macro Project

Bu proje, Windows üzerinde **sanal bir HID (klavye/fare) aygıtı** oluşturarak, 
C# tarafında geliştirilen bir uygulamanın **makro tuş basımı** ve **mouse hareketi** 
gönderebilmesini sağlar. Böylece işletim sistemi, sanki gerçek bir klavye ve fare 
takılıymış gibi gelen girdileri algılayabilir.

## Özellikler
- **KMDF tabanlı driver**: `Driver` klasörü altındaki kaynak kodlar.
- **C# Makro Uygulaması**: `App` klasörü altındaki `Program.cs`.
- **IOCTL** ile haberleşme: C# uygulaması, driver'a `DeviceIoControl` aracılığıyla 
  özel komutlar (IOCTL) gönderir.
- **Global etki**: Sanal klavye/fare girişleri, tüm uygulamalarda gerçek giriş 
  olarak algılanır.

## Klasör Yapısı
MyVirtualHIDProject ├─ Driver │ ├─ Driver.c │ ├─ Driver.h │ ├─ myDriver.inf │ └─ ... ├─ App │ ├─ VirtualHidClient.csproj │ └─ Program.cs ├─ LICENSE └─ README.md


### Driver (KMDF)
- `Driver.c` / `Driver.h`: KMDF sürücünüzün temel fonksiyonlarını içerir. 
  - `DriverEntry`, `EvtDeviceAdd`, `EvtIoDeviceControl` gibi WDF callback’ler tanımlı.
- `myDriver.inf`: Sürücünün nasıl yükleneceğini tarif eder (DeviceClass, ServiceBinary, vb.).

### App (C#)
- `Program.cs`: Örnek olarak `DeviceIoControl` ile `IOCTL_VHID_INJECT_REPORT` komutunu 
  driver'a gönderir.
- `VirtualHidClient.csproj`: C# proje ayarları.

## Kurulum & Kullanım

### 1. Driver'ı Derleme (WDK / Visual Studio)
1. Windows 10/11 WDK ve Visual Studio (uyumlu sürüm) kurulu olmalı.
2. Visual Studio'da `Driver` klasöründeki `.vcxproj` (varsa) açın veya yeni bir 
   “KMDF Driver” projesi oluşturup kaynak dosyalarını ekleyin.
3. `x64 Debug` (veya `x64 Release`) modda derleyin.
4. Ortaya çıkan `.sys` dosyasını ve `.inf` dosyasını bir klasörde tutun.

### 2. Sürücüyü İmzalama / Test Mod
- **Test modda** deneme yapmak için:
  ```cmd
  bcdedit /set testsigning on
  reboot
