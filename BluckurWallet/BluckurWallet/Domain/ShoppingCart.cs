using System;
using System.Collections.Generic;

namespace BluckurWallet.Domain
{
    public class ShoppingCart
    {
        /// <summary>
        /// Contains shopItems in cart.
        /// </summary>
        /// <value>ShopItem: shopItem, int: amount of shopItems</value>
        public Dictionary<ShopItem, int> shopItems { get; set; }

        private ShoppingCart()
        {
            shopItems = new Dictionary<ShopItem, int>();
        }

        private static readonly ShoppingCart shoppingCart = new ShoppingCart();

        public static ShoppingCart GetShoppingCart()
        {
            return shoppingCart;
        }

        /// <summary>
        /// Get the total price and item count of cart items.
        /// String: Price
        /// Int: Item count
        /// </summary>
        /// <returns>The total price and item count.</returns>
        public Tuple<string, int> GetTotalPriceAndItemCount()
        {
            string price = "0,00 Blucker";
            double priceDouble = 0.00;
            int itemCount = 0;

            foreach(KeyValuePair<ShopItem, int> keyPair in shopItems)
            {
                priceDouble += keyPair.Key.Price * keyPair.Value;
                itemCount += keyPair.Value;
            }

            price = priceDouble.ToString();
            price = price.Replace('.', ',');
            price = price + " Blucker";

            return new Tuple<string, int>(price, itemCount);
        }
    }
}
