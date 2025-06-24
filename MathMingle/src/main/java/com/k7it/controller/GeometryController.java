package com.k7it.controller;

public class GeometryController {
    public static double calculateRectangleArea(double length, double width) {
        return length * width;
    }

    public static double calculateRectanglePerimeter(double length, double width) {
        return 2 * (length + width);
    }

    public static double calculateCircleArea(double radius) {
        return Math.PI * radius * radius;
    }

    public static double calculateCircleCircumference(double radius) {
        return 2 * Math.PI * radius;
    }
}
