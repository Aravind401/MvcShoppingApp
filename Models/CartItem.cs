namespace MvcShoppingApp.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public string SessionId { get; internal set; }
    }
}