using System;
namespace BluckurWallet.Domain
{
    public class ShopItem
    {
		public int Id { get; set; }
		public int Price { get; set; }
		public string ImagePath { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Stock { get; set; }      

        public ShopItem(int id, int price, string imagePath, string name, string description, int stock)
        {
			this.Id = id;
			this.Price = price;
			this.ImagePath = imagePath;
			this.Name = name;
			this.Description = description;
			this.Stock = stock;
        }
    }
}
