package com.k7it.controller;


import org.springframework.web.bind.annotation.*;
import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/api/square")
public class SquareController {

    @PostMapping("/calculate")
    public Map<String, Object> calculateSquare(@RequestBody Map<String, Object> request) {
        double side = convertToDouble(request.get("side"));
        Map<String, Object> response = new HashMap<>();
        response.put("area", side * side);
        response.put("perimeter", 4 * side);
        response.put("diagonal", Math.sqrt(2) * side);
        return response;
    }

    private double convertToDouble(Object value) {
        if (value instanceof Integer) {
            return ((Integer) value).doubleValue();
        } else if (value instanceof Double) {
            return (Double) value;
        } else {
            throw new IllegalArgumentException("Unsupported type: " + value.getClass());
        }
    }
}
