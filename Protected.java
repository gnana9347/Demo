package com.k7it;
import com.k7it.pack1.Protect;

public class Protected extends Protect {

	public Protected() {
		
	}
	public static void main(String[] args) {
		Protect s2=new Protect();
		Protected p1=new Protected();
		System.out.println(p1.i);
		//System.out.println(s2.i);
		//bcz i will become specific to protected class 
		//so, it is not possible to access by class protect.....
		
	}
}
