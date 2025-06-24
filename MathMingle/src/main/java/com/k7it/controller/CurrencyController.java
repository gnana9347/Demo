package com.k7it.controller;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.client.RestTemplate;
import com.fasterxml.jackson.databind.JsonNode;

@RestController
public class CurrencyController {

    private static final String API_URL = "https://openexchangerates.org/api/latest.json?app_id=060d5cca5ad549d2aa19e2ee9e1f9769"; // Replace with your API key

    @GetMapping("/convert")
    public CurrencyConversionResponse convertCurrency(
            @RequestParam String from,
            @RequestParam String to,
            @RequestParam double amount) {
        double conversionRate = getConversionRate(from, to);
        double convertedAmount = amount * conversionRate;
        return new CurrencyConversionResponse(convertedAmount);
    }

    private double getConversionRate(String from, String to) {
        RestTemplate restTemplate = new RestTemplate();
        String url = String.format("%s&symbols=%s", API_URL, to); // Removed `base` parameter as Open Exchange Rates API defaults to USD
        try {
            JsonNode response = restTemplate.getForObject(url, JsonNode.class);
            JsonNode rates = response.get("rates");
            return rates.get(to).asDouble();
        } catch (Exception e) {
            // Handle the error appropriately
            return 1.0; // Default conversion rate if API call fails
        }
    }

    public static class CurrencyConversionResponse {
        private double convertedAmount;

        public CurrencyConversionResponse(double convertedAmount) {
            this.convertedAmount = convertedAmount;
        }

        public double getConvertedAmount() {
            return convertedAmount;
        }

        public void setConvertedAmount(double convertedAmount) {
            this.convertedAmount = convertedAmount;
        }
    }
}
