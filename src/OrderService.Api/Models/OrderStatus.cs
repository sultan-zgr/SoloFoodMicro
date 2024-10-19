namespace OrderService.Api.Models
{
    public enum OrderStatus
    {
        Pending,     // Ödeme bekleniyor
        Completed,   // Ödeme başarılı
        Failed,      // Ödeme başarısız
        Cancelled    // Kullanıcı iptal etti
    }
}
