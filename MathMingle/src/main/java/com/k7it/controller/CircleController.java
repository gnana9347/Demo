package com.k7it.controller;


import org.springframework.context.annotation.Configuration;
import org.springframework.web.bind.annotation.*;
import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/api/circle")
public class CircleController {

    @PostMapping("/calculate")
    public Map<String, Object> calculateCircle(@RequestBody Map<String, Object> request) {
        double radius = convertToDouble(request.get("radius"));
        Map<String, Object> response = new HashMap<>();
        response.put("area", Math.PI * radius * radius);
        response.put("perimeter", 2 * Math.PI * radius);
        response.put("diagonal", 2 * radius); // Circle diameter
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
