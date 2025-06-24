package com.k7it.pack2;

public class EqIgnCase {
	static String str="Gnana";
	static String str1="Gnana";
	static String str2="Gana";
	public static void main(String[] args) {
		System.out.println(str.equals(str1));
		System.out.println(str.equalsIgnoreCase(str1));
		System.out.println(str1.equals(str2));
	}
}
