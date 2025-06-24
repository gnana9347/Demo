package com.k7it.pack2;

public class CharAt {

	static String str="GnanaSekhar";
	
	public static void main(String[] args) {
		System.out.println(str.charAt(5));//this is called hot coding
		System.out.println(str.charAt(10));
		System.out.println(str.charAt(1));
		//System.out.println(str.charAt(15));//java.lang.StringIndexOutOfBoundsException:
		
		for (int i=0;i<str.length();i++) {//this is called dynamic values
			str.charAt(i);
			System.out.println(i);
			
		}
	}

}
