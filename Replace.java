package com.k7it.pack2;

public class Replace {
	static String str="hello good morning";
	
	public static void main(String[] args) {
		System.out.println(str.replace("good", "bad"));
		System.out.println(str.replace(" ", ""));
		System.out.println(str.replaceAll("bad", "good"));
	}
}
