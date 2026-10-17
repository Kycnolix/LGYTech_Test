LGYTech Demo — Sipariş ve Stok Yönetimi

Bu projeyi ASP.NET MVC 5 ve .NET Framework 4.8 kullanarak geliştirdim. Amaç, ürünlerin yönetilebildiği, sipariş oluşturulabildiği ve her siparişin stok üzerindeki etkisinin takip edilebildiği küçük bir uygulama hazırlamaktı.

Projede özellikle sipariş işleminin güvenilir olmasına odaklandım. Bir siparişte birden fazla ürün varsa ve ürünlerden birinin stoku yetmiyorsa hiçbir değişikliğin veritabanına yansımaması gerekiyor. Aynı ürüne eş zamanlı sipariş geldiğinde de stok eksiye düşmemeli.

Kullanılan teknolojiler

\- C# / ASP.NET MVC 5 — Web uygulaması

\- .NET Framework 4.8 — Uygulama altyapısı

\- Entity Framework 6.1 (Code First) — Veri erişimi ve migrations

\- SQL Server — Veritabanı

\- Razor, Bootstrap 5 ve jQuery — Arayüz

\- T-SQL Stored Procedure ve Table-Valued Parameter (TVP) — Sipariş ve stok işlemleri

Uygulamada neler var?

Ürünleri listeleyebilir, isme göre arayabilir, yeni ürün ekleyebilir ve mevcut ürünleri düzenleyebilirsiniz. Ürün listesinde sayfalama bulunuyor; her sayfada 10 kayıt gösteriliyor. Yeni ürün oluşturulurken SKU otomatik üretiliyor.

Sipariş ekranında birden fazla ürün seçilip miktarları girilebiliyor. Sipariş tamamlandığında ürün stokları güncelleniyor, sipariş satırları kaydediliyor ve her stok değişikliği için bir hareket oluşturuluyor. Sipariş geçmişi ile stok hareketleri ayrı ekranlardan görüntülenebiliyor.

Sipariş toplamı veritabanında hesaplanıyor. Ayrıca ürünün satış anındaki birim fiyatı OrderLines.UnitPrice alanında saklanıyor; böylece ürün fiyatı sonradan değişse bile eski siparişlerin tutarı korunuyor.

Veritabanı yapısı

Projede dört ana tablo var:

Tablo	İçerik

Products	Ürün adı, benzersiz SKU, fiyat ve güncel stok

Orders	Müşteri adı, sipariş tarihi ve toplam tutar

OrderLines	Siparişteki ürünler, miktarlar ve satış anındaki birim fiyatlar

StockMovements	Hangi ürünün stokunun ne zaman, ne kadar ve hangi nedenle değiştiği





Fiyatlar decimal(18,2) olarak saklanıyor. Ürün ve sipariş girişlerinde geçersiz fiyat, stok ve miktar değerlerine karşı doğrulamalar bulunuyor.

Projeyi çalıştırma

Projeyi açmak için Visual Studio'da .NET Framework 4.8 ve ASP.NET MVC geliştirme bileşenlerinin yüklü olması gerekiyor. Veritabanı olarak SQL Server, SQL Server Express veya LocalDB kullanabilirsiniz.

1\. Repoyu klonlayıp .sln dosyasını Visual Studio'da açın.

2\. Web.config içindeki connection string'i kendi SQL Server ortamınıza göre düzenleyin. Bağlantı adının AppDbContext yapılandırmasıyla uyumlu olduğundan emin olun.

3\. NuGet paketlerini geri yükleyip projeyi derleyin.

4\. Package Manager Console üzerinden migration'ların bulunduğu projeyi seçin ve şu komutu çalıştırın:

&#x20;  Update-Database -Verbose

5\. Migration işlemi tabloları ve sipariş için kullanılan veritabanı nesnelerini oluşturur. Başlangıç ürünleri de seed işlemiyle eklenir.

6\. Uygulamayı IIS Express ile başlatın.

Kullanabileceğiniz sayfalar:

Adres	Sayfa

/Products	Ürün listesi

/Products/Create	Yeni ürün

/Orders/Create	Sipariş oluşturma

/Orders	Sipariş geçmişi

/StockMovements	Stok hareketleri





Stored procedure notu: Sql/OrderItemType.sql ve Sql/CreateOrder.sql dosyaları migration içinde SqlResource(...) ile kullanılıyor. Bunun için Visual Studio'da bu iki dosyanın Build Action özelliği Embedded Resource olmalı.

Başlangıç ürünleri

Uygulama ilk kurulduğunda şu beş ürün ekleniyor:

Ürün	SKU	Fiyat	Stok

Kalem	KLM-001	10,00	100

Defter	DFT-001	35,00	50

Silgi	SLG-001	5,00	3

Cetvel	CTV-001	15,00	1

Çanta	CNT-001	250,00	0





Seed işlemi aynı SKU'ya sahip ürünleri tekrar eklemiyor ve mevcut stokları başlangıç değerine döndürmüyor. Bu yüzden testleri yeniden çalıştırırken güncel stokları kontrol etmek önemli.

Sipariş ve stok işlemi nasıl çalışıyor?

Sipariş formundan gelen veriler önce controller tarafında kontrol ediliyor. Ardından OrderService, ürünleri ve miktarlarını bir Table-Valued Parameter (TVP) içinde dbo.CreateOrder stored procedure'üne gönderiyor.

Asıl sipariş işlemi SQL Server'da, tek bir transaction içinde tamamlanıyor. Stored procedure; stok kontrolünü, stok düşümünü, sipariş ve sipariş satırlarının eklenmesini ve stok hareketlerinin kaydedilmesini birlikte yönetiyor.

