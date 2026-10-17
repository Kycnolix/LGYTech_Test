Projeyi geliştirirken öğrendiklerim

Bu proje benim için sadece ASP.NET MVC 5 ile çalışan bir sipariş ekranı hazırlamaktan ibaret değildi. Daha önce aşina olduğum backend tarafının yanında, Razor ile arayüz oluşturmayı ve yazdığım işlemleri gerçekten test etmeyi de deneyimledim.

En çok zorlandığım yerler

Razor (.cshtml) tarafı beklediğimden fazla zaman aldı. Controller'dan gelen veriyi View'da göstermek, form alanlarını doğru bağlamak ve JavaScript ile Razor'ın birlikte çalışmasını sağlamak başta karışıktı. Özellikle \_Layout.cshtml içinde navbar'ı düzenlerken @RenderBody() konumunu bozup sayfanın hata vermesi, layout yapısının neden önemli olduğunu anlamamı sağladı. Bootstrap dosyalarının yüklenmesini ve sürümü yüzünden sitenin çökmesini engellemek için ayrı uğraşmak zorunda kaldım.

JavaScript testleri de en çok uğraştığım konulardan biriydi. Sipariş oluşturma isteğini tarayıcı konsolundan göndermek için form verilerini ve AntiForgeryToken değerini doğru taşımam gerekti. İlk denemelerde Promise çıktısına bakıp gerçek API yanıtını okumakta zorlandım. Sonrasında async/await, fetch ve Promise.all() kullanarak iki eş zamanlı sipariş isteğinin sonucunu doğrudan görebildim.

Backend tarafında öğrendiklerim

Sipariş oluşturmanın yalnızca veritabanına kayıt eklemek olmadığını daha iyi anladım. Bir siparişte stok düşme, sipariş satırlarını ekleme ve stok hareketi oluşturma işlemlerinin birlikte başarılı olması gerekiyor. Bu yüzden işlemleri SQL Server'da bir stored procedure ve transaction içinde topladım.

Yetersiz stok olduğunda bütün işlemlerin geri alınmasını, aynı ürüne eş zamanlı iki sipariş geldiğinde ise stokun eksiye düşmemesini test ettim. Özellikle bu testler, çalışan bir ekranla güvenilir çalışan bir sistemin aynı şey olmadığını gösterdi.

Sonuç

En fazla Razor ve JavaScript testlerinde zorlandım ama en çok bu kısımlardan şey öğrendim. Projeyi bitirirken sadece özelliğin çalışmasına değil, hata durumunda ne yaptığına ve bunu nasıl doğrulayabileceğime de bakmaya başladım.

