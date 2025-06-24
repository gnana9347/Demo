package com.k7it.controller;

import org.springframework.web.bind.annotation.*;
import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/api/pentagon")
public class PentagonController {

    @PostMapping("/calculate")
    public Map<String, Object> calculatePentagon(@RequestBody Map<String, Object> request) {
        double side = convertToDouble(request.get("side"));
        Map<String, Object> response = new HashMap<>();

        // Calculate the perimeter of the pentagon
        double perimeter = 5 * side;
        response.put("perimeter", perimeter);

        // Calculate the area of the pentagon using the formula: (1/4) * √(5(5 + 2√5)) * side^2
        double area = (1.0 / 4.0) * Math.sqrt(5 * (5 + 2 * Math.sqrt(5))) * side * side;
        response.put("area", area);

        // Calculate the diagonal of the pentagon using the formula: side * (1 + √5) / 2
        double diagonal = side * (1 + Math.sqrt(5)) / 2;
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
