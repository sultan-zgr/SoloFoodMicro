
🍔 SoloFoodMicro - Food Order and Payment Microservices

SoloFoodMicro, yemek siparişi ve ödeme süreçlerini yönetmek için geliştirilen .NET 8 tabanlı bir mikroservis mimarisi projesidir. Restoranlardan sipariş alma ve ödeme işleme işlevlerini içerir. Bu proje, Saga Pattern ve eventual consistency gibi modern mikroservis mimarisi kavramlarını uygular.

🎯 Amaç
SoloFoodMicro, sipariş süreçlerini kusursuz bir şekilde yönetirken dağıtık sistemlerde tutarlılığı sağlamak için tasarlandı. Sipariş ve ödeme servisleri birbiriyle entegre çalışır, hatalı durumlar Saga Pattern ile ele alınır.

📦 Proje Yapısı
1. Order Service (Sipariş Servisi)
Kullanıcıların sipariş oluşturmasını sağlar.
Siparişlerin durumlarını günceller (Alındı, Hazırlanıyor, Teslim Edildi).
MSSQL kullanılarak sipariş verilerini depolar.
2. Payment Service (Ödeme Servisi)
Ödeme işlemlerini alır ve doğrular.
Ödeme başarısız olursa siparişi otomatik olarak iptal eder (Saga Pattern ile).
Redis kullanarak ödeme bilgilerini geçici olarak saklar.

🛠️ Kullanılan Teknolojiler
.NET 8 – Mikroservis geliştirme
MSSQL – Veritabanı yönetimi
RabbitMQ – Servisler arası mesajlaşma
Redis – Geçici veri saklama ve önbellekleme
Docker – Konteyner yönetimi
API Gateway – Servis yönetimi ve dış dünyaya açılma
Saga Pattern – Dağıtık işlemler için eventual consistency

🚀 Kurulum ve Çalıştırma
1. Gereksinimler
Docker ve Docker Compose yüklü olmalıdır.
MSSQL ve Redis imajları hazır olmalıdır.
2. Docker Compose ile Başlatma
--> docker-compose up -d
3. RabbitMQ Yönetim Paneli
http://localhost:15672
Kullanıcı adı: guest
Şifre: guest
4. API Gateway Üzerinden Servislere Erişim
Order Service: http://localhost:5000/api/orders
Payment Service: http://localhost:5001/api/payments

📖 Örnek Akış
Sipariş Oluşturma
Kullanıcı, siparişini Order Service üzerinden oluşturur.
Ödeme İşleme
Payment Service, ödeme sürecini yönetir. Başarılıysa sipariş hazırlanır.
Hata Yönetimi (Saga Pattern)
Ödeme başarısız olursa sipariş iptal edilir ve stok güncellenir.

🔗 API Endpointleri
Order Service
GET /api/orders: Tüm siparişleri getirir.
POST /api/orders: Yeni sipariş oluşturur.
PUT /api/orders/{id}: Sipariş durumunu günceller.
Payment Service
POST /api/payments: Yeni ödeme başlatır.
GET /api/payments/{id}: Ödeme detaylarını getirir.


İşte GitHub uyumlu, temiz ve açıklayıcı bir README.md dosyası:

🍔 SoloFoodMicro - Food Order and Payment Microservices
SoloFoodMicro, yemek siparişi ve ödeme süreçlerini yönetmek için geliştirilen .NET 8 tabanlı bir mikroservis mimarisi projesidir. Restoranlardan sipariş alma ve ödeme işleme işlevlerini içerir. Bu proje, Saga Pattern ve eventual consistency gibi modern mikroservis mimarisi kavramlarını uygular.

🎯 Amaç
SoloFoodMicro, sipariş süreçlerini kusursuz bir şekilde yönetirken dağıtık sistemlerde tutarlılığı sağlamak için tasarlandı. Sipariş ve ödeme servisleri birbiriyle entegre çalışır, hatalı durumlar Saga Pattern ile ele alınır.

📦 Proje Yapısı
1. Order Service (Sipariş Servisi)
Kullanıcıların sipariş oluşturmasını sağlar.
Siparişlerin durumlarını günceller (Alındı, Hazırlanıyor, Teslim Edildi).
MSSQL kullanılarak sipariş verilerini depolar.
2. Payment Service (Ödeme Servisi)
Ödeme işlemlerini alır ve doğrular.
Ödeme başarısız olursa siparişi otomatik olarak iptal eder (Saga Pattern ile).
Redis kullanarak ödeme bilgilerini geçici olarak saklar.

🛠️ Kullanılan Teknolojiler
.NET 8 – Mikroservis geliştirme
MSSQL – Veritabanı yönetimi
RabbitMQ – Servisler arası mesajlaşma
Redis – Geçici veri saklama ve önbellekleme
Docker – Konteyner yönetimi
API Gateway – Servis yönetimi ve dış dünyaya açılma
Saga Pattern – Dağıtık işlemler için eventual consistency

🚀 Kurulum ve Çalıştırma
1. Gereksinimler
Docker ve Docker Compose yüklü olmalıdır.
MSSQL ve Redis imajları hazır olmalıdır.
2. Docker Compose ile Başlatma
bash
Kodu kopyala
docker-compose up -d
3. RabbitMQ Yönetim Paneli
http://localhost:15672
Kullanıcı adı: guest
Şifre: guest
4. API Gateway Üzerinden Servislere Erişim
Order Service: http://localhost:5000/api/orders
Payment Service: http://localhost:5001/api/payments

📖 Örnek Akış
Sipariş Oluşturma
Kullanıcı, siparişini Order Service üzerinden oluşturur.
Ödeme İşleme
Payment Service, ödeme sürecini yönetir. Başarılıysa sipariş hazırlanır.
Hata Yönetimi (Saga Pattern)
Ödeme başarısız olursa sipariş iptal edilir ve stok güncellenir.

🔗 API Endpointleri
Order Service
GET /api/orders: Tüm siparişleri getirir.
POST /api/orders: Yeni sipariş oluşturur.
PUT /api/orders/{id}: Sipariş durumunu günceller.
Payment Service
POST /api/payments: Yeni ödeme başlatır.
GET /api/payments/{id}: Ödeme detaylarını getirir.

🌀 Saga Pattern Kullanımı
Projede Saga Pattern, ödeme ve sipariş süreçlerini koordine eder. Her işlem bağımsız çalışır ve başarısızlık durumunda süreç geri alınır. Böylece sistemde eventual consistency sağlanır.
