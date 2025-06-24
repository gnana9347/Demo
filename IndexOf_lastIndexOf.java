package com.k7it.pack2;

public class IndexOf_lastIndexOf {
	
	static String str="Gnana Sekhar";
	
	public static void main(String[] args) {
		
		System.out.println(str.indexOf("n"));
		System.out.println(str.indexOf(" "));
		System.out.println(str.indexOf("f"));
		System.out.println(str.indexOf("a"));
		System.out.println(str.lastIndexOf("a"));
		System.out.println(str.lastIndexOf("ana"));
		
		if(str.indexOf("g")==str.lastIndexOf("g")) {
			System.out.println("once");
		}else {
		System.out.println("multiple");}
		
		
		if(str.indexOf("n")==str.lastIndexOf("n")) {
			System.out.println("once");
		}else {
			System.out.println("multiple");}
		
		
		if(str.indexOf("s")==str.lastIndexOf("s")) {
			System.out.println("once");
		}else {
			System.out.println("multiple");}
		
	}
}
