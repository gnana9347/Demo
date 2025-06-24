package com.k7it;

abstract class Ab {
	int i;

	Ab(int i) {
		System.out.println("Ab");
	}

	Ab() {
		System.out.println("super");
	}

}

class Abc extends Ab {
	int j;

	public void vehicle() {

	}

	Abc() {
	
	}

	Abc(int j) {
		super(j);
		this.j=j;
		super.i = j;
		this.i = j;
		System.out.println("Abc");
	}

	public static void main(String[] args) {
		Abc a1 = new Abc();
		Abc a2 = new Abc(20);

	}

}

