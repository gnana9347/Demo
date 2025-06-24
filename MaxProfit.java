package com.k7it.pack2;

public class MaxProfit {

    public static int maxProfit(int[] prices) {
        // Initialize variables
        int minPrice = Integer.MAX_VALUE; // Set to a very high number initially
        int maxProfit = 0; // Start with 0 profit

        // Iterate through the array of prices
        for (int price : prices) {
            // Update minPrice if the current price is lower
            if (price < minPrice) {
                minPrice = price;
            }
            // Calculate potential profit
            int profit = price - minPrice;
            // Update maxProfit if the calculated profit is higher
            if (profit > maxProfit) {
                maxProfit = profit;
            }
        }

        return maxProfit;
    }

    public static void main(String[] args) {
        // Example usage
        int[] prices1 = {7, 1, 5, 3, 6, 4};
        System.out.println(maxProfit(prices1)); // Output: 5

        int[] prices2 = {7, 6, 4, 3, 1};
        System.out.println(maxProfit(prices2)); // Output: 0
    }
}