Stok düşümü için kullandığım temel sorgu şu:

UPDATE dbo.Products

SET Stock = Stock - @Quantity

WHERE Id = @ProductId AND Stock >= @Quantity;

Burada stok yeterli değilse ilgili satır güncellenmiyor. Stored procedure bunu hata olarak ele alıyor ve ROLLBACK çalıştırıyor. Böylece siparişin yalnızca bir kısmının kaydedilmesi engelleniyor.

Eş zamanlı iki istek aynı ürünü satın almaya çalıştığında da stok kontrolü ve güncelleme aynı SQL işlemi içinde yapıldığı için stok eksiye düşmüyor.

A, B ve C test senaryoları

Sipariş akışını üç senaryoyla kontrol ettim. Testler tarayıcının F12 → Console bölümünden çalıştırılabiliyor. Kullanılan JavaScript dosyalarını Tests/ klasöründe tutabilirsiniz.

Önemli: Bunlar gerçek sipariş isteği gönderiyor. Testleri yalnızca geliştirme/test veritabanında çalıştırın. Kodlardaki ürün ID'lerini kullanmadan önce aşağıdaki sorguyla kontrol edin:

SELECT Id, Name, Sku, Stock

FROM dbo.Products

ORDER BY Id;

Testler /Orders/Create sayfasında çalıştırılmalı. Sipariş formundaki @Html.AntiForgeryToken() alanı da POST isteğine gönderilmeli.

A — Başarılı sipariş

İlk testte 2 Kalem ve 5 Defter içeren bir sipariş gönderiliyor.

Beklediğim sonuç, success: true yanıtı ve yeni bir orderId dönmesi. Veritabanında bir sipariş, iki sipariş satırı ve iki stok hareketi oluşmalı. Kalem stoku 2, Defter stoku 5 azalmalı.

Bu testi çalıştırmadan önce Kalem için en az 2, Defter için en az 5 adet stok olduğundan emin olun.

B — Stok yetersizliği ve rollback

İkinci testte 2 Kalem ve 999999 Silgi isteniyor. Silgi stoku yetersiz olduğu için siparişin reddedilmesi gerekiyor.

Beklenen yanıt:

{

&#x20;   success: false,

&#x20;   message: "Insufficient stock for one or more products."

}

Burada kontrol etmek istediğim şey yalnızca hata mesajı değil. İşlem sırasında Kalem'in stoku düşürülmüş olsa bile transaction geri alındığında eski değerine dönmeli. Başarısız siparişe ait Orders, OrderLines veya StockMovements kaydı kalmamalı.

Bunu doğrulamak için test öncesindeki ve sonrasındaki veritabanı kayıtlarını karşılaştırıyorum.

C — Eş zamanlı sipariş

Son testte, stoku 1 olan Cetvel için Promise.all() ile aynı anda iki sipariş gönderiliyor.

İki isteğin de başarılı olmaması gerekiyor. Biri siparişi oluşturmalı, diğeri stok yetersizliği hatası almalı.

Örnek Console sonucu:

\[

&#x20;   {

&#x20;       success: false,

&#x20;       message: "Insufficient stock for one or more products."

&#x20;   },

&#x20;   {

&#x20;       success: true,

&#x20;       message: "Order created successfully.",

&#x20;       orderId: 16

&#x20;   }

]

Yanıtların sırası ve orderId değişebilir. Önemli olan bir isteğin başarılı, diğerinin başarısız olması. Testten sonra Cetvel stoku 0 olmalı ve yalnızca bir sipariş kaydedilmeli.

Not: C testini yeniden çalıştırmak için Cetvel stokunun tekrar 1 olduğu izole bir test ortamı kullanılmalı. Mevcut sipariş kayıtlarının tutarlılığını bozacak şekilde canlı verileri değiştirmeyin.

SQL ile sonuçları kontrol etme

Testlerde API yanıtlarını görmek yeterli değil; özellikle rollback ve stok hareketleri için veritabanını da kontrol etmek gerekiyor.

\-- Current stock

SELECT Id, Name, Stock

FROM dbo.Products

ORDER BY Id;



\-- Recent orders

SELECT TOP (20) Id, CustomerName, CreatedAt, Total

FROM dbo.Orders

ORDER BY Id DESC;



\-- Order lines

SELECT TOP (30) Id, OrderId, ProductId, Quantity, UnitPrice

FROM dbo.OrderLines

ORDER BY Id DESC;



\-- Stock movements

SELECT TOP (30) Id, ProductId, \[Change], Reason, CreatedAt

FROM dbo.StockMovements

ORDER BY Id DESC;

Klasörler

Controllers/    MVC controller'ları

Models/         Veritabanı entity'leri

ViewModels/     Ekranlara gönderilen modeller

Services/       Sipariş işlemlerinin servis katmanı

Views/          Razor sayfaları

Migrations/     EF6 migrations ve seed

Sql/            TVP ve stored procedure dosyaları

Tests/          Testlerde kullanılan JavaScript dosyaları

Son not

Bu çalışmada en çok üzerinde durduğum konu stok tutarlılığıydı. Normal bir sipariş oluşturmak kadar, stok yetmediğinde hiçbir yarım kaydın kalmaması ve iki kişinin aynı son ürünü almaya çalıştığında sistemin doğru davranması da önemliydi. Bu nedenle sipariş oluşturma sürecini stored procedure ve transaction üzerinden yönettim; A, B ve C senaryolarıyla da bu davranışları test ettim.

