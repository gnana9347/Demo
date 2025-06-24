package com.k7it.controller;

import org.springframework.web.bind.annotation.*;
import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/api/rhombus")
public class RhombusController {

    @PostMapping("/calculate")
    public Map<String, Object> calculateRhombus(@RequestBody Map<String, Object> request) {
        double side = convertToDouble(request.get("side"));
        double angleInDegrees = convertToDouble(request.get("angle")); // The angle in degrees between two adjacent sides
        Map<String, Object> response = new HashMap<>();

        // Calculate the perimeter of the rhombus
        double perimeter = 4 * side;
        response.put("perimeter", perimeter);

        // Calculate the area of the rhombus using the formula: side^2 * sin(angle)
        double angleInRadians = Math.toRadians(angleInDegrees);
        double area = side * side * Math.sin(angleInRadians);
        response.put("area", area);

        // Calculate the diagonals using the formulas: d1 = side * sqrt(2 + 2 * cos(angle)), d2 = side * sqrt(2 - 2 * cos(angle))
        double diagonal1 = side * Math.sqrt(2 + 2 * Math.cos(angleInRadians));
        double diagonal2 = side * Math.sqrt(2 - 2 * Math.cos(angleInRadians));
        response.put("diagonal1", diagonal1);
        response.put("diagonal2", diagonal2);

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
