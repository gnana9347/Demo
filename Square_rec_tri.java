package com.k7it.pack1;

public interface Square_rec_tri {
	public static final int i=10;
	
	public abstract void area();
	public abstract void radius();
	public abstract void circumference();
	default void perimeter() {
		System.out.println("Default method to every extended class");
	}
	public static void daimeter() {
		System.out.println("static method");
	}
}
