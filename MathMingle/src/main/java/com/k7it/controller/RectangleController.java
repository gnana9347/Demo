
package com.k7it.controller;


import org.springframework.web.bind.annotation.*;
import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/api/rectangle")
public class RectangleController {

    @PostMapping("/calculate")
    public Map<String, Object> calculateRectangle(@RequestBody Map<String, Object> request) {
        double length = convertToDouble(request.get("length"));
        double breadth = convertToDouble(request.get("breadth"));
        Map<String, Object> response = new HashMap<>();
        response.put("area", length * breadth);
        response.put("perimeter", 2 * (length + breadth));
        response.put("diagonal", Math.sqrt(length * length + breadth * breadth));
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
