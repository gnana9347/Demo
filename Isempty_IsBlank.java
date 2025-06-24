package com.k7it.pack2;

public class Isempty_IsBlank {

	static String str1="Gnana";
	static String str2="";
	static String str3=null;
	public static void main(String[] args) {
		System.out.println(str1.isEmpty());
		System.out.println(str2.isEmpty());
		//System.out.println(str3.isEmpty());//null pointer exception.
		System.out.println(str1.isBlank());
		System.out.println(str2.isBlank());
		System.out.println(str3.isBlank());//null pointer exception
	}
}
