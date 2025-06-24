package com.k7it.pack1;

public final class Singletone {

	private static Singletone s1;
	
	private Singletone() {
		
	}
	public static void main(String [] args) {
		System.out.println(Singletone.get_s1());
		System.out.println(Singletone.get_s1());
	}
	public static Singletone get_s1 () {
		if(s1==null) {
			s1=new Singletone();
			return s1;
		}
		return s1;
	}
}
