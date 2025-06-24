package com.k7it.controller;

import org.springframework.web.bind.annotation.*;
import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/api/triangle")
public class TriangleController {

    @PostMapping("/calculate")
    public Map<String, Object> calculateTriangle(@RequestBody Map<String, Object> request) {
        double side1 = convertToDouble(request.get("side1"));
        double side2 = convertToDouble(request.get("side2"));
        double side3 = convertToDouble(request.get("side3"));

        // Check if the sides form a valid triangle
        if (side1 + side2 <= side3 || side1 + side3 <= side2 || side2 + side3 <= side1) {
            throw new IllegalArgumentException("The provided sides do not form a valid triangle.");
        }

        double s = (side1 + side2 + side3) / 2;
        double area = Math.sqrt(s * (s - side1) * (s - side2) * (s - side3));
        double perimeter = side1 + side2 + side3;
        double diagonal = Math.max(side1, Math.max(side2, side3));

        Map<String, Object> response = new HashMap<>();
        response.put("area", area);
        response.put("perimeter", perimeter);
        response.put("diagonal", diagonal);
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
