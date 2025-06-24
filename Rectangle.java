package com.k7it.pack1;

public class Rectangle implements Square_rec_tri {
public static void main(String[] args) {
	Rectangle r1=new Rectangle();
	r1.perimeter();
	Square_rec_tri.daimeter();
}
	public Rectangle() {
		// TODO Auto-generated constructor stub
	}
	public void area() {
		System.out.println("Area of Rectangle");
	}
	public void radius() {
		System.out.println("radius of Rectangle");
	}
	public void circumference() {
		System.out.println("circumference of Rectangle");
	}
}
